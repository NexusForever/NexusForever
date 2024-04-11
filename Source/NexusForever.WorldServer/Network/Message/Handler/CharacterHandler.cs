using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Cryptography;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Server;
using NexusForever.Game.Character;
using NexusForever.Game.Customisation;
using NexusForever.Game.Entity;
using NexusForever.Game.Guild;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Server;
using NexusForever.Game.Spell;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Map;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;
using NexusForever.Shared.Game.Events;
using NLog;
using NetworkMessage = NexusForever.Network.Message.Model.Shared.Message;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CharacterHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientRealmList)]
        public static void HandleRealmList(IWorldSession session, ClientRealmList realmList)
        {
            var serverRealmList = new ServerRealmList();

            foreach (ServerInfo server in ServerManager.Instance.Servers)
            {
                RealmStatus status = RealmStatus.Up;
                if (!server.IsOnline && server.Model.Id != RealmContext.Instance.RealmId)
                    status = RealmStatus.Down;

                serverRealmList.Realms.Add(new ServerRealmList.RealmInfo
                {
                    RealmId          = server.Model.Id,
                    RealmName        = server.Model.Name,
                    Type             = (RealmType)server.Model.Type,
                    Status           = status,
                    Population       = RealmPopulation.Low,
                    Unknown8         = new byte[16],
                    AccountRealmInfo = new ServerRealmList.RealmInfo.AccountRealmData
                    {
                        RealmId = server.Model.Id
                    }
                });
            }

            serverRealmList.Messages = ServerManager.Instance.ServerMessages
                .Select(m => new NetworkMessage
                {
                    Index    = m.Index,
                    Messages = m.Messages
                })
                .ToList();

            session.EnqueueMessageEncrypted(serverRealmList);
        }

        [MessageHandler(GameMessageOpcode.ClientSelectRealm)]
        public static void HandleSelectRealm(IWorldSession session, ClientSelectRealm selectRealm)
        {
            IServerInfo server = ServerManager.Instance.Servers.SingleOrDefault(s => s.Model.Id == selectRealm.RealmId);
            if (server == null)
                throw new InvalidPacketValueException();

            // clicking back or selecting the current realm also triggers this packet, client crashes if we don't ignore it
            if (server.Model.Id == RealmContext.Instance.RealmId)
                return;

            // TODO: Return proper error packet if server is not online
            if (!server.IsOnline)
            {
                session.EnqueueMessageEncrypted(new ServerForceKick());
                return;
            }

            byte[] sessionKey = RandomProvider.GetBytes(16u);
            session.Events.EnqueueEvent(new TaskEvent(DatabaseManager.Instance.GetDatabase<AuthDatabase>().UpdateAccountSessionKey(session.Account.Id, Convert.ToHexString(sessionKey)),
                () =>
            {
                session.EnqueueMessageEncrypted(new ServerNewRealm
                {
                    SessionKey  = sessionKey,
                    GatewayData = new ServerNewRealm.Gateway
                    {
                        Address = server.Address,
                        Port    = server.Model.Port
                    },
                    RealmName = server.Model.Name,
                    Type      = (RealmType)server.Model.Type
                });
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterList)]
        public static void HandleCharacterList(WorldSession session, ClientCharacterList _)
        {
            // only handle session in queue once
            // TODO: might need to move this as HandleCharacterList is called multiple times
            if (!session.IsQueued.HasValue)
                LoginQueueManager.Instance.OnNewSession(session);

            if (session.IsQueued == false)
                SendCharacterListPackets(session);
        }

        public static void SendCharacterListPackets(IWorldSession session)
        {
            session.Events.EnqueueEvent(new TaskGenericEvent<List<CharacterModel>>(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetCharacters(session.Account.Id),
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
                    ServerTime                     = RealmContext.Instance.GetServerTime(),
                    RealmId                        = RealmContext.Instance.RealmId,
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
                        RealmId           = RealmContext.Instance.RealmId,
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
                                Slot = (ItemSlot)appearance.Slot,
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
                        log.Fatal(ex, $"An error has occured while loading character '{character.Name}'");
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

        [MessageHandler(GameMessageOpcode.ClientCharacterCreate)]
        public static void HandleCharacterCreate(IWorldSession session, ClientCharacterCreate characterCreate)
        {
            if (session.IsQueued == true)
                throw new InvalidPacketValueException();

            CharacterModifyResult? GetResult()
            {
                // TODO: validate path
                if (!TextFilterManager.Instance.IsTextValid(characterCreate.Name)
                    || !TextFilterManager.Instance.IsTextValid(characterCreate.Name, UserText.CharacterName))
                    return CharacterModifyResult.CreateFailed_InvalidName;

                // UserText.CharacterName checks if there is a space
                foreach (string name in characterCreate.Name.Split(' '))
                    if (!TextFilterManager.Instance.IsTextValid(name, UserText.CharacterNamePart))
                        return CharacterModifyResult.CreateFailed_InvalidName;

                if (DatabaseManager.Instance.GetDatabase<CharacterDatabase>().CharacterNameExists(characterCreate.Name))
                    return CharacterModifyResult.CreateFailed_UniqueName;

                CharacterCreationEntry creationEntry = GameTableManager.Instance.CharacterCreation.GetEntry(characterCreate.CharacterCreationId);
                if (creationEntry == null)
                    return CharacterModifyResult.CreateFailed_Internal;

                if ((CharacterCreationStart)creationEntry.CharacterCreationStartEnum == CharacterCreationStart.Level50
                    && !session.Account.CurrencyManager.CanAfford(AccountCurrencyType.MaxLevelToken, 1ul))
                    return CharacterModifyResult.CreateFailed_InsufficientFunds;

                List<(uint Label, uint Value)> customisations = characterCreate.Labels
                    .Zip(characterCreate.Values, ValueTuple.Create)
                    .ToList();

                if (!CustomisationManager.Instance.Validate((Race)creationEntry.RaceId, (Sex)creationEntry.Sex, (Faction)creationEntry.FactionId, customisations))
                    return CharacterModifyResult.CreateFailed;

                return null;
            }

            try
            {
                CharacterModifyResult? result = GetResult();
                if (result.HasValue)
                {
                    session.EnqueueMessageEncrypted(new ServerCharacterCreate
                    {
                        Result = result.Value
                    });

                    return;
                }

                CharacterCreationEntry creationEntry = GameTableManager.Instance.CharacterCreation.GetEntry(characterCreate.CharacterCreationId);
                if (creationEntry == null)
                    throw new InvalidPacketValueException();

                if (creationEntry.EntitlementIdRequired != 0u)
                {
                    // TODO: Aurin engineer has this
                }

                var character = new CharacterModel
                {
                    AccountId  = session.Account.Id,
                    Id         = CharacterManager.Instance.NextCharacterId,
                    Name       = characterCreate.Name,
                    Race       = (byte)creationEntry.RaceId,
                    Sex        = (byte)creationEntry.Sex,
                    Class      = (byte)creationEntry.ClassId,
                    FactionId  = (ushort)creationEntry.FactionId,
                    ActivePath = characterCreate.Path,
                    TotalXp    = creationEntry.Xp
                };

                uint startingLevel = GameTableManager.Instance.XpPerLevel.Entries.First(l => l.MinXpForLevel >= creationEntry.Xp).Id;

                for (Path path = Path.Soldier; path <= Path.Explorer; path++)
                {
                    character.Path.Add(new CharacterPathModel
                    {
                        Path     = (byte)path,
                        Unlocked = Convert.ToByte(characterCreate.Path == (byte)path)
                    });
                }

                IList<(uint Label, uint Value)> customisations = characterCreate.Labels
                    .Zip(characterCreate.Values, ValueTuple.Create)
                    .ToList();

                foreach ((uint label, uint value) in customisations)
                {
                    character.Customisation.Add(new CharacterCustomisationModel
                    {
                        Label = label,
                        Value = value
                    });
                }

                foreach (IItemVisual visual in CustomisationManager.Instance.GetItemVisuals((Race)character.Race, (Sex)character.Sex, customisations))
                {
                    character.Appearance.Add(new CharacterAppearanceModel
                    {
                        Slot = (byte)visual.Slot,
                        DisplayId = (ushort)visual.DisplayId
                    });
                }

                for (int i = 0; i < characterCreate.Bones.Count; ++i)
                {
                    character.Bone.Add(new CharacterBoneModel
                    {
                        BoneIndex = (byte)(i),
                        Bone      = characterCreate.Bones[i]
                    });
                }

                ILocation startingLocation = CharacterManager.Instance.GetStartingLocation((Race)creationEntry.RaceId, (Faction)creationEntry.FactionId, (CharacterCreationStart)creationEntry.CharacterCreationStartEnum);
                if (startingLocation == null)
                    throw new ArgumentNullException(nameof(startingLocation));

                character.LocationX = startingLocation.Position.X;
                character.LocationY = startingLocation.Position.Y;
                character.LocationZ = startingLocation.Position.Z;
                character.WorldId = (ushort)startingLocation.World.Id;

                character.ActiveSpec = 0;

                // create initial LAS abilities
                UILocation location = 0;
                foreach (SpellLevelEntry spellLevelEntry in GameTableManager.Instance.SpellLevel.Entries
                    .Where(s => s.ClassId == character.Class && s.CharacterLevel == 1))
                {
                    Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(spellLevelEntry.Spell4Id);
                    if (spell4Entry == null)
                        continue;

                    character.Spell.Add(new CharacterSpellModel
                    {
                        Id           = character.Id,
                        Spell4BaseId = spell4Entry.Spell4BaseIdBaseSpell,
                        Tier         = 1
                    });

                    character.ActionSetShortcut.Add(new CharacterActionSetShortcutModel
                    {
                        Id           = character.Id,
                        SpecIndex    = 0,
                        Location     = (ushort)location,
                        ShortcutType = (byte)ShortcutType.Spell,
                        ObjectId     = spell4Entry.Spell4BaseIdBaseSpell,
                        Tier         = 1
                    });

                    location++;
                }

                // create a temporary inventory to create starting gear
                var inventory = new Inventory(character.Id, creationEntry);
                IEnumerable<IItem> items = inventory
                    .SelectMany(b => b)
                    .Select(i => i);

                //TODO: handle starting stats per class/race
                character.Stat.Add(new CharacterStatModel
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Health,
                    Value = 800
                });
                character.Stat.Add(new CharacterStatModel
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Dash,
                    Value = 200
                });
                character.Stat.Add(new CharacterStatModel
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Level,
                    Value = startingLevel
                });
                character.Stat.Add(new CharacterStatModel
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.StandState,
                    Value = 3
                });
                character.Stat.Add(new CharacterStatModel
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Sheathed,
                    Value = 1
                });

                // TODO: actually error check this
                session.Events.EnqueueEvent(new TaskEvent(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(c =>
                    {
                        c.Character.Add(character);
                        foreach (IItem item in items)
                            item.Save(c);
                    }),
                    () =>
                {
                    session.Characters.Add(character);
                    CharacterManager.Instance.AddCharacter(character);

                    if ((CharacterCreationStart)creationEntry.CharacterCreationStartEnum == CharacterCreationStart.Level50)
                        session.Account.CurrencyManager.CurrencySubtractAmount(AccountCurrencyType.MaxLevelToken, 1u);

                    session.EnqueueMessageEncrypted(new ServerCharacterCreate
                    {
                        CharacterId = character.Id,
                        WorldId     = character.WorldId,
                        Result      = CharacterModifyResult.CreateOk
                    });
                }));
            }
            catch
            {
                session.EnqueueMessageEncrypted(new ServerCharacterCreate
                {
                    Result = CharacterModifyResult.CreateFailed
                });

                throw;
            }
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterDelete)]
        public static void HandleCharacterDelete(IWorldSession session, ClientCharacterDelete characterDelete)
        {
            if (session.IsQueued == true)
                throw new InvalidPacketValueException();

            CharacterModel characterToDelete = session.Characters.FirstOrDefault(c => c.Id == characterDelete.CharacterId);

            (CharacterModifyResult, uint) GetResult()
            {
                if (characterToDelete == null)
                    return (CharacterModifyResult.DeleteFailed, 0);

                // TODO: Not sure if this is definitely the case, but put it in for good measure
                if (characterToDelete.Mail.Count > 0)
                {
                    foreach (CharacterMailModel characterMail in characterToDelete.Mail)
                    {
                        if (characterMail.Attachment.Count > 0)
                            return (CharacterModifyResult.DeleteFailed, 0);
                    }
                }

                uint leaderCount = (uint)GlobalGuildManager.Instance.GetCharacterGuilds(characterToDelete.Id)
                    .Count(g => g.LeaderId == characterDelete.CharacterId);
                if (leaderCount > 0)
                    return (CharacterModifyResult.DeleteFailed, leaderCount);

                return (CharacterModifyResult.DeleteOk, 0);
            }

            (CharacterModifyResult result, uint data) deleteCheck = GetResult();
            if (deleteCheck.result != CharacterModifyResult.DeleteOk)
            {
                session.EnqueueMessageEncrypted(new ServerCharacterDeleteResult
                {
                    Result = deleteCheck.result,
                    Data = deleteCheck.data
                });
                return;
            }

            session.CanProcessIncomingPackets = false;

            void Save(CharacterContext context)
            {
                var model = new CharacterModel
                {
                    Id = characterToDelete.Id
                };

                EntityEntry<CharacterModel> entity = context.Attach(model);

                model.DeleteTime = DateTime.UtcNow;
                entity.Property(e => e.DeleteTime).IsModified = true;
                
                model.OriginalName = characterToDelete.Name;
                entity.Property(e => e.OriginalName).IsModified = true;

                model.Name = null;
                entity.Property(e => e.Name).IsModified = true;
            }

            session.Events.EnqueueEvent(new TaskEvent(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(Save),
                () =>
            {
                session.CanProcessIncomingPackets = true;

                GlobalResidenceManager.Instance.RemoveResidence(characterToDelete.Name);
                CharacterManager.Instance.DeleteCharacter(characterToDelete.Id, characterToDelete.Name);

                session.EnqueueMessageEncrypted(new ServerCharacterDeleteResult
                {
                    Result = deleteCheck.result
                });
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterSelect)]
        public static void HandleCharacterSelect(IWorldSession session, ClientCharacterSelect characterSelect)
        {
            if (session.IsQueued == true)
                throw new InvalidPacketValueException();

            CharacterModel character = session.Characters.SingleOrDefault(c => c.Id == characterSelect.CharacterId);
            if (character == null)
            {
                session.EnqueueMessageEncrypted(new ServerCharacterSelectFail
                {
                    Result = CharacterSelectResult.Failed
                });
                return;
            }

            if (CleanupManager.Instance.IsAccountLocked(session.Account))
            {
                session.EnqueueMessageEncrypted(new ServerCharacterSelectFail
                {
                    Result = CharacterSelectResult.FailedCharacterInWorld
                });
                return;
            }

            // TODO: needs to be replaced once network handlers aren't static
            var factory = LegacyServiceProvider.Provider.GetService<IEntityFactory>();

            session.Player = factory.CreateEntity<IPlayer>();
            session.Player.Initialise(session, session.Account, character);

            WorldEntry entry = GameTableManager.Instance.World.GetEntry(character.WorldId);
            if (entry == null)
                throw new ArgumentOutOfRangeException();

            switch ((MapType)entry.Type)
            {
                case MapType.Residence:
                case MapType.Community:
                {
                    // characters logging in to a housing map are returned to their own residence
                    IResidence residence = session.Player.ResidenceManager.Residence;
                    residence ??= GlobalResidenceManager.Instance.CreateResidence(session.Player);

                    IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
                    session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
                    MapManager.Instance.AddToMap(session.Player, new MapPosition
                    {
                        Info     = new MapInfo
                        {
                            Entry = entrance.Entry,
                            InstanceId = residence.Parent?.Id ?? residence.Id
                        },
                        Position = entrance.Position
                    });

                    break;
                }
                default:
                {
                    session.Player.Rotation = new Vector3(character.RotationX, character.RotationY, character.RotationZ);
                    MapManager.Instance.AddToMap(session.Player, new MapPosition
                    {
                        Info     = new MapInfo
                        {
                            Entry = entry
                        },
                        Position = new Vector3(character.LocationX, character.LocationY, character.LocationZ)
                    });
                    break;
                }
            }
        }

        [MessageHandler(GameMessageOpcode.ClientLogoutRequest)]
        public static void HandleRequestLogout(IWorldSession session, ClientLogoutRequest logoutRequest)
        {
            if (logoutRequest.Initiated)
            {
                bool instantLogout = session.Account.RbacManager.HasPermission(Permission.InstantLogout);
                session.Player.LogoutManager.Start(instantLogout ? 0D : 30D);
            }
            else
                session.Player.LogoutManager.Cancel();
        }

        [MessageHandler(GameMessageOpcode.ClientLogoutConfirm)]
        public static void HandleLogoutConfirm(IWorldSession session, ClientLogoutConfirm logoutConfirm)
        {
            session.Player.LogoutManager.Finish();
        }

        [MessageHandler(GameMessageOpcode.ClientTitleSet)]
        public static void HandleTitleSet(IWorldSession session, ClientTitleSet request)
        {
            session.Player.TitleManager.ActiveTitleId = request.TitleId;
        }

        [MessageHandler(GameMessageOpcode.ClientEntitySelect)]
        public static void HandleClientTarget(IWorldSession session, ClientEntitySelect target)
        {
            session.Player.SetTarget(target.Guid > 0 ? target.Guid : null);
        }

        [MessageHandler(GameMessageOpcode.ClientReplayLevelRequest)]
        public static void HandleReplayLevel(IWorldSession session, ClientReplayLevelUp request)
        {
            session.Player.CastSpell(53378, (byte)(request.Level - 1), new SpellParameters());
        }

        [MessageHandler(GameMessageOpcode.ClientRequestPlayed)]
        public static void HandleClientRequestPlayed(IWorldSession session, ClientRequestPlayed requestPlayed)
        {
            double diff = session.Player.GetTimeSinceLastSave();
            session.EnqueueMessageEncrypted(new ServerPlayerPlayed
            {
                CreateTime        = session.Player.CreateTime,
                TimePlayedSession = (uint)(session.Player.TimePlayedSession + diff),
                TimePlayedTotal   = (uint)(session.Player.TimePlayedTotal + diff),
                TimePlayedLevel   = (uint)(session.Player.TimePlayedLevel + diff)
            });
        }

        [MessageHandler(GameMessageOpcode.ClientRapidTransport)]
        public static void HandleRapidTransport(IWorldSession session, ClientRapidTransport rapidTransport)
        {
            //TODO: check for cooldown
            //TODO: handle payment

            TaxiNodeEntry taxiNode = GameTableManager.Instance.TaxiNode.GetEntry(rapidTransport.TaxiNode);
            if (taxiNode == null)
                throw new InvalidPacketValueException();

            if (session.Player.Level < taxiNode.AutoUnlockLevel)
                throw new InvalidPacketValueException();

            WorldLocation2Entry worldLocation = GameTableManager.Instance.WorldLocation2.GetEntry(taxiNode.WorldLocation2Id);
            if (worldLocation == null)
                throw new InvalidPacketValueException();

            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1307);
            session.Player.CastSpell(entry.Dataint0, new SpellParameters
            {
                TaxiNode = rapidTransport.TaxiNode
            });
        }

        [MessageHandler(GameMessageOpcode.ClientInnateChange)]
        public static void HandleInnateChange(IWorldSession session, ClientInnateChange innateChange)
        {
            // TODO: Validate that index exists and which ability it is

            session.Player.InnateIndex = innateChange.InnateIndex;

            session.EnqueueMessageEncrypted(new ServerPlayerInnate
            {
                InnateIndex = session.Player.InnateIndex
            });
        }

        [MessageHandler(GameMessageOpcode.ClientInspectPlayerRequest)]
        public static void HandleInspectPlayerRequest(IWorldSession session, ClientInspectPlayerRequest inspectPlayer)
        {
            // TODO: Remove this since Raw- Lazy is rewriting something.
            WorldSession inspectSession = NetworkManager<WorldSession>.Instance.GetSession(s => s.Player?.Guid == inspectPlayer.Guid);
            if (inspectSession == null)
                return;

            session.EnqueueMessageEncrypted(new ServerInspectPlayerResponse
            {
                Guid  = inspectPlayer.Guid,
                Items = inspectSession.Player.Inventory
                    .Single(b => b.Location == InventoryLocation.Equipped)
                    .Select(i => i.Build())
                    .ToList()
            });
        }
        
        [MessageHandler(GameMessageOpcode.ClientCharacterAppearanceChange)]
        public static void HandleAppearanceChange(WorldSession session, ClientCharacterAppearanceChange appearanceChange)
        {
            List<(uint Label, uint Value)> customisations = appearanceChange.Labels
                .Zip(appearanceChange.Values, ValueTuple.Create)
                .ToList();

            if (!CustomisationManager.Instance.Validate(appearanceChange.Race, appearanceChange.Sex, session.Player.Faction1, customisations))
                throw new InvalidPacketValueException();

            if (appearanceChange.UseServiceTokens)
            {
                uint cost = CustomisationManager.Instance.CalculateCostTokens(session.Player, appearanceChange.Race, appearanceChange.Sex);
                if (!session.Player.Account.CurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, cost))
                    return;

                session.Player.Account.CurrencyManager.CurrencySubtractAmount(AccountCurrencyType.ServiceToken, cost);
            }
            else
            {
                uint cost = CustomisationManager.Instance.CalculateCostCredits(session.Player, appearanceChange.Race, appearanceChange.Sex, customisations, appearanceChange.Bones);
                if (!session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, cost))
                    return;

                session.Player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, cost);
            }

            session.Player.AppearanceManager.Update(appearanceChange.Race, appearanceChange.Sex, customisations, appearanceChange.Bones);
        }
    }
}
