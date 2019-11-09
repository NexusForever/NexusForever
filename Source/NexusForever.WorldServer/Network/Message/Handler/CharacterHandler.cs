using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Game;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NexusForever.WorldServer.Network.Message.Static;
using CostumeEntity = NexusForever.WorldServer.Game.Entity.Costume;
using Item = NexusForever.WorldServer.Game.Entity.Item;
using Residence = NexusForever.WorldServer.Game.Housing.Residence;
using NetworkMessage = NexusForever.Shared.Network.Message.Model.Shared.Message;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CharacterHandler
    {
        [MessageHandler(GameMessageOpcode.ClientRealmList)]
        public static void HandleRealmList(WorldSession session, ClientRealmList realmList)
        {
            var serverRealmList = new ServerRealmList
            {
                Messages = ServerManager.ServerMessages
                    .Select(m => new NetworkMessage
                    {
                        Index    = m.Index,
                        Messages = m.Messages
                    })
                    .ToList()
            };

            foreach (ServerInfo server in ServerManager.Servers)
            {
                serverRealmList.Realms.Add(new ServerRealmList.RealmInfo
                {
                    RealmId    = server.Model.Id,
                    RealmName  = server.Model.Name,
                    Type       = (RealmType)server.Model.Type,
                    Status     = RealmStatus.Up,
                    Population = RealmPopulation.Low,
                    Unknown8   = new byte[16],
                    AccountRealmInfo = new ServerRealmList.RealmInfo.AccountRealmData
                    {
                        RealmId = server.Model.Id
                    }
                });
            }

            session.EnqueueMessageEncrypted(serverRealmList);
        }

        [MessageHandler(GameMessageOpcode.ClientSelectRealm)]
        public static void HandleSelectRealm(WorldSession session, ClientSelectRealm selectRealm)
        {
            ServerInfo server = ServerManager.Servers.SingleOrDefault(s => s.Model.Id == selectRealm.RealmId);
            if (server == null)
                throw new InvalidPacketValueException();

            // clicking back or selecting the current realm also triggers this packet, client crashes if we don't ignore it
            if (server.Model.Id == WorldServer.RealmId)
                return;

            byte[] sessionKey = RandomProvider.GetBytes(16u);
            session.EnqueueEvent(new TaskEvent(AuthDatabase.UpdateAccountSessionKey(session.Account, sessionKey),
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
                    RealmName   = server.Model.Name,
                    Type        = (RealmType)server.Model.Type
                });
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterList)]
        public static void HandleCharacterList(WorldSession session, ClientCharacterList characterList)
        {
            session.EnqueueEvent(new TaskGenericEvent<List<Character>>(CharacterDatabase.GetCharacters(session.Account.Id),
                characters =>
            {
                byte MaxCharacterLevelAchieved = 1;

                session.Characters.Clear();
                session.Characters.AddRange(characters);

                session.AccountCurrencyManager.SendCharacterListPacket();
                session.GenericUnlockManager.SendUnlockList();

                session.EnqueueMessageEncrypted(new ServerAccountEntitlements
                {
                    Entitlements = session.EntitlementManager.GetAccountEntitlements()
                        .Select(e => new ServerAccountEntitlements.AccountEntitlementInfo
                        {
                            Entitlement = e.Type,
                            Count       = e.Amount
                        })
                        .ToList()
                });

                var serverCharacterList = new ServerCharacterList
                {
                    RealmId = WorldServer.RealmId
                };

                foreach (Character character in characters)
                {
                    if (character.DeleteTime != null)
                        continue;

                    var listCharacter = new ServerCharacterList.Character
                    {
                        Id          = character.Id,
                        Name        = character.Name,
                        Sex         = (Sex)character.Sex,
                        Race        = (Race)character.Race,
                        Class       = (Class)character.Class,
                        Faction     = character.FactionId,
                        Level       = character.Level,
                        WorldId     = character.WorldId,
                        WorldZoneId = character.WorldZoneId,
                        RealmId     = WorldServer.RealmId,
                        Path        = (byte)character.ActivePath
                    };

                    MaxCharacterLevelAchieved = (byte)Math.Max(MaxCharacterLevelAchieved, character.Level);

                    // create a temporary Inventory and CostumeManager to show equipped gear
                    var inventory      = new Inventory(null, character);
                    var costumeManager = new CostumeManager(null, session.Account, character);

                    CostumeEntity costume = null;
                    if (character.ActiveCostumeIndex >= 0)
                        costume = costumeManager.GetCostume((byte)character.ActiveCostumeIndex);

                    listCharacter.GearMask = costume?.Mask ?? 0xFFFFFFFF;

                    foreach (ItemVisual itemVisual in inventory.GetItemVisuals(costume))
                        listCharacter.Gear.Add(itemVisual);

                    foreach (CharacterAppearance appearance in character.CharacterAppearance)
                    {
                        listCharacter.Appearance.Add(new ItemVisual
                        {
                            Slot      = (ItemSlot)appearance.Slot,
                            DisplayId = appearance.DisplayId
                        });
                    }

                    /*foreach (CharacterCustomisation customisation in character.CharacterCustomisation)
                    {
                        listCharacter.Labels.Add(customisation.Label);
                        listCharacter.Values.Add(customisation.Value);
                    }*/

                    foreach (CharacterBone bone in character.CharacterBone.OrderBy(bone => bone.BoneIndex))
                    {
                        listCharacter.Bones.Add(bone.Bone);
                    }

                    foreach (CharacterStats stat in character.CharacterStats)
                    {
                        if ((Stat)stat.Stat == Stat.Level)
                        {
                            listCharacter.Level = (uint)stat.Value;
                            break;
                        }
                    }

                    serverCharacterList.Characters.Add(listCharacter);
                }

                session.EnqueueMessageEncrypted(new ServerMaxCharacterLevelAchieved
                {
                    Level = MaxCharacterLevelAchieved
                });

                session.EnqueueMessageEncrypted(serverCharacterList);
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterCreate)]
        public static void HandleCharacterCreate(WorldSession session, ClientCharacterCreate characterCreate)
        {
            try
            {
                // TODO: validate name and path
                if (CharacterDatabase.CharacterNameExists(characterCreate.Name))
                {
                    session.EnqueueMessageEncrypted(new ServerCharacterCreate
                    {
                        Result = CharacterModifyResult.CreateFailed_UniqueName
                    });

                    return;
                }

                CharacterCreationEntry creationEntry = GameTableManager.CharacterCreation.GetEntry(characterCreate.CharacterCreationId);
                if (creationEntry == null)
                    throw new InvalidPacketValueException();

                if (creationEntry.EntitlementIdRequired != 0u)
                {
                    // TODO: Aurin engineer has this
                }

                var character = new Character
                {
                    AccountId  = session.Account.Id,
                    Id         = AssetManager.NextCharacterId,
                    Name       = characterCreate.Name,
                    Race       = (byte)creationEntry.RaceId,
                    Sex        = (byte)creationEntry.Sex,
                    Class      = (byte)creationEntry.ClassId,
                    FactionId  = (ushort)creationEntry.FactionId,
                    ActivePath = characterCreate.Path
                };

                for (Path path = Path.Soldier; path <= Path.Explorer; path++)
                {
                    character.CharacterPath.Add(new CharacterPath
                    {
                        Path     = (byte)path,
                        Unlocked = Convert.ToByte(characterCreate.Path == (byte)path)
                    });
                }

                // merge seperate label and value lists into a single dictonary
                Dictionary<uint, uint> customisations = characterCreate.Labels
                    .Zip(characterCreate.Values, (l, v) => new { l, v })
                    .ToDictionary(p => p.l, p => p.v);

                foreach ((uint label, uint value) in customisations)
                {
                    character.CharacterCustomisation.Add(new CharacterCustomisation
                    {
                        Label = label,
                        Value = value
                    });

                    CharacterCustomizationEntry entry = GetCharacterCustomisation(customisations, creationEntry.RaceId, creationEntry.Sex, label, value);
                    if (entry == null)
                        continue;

                    character.CharacterAppearance.Add(new CharacterAppearance
                    {
                        Slot      = (byte)entry.ItemSlotId,
                        DisplayId = (ushort)entry.ItemDisplayId
                    });
                }

                for (int i = 0; i < characterCreate.Bones.Count; ++i)
                {
                    character.CharacterBone.Add(new CharacterBone
                    {
                        BoneIndex = (byte)(i),
                        Bone = characterCreate.Bones[i]
                    });
                }

                //TODO: handle starting locations per race
                character.LocationX = -7683.809f;
                character.LocationY = -942.5914f;
                character.LocationZ = -666.6343f;
                character.WorldId = 870;

                character.ActiveSpec = 0;

                // create initial LAS abilities
                UILocation location = 0;
                foreach (SpellLevelEntry spellLevelEntry in GameTableManager.SpellLevel.Entries
                    .Where(s => s.ClassId == character.Class && s.CharacterLevel == 1))
                {
                    Spell4Entry spell4Entry = GameTableManager.Spell4.GetEntry(spellLevelEntry.Spell4Id);
                    if (spell4Entry == null)
                        continue;

                    character.CharacterSpell.Add(new CharacterSpell
                    {
                        Id           = character.Id,
                        Spell4BaseId = spell4Entry.Spell4BaseIdBaseSpell,
                        Tier         = 1
                    });

                    character.CharacterActionSetShortcut.Add(new CharacterActionSetShortcut
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
                IEnumerable<Item> items = inventory
                    .SelectMany(b => b)
                    .Select(i => i);

                //TODO: handle starting stats per class/race
                character.CharacterStats.Add(new CharacterStats
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Health,
                    Value = 800
                });
                character.CharacterStats.Add(new CharacterStats
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Shield,
                    Value = 450
                });
                character.CharacterStats.Add(new CharacterStats
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Dash,
                    Value = 200
                });
                character.CharacterStats.Add(new CharacterStats
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.Level,
                    Value = 1
                });
                character.CharacterStats.Add(new CharacterStats
                {
                    Id    = character.Id,
                    Stat  = (byte)Stat.StandState,
                    Value = 3
                });

                // TODO: actually error check this
                session.EnqueueEvent(new TaskEvent(CharacterDatabase.CreateCharacter(character, items),
                    () =>
                {
                    session.Characters.Add(character);
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

            CharacterCustomizationEntry GetCharacterCustomisation(Dictionary<uint, uint> customisations, uint race, uint sex, uint primaryLabel, uint primaryValue)
            {
                ImmutableList<CharacterCustomizationEntry> entries = AssetManager.GetPrimaryCharacterCustomisation(race, sex, primaryLabel, primaryValue);
                if (entries == null)
                    return null;
                if (entries.Count == 1)
                    return entries[0];

                // customisation has multiple results, filter with secondary label and value 
                uint secondaryLabel = entries.First(e => e.CharacterCustomizationLabelId01 != 0).CharacterCustomizationLabelId01;
                uint secondaryValue = customisations[secondaryLabel];

                CharacterCustomizationEntry entry = entries.SingleOrDefault(e => e.CharacterCustomizationLabelId01 == secondaryLabel && e.Value01 == secondaryValue);
                return entry ?? entries.Single(e => e.CharacterCustomizationLabelId01 == 0 && e.Value01 == 0);
            }
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterDelete)]
        public static void HandleCharacterDelete(WorldSession session, ClientCharacterDelete characterDelete)
        {
            Character characterToDelete = session.Characters.FirstOrDefault(c => c.Id == characterDelete.CharacterId);

            CharacterModifyResult GetResult()
            {
                if (characterToDelete == null)
                    return CharacterModifyResult.DeleteFailed;

                // TODO: Not sure if this is definitely the case, but put it in for good measure
                if (characterToDelete.CharacterMail.Count > 0)
                {
                    foreach (CharacterMail characterMail in characterToDelete.CharacterMail)
                    {
                        if (characterMail.CharacterMailAttachment.Count > 0)
                            return CharacterModifyResult.DeleteFailed;
                    }
                }

                // TODO: Ensure character is not a guild master

                return CharacterModifyResult.DeleteOk;
            }

            CharacterModifyResult result = GetResult();
            if (result != CharacterModifyResult.DeleteOk)
            {
                session.EnqueueMessageEncrypted(new ServerCharacterDeleteResult
                {
                    Result = result
                });
                return;
            }

            session.CanProcessPackets = false;

            void Save(CharacterContextExtended context)
            {
                var model = new Character
                {
                    Id = characterToDelete.Id
                };

                EntityEntry<Character> entity = context.Attach(model);

                model.DeleteTime = DateTime.UtcNow;
                entity.Property(e => e.DeleteTime).IsModified = true;
                
                model.OriginalName = characterToDelete.Name;
                entity.Property(e => e.OriginalName).IsModified = true;

                model.Name = null;
                entity.Property(e => e.Name).IsModified = true;
            }

            session.EnqueueEvent(new TaskEvent(CharacterDatabase.Save(Save),
                () =>
            {
                session.CanProcessPackets = true;

                // TODO: De-register from any character cache

                session.EnqueueMessageEncrypted(new ServerCharacterDeleteResult
                {
                    Result = result
                });
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterSelect)]
        public static void HandleCharacterSelect(WorldSession session, ClientCharacterSelect characterSelect)
        {
            Character character = session.Characters.SingleOrDefault(c => c.Id == characterSelect.CharacterId);
            if (character == null)
            {
                session.EnqueueMessageEncrypted(new ServerCharacterSelectFail
                {
                    Result = CharacterSelectResult.Failed
                });
                return;
            }

            if (CleanupManager.HasPendingCleanup(session.Account))
            {
                session.EnqueueMessageEncrypted(new ServerCharacterSelectFail
                {
                    Result = CharacterSelectResult.FailedCharacterInWorld
                });
                return;
            }

            session.Player = new Player(session, character);

            WorldEntry entry = GameTableManager.World.GetEntry(character.WorldId);
            if (entry == null)
                throw new ArgumentOutOfRangeException();

            switch (entry.Type)
            {
                // housing map
                case 5:
                {
                    // characters logging in to a housing map are returned to their own residence
                    session.EnqueueEvent(new TaskGenericEvent<Residence>(ResidenceManager.GetResidence(session.Player.Name),
                        residence =>
                    {
                        if (residence == null)
                            residence = ResidenceManager.CreateResidence(session.Player);

                        ResidenceEntrance entrance = ResidenceManager.GetResidenceEntrance(residence);
                        var mapInfo = new MapInfo(entrance.Entry, 0u, residence.Id);
                        MapManager.AddToMap(session.Player, mapInfo, entrance.Position);
                    }));

                    break;
                }
                default:
                {
                    var mapInfo = new MapInfo(entry);
                    var vector3 = new Vector3(character.LocationX, character.LocationY, character.LocationZ);
                    MapManager.AddToMap(session.Player, mapInfo, vector3);
                    break;
                }
            }
        }

        [MessageHandler(GameMessageOpcode.ClientLogoutRequest)]
        public static void HandleRequestLogout(WorldSession session, ClientLogoutRequest logoutRequest)
        {
            if (logoutRequest.Initiated)
                session.Player.LogoutStart();
            else
                session.Player.LogoutCancel();
        }

        [MessageHandler(GameMessageOpcode.ClientLogoutConfirm)]
        public static void HandleLogoutConfirm(WorldSession session, ClientLogoutConfirm logoutConfirm)
        {
            session.Player.LogoutFinish();
        }

        [MessageHandler(GameMessageOpcode.ClientTitleSet)]
        public static void HandleTitleSet(WorldSession session, ClientTitleSet request)
        {
            session.Player.TitleManager.ActiveTitleId = request.TitleId;
        }

        [MessageHandler(GameMessageOpcode.ClientEntitySelect)]
        public static void HandleClientTarget(WorldSession session, ClientEntitySelect target)
        {
            session.Player.TargetGuid = target.Guid;
        }

        [MessageHandler(GameMessageOpcode.ClientRequestPlayed)]
        public static void HandleClientRequestPlayed(WorldSession session, ClientRequestPlayed requestPlayed)
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
        public static void HandleClientTarget(WorldSession session, ClientRapidTransport rapidTransport)
        {
            //TODO: check for cooldown
            //TODO: handle payment

            TaxiNodeEntry taxiNode = GameTableManager.TaxiNode.GetEntry(rapidTransport.TaxiNode);
            if (taxiNode == null)
                throw new InvalidPacketValueException();

            if (session.Player.Level < taxiNode.AutoUnlockLevel)
                throw new InvalidPacketValueException();

            WorldLocation2Entry worldLocation = GameTableManager.WorldLocation2.GetEntry(taxiNode.WorldLocation2Id);
            if (worldLocation == null)
                throw new InvalidPacketValueException();

            GameFormulaEntry entry = GameTableManager.GameFormula.GetEntry(1307);
            session.Player.CastSpell(entry.Dataint0, new SpellParameters
            {
                TaxiNode = rapidTransport.TaxiNode
            });
        }

        [MessageHandler(GameMessageOpcode.ClientInnateChange)]
        public static void HandleInnateChange(WorldSession session, ClientInnateChange innateChange)
        {
            // TODO: Validate that index exists and which ability it is

            session.Player.InnateIndex = innateChange.InnateIndex;

            session.EnqueueMessageEncrypted(new ServerPlayerInnate
            {
                InnateIndex = session.Player.InnateIndex
            });
        }

        [MessageHandler(GameMessageOpcode.ClientInspectPlayerRequest)]
        public static void HandleInspectPlayerRequest(WorldSession session, ClientInspectPlayerRequest inspectPlayer)
        {
            // TODO: Remove this since Raw- Lazy is rewriting something.
            WorldSession inspectSession = NetworkManager<WorldSession>.GetSession(s => s.Player?.Guid == inspectPlayer.Guid);
            if (inspectSession == null)
                return;

            session.EnqueueMessageEncrypted(new ServerInspectPlayerResponse
            {
                Guid  = inspectPlayer.Guid,
                Items = inspectSession.Player.Inventory
                    .Single(b => b.Location == InventoryLocation.Equipped)
                    .Select(i => i.BuildNetworkItem())
                    .ToList()
            });
        }
    }
}
