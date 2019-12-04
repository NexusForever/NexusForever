using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using ItemEntity = NexusForever.WorldServer.Game.Entity.Item;

namespace NexusForever.WorldServer.Game.Spell
{
    public class UnlockedSpell : ISaveCharacter, IUpdate
    {
        public Player Owner { get; }
        public SpellBaseInfo Info { get; }
        public ItemEntity Item { get; }
        public uint MaxAbilityCharges => Info.GetSpellInfo(Tier).Entry.AbilityChargeCount;
        public uint AbilityRechargeTime => Info.GetSpellInfo(Tier).Entry.AbilityRechargeTime;
        public uint AbilityRechargeCount => Info.GetSpellInfo(Tier).Entry.AbilityRechargeCount;
        public uint AbilityCharges { get; private set; }

        public byte Tier
        {
            get => tier;
            set
            {
                tier = value;
                saveMask |= UnlockedSpellSaveMask.Tier;
            }
        }
        private byte tier;

        private UnlockedSpellSaveMask saveMask;

        private UpdateTimer rechargeTimer;

        /// <summary>
        /// Create a new <see cref="UnlockedSpell"/> from an existing database model.
        /// </summary>
        public UnlockedSpell(Player player, SpellBaseInfo info, CharacterSpellModel model, ItemEntity item)
        {
            Owner = player;
            Info  = info;
            tier  = model.Tier;
            Item  = item;

            InitialiseAbilityCharges();
        }

        /// <summary>
        /// Create a new <see cref="UnlockedSpell"/> from a <see cref="SpellBaseInfo"/>.
        /// </summary>
        public UnlockedSpell(Player player, SpellBaseInfo info, byte tier, ItemEntity item)
        {
            Owner = player;
            Info  = info ?? throw new ArgumentNullException();
            Tier  = tier;
            Item  = item;

            InitialiseAbilityCharges();

            saveMask = UnlockedSpellSaveMask.Create;
        }

        public void Update(double lastTick)
        {
            if (MaxAbilityCharges > 0 && AbilityCharges < MaxAbilityCharges)
            {
                rechargeTimer.Update(lastTick);
                if (rechargeTimer.HasElapsed)
                {
                    AbilityCharges = Math.Clamp(AbilityCharges + AbilityRechargeCount, 0u, MaxAbilityCharges);
                    SendChargeUpdate();
                    rechargeTimer.Reset();
                }
            }
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == UnlockedSpellSaveMask.None)
                return;

            if ((saveMask & UnlockedSpellSaveMask.Create) != 0)
            {
                var model = new CharacterSpellModel
                {
                    Id           = Owner.CharacterId,
                    Spell4BaseId = Info.Entry.Id,
                    Tier         = tier
                };

                context.Add(model);
            }
            else
            {
                var model = new CharacterSpellModel
                {
                    Id           = Owner.CharacterId,
                    Spell4BaseId = Info.Entry.Id,
                };

                EntityEntry<CharacterSpellModel> entity = context.Attach(model);
                if ((saveMask & UnlockedSpellSaveMask.Tier) != 0)
                {
                    model.Tier = tier;
                    entity.Property(p => p.Tier).IsModified = true;
                }
            }

            saveMask = UnlockedSpellSaveMask.None;
        }

        private void InitialiseAbilityCharges()
        {
            if (MaxAbilityCharges > 0)
            {
                rechargeTimer = new UpdateTimer(AbilityRechargeTime / 1000d);
                AbilityCharges = MaxAbilityCharges;
                SendChargeUpdate();
            }
        }

        // <summary>
        /// Used for when the client does not have continuous casting enabled
        /// </summary>
        public void Cast()
        {
            CastSpell();
        }

        /// <summary>
        /// Used for continuous casting when the client has it enabled, or spells with Cast Methods like ChargeRelease
        /// </summary>
        public void Cast(bool buttonPressed)
        {
            // TODO: Handle continuous casting of spell for Player if button remains depressed

            // If the player depresses button after the spell had exceeded its threshold, don't try and recast the spell until button is pressed down again.
            if (!buttonPressed)
                return;

            CastSpell();
        }

        private void CastSpell()
        {
            Owner.CastSpell(new SpellParameters
            {
                UnlockedSpell = this,
                SpellInfo = Info.GetSpellInfo(Tier),
                UserInitiatedSpellCast = true
            });
        }

        public void UseCharge()
        {
            if (AbilityCharges > 0)
            {
                AbilityCharges -= 1;
                SendChargeUpdate();
            }
            else
                throw new SpellException("No charges available.");
        }

        private void SendChargeUpdate()
        {
            Owner.Session.EnqueueMessageEncrypted(new ServerSpellAbilityCharges
            {
                SpellId = Item.Id,
                AbilityChargeCount = AbilityCharges
            });
        }
    }
}
