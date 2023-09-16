using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Entity
{
    public class ResurrectionManager : IResurrectionManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

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
        private bool hasCasterResurrectionRequest;

        private UpdateTimer wakeHereTimer = new UpdateTimer(TimeSpan.FromMinutes(30d), false);

        private IPlayer owner;

        /// <summary>
        /// Create a new <see cref="IResurrectionManager"/> for <see cref="IPlayer"/>.
        /// </summary>
        public ResurrectionManager(IPlayer owner)
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
        }

        /// <summary>
        /// Show resurrection options to owner <see cref="IPlayer"/>.
        /// </summary>
        public void ShowResurrection()
        {
            if (ResurrectionType != ResurrectionType.None)
                return;

            // delbrately not using property to prevent sending update packet
            resurrectionType = GetResurrectionType();

            owner.Session.EnqueueMessageEncrypted(new ServerResurrectionShow
            {
                GhostId             = owner.ControlGuid ?? 0u,
                RezCost             = GetCostForResurrection(),
                TimeUntilRezMs      = 0u,
                Dead                = true,
                ShowRezFlags        = ResurrectionType,
                HasCasterRezRequest = false,
                TimeUntilForceRezMs = 0u,
                TimeUntilWakeHereMs = wakeHereTimer.IsTicking ? (uint)wakeHereTimer.Duration * 1000u : 0u
            });

            log.Trace($"Player {owner.Guid} has resurrect options {resurrectionType}.");
        }

        private bool CanWakeHere()
        {
            return !wakeHereTimer.IsTicking;
        }

        private ResurrectionType GetResurrectionType()
        {
            ResurrectionType type = owner.Map.GetResurrectionType();
            if (CanWakeHere())
                type |= ResurrectionType.WakeHere;

            return type |= ResurrectionType.WakeHereServiceToken;
        }

        /// <summary>
        /// Resurrect owner <see cref="IPlayer"/> with the specified <see cref="ResurrectionType"/>.
        /// </summary>
        public void Resurrect(ResurrectionType type)
        {
            if (owner.IsAlive)
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
                default:
                    throw new NotImplementedException();
            }

            // delbrately not using property to prevent sending update packet
            resurrectionType = ResurrectionType.None;
            hasCasterResurrectionRequest = false;

            owner.ModifyHealth(owner.MaxHealth / 2, DamageType.Heal, null);
            owner.Shield = 0;

            log.Trace($"Player {owner.Guid} has accepted resurrection.");
        }

        private bool HandleWakeHere()
        {
            if (!CanWakeHere())
                return false;

            uint cost = GetCostForResurrection();
            if (!owner.CurrencyManager.CanAfford(CurrencyType.Credits, cost))
                return false;

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
        public void Resurrect(uint targetUnitId)
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

            log.Trace($"Player {owner.Guid} is resurrecting target player {targetUnitId}.");
        }

        /// <summary>
        /// Recieve resurrection request from <see cref="IPlayer"/>.
        /// </summary>
        public void ResurrectRequest(uint unitId)
        {
            if (ResurrectionType == ResurrectionType.None)
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

            log.Trace($"Player {owner.Guid} got resurrect request from player {unitId}.");
        }

        private uint GetCostForResurrection()
        {
            // TODO: Calculate credit cost correctly. 0 for now.
            return 0u;
        }

        private uint GetServiceTokenCostForResurrection()
        {
            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1315);
            return entry.Dataint0;
        }
    }
}
