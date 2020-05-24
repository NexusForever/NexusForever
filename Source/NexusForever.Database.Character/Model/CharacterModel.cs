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
        public DateTime? LastOnline { get; set; }
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public float LocationZ { get; set; }
        public float RotationX { get; set; }
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
        public uint TotalXp { get; set; }
        public uint RestBonusXp { get; set; }

        public ResidenceModel Residence { get; set; }
        public ICollection<CharacterAchievementModel> Achievement { get; set; } = new HashSet<CharacterAchievementModel>();
        public ICollection<CharacterActionSetAmpModel> ActionSetAmp { get; set; } = new HashSet<CharacterActionSetAmpModel>();
        public ICollection<CharacterActionSetShortcutModel> ActionSetShortcut { get; set; } = new HashSet<CharacterActionSetShortcutModel>();
        public ICollection<CharacterAppearanceModel> Appearance { get; set; } = new HashSet<CharacterAppearanceModel>();
        public ICollection<CharacterBoneModel> Bone { get; set; } = new HashSet<CharacterBoneModel>();
        public ICollection<CharacterCostumeModel> Costume { get; set; } = new HashSet<CharacterCostumeModel>();
        public ICollection<CharacterCurrencyModel> Currency { get; set; } = new HashSet<CharacterCurrencyModel>();
        public ICollection<CharacterCustomisationModel> Customisation { get; set; } = new HashSet<CharacterCustomisationModel>();
        public ICollection<CharacterDatacubeModel> Datacube { get; set; } = new HashSet<CharacterDatacubeModel>();
        public ICollection<CharacterEntitlementModel> Entitlement { get; set; } = new HashSet<CharacterEntitlementModel>();
        public ICollection<CharacterKeybindingModel> Keybinding { get; set; } = new HashSet<CharacterKeybindingModel>();
        public ICollection<CharacterMailModel> Mail { get; set; } = new HashSet<CharacterMailModel>();
        public ICollection<CharacterPathModel> Path { get; set; } = new HashSet<CharacterPathModel>();
        public ICollection<CharacterPetCustomisationModel> PetCustomisation { get; set; } = new HashSet<CharacterPetCustomisationModel>();
        public ICollection<CharacterPetFlairModel> PetFlair { get; set; } = new HashSet<CharacterPetFlairModel>();
        public ICollection<CharacterQuestModel> Quest { get; set; } = new HashSet<CharacterQuestModel>();
        public ICollection<CharacterSpellModel> Spell { get; set; } = new HashSet<CharacterSpellModel>();
        public ICollection<CharacterStatModel> Stat { get; set; } = new HashSet<CharacterStatModel>();
        public ICollection<CharacterTitleModel> CharacterTitle { get; set; } = new HashSet<CharacterTitleModel>();
        public ICollection<CharacterTradeskillMaterialModel> TradeskillMaterials { get; set; } = new HashSet<CharacterTradeskillMaterialModel>();
        public ICollection<CharacterZonemapHexgroupModel> ZonemapHexgroup { get; set; } = new HashSet<CharacterZonemapHexgroupModel>();
        public ICollection<ItemModel> Item { get; set; } = new HashSet<ItemModel>();
    }
}
