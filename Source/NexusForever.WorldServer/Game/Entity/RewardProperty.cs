using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.Entity
{
    public class RewardProperty : IBuildable<ServerRewardPropertySet.RewardProperty>
    {
        public RewardPropertyType Type { get; }
        public uint Data { get; }
        public float Value { get; private set; }

        private byte dataType;

        /// <summary>
        /// Create a <see cref="RewardProperty"/> with the provided <see cref="RewardPropertyPremiumModifierEntry"/>
        /// </summary>
        public RewardProperty(RewardPropertyPremiumModifierEntry entry)
        {
            Type = (RewardPropertyType)entry.RewardPropertyId;
            Data = entry.RewardPropertyData;
            dataType = (byte)(entry.RewardPropertyData > 0 ? 0 : 1);
        }

        /// <summary>
        /// Adds data from the provided <see cref="RewardPropertyPremiumModifierEntry"/> to this <see cref="RewardProperty"/>. Should only be used when loading in to world.
        /// </summary>
        public void AddValue(RewardPropertyPremiumModifierEntry entry, EntitlementManager manager)
        {
            if (entry.ModifierValueInt > 0)
                Value += entry.ModifierValueInt;

            if (entry.ModifierValueFloat > 0 && entry.EntitlementIdModifierCount == 0)
            {
                Value += entry.ModifierValueFloat;

                if (dataType != 0u)
                    dataType = 2;
            }

            if (entry.EntitlementIdModifierCount > 0)
            {
                Value += manager.GetAccountEntitlement((EntitlementType)entry.EntitlementIdModifierCount)?.Amount ?? 0u;
                Value += manager.GetCharacterEntitlement((EntitlementType)entry.EntitlementIdModifierCount)?.Amount ?? 0u;

                // TODO: If the RewardProperty value is higher on Load that the Entitlement. Should we set the Entitlement to match? This is only necessary for things like Bank Slots (4 for Signature, 2 for Basic), Auction Slots, and Commodity Slots. Do we know if you subscribed, then unsubscribed, that you would keep those Bank Slots? Did they get greyed out and unusable?
            }
        }

        /// <summary>
        /// Adds to the current Value of this <see cref="RewardProperty"/>. If the <see cref="Player"/> is in World, it will also update them with the changes.
        /// </summary>
        public void AddValue(float value, Player player)
        {
            Value += value;

            if (player.IsLoading)
                return;

            SendUpdate(player.Session);
        }

        /// <summary>
        /// Sets the Value of this <see cref="RewardProperty"/>. This will overwrite all existing values.
        /// </summary>
        /// <remarks>Good for setting weekly Omnibit Caps with configuration.</remarks>
        public void SetValue(float amount)
        {
            Value = amount;
        }

        /// <summary>
        /// Returns a <see cref="ServerRewardPropertySet.RewardProperty"/> with the data from this <see cref="RewardProperty"/>.
        /// </summary>
        public ServerRewardPropertySet.RewardProperty Build()
        {
            return new ServerRewardPropertySet.RewardProperty
            {
                Id = Type,
                Data = Data,
                Type = dataType,
                Value = Value,
            };
        }

        /// <summary>
        /// Sends <see cref="ServerRewardPropertySet"/> to the <see cref="WorldSession"/> with data from this <see cref="RewardProperty"/>.
        /// </summary>
        private void SendUpdate(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerRewardPropertySet
            {
                Variables = new List<ServerRewardPropertySet.RewardProperty>
                {
                    Build()
                }
            });
        }
    }
}
