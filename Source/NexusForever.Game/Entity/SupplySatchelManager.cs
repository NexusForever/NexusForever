using System.Collections;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Entity
{
    public class SupplySatchelManager : ISupplySatchelManager
    {
        private readonly IPlayer player;
        private readonly uint maximumStackAmount = 100;
        private readonly Dictionary</* materialId */ushort, ITradeskillMaterial> tradeskillMaterials = new();

        public SupplySatchelManager(IPlayer owner, CharacterModel model)
        {
            player = owner;
            maximumStackAmount = (uint)(owner.Account.RewardPropertyManager.GetRewardProperty(RewardPropertyType.TradeskillMatStackLimit)?.GetValue(0u) ?? 0f);

            foreach (CharacterTradeskillMaterialModel tradeskillMaterial in model.TradeskillMaterials)
                tradeskillMaterials.Add(tradeskillMaterial.MaterialId, new TradeskillMaterial(tradeskillMaterial));
        }

        public void Save(CharacterContext context)
        {
            foreach (ITradeskillMaterial material in tradeskillMaterials.Values)
                material.Save(context);
        }

        public ushort[] BuildNetworkPacket()
        {
            ushort[] tradeskillMaterialsCount = new ushort[512];

            foreach ((ushort materialId, ITradeskillMaterial material) in tradeskillMaterials)
                tradeskillMaterialsCount[materialId] = material.Amount;

            return tradeskillMaterialsCount;
        }

        /// <summary>
        /// Add the provided amount to the <see cref="ITradeskillMaterial"/> that is associated with the provided <see cref="IItem"/>
        /// </summary>
        /// <returns>
        /// Returns the remainder that could not be added.
        /// </returns>
        public uint AddAmount(IItem item, uint amount)
        {
            if (amount == 0)
                throw new ArgumentException("amount must be more than 0.");

            TradeskillMaterialEntry entry = GameTableManager.Instance.TradeskillMaterial.Entries.SingleOrDefault(i => i.Item2IdStatRevolution == item.Info.Entry.Id);
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
        /// Add the provided amount to the <see cref="ITradeskillMaterial"/> for the given Material ID
        /// </summary>
        /// <returns>
        /// Returns the remainder that could not be added.
        /// </returns>
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
            if (!tradeskillMaterials.TryGetValue(materialId, out ITradeskillMaterial material))
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
        /// Remove the provided amount to the <see cref="ITradeskillMaterial"/> that is associated with the provided <see cref="IItem"/>
        /// </summary>
        public void RemoveAmount(IItem item, uint amount)
        {
            TradeskillMaterialEntry entry = GameTableManager.Instance.TradeskillMaterial.Entries.SingleOrDefault(i => i.Item2IdStatRevolution == item.Info.Entry.Id);
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
        /// Moves the given amount of the given material to the player's <see cref="IInventory"/>
        /// </summary>
        public void MoveToInventory(ushort materialId, uint amount)
        {
            if (amount > tradeskillMaterials[materialId].Amount)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (tradeskillMaterials.TryGetValue(materialId, out ITradeskillMaterial material))
            {
                // Remove the amount first. The Inventory will replace what it couldn't create items for.
                RemoveAmount(materialId, amount);
                player.Inventory.ItemCreate(InventoryLocation.Inventory, material.Entry.Item2IdStatRevolution, amount, ItemUpdateReason.ResourceConversion);
            }
        }

        /// <summary>
        /// Returns whether the <see cref="ITradeskillMaterial"/> associated with the given <see cref="IItem"/> is currently at its maximum amount
        /// </summary>
        public bool IsFull(IItem item)
        {
            TradeskillMaterialEntry entry = GameTableManager.Instance.TradeskillMaterial.Entries.SingleOrDefault(i => i.Item2IdStatRevolution == item.Info.Entry.Id);
            if (entry == null)
                throw new InvalidOperationException("TradeskillMaterialEntry does not exist.");

            return IsFull((ushort)entry.Id);
        }

        private bool IsFull(ushort materialId)
        {
            if (tradeskillMaterials.TryGetValue(materialId, out ITradeskillMaterial material))
                return material.Amount >= maximumStackAmount;
            else
                return false;
        }

        /// <summary>
        /// Returns whether there is enough of the given material
        /// </summary>
        public bool CanAfford(ushort materialId, uint amount)
        {
            if (tradeskillMaterials.TryGetValue(materialId, out ITradeskillMaterial material))
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

        public IEnumerator<ITradeskillMaterial> GetEnumerator()
        {
            return tradeskillMaterials.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
