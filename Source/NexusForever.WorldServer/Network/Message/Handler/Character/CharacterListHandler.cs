using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game.Events;

namespace NexusForever.WorldServer.Network.Message.Handler.Character
{
    public class CharacterListHandler : IMessageHandler<IWorldSession, ClientCharacterList>
    {
        #region Dependency Injection

        private ILogger<CharacterListHandler> log;

        private readonly IDatabaseManager databaseManager;
        private readonly IRealmContext realmContext;
        private readonly ILoginQueueManager loginQueueManager;

        public CharacterListHandler(
            ILogger<CharacterListHandler> log,
            IDatabaseManager databaseManager,
            IRealmContext realmContext,
            ILoginQueueManager loginQueueManager)
        {
            this.log               = log;
            this.databaseManager   = databaseManager;
            this.realmContext      = realmContext;
            this.loginQueueManager = loginQueueManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientCharacterList _)
        {
            // only handle session in queue once
            // TODO: might need to move this as HandleCharacterList is called multiple times
            if (!session.IsQueued.HasValue)
                loginQueueManager.OnNewSession(session);

            if (session.IsQueued == false)
                SendCharacterListPackets(session);
        }

        public void SendCharacterListPackets(IWorldSession session)
        {
            session.Events.EnqueueEvent(new TaskGenericEvent<List<CharacterModel>>(databaseManager.GetDatabase<CharacterDatabase>().GetCharacters(session.Account.Id),
                characters =>
            {
                byte maxCharacterLevelAchieved = 1;

                session.Characters.Clear();
                session.Characters.AddRange(characters);

                session.Account.CurrencyManager.SendCharacterListPacket();
                session.Account.GenericUnlockManager.SendUnlockList();

                session.EnqueueMessageEncrypted(new ServerAccountEntitlements
                {
                    Entitlements = session.Account.EntitlementManager
                        .Select(e => new ServerAccountEntitlements.AccountEntitlementInfo
                        {
                            Entitlement = e.Type,
                            Count       = e.Amount
                        })
                        .ToList()
                });

                session.EnqueueMessageEncrypted(new ServerAccountTier
                {
                    Tier = session.Account.AccountTier
                });

                // 2 is just a fail safe for the minimum amount of character slots
                // this is set in the tbl files so a value should always exist
                uint characterSlots = (uint)(session.Account.RewardPropertyManager.GetRewardProperty(RewardPropertyType.CharacterSlots).GetValue(0u) ?? 2u);
                uint characterCount = (uint)characters.Count(c => c.DeleteTime == null);

                var serverCharacterList = new ServerCharacterList
                {
                    ServerTime                     = realmContext.GetServerTime(),
                    RealmId                        = realmContext.RealmId,
                    // no longer used as replaced by entitlements but retail server still used to send this
                    AdditionalCount                = characterCount,
                    AdditionalAllowedCharCreations = (uint)Math.Max(0, (int)(characterSlots - characterCount)),
                    // Free Level 50 needs(?) support. It appears to have just been a custom flag on the account that was consume when used up.
                    // FreeLevel50 = true
                };

                foreach (CharacterModel character in characters.Where(c => c.DeleteTime == null))
                {
                    var listCharacter = new ServerCharacterList.Character
                    {
                        Id                = character.Id,
                        Name              = character.Name,
                        Sex               = (Sex)character.Sex,
                        Race              = (Race)character.Race,
                        Class             = (Class)character.Class,
                        Faction           = character.FactionId,
                        Level             = character.Level,
                        WorldId           = character.WorldId,
                        WorldZoneId       = character.WorldZoneId,
                        RealmId           = realmContext.RealmId,
                        Path              = (byte)character.ActivePath,
                        LastLoggedOutDays = (float)DateTime.UtcNow.Subtract(character.LastOnline ?? DateTime.UtcNow).TotalDays * -1f
                    };

                    maxCharacterLevelAchieved = Math.Max(maxCharacterLevelAchieved, character.Level);

                    try
                    {
                        // create a temporary Inventory and CostumeManager to show equipped gear
                        var inventory      = new Inventory(null, character);
                        var costumeManager = new CostumeManager(null, character);

                        ICostume costume = null;
                        if (costumeManager.CostumeIndex.HasValue)
                            costume = costumeManager.GetCostume((byte)character.ActiveCostumeIndex);

                        listCharacter.GearMask = costume?.Mask ?? 0xFFFFFFFF;

                        Dictionary<ItemSlot, IItemVisual> costumeVisuals = costume?.GetItemVisuals().ToDictionary(c => c.Slot);
                        foreach (IItemVisual itemVisual in inventory.GetItemVisuals())
                        {
                            if (costumeVisuals != null
                                && costumeVisuals.TryGetValue(itemVisual.Slot, out IItemVisual costumeVisual)
                                && costumeVisual.DisplayId.HasValue)
                                listCharacter.Gear.Add(costumeVisual.Build());
                            else
                                listCharacter.Gear.Add(itemVisual.Build());
                        }   

                        foreach (CharacterAppearanceModel appearance in character.Appearance)
                        {
                            listCharacter.Appearance.Add(new NexusForever.Network.World.Message.Model.Shared.ItemVisual
                            {
                                Slot      = (ItemSlot)appearance.Slot,
                                DisplayId = appearance.DisplayId
                            });
                        }

                        foreach (CharacterBoneModel bone in character.Bone.OrderBy(bone => bone.BoneIndex))
                        {
                            listCharacter.Bones.Add(bone.Bone);
                        }

                        foreach (CharacterStatModel stat in character.Stat)
                        {
                            if ((Stat)stat.Stat == Stat.Level)
                            {
                                listCharacter.Level = (uint)stat.Value;
                                break;
                            }
                        }

                        serverCharacterList.Characters.Add(listCharacter);
                    }
                    catch (Exception ex)
                    {
                        log.LogCritical(ex, $"An error has occured while loading character '{character.Name}'");
                        continue;
                    }
                }

                session.EnqueueMessageEncrypted(new ServerMaxCharacterLevelAchieved
                {
                    Level = maxCharacterLevelAchieved
                });

                session.EnqueueMessageEncrypted(serverCharacterList);
            }));
        }
    }
}
