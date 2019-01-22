using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Spell
{
    public class ActionSet : ISaveCharacter, IEnumerable<ActionSetAction>
    {
        private readonly Player player;

        public const byte MaxTierPoints = 42;
        public const byte MaxActionSets = 4;
        public const byte MaxActionCount = 48;
        public const byte MaxAMPPoints = 45 + 10; // 10 are bonus unlocked
        public const byte MaxTier = 9;

        public byte Index { get; }
        public byte TierPoints { get; private set; }
        public byte AMPPoints { get; private set; }

        public List<ActionSetAction> Actions { get; private set; } = new List<ActionSetAction>();
        public List<AMP> AMPs { get; private set; } = new List<AMP>();

        public ActionSetSaveMask saveMask { get; set; }


        /// <summary>
        /// Create a new <see cref="ActionSet"/> with supplied index.
        /// </summary>
        public ActionSet(byte index, Player owner)
        {
            player      = owner;
            Index       = index;
            TierPoints  = MaxTierPoints;
            AMPPoints   = MaxAMPPoints -10;
        }

        /// <summary>
        /// Add <see cref="UnlockedSpell"/> to supplied <see cref="UILocation"/>.
        /// </summary>
        public void AddSpell(UnlockedSpell spell, UILocation location, byte tier, bool init = false)
        {
            TierPoints -= TierPointCost(tier);

            if (TierPoints < 0 || TierPoints > MaxTierPoints)
                throw new InvalidOperationException();

            var itemToUpdate = Actions.FirstOrDefault(a => a.ObjectId == spell.Info.Entry.Id && a.Location == location);
            if (itemToUpdate != null)
            {
                itemToUpdate.Tier = tier;
                if ((itemToUpdate.saveMask & ActionSaveMask.Delete) != 0)
                    itemToUpdate.saveMask = ActionSaveMask.Create;
                else
                    itemToUpdate.saveMask |= ActionSaveMask.Modify;
            }
            else
                Actions.Add(new ActionSetAction(4, spell.Info.Entry.Id, location, tier, init));

            if (!init)
                saveMask |= ActionSetSaveMask.ActionSetActions;
        }

        public void UpdateSpellTier(UnlockedSpell spell, byte tier)
        {
            var itemToUpdate = Actions.FirstOrDefault(a => a.ObjectId == spell.Info.Entry.Id && (a.saveMask & ActionSaveMask.Delete) == 0);
            if (itemToUpdate == null)
                return;

            TierPoints += TierPointCost(itemToUpdate.Tier);
            TierPoints -= TierPointCost(tier);

            if (TierPoints < 0 || TierPoints > MaxTierPoints)
                throw new InvalidOperationException();

            itemToUpdate.Tier = tier;
            itemToUpdate.saveMask |= ActionSaveMask.Modify;
            saveMask |= ActionSetSaveMask.ActionSetActions;
        }

        public void RemoveSpell(UILocation location)
        {
            var itemToRemove = Actions.FirstOrDefault(a => a.Location == location && (a.saveMask & ActionSaveMask.Delete) == 0);
            if (itemToRemove == null)
                return;

            TierPoints += TierPointCost(itemToRemove.Tier);

            if (TierPoints < 0 || TierPoints > MaxTierPoints)
                throw new InvalidOperationException();

            if ((itemToRemove.saveMask &  ActionSaveMask.Create) != 0)
            {
                Actions.Remove(itemToRemove);
                return;
            }
            else
                itemToRemove.saveMask = ActionSaveMask.Delete;

            saveMask |= ActionSetSaveMask.ActionSetActions;
        }

        public byte TierPointCost(byte tier)
        {
            if (tier > MaxTier)
                throw new InvalidOperationException();
                
            byte tierPointCost = 0;
            for (byte i = 2; i <= tier; i++)
                tierPointCost += (byte)(i == 5 || i == 9 ? 5 : 1);

            return tierPointCost;
        }
        
        public ActionSetAction GetSpell(UILocation location)
        { 
            return Actions.FirstOrDefault(a => a.Location == location && (a.saveMask & ActionSaveMask.Delete) == 0);
        }

        public void AddAMP(ushort amp, bool init = false)
        {
            var itemToUpdate = AMPs.FirstOrDefault(a => a.Id == amp);
            if (itemToUpdate != null)
            {
                if ((itemToUpdate.saveMask & AMPSaveMask.Clear) != 0)
                    itemToUpdate.saveMask &= ~AMPSaveMask.Clear;
                if ((itemToUpdate.saveMask & AMPSaveMask.Delete) != 0)
                    itemToUpdate.saveMask &= ~AMPSaveMask.Delete;
            }
            else
                AMPs.Add(new AMP(amp, init));

            //TODO: consume points

            if (!init)
                saveMask |= ActionSetSaveMask.ActionSetAMPs;
        }

        public void ClearAMPs()
        {
            foreach (var amp in AMPs.ToList())
                amp.saveMask |= AMPSaveMask.Clear;

            //TODO: yield points
            saveMask |= ActionSetSaveMask.ActionSetAMPs;
        }

        public void RemoveAMP(ushort amp)
        {
            var itemToRemove = AMPs.FirstOrDefault(a => a.Id == amp);
            if (itemToRemove == null)
                return;

            if (itemToRemove.saveMask == AMPSaveMask.Create)
            {
                AMPs.Remove(itemToRemove);
                return;
            }

            itemToRemove.saveMask = AMPSaveMask.Delete;
            //TODO: yield points
            saveMask |= ActionSetSaveMask.ActionSetAMPs;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ActionSetSaveMask.None)
                return;

            if ((saveMask & ActionSetSaveMask.ActionSetAMPs) != 0)
            {
                foreach (var amp in AMPs.ToList())
                {
                    if (amp.saveMask == AMPSaveMask.None)
                        continue;

                    var model = new CharacterAMP
                    {
                        Id = player.CharacterId,
                        SpecIndex = Index,
                        AMPId = amp.Id
                    };

                    if ((amp.saveMask & AMPSaveMask.Create) != 0 && (amp.saveMask & AMPSaveMask.Clear) == 0)
                    {
                        context.Add(model);
                        amp.saveMask = AMPSaveMask.None;
                    }
                    else if ((amp.saveMask & AMPSaveMask.Delete) != 0 || ((amp.saveMask & AMPSaveMask.Clear) != 0 && (amp.saveMask & AMPSaveMask.Create) == 0))
                    {
                        context.Entry(model).State = EntityState.Deleted;
                        AMPs.Remove(amp);
                    }
                }
            }

            if ((saveMask & ActionSetSaveMask.ActionSetActions) != 0)
            {
                foreach (var action in Actions.ToList())
                {
                    if (action.saveMask == ActionSaveMask.None)
                        continue;

                    var model = new CharacterAction
                    {
                        Id = player.CharacterId,
                        SpecIndex = Index,
                        Location = (ushort)action.Location,
                        Action = action.ObjectId
                    };

                    if ((action.saveMask & ActionSaveMask.Create) != 0)
                    {
                        model.TierIndex = action.Tier;
                        context.Add(model);
                        action.saveMask = ActionSaveMask.None;
                    }
                    else if ((action.saveMask & ActionSaveMask.Delete) != 0)
                    {
                        context.Entry(model).State = EntityState.Deleted;
                        Actions.Remove(action);
                    }
                    else if ((action.saveMask & ActionSaveMask.Modify) != 0)
                    {
                        EntityEntry<CharacterAction> entity = context.Attach(model);
                        model.TierIndex = action.Tier;
                        entity.Property(p => p.TierIndex).IsModified = true;
                        action.saveMask = ActionSaveMask.None;
                    }
                }
            }

            saveMask = ActionSetSaveMask.None;
        }

        public IEnumerator<ActionSetAction> GetEnumerator()
        {
            return Actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
