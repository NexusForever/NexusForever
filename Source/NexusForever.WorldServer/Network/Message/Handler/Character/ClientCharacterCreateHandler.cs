using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Text.Filter;
using NexusForever.Game.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.TextFilter;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared.Game.Events;

namespace NexusForever.WorldServer.Network.Message.Handler.Character
{
    public class ClientCharacterCreateHandler : IMessageHandler<IWorldSession, ClientCharacterCreate>
    {
        #region Dependency Injection

        private readonly ITextFilterManager textFilterManager;
        private readonly IDatabaseManager databaseManager;
        private readonly IGameTableManager gameTableManager;
        private readonly ICustomisationManager customisationManager;
        private readonly ICharacterManager characterManager;

        public ClientCharacterCreateHandler(
            ITextFilterManager textFilterManager,
            IDatabaseManager databaseManager,
            IGameTableManager gameTableManager,
            ICustomisationManager customisationManager,
            ICharacterManager characterManager)
        {
            this.textFilterManager    = textFilterManager;
            this.databaseManager      = databaseManager;
            this.gameTableManager     = gameTableManager;
            this.customisationManager = customisationManager;
            this.characterManager     = characterManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientCharacterCreate characterCreate)
        {
            if (session.IsQueued == true)
                throw new InvalidPacketValueException();

            CharacterModifyResult? GetResult()
            {
                // TODO: validate path
                if (!textFilterManager.IsTextValid(characterCreate.Name)
                    || !textFilterManager.IsTextValid(characterCreate.Name, UserText.CharacterName))
                    return CharacterModifyResult.CreateFailed_InvalidName;

                // UserText.CharacterName checks if there is a space
                foreach (string name in characterCreate.Name.Split(' '))
                    if (!textFilterManager.IsTextValid(name, UserText.CharacterNamePart))
                        return CharacterModifyResult.CreateFailed_InvalidName;

                if (databaseManager.GetDatabase<CharacterDatabase>().CharacterNameExists(characterCreate.Name))
                    return CharacterModifyResult.CreateFailed_UniqueName;

                CharacterCreationEntry creationEntry = gameTableManager.CharacterCreation.GetEntry(characterCreate.CharacterCreationId);
                if (creationEntry == null)
                    return CharacterModifyResult.CreateFailed_Internal;

                if (creationEntry.CharacterCreationStartEnum == CharacterCreationStart.Level50
                    && !session.Account.CurrencyManager.CanAfford(AccountCurrencyType.MaxLevelToken, 1ul))
                    return CharacterModifyResult.CreateFailed_InsufficientFunds;

                List<(uint Label, uint Value)> customisations = characterCreate.Labels
                    .Zip(characterCreate.Values, ValueTuple.Create)
                    .ToList();

                if (!customisationManager.Validate(creationEntry.RaceId, creationEntry.Sex, creationEntry.FactionId, customisations))
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

                CharacterCreationEntry creationEntry = gameTableManager.CharacterCreation.GetEntry(characterCreate.CharacterCreationId);
                if (creationEntry == null)
                    throw new InvalidPacketValueException();

                if (creationEntry.EntitlementIdRequired != 0u)
                {
                    // TODO: Aurin engineer has this
                }

                var character = new CharacterModel
                {
                    AccountId  = session.Account.Id,
                    Id         = characterManager.NextCharacterId,
                    Name       = characterCreate.Name,
                    Race       = (byte)creationEntry.RaceId,
                    Sex        = (byte)creationEntry.Sex,
                    Class      = (byte)creationEntry.ClassId,
                    FactionId  = (ushort)creationEntry.FactionId,
                    ActivePath = characterCreate.Path,
                    TotalXp    = creationEntry.Xp
                };

                uint startingLevel = gameTableManager.XpPerLevel.Entries.First(l => l.MinXpForLevel >= creationEntry.Xp).Id;

                for (Game.Static.Entity.Path path = Game.Static.Entity.Path.Soldier; path <= Game.Static.Entity.Path.Explorer; path++)
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

                foreach (IItemVisual visual in customisationManager.GetItemVisuals((Race)character.Race, (Sex)character.Sex, customisations))
                {
                    character.Appearance.Add(new CharacterAppearanceModel
                    {
                        Slot      = (byte)visual.Slot,
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

                ILocation startingLocation = characterManager.GetStartingLocation(creationEntry.RaceId, creationEntry.FactionId, creationEntry.CharacterCreationStartEnum);
                if (startingLocation == null)
                    throw new ArgumentNullException(nameof(startingLocation));

                character.LocationX = startingLocation.Position.X;
                character.LocationY = startingLocation.Position.Y;
                character.LocationZ = startingLocation.Position.Z;
                character.WorldId = (ushort)startingLocation.World.Id;

                character.ActiveSpec = 0;

                // create initial LAS abilities
                UILocation location = 0;
                foreach (SpellLevelEntry spellLevelEntry in gameTableManager.SpellLevel.Entries
                    .Where(s => s.ClassId == character.Class && s.CharacterLevel == 1))
                {
                    Spell4Entry spell4Entry = gameTableManager.Spell4.GetEntry(spellLevelEntry.Spell4Id);
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
                session.Events.EnqueueEvent(new TaskEvent(databaseManager.GetDatabase<CharacterDatabase>().Save(c =>
                    {
                        c.Character.Add(character);
                        foreach (IItem item in items)
                            item.Save(c);
                    }),
                    () =>
                {
                    session.Characters.Add(character);
                    characterManager.AddCharacter(character);

                    if (creationEntry.CharacterCreationStartEnum == CharacterCreationStart.Level50)
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
    }
}
