using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Entity
{
    public class CostumeManager : ICostumeManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public byte CostumeCap => (byte)(player.Account.RewardPropertyManager.GetRewardProperty(RewardPropertyType.CostumeSlots).GetValue(0) ?? 4u);

        public sbyte CostumeIndex
        {
            get => costumeIndex;
            set
            {
                costumeIndex = value;
                isDirty = true;
            }
        }
        private sbyte costumeIndex;

        private bool isDirty;

        // hard limit, array storing costumes at client is 12 in size 
        private const byte MaxCostumes = 12;
        private const double CostumeSwapCooldown = 15d;

        private readonly IPlayer player;
        private readonly Dictionary<byte, ICostume> costumes = new();
        private double costumeSwapCooldown;

        /// <summary>
        /// Create a new <see cref="ICostumeManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public CostumeManager(IPlayer owner, CharacterModel characterModel)
        {
            player = owner;

            costumeIndex = characterModel.ActiveCostumeIndex;

            foreach (CharacterCostumeModel costumeModel in characterModel.Costume)
                costumes.Add(costumeModel.Index, new Costume(costumeModel));
        }

        public void Save(CharacterContext context)
        {
            foreach (ICostume costume in costumes.Values)
                costume.Save(context);

            if (isDirty)
            {
                // character is attached in Player::Save, this will only be local lookup
                CharacterModel character = context.Character.Find(player.CharacterId);
                EntityEntry<CharacterModel> entity = context.Entry(character);

                character.ActiveCostumeIndex = CostumeIndex;
                entity.Property(p => p.ActiveCostumeIndex).IsModified = true;

                isDirty = false;
            }
        }

        public void Update(double lastTick)
        {
            if (costumeSwapCooldown == 0d)
                return;

            costumeSwapCooldown -= lastTick;
            if (costumeSwapCooldown < 0d)
            {
                costumeSwapCooldown = 0d;
                log.Trace("Costume change cooldown has reset!");
            }
        }

        /// <summary>
        /// Return <see cref="ICostume"/> at supplied index.
        /// </summary>
        public ICostume GetCostume(byte index)
        {
            costumes.TryGetValue(index, out ICostume costume);
            return costume;
        }

        /// <summary>
        /// Validate then save or update <see cref="ICostume"/> from <see cref="ClientCostumeSave"/> packet.
        /// </summary>
        public void SaveCostume(ClientCostumeSave costumeSave)
        {
            // TODO: used for housing mannequins
            if (costumeSave.MannequinIndex != 0)
                throw new NotImplementedException();

            if (costumeSave.Index < 0 || costumeSave.Index >= MaxCostumes)
            {
                SendCostumeSaveResult(CostumeSaveResult.InvalidCostumeIndex);
                return;
            }

            if (costumeSave.Index >= CostumeCap)
            {
                SendCostumeSaveResult(CostumeSaveResult.CostumeIndexNotUnlocked);
                return;
            }

            foreach (ClientCostumeSave.CostumeItem costumeItem in costumeSave.Items)
            {
                if (costumeItem.ItemId == 0)
                    continue;

                Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(costumeItem.ItemId);
                if (itemEntry == null)
                {
                    SendCostumeSaveResult(CostumeSaveResult.InvalidItem);
                    return;
                }

                // TODO: check item family
                /*if ()
                {
                    SendCostumeSaveResult(CostumeSaveResult.UnusableItem);
                    return;
                }*/

                if (!player.Account.CostumeManager.HasItemUnlock(costumeItem.ItemId))
                {
                    SendCostumeSaveResult(CostumeSaveResult.ItemNotUnlocked);
                    return;
                }

                ItemDisplayEntry itemDisplayEntry = GameTableManager.Instance.ItemDisplay.GetEntry(Item.GetDisplayId(itemEntry));
                for (int i = 0; i < costumeItem.Dyes.Length; i++)
                {
                    if (costumeItem.Dyes[i] == 0u)
                        continue;

                    if (itemDisplayEntry == null)
                    {
                        SendCostumeSaveResult(CostumeSaveResult.InvalidDye);
                        return;
                    }

                    uint dyeChannelFlag = 1u << i;
                    if ((itemDisplayEntry.DyeChannelFlags & dyeChannelFlag) == 0)
                    {
                        SendCostumeSaveResult(CostumeSaveResult.InvalidDye);
                        return;
                    }

                    if (!player.Account.GenericUnlockManager.IsDyeUnlocked(costumeItem.Dyes[i]))
                    {
                        SendCostumeSaveResult(CostumeSaveResult.DyeNotUnlocked);
                        return;
                    }
                }
            }

            // TODO: charge player

            if (costumes.TryGetValue((byte)costumeSave.Index, out ICostume costume))
                costume.Update(costumeSave);
            else
            {
                costume = new Costume(player, costumeSave);
                costumes.Add(costume.Index, costume);
            }

            if (costumeSave.Index == CostumeIndex)
                player.Inventory.VisualUpdate(costume);

            SendCostume(costume);
            SendCostumeSaveResult(CostumeSaveResult.Saved, costumeSave.Index, costumeSave.MannequinIndex);
        }

        /// <summary>
        /// Equip <see cref="ICostume"/> at supplied index.
        /// </summary>
        public void SetCostume(int index)
        {
            // TODO: some packet to respond? client starts timer and sets index before sending packet so maybe not?
            if (index < -1 || index >= MaxCostumes)
                throw new ArgumentOutOfRangeException();

            if (index >= CostumeCap)
                throw new ArgumentOutOfRangeException();

            // if costume is null appearance will be returned to default
            costumes.TryGetValue((byte)index, out ICostume costume);

            if (costumeSwapCooldown > 0d)
                throw new InvalidOperationException();

            SetCostume((sbyte)index, costume);
        }

        private void SetCostume(sbyte index, ICostume costume)
        {
            player.Inventory.VisualUpdate(costume);
            CostumeIndex = index;

            // 15 second cooldown for changing costumes, hardcoded in binary
            costumeSwapCooldown = CostumeSwapCooldown;

            log.Trace($"Set costume to index {index}");
        }

        public void SendInitialPackets()
        {
            player.Session.EnqueueMessageEncrypted(new ServerCostumeList
            {
                Costumes = costumes.Values.Select(c => c.Build()).ToList()
            });
        }

        /// <summary>
        /// Send <see cref="ServerCostume"/> with supplied <see cref="ICostume"/>.
        /// </summary>
        private void SendCostume(ICostume costume)
        {
            player.Session.EnqueueMessageEncrypted(new ServerCostume
            {
                Costume = costume.Build()
            });
        }

        /// <summary>
        /// Send <see cref="ServerCostumeSave"/> with supplied <see cref="CostumeSaveResult"/> and optional index and mannequin index.
        /// </summary>
        private void SendCostumeSaveResult(CostumeSaveResult result, int index = 0, byte mannequinIndex = 0)
        {
            player.Session.EnqueueMessageEncrypted(new ServerCostumeSave
            {
                Index          = index,
                Result         = result,
                MannequinIndex = mannequinIndex
            });
        }
    }
}
