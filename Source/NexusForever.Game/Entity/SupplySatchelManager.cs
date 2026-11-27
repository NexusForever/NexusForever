using System.Collections;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reward;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Entity
{
    public class SupplySatchelManager : ISupplySatchelManager
    {
        private const int MaterialSlotCount = 512;
        private const uint DefaultStackLimit = 100;

        private readonly IPlayer player;
        private readonly uint maximumStackAmount;
        private readonly Dictionary</* materialId */ushort, ITradeskillMaterial> tradeskillMaterials = new();

        public SupplySatchelManager(IPlayer owner, CharacterModel model)
        {
            player = owner;
            float configuredLimit = owner.Account.RewardPropertyManager.GetRewardProperty(RewardPropertyType.TradeskillMatStackLimit)
                ?.GetValue(0u) ?? DefaultStackLimit;
            maximumStackAmount = float.IsFinite(configuredLimit)
                ? (uint)Math.Clamp(configuredLimit, 0f, ushort.MaxValue)
                : DefaultStackLimit;

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
            ushort[] tradeskillMaterialsCount = new ushort[MaterialSlotCount];

            foreach ((ushort materialId, ITradeskillMaterial material) in tradeskillMaterials)
                if (materialId < tradeskillMaterialsCount.Length)
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
            TradeskillMaterialEntry entry = FindMaterial(item);
            return entry == null ? amount : AddAmount((ushort)entry.Id, amount);
        }

        /// <summary>
        /// Adds material up to its current stack limit and returns any amount that did not fit.
        /// </summary>
        public uint AddAmount(ushort materialId, uint amount)
        {
            if (amount == 0u || maximumStackAmount == 0u || materialId >= MaterialSlotCount
                || GameTableManager.Instance.TradeskillMaterial.GetEntry(materialId) is not { Item2IdStatRevolution: > 0u })
                return amount;

            if (!tradeskillMaterials.TryGetValue(materialId, out ITradeskillMaterial material))
            {
                material = new TradeskillMaterial(player.CharacterId, materialId);
                tradeskillMaterials.Add(materialId, material);
            }

            // A lower account limit does not invalidate materials already in the satchel.
            if (material.Amount >= maximumStackAmount)
                return amount;

            uint added = Math.Min(amount, maximumStackAmount - material.Amount);
            material.Amount += (ushort)added;
            SendSupplySatchelStackUpdate(materialId);
            return amount - added;
        }

        public void RemoveAmount(IItem item, uint amount)
        {
            TradeskillMaterialEntry entry = FindMaterial(item);
            if (entry == null || amount == 0u || !tradeskillMaterials.TryGetValue((ushort)entry.Id, out ITradeskillMaterial material)
                || material.Amount < amount)
                return;

            material.Amount -= (ushort)amount;
            SendSupplySatchelStackUpdate((ushort)entry.Id);
        }

        /// <summary>
        /// Moves the given material to the inventory and keeps any remainder in the satchel.
        /// </summary>
        public void MoveToInventory(ushort materialId, uint amount)
        {
            if (amount == 0u || materialId >= MaterialSlotCount || !tradeskillMaterials.TryGetValue(materialId, out ITradeskillMaterial material)
                || material.Amount < amount || material.Entry is not { Item2IdStatRevolution: > 0u } entry)
                return;

            IItemInfo item = ItemManager.Instance.GetItemInfo(entry.Item2IdStatRevolution);
            if (item == null)
                return;

            uint remaining = player.Inventory.ItemCreateAvailable(InventoryLocation.Inventory, item, amount, ItemUpdateReason.ResourceConversion);
            uint moved = amount - remaining;
            if (moved == 0u)
            {
                player.SendGenericError(GenericError.ItemInventoryFull);
                return;
            }

            material.Amount -= (ushort)moved;
            SendSupplySatchelStackUpdate(materialId);
        }

        public bool IsFull(IItem item)
        {
            TradeskillMaterialEntry entry = FindMaterial(item);
            return entry != null && (maximumStackAmount == 0u
                || (tradeskillMaterials.TryGetValue((ushort)entry.Id, out ITradeskillMaterial material) && material.Amount >= maximumStackAmount));
        }

        private static TradeskillMaterialEntry FindMaterial(IItem item)
        {
            if (item?.Info?.Entry is not { Id: > 0u } entry)
                return null;

            return GameTableManager.Instance.TradeskillMaterial.Entries.FirstOrDefault(
                material => material.Id < MaterialSlotCount && material.Item2IdStatRevolution == entry.Id);
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
