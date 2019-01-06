using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
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
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NexusForever.WorldServer.Network.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CharacterHandler
    {
        [MessageHandler(GameMessageOpcode.ClientRealmList)]
        public static void HandleRealmList(WorldSession session, ClientRealmList realmList)
        {
            var serverRealmList = new ServerRealmList();
            foreach (ServerManager.ServerInfo server in ServerManager.Servers)
            {
                // TODO: finish this...
                serverRealmList.Realms.Add(new ServerRealmList.RealmInfo
                {
                    Unknown0   = 1,
                    Realm      = server.Model.Name,
                    Type       = (RealmType)server.Model.Type,
                    Status     = RealmStatus.Up,
                    Population = RealmPopulation.Low
                });
            }

            session.EnqueueMessageEncrypted(serverRealmList);
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterList)]
        public static void HandleCharacterList(WorldSession session, ClientCharacterList characterList)
        {
            session.EnqueueEvent(new TaskGenericEvent<List<Character>>(CharacterDatabase.GetCharacters(session.Account.Id),
                characters =>
            {
                session.Characters.Clear();
                session.Characters.AddRange(characters);

                session.EnqueueMessageEncrypted(new ServerAccountEntitlements
                {
                    Entitlements =
                    {
                        new ServerAccountEntitlements.AccountEntitlementInfo
                        {
                            Entitlement = Entitlement.BaseCharacterSlots,
                            Count       = 12
                        },
                        new ServerAccountEntitlements.AccountEntitlementInfo
                        {
                            Entitlement = Entitlement.ChuaWarriorUnlock,
                            Count       = 1
                        },
                        new ServerAccountEntitlements.AccountEntitlementInfo
                        {
                            Entitlement = Entitlement.AurinEngineerUnlock,
                            Count       = 1
                        }
                    }
                });

                session.EnqueueMessageEncrypted(new ServerMaxCharacterLevelAchieved
                {
                    Level = 50
                });

                var serverCharacterList = new ServerCharacterList();
                foreach (Character character in characters)
                {
                    var listCharacter = new ServerCharacterList.Character
                    {
                        Id          = character.Id,
                        Name        = character.Name,
                        Sex         = (Sex)character.Sex,
                        Race        = (Race)character.Race,
                        Class       = (Class)character.Class,
                        Faction     = character.FactionId,
                        Level       = character.Level,
                        WorldId     = 3460,
                        WorldZoneId = 5967,
                        Unknown38   = 358
                    };

                    // create a temporary inventory to show equipped gear
                    var inventory = new Inventory(null, character);
                    foreach (ItemVisual itemVisual in inventory.GetItemVisuals())
                        listCharacter.Gear.Add(itemVisual);

                    foreach (CharacterAppearance appearance in character.CharacterAppearance)
                    {
                        listCharacter.Appearance.Add(new ItemVisual
                        {
                            Slot      = (ItemSlot)appearance.Slot,
                            DisplayId = appearance.DisplayId
                        });
                    }
                        
                    foreach (CharacterCustomisation customisation in character.CharacterCustomisation)
                    {
                        listCharacter.Labels.Add(customisation.Label);
                        listCharacter.Values.Add(customisation.Value);
                    }

                    foreach (CharacterBone bone in character.CharacterBone.OrderBy(bone => bone.BoneIndex))
                    {
                        listCharacter.Bones.Add(bone.Bone);
                    }

                    serverCharacterList.Characters.Add(listCharacter);
                }

                session.EnqueueMessageEncrypted(serverCharacterList);
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterCreate)]
        public static void HandleCharacterCreate(WorldSession session, ClientCharacterCreate characterCreate)
        {
            try
            {
                // TODO: validate name and path

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
                    Level      = 1,
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

                for(int i = 0; i < characterCreate.Bones.Count; ++i)
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

                // create a temporary inventory to create starting gear
                var inventory = new Inventory(character.Id, creationEntry);
                var items = inventory
                    .SelectMany(b => b)
                    .Select(i => i);

                // TODO: actually error check this
                session.EnqueueEvent(new TaskEvent(CharacterDatabase.CreateCharacter(character, items),
                    () =>
                {
                    session.Characters.Add(character);
                    session.EnqueueMessageEncrypted(new ServerCharacterCreate
                    {
                        CharacterId = character.Id,
                        WorldId     = character.WorldId,
                        Result      = 3
                    });
                }));
            }
            catch
            {
                session.EnqueueMessageEncrypted(new ServerCharacterCreate
                {
                    Result = 0
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
        public static void HandleCreateDelete(WorldSession session, ClientCharacterDelete characterDelete)
        {

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

            session.Player = new Player(session, character);
            Vector3 vector = new Vector3(character.LocationX, character.LocationY, character.LocationZ);

            MapManager.AddToMap(session.Player, character.WorldId, vector);
        }

        [MessageHandler(GameMessageOpcode.ClientCharacterLogout)]
        public static void HandleCharacterLogout(WorldSession session, ClientCharacterLogout characterLogout)
        {
            if (characterLogout.Initiated)
                session.Player.LogoutStart();
            else
                session.Player.LogoutCancel();
        }

        [MessageHandler(GameMessageOpcode.ClientLogout)]
        public static void HandleLogout(WorldSession session, ClientLogout logout)
        {
            session.Player.LogoutFinish();
        }

        [MessageHandler(GameMessageOpcode.ClientTitleSet)]
        public static void HandleTitleSet(WorldSession session, ClientTitleSet request)
        {
            session.Player.TitleManager.ActiveTitleId = request.TitleId;
        }
    }
}
