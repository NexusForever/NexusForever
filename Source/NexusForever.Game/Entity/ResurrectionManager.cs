using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.AccountInventory;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity
{
    public class ResurrectionManager : IResurrectionManager
    {
        /// <summary>
        /// Determines if owner <see cref="IPlayer"/> can resurrect another player.
        /// </summary>
        public bool CanResurrectOtherPlayer
        {
            get => canResurrectOtherPlayer;
            set
            {
                canResurrectOtherPlayer = value;
                SendResurrectionState();
            }
        }

        private bool canResurrectOtherPlayer = true;

        private ResurrectionType ResurrectionType
        {
            get => resurrectionType;
            set
            {
                resurrectionType = value;

                owner.Session.EnqueueMessageEncrypted(new ServerResurrectionUpdate
                {
                    ShowRezFlags        = resurrectionType,
                    HasCasterRezRequest = hasCasterResurrectionRequest
                });
            }
        }

        private ResurrectionType resurrectionType;
        private bool hasResurrection;
        private bool hasCasterResurrectionRequest;

        private readonly UpdateTimer wakeHereTimer = new UpdateTimer(TimeSpan.FromMinutes(30d), false);
        private UpdateTimer timeUntilResurrectionTimer;
        private UpdateTimer timeUntilForcedResurrectionTimer;

        private IPlayer owner;

        #region Dependency Injection

        private readonly ILogger<ResurrectionManager> log;
        private readonly IGameTableManager gameTableManager;

        public ResurrectionManager(
            ILogger<ResurrectionManager> log,
            IGameTableManager gameTableManager)
        {
            this.log              = log;
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Initialise a new <see cref="IResurrectionManager"/> for <see cref="IPlayer"/>.
        /// </summary>
        public void Initalise(IPlayer owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Send initial resurrection packets to owner <see cref="IPlayer"/>.
        /// </summary>
        public void SendInitialPackets()
        {
            SendResurrectionState();
        }

        private void SendResurrectionState()
        {
            owner.Session.EnqueueMessageEncrypted(new ServerResurrectionState
            {
                RezType = canResurrectOtherPlayer ? ResurrectionType.SpellCasterLocation : ResurrectionType.None
            });
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            // TODO: timer
            //wakeHereTimer.Update(lastTick);

            if (timeUntilResurrectionTimer != null && timeUntilResurrectionTimer.IsTicking)
                timeUntilResurrectionTimer.Update(lastTick);
            
            if (timeUntilForcedResurrectionTimer != null && timeUntilForcedResurrectionTimer.IsTicking)
            {
                timeUntilForcedResurrectionTimer.Update(lastTick);
                if (timeUntilForcedResurrectionTimer.HasElapsed)
                    ResurrectSelfForced();
            }
        }

        /// <summary>
        /// Show resurrection options to owner <see cref="IPlayer"/>.
        /// </summary>
        public void ShowResurrection(ResurrectionType type, TimeSpan? timeUntilResurrection, TimeSpan? timeUntilForcedResurrection)
        {
            if (hasResurrection)
                return;

            hasResurrection = true;

            // delbrately not using property to prevent sending update packet
            resurrectionType = GetResurrectionType(type);

            if (timeUntilResurrection != null)
                timeUntilResurrectionTimer = new UpdateTimer(timeUntilResurrection.Value, true);

            if (timeUntilForcedResurrection != null)
                timeUntilForcedResurrectionTimer = new UpdateTimer(timeUntilForcedResurrection.Value, true);

            owner.Session.EnqueueMessageEncrypted(new ServerResurrectionShow
            {
                GhostId             = owner.ControlGuid ?? 0u,
                RezCost             = GetCostForResurrection(),
                TimeUntilRezMs      = timeUntilResurrectionTimer != null ? (uint)timeUntilResurrectionTimer.Duration * 1000u : 0u,
                Dead                = true,
                ShowRezFlags        = ResurrectionType,
                HasCasterRezRequest = false,
                TimeUntilForceRezMs = timeUntilForcedResurrection != null ? (uint)timeUntilForcedResurrectionTimer.Duration * 1000u : 0u, 
                TimeUntilWakeHereMs = wakeHereTimer.IsTicking ? (uint)wakeHereTimer.Duration * 1000u : 0u
            });

            log.LogTrace($"Player {owner.Guid} has resurrect options {resurrectionType}.");
        }

        private bool CanWakeHere()
        {
            uint cost = GetCostForResurrection();
            if (!owner.CurrencyManager.CanAfford(CurrencyType.Credits, cost))
                return false;

            return !wakeHereTimer.IsTicking;
        }

        private bool CanWakeHereServiceToken()
        {
            uint cost = GetServiceTokenCostForResurrection();
            return owner.Account.CurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, cost);
        }

        private ResurrectionType GetResurrectionType(ResurrectionType type)
        {
            if ((type & ResurrectionType.WakeHere) != 0 && !CanWakeHere())
            {
                type &= ~ResurrectionType.WakeHere;
                type |= ResurrectionType.Holocrypt;
            }

            if (CanWakeHereServiceToken())
                type |= ResurrectionType.WakeHereServiceToken;

            return type;
        }

        private void ResurrectSelfForced()
        {
            ResurrectSelf(ResurrectionType.Holocrypt);
            owner.Session.EnqueueMessageEncrypted(new ServerResurrectionForced());
        }

        /// <summary>
        /// Resurrect owner <see cref="IPlayer"/> with the specified <see cref="ResurrectionType"/>.
        /// </summary>
        public void ResurrectSelf(ResurrectionType type)
        {
            if ((ResurrectionType & type) == 0)
                return;

            if (owner.IsAlive)
                return;

            if (timeUntilResurrectionTimer != null && timeUntilResurrectionTimer.IsTicking)
                return;

            switch (type)
            {
                case ResurrectionType.WakeHere:
                    if (!HandleWakeHere())
                        return;
                    break;
                case ResurrectionType.WakeHereServiceToken:
                    if (!HandleWakeHereServiceToken())
                        return;
                    break;
                case ResurrectionType.SpellCasterLocation:
                    if (!CanSpellCasterLocation())
                        return;
                    break;
                /*default:
                    throw new NotImplementedException();*/
            }

            // delbrately not using property to prevent sending update packet
            resurrectionType = ResurrectionType.None;

            hasResurrection                  = false;
            hasCasterResurrectionRequest     = false;
            timeUntilResurrectionTimer       = null;
            timeUntilForcedResurrectionTimer = null;

            owner.Map.Resurrect(type, owner);

            log.LogTrace($"Player {owner.Guid} has accepted resurrection.");
        }

        private bool HandleWakeHere()
        {
            if (!CanWakeHere())
                return false;

            uint cost = GetCostForResurrection();
            owner.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, cost);
            
            // TODO: timer
            //wakeHereTimer.Reset();

            return true;
        }

        private bool HandleWakeHereServiceToken()
        {
            if (CanWakeHere())
                return false;

            uint cost = GetServiceTokenCostForResurrection();
            if (!owner.Account.CurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, cost))
                return false;

            owner.Account.CurrencyManager.CurrencySubtractAmount(AccountCurrencyType.ServiceToken, cost);
            return true;
        }

        private bool CanSpellCasterLocation()
        {
            return hasCasterResurrectionRequest;
        }

        /// <summary>
        /// Send resurrection request target <see cref="IPlayer>"/>.
        /// </summary>
        public void ResurrectTarget(uint targetUnitId)
        {
            IPlayer target = owner.GetVisible<IPlayer>(targetUnitId);
            if (target == null || target.IsAlive)
                return;

            if (!canResurrectOtherPlayer)
                return;

            uint spellId = owner.Class switch
            {
                Class.Medic    => 30330,
                Class.Engineer => 42838,
                Class.Warrior  => 42839,
                Class.Stalker  => 42840,
                _              => 30330, // Esper and Spellslinger don't have a resurrection spell, default to Medic
            };

            owner.CastSpell(spellId, new SpellParameters
            {
                PrimaryTargetId = target.Guid,
            });

            log.LogTrace($"Player {owner.Guid} is resurrecting target player {targetUnitId}.");
        }

        /// <summary>
        /// Recieve resurrection request from <see cref="IPlayer"/>.
        /// </summary>
        public void OnResurrectRequest(uint unitId)
        {
            if (!hasResurrection)
                return;

            if (hasCasterResurrectionRequest)
                return;

            hasCasterResurrectionRequest = true;
            ResurrectionType |= ResurrectionType.SpellCasterLocation;

            // client only reads the unit id from this packet, other properties look to be legacy
            owner.Session.EnqueueMessageEncrypted(new ServerResurrectRequest
            {
                UnitId = unitId
            });

            log.LogTrace($"Player {owner.Guid} got resurrect request from player {unitId}.");
        }

        private uint GetCostForResurrection()
        {
            // TODO: Calculate credit cost correctly. 0 for now.
            return 0u;
        }

        private uint GetServiceTokenCostForResurrection()
        {
            GameFormulaEntry entry = gameTableManager.GameFormula.GetEntry(1315);
            return entry.Dataint0;
        }
    }
}
