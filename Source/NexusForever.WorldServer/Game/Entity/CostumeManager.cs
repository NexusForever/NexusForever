using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using NetworkCostume = NexusForever.WorldServer.Network.Message.Model.Shared.Costume;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CostumeManager : ISaveAuth, ISaveCharacter, IUpdate
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // TODO: need to research Server092C more to find a permanent home for this
        public const byte CostumeCap = 4;

        // hard limit, array storing costumes at client is 12 in size 
        private const byte MaxCostumes = 12;
        private const double CostumeSwapCooldown = 15d;

        private readonly Player player;
        private readonly Dictionary<byte, Costume> costumes = new Dictionary<byte, Costume>();
        private readonly Dictionary<uint, CostumeUnlock> costumeUnlocks = new Dictionary<uint, CostumeUnlock>();
        private double costumeSwapCooldown;

        /// <summary>
        /// Create a new <see cref="CurrencyManager"/> from existing <see cref="AccountModel"/> and <see cref="CharacterModel"/> database models.
        /// </summary>
        public CostumeManager(Player owner, AccountModel accountModel, CharacterModel characterModel)
        {
            player = owner;

            foreach (CharacterCostumeModel costumeModel in characterModel.Costume)
                costumes.Add(costumeModel.Index, new Costume(costumeModel));

            foreach (AccountCostumeUnlockModel costumeUnlockModel in accountModel.AccountCostumeUnlock)
                costumeUnlocks.Add(costumeUnlockModel.ItemId, new CostumeUnlock(costumeUnlockModel));
        }

        public void Save(AuthContext context)
        {
            foreach (CostumeUnlock costumeItem in costumeUnlocks.Values)
                costumeItem.Save(context);
        }

        public void Save(CharacterContext context)
        {
            foreach (Costume costume in costumes.Values)
                costume.Save(context);
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
        /// Return <see cref="Costume"/> at supplied index.
        /// </summary>
        public Costume GetCostume(byte index)
        {
            costumes.TryGetValue(index, out Costume costume);
            return costume;
        }

        /// <summary>
        /// Validate then save or update <see cref="Costume"/> from <see cref="ClientCostumeSave"/> packet.
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

                if (!costumeUnlocks.ContainsKey(costumeItem.ItemId))
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

                    if (!player.Session.GenericUnlockManager.IsDyeUnlocked(costumeItem.Dyes[i]))
                    {
                        SendCostumeSaveResult(CostumeSaveResult.DyeNotUnlocked);
                        return;
                    }
                }
            }

            // TODO: charge player

            if (costumes.TryGetValue((byte)costumeSave.Index, out Costume costume))
                costume.Update(costumeSave);
            else
            {
                costume = new Costume(player, costumeSave);
                costumes.Add(costume.Index, costume);
            }

            SetCostume((sbyte)costumeSave.Index, costume);

            SendCostume(costume);
            SendCostumeSaveResult(CostumeSaveResult.Saved, costumeSave.Index, costumeSave.MannequinIndex);
        }

        /// <summary>
        /// Equip <see cref="Costume"/> at supplied index.
        /// </summary>
        public void SetCostume(int index)
        {
            // TODO: some packet to respond? client starts timer and sets index before sending packet so maybe not?
            if (index < -1 || index >= MaxCostumes)
                throw new ArgumentOutOfRangeException();

            if (index >= CostumeCap)
                throw new ArgumentOutOfRangeException();

            // if costume is null appearance will be returned to default
            costumes.TryGetValue((byte)index, out Costume costume);

            if (costumeSwapCooldown > 0d)
                throw new InvalidOperationException();

            SetCostume((sbyte)index, costume);
        }

        private void SetCostume(sbyte index, Costume costume)
        {
            player.Inventory.VisualUpdate(costume);
            player.CostumeIndex = index;

            // 15 second cooldown for changing costumes, hardcoded in binary
            costumeSwapCooldown = CostumeSwapCooldown;

            log.Trace($"Set costume to index {index}");
        }

        /// <summary>
        /// Unlock costume item for <see cref="Item"/> at supplied <see cref="ItemLocation"/>.
        /// </summary>
        public void UnlockItem(ItemLocation location)
        {
            Item item = player.Inventory.GetItem(location);
            if (item == null)
            {
                SendCostumeItemUnlock(CostumeUnlockResult.InvalidItem);
                return;
            }

            if (costumeUnlocks.TryGetValue(item.Id, out CostumeUnlock costumeUnlock) && !costumeUnlock.PendingDelete)
            {
                SendCostumeItemUnlock(CostumeUnlockResult.AlreadyKnown);
                return;
            }

            if (costumeUnlocks.Count >= GetMaxUnlockItemCount())
            {
                SendCostumeItemUnlock(CostumeUnlockResult.OutOfSpace);
                return;
            }

            // TODO: make item soulbound

            if (costumeUnlock != null)
                costumeUnlock.EnqueueDelete(false);
            else
                costumeUnlocks.Add(item.Id, new CostumeUnlock(player.Session.Account, item.Id));
            
            SendCostumeItemUnlock(CostumeUnlockResult.UnlockSuccess, item.Id);
        }

        private uint GetMaxUnlockItemCount()
        {
            // client defaults to 1000 if entry doesn't exist
            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1203);
            if (entry == null)
                return 1000u;

            return entry.Dataint0/* + countFromEntitlements*/;
        }

        /// <summary>
        /// Forget costume item unlock of supplied item id.
        /// </summary>
        public void ForgetItem(uint itemId)
        {
            Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(itemId);
            if (itemEntry == null)
            {
                SendCostumeItemUnlock(CostumeUnlockResult.InvalidItem);
                return;
            }

            if (!costumeUnlocks.TryGetValue(itemId, out CostumeUnlock costumeUnlock))
            {
                SendCostumeItemUnlock(CostumeUnlockResult.ForgetItemFailed);
                return;
            }

            costumeUnlock.EnqueueDelete(true);
            SendCostumeItemUnlock(CostumeUnlockResult.ForgetItemSuccess, itemId);
        }

        public void SendInitialPackets()
        {
            player.Session.EnqueueMessageEncrypted(new ServerCostumeItemList
            {
                Items = costumeUnlocks.Keys.ToList()
            });

            player.Session.EnqueueMessageEncrypted(new ServerCostumeList
            {
                Costumes = costumes.Values.Select(BuildNetworkCostume).ToList()
            });
        }

        /// <summary>
        /// Send <see cref="ServerCostume"/> with supplied <see cref="Costume"/>.
        /// </summary>
        private void SendCostume(Costume costume)
        {
            player.Session.EnqueueMessageEncrypted(new ServerCostume
            {
                Costume = BuildNetworkCostume(costume)
            });
        }

        private NetworkCostume BuildNetworkCostume(Costume costume)
        {
            var networkCostume = new NetworkCostume
            {
                Index = costume.Index,
                Mask  = costume.Mask
            };

            foreach (CostumeItem costumeItem in costume)
            {
                networkCostume.ItemIds[(byte)costumeItem.Slot] = costumeItem.ItemId;
                networkCostume.DyeData[(byte)costumeItem.Slot] = costumeItem.DyeData;
            }

            return networkCostume;
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

        /// <summary>
        /// Send <see cref="ServerCostumeItemUnlock"/> will supplied <see cref="CostumeUnlockResult"/> and optional item id.
        /// </summary>
        private void SendCostumeItemUnlock(CostumeUnlockResult result, uint itemId = 0u)
        {
            var costumeItemUnlock = new ServerCostumeItemUnlock
            {
                Result = result
            };

            if (itemId != 0u)
                costumeItemUnlock.ItemId = itemId;

            player.Session.EnqueueMessageEncrypted(costumeItemUnlock);
        }
    }
}
