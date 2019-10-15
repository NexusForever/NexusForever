using System;
using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class CharacterModel
    {
        public ulong Id { get; set; }
        public uint AccountId { get; set; }
        public string Name { get; set; }
        public byte Sex { get; set; }
        public byte Race { get; set; }
        public byte Class { get; set; }
        public byte Level { get; set; }
        public ushort FactionId { get; set; }
        public DateTime CreateTime { get; set; }
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public float LocationZ { get; set; }
        public ushort WorldId { get; set; }
        public ushort WorldZoneId { get; set; }
        public ushort Title { get; set; }
        public uint ActivePath { get; set; }
        public DateTime PathActivatedTimestamp { get; set; }
        public sbyte ActiveCostumeIndex { get; set; }
        public sbyte InputKeySet { get; set; }
        public byte ActiveSpec { get; set; }
        public byte InnateIndex { get; set; }
        public uint TimePlayedTotal { get; set; }
        public uint TimePlayedLevel { get; set; }
        public DateTime? DeleteTime { get; set; }
        public string OriginalName { get; set; }

        public ResidenceModel Residence { get; set; }
        public HashSet<CharacterAchievementModel> Achievements { get; set; } = new HashSet<CharacterAchievementModel>();
        public HashSet<CharacterActionSetAmpModel> ActionSetAmps { get; set; } = new HashSet<CharacterActionSetAmpModel>();
        public HashSet<CharacterActionSetShortcutModel> ActionSetShortcuts { get; set; } = new HashSet<CharacterActionSetShortcutModel>();
        public HashSet<CharacterAppearanceModel> Appearance { get; set; } = new HashSet<CharacterAppearanceModel>();
        public HashSet<CharacterBoneModel> Bones { get; set; } = new HashSet<CharacterBoneModel>();
        public HashSet<CharacterCostumeModel> Costumes { get; set; } = new HashSet<CharacterCostumeModel>();
        public HashSet<CharacterCurrencyModel> Currencies { get; set; } = new HashSet<CharacterCurrencyModel>();
        public HashSet<CharacterCustomisationModel> Customisations { get; set; } = new HashSet<CharacterCustomisationModel>();
        public HashSet<CharacterDatacubeModel> Datacubes { get; set; } = new HashSet<CharacterDatacubeModel>();
        public HashSet<CharacterEntitlementModel> Entitlements { get; set; } = new HashSet<CharacterEntitlementModel>();
        public HashSet<CharacterKeybindingModel> Keybindings { get; set; } = new HashSet<CharacterKeybindingModel>();
        public HashSet<MailModel> Mail { get; set; } = new HashSet<MailModel>();
        public HashSet<CharacterPathModel> Paths { get; set; } = new HashSet<CharacterPathModel>();
        public HashSet<CharacterPetCustomisationModel> PetCustomisations { get; set; } = new HashSet<CharacterPetCustomisationModel>();
        public HashSet<CharacterPetFlairModel> PetFlairs { get; set; } = new HashSet<CharacterPetFlairModel>();
        public HashSet<CharacterQuestModel> Quests { get; set; } = new HashSet<CharacterQuestModel>();
        public HashSet<CharacterSpellModel> Spells { get; set; } = new HashSet<CharacterSpellModel>();
        public HashSet<CharacterStatModel> Stats { get; set; } = new HashSet<CharacterStatModel>();
        public HashSet<CharacterTitleModel> Titles { get; set; } = new HashSet<CharacterTitleModel>();
        public HashSet<CharacterZonemapHexgroupModel> ZonemapHexgroups { get; set; } = new HashSet<CharacterZonemapHexgroupModel>();
        public HashSet<ItemModel> Items { get; set; } = new HashSet<ItemModel>();
    }
}
