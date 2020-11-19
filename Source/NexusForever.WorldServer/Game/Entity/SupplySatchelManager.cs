using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class SupplySatchelManager: IEnumerable<TradeskillMaterial>, ISaveCharacter
    {
        private readonly Player player;
        private readonly uint maximumStackAmount = 100;
        private readonly Dictionary</* materialId */ushort, TradeskillMaterial> tradeskillMaterials = new Dictionary<ushort, TradeskillMaterial>();

        public SupplySatchelManager(Player owner, CharacterModel model)
        {
            player = owner;
            maximumStackAmount = (uint)(owner.Session.EntitlementManager.GetRewardProperty(RewardPropertyType.TradeskillMatStackLimit)?.GetValue(0u) ?? 0f);

            foreach (CharacterTradeskillMaterialModel tradeskillMaterial in model.TradeskillMaterials)
                tradeskillMaterials.Add(tradeskillMaterial.MaterialId, new TradeskillMaterial(tradeskillMaterial));
        }

        public void Save(CharacterContext context)
        {
            foreach (TradeskillMaterial material in tradeskillMaterials.Values)
                material.Save(context);
        }

        /// <summary>
        /// Builds the Network Packet for <see cref="ServerPlayerCreate"/>
        /// </summary>
        public ushort[] BuildNetworkPacket()
        {
            ushort[] tradeskillMaterialsCount = new ushort[512];

            foreach ((ushort materialId, TradeskillMaterial material) in tradeskillMaterials)
                tradeskillMaterialsCount[materialId] = material.Amount;

            return tradeskillMaterialsCount;
        }

        /// <summary>
        /// Add the provided amount to the <see cref="TradeskillMaterial"/> that is associated with the provided <see cref="Item"/>
        /// </summary>
        /// <returns>Returns the remainder that could not be added.</returns>
        public uint AddAmount(Item item, uint amount)
        {
            if (amount == 0)
                throw new ArgumentException("amount must be more than 0.");

            TradeskillMaterialEntry entry = GameTableManager.Instance.TradeskillMaterial.Entries.SingleOrDefault(i => i.Item2IdStatRevolution == item.Entry.Id);
            if (entry == null)
                throw new InvalidOperationException("TradeskillMaterialEntry does not exist.");

            ushort materialId = (ushort)entry.Id;
            uint amountAdded = 0;
            if (tradeskillMaterials.ContainsKey(materialId))
                amountAdded = AddAmountToMaterial(materialId, amount);
            else
            {
                tradeskillMaterials.Add(materialId, new TradeskillMaterial(player.CharacterId, materialId));
                amountAdded = AddAmountToMaterial(materialId, amount);
            }

            return amountAdded;
        }

        /// <summary>
        /// Add the provided amount to the <see cref="TradeskillMaterial"/> for the given Material ID
        /// </summary>
        /// <returns>Returns the remainder that could not be added.</returns>
        public uint AddAmount(ushort materialId, uint amount)
        {
            if (amount == 0)
                throw new ArgumentException("amount must be more than 0");

            TradeskillMaterialEntry entry = GameTableManager.Instance.TradeskillMaterial.GetEntry(materialId);
            if (entry == null || entry.Item2IdStatRevolution == 0)
                throw new InvalidOperationException("TradeskillMaterialEntry does not exist.");

            uint amountAdded;
            if (tradeskillMaterials.ContainsKey(materialId))
                amountAdded = AddAmountToMaterial(materialId, amount);
            else
            {
                tradeskillMaterials.Add(materialId, new TradeskillMaterial(player.CharacterId, materialId));
                amountAdded = AddAmountToMaterial(materialId, amount);
            }

            return amountAdded;
        }

        private uint AddAmountToMaterial(ushort materialId, uint amount)
        {
            if (!tradeskillMaterials.TryGetValue(materialId, out TradeskillMaterial material))
                throw new InvalidOperationException("Material not found in cache.");

            uint amountAllowed = maximumStackAmount - material.Amount;
            if (amountAllowed >= amount)
            {
                material.Amount += (ushort)amount;
                SendSupplySatchelStackUpdate(materialId);
                return 0;
            }            
            
            material.Amount = (ushort)maximumStackAmount;
            SendSupplySatchelStackUpdate(materialId);
            return amount - amountAllowed;
        }

        /// <summary>
        /// Remove the provided amount to the <see cref="TradeskillMaterial"/> that is associated with the provided <see cref="Item"/>
        /// </summary>
        public void RemoveAmount(Item item, uint amount)
        {
            TradeskillMaterialEntry entry = GameTableManager.Instance.TradeskillMaterial.Entries.SingleOrDefault(i => i.Item2IdStatRevolution == item.Entry.Id);
            if (entry == null)
                throw new InvalidOperationException("TradeskillMaterialEntry does not exist.");

            ushort materialId = (ushort)entry.Id;
            if (tradeskillMaterials.ContainsKey(materialId))
            {
                if (amount > tradeskillMaterials[materialId].Amount)
                    return; // Swallow the issue for now.
            }
            else
                tradeskillMaterials.Add(materialId, new TradeskillMaterial(player.CharacterId, materialId));

            RemoveAmount(materialId, amount);
        }

        private void RemoveAmount(ushort materialId, uint amount)
        {
            if (amount > tradeskillMaterials[materialId].Amount)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (tradeskillMaterials[materialId].Amount >= amount)
                tradeskillMaterials[materialId].Amount -= (ushort)amount;

            SendSupplySatchelStackUpdate(materialId);
        }

        /// <summary>
        /// Moves the given amount of the given material to the player's <see cref="Inventory"/>
        /// </summary>
        public void MoveToInventory(ushort materialId, uint amount)
        {
            if (amount > tradeskillMaterials[materialId].Amount)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (tradeskillMaterials.TryGetValue(materialId, out TradeskillMaterial material))
            {
                // Remove the amount first. The Inventory will replace what it couldn't create items for.
                RemoveAmount(materialId, amount);
                player.Inventory.ItemCreate(material.Entry.Item2IdStatRevolution, amount, Static.ItemUpdateReason.ResourceConversion);
            }
                
        }

        /// <summary>
        /// Returns whether the <see cref="TradeskillMaterial"/> associated with the given <see cref="Item"/> is currently at its maximum amount
        /// </summary>
        public bool IsFull(Item item)
        {
            TradeskillMaterialEntry entry = GameTableManager.Instance.TradeskillMaterial.Entries.SingleOrDefault(i => i.Item2IdStatRevolution == item.Entry.Id);
            if (entry == null)
                throw new InvalidOperationException("TradeskillMaterialEntry does not exist.");

            return IsFull((ushort)entry.Id);
        }

        private bool IsFull(ushort materialId)
        {
            if (tradeskillMaterials.TryGetValue(materialId, out TradeskillMaterial material))
                return material.Amount >= maximumStackAmount;
            else
                return false;
        }

        /// <summary>
        /// Returns whether there is enough of the given material
        /// </summary>
        public bool CanAfford(ushort materialId, uint amount)
        {
            if (tradeskillMaterials.TryGetValue(materialId, out TradeskillMaterial material))
                return amount <= material.Amount;
            else
                return false;
        }

        private void SendSupplySatchelStackUpdate(ushort materialId)
        {
            player.Session.EnqueueMessageEncrypted(new ServerSupplySatchelUpdate
            {
                MaterialId = materialId,
                StackCount = tradeskillMaterials[materialId].Amount
            });
        }

        public IEnumerator<TradeskillMaterial> GetEnumerator()
        {
            return tradeskillMaterials.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
