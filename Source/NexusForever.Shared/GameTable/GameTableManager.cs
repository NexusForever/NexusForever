using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NLog;

namespace NexusForever.Shared.GameTable
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public static class GameTableManager
    {
        private const int minimumThreads = 2;
        private const int maximumThreads = 16;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [GameData]
        public static GameTable<AccountCurrencyTypeEntry> AccountCurrencyType { get; private set; }

        [GameData]
        public static GameTable<AccountItemEntry> AccountItem { get; private set; }

        [GameData]
        public static GameTable<AccountItemCooldownGroupEntry> AccountItemCooldownGroup { get; private set; }

        [GameData]
        public static GameTable<AchievementEntry> Achievement { get; private set; }

        [GameData]
        public static GameTable<AchievementCategoryEntry> AchievementCategory { get; private set; }

        [GameData]
        public static GameTable<AchievementChecklistEntry> AchievementChecklist { get; private set; }

        [GameData]
        public static GameTable<AchievementGroupEntry> AchievementGroup { get; private set; }

        [GameData]
        public static GameTable<AchievementSubGroupEntry> AchievementSubGroup { get; private set; }

        public static GameTable<AchievementTextEntry> AchievementText { get; private set; }
        public static GameTable<ActionBarShortcutSetEntry> ActionBarShortcutSet { get; private set; }
        public static GameTable<ActionSlotPrereqEntry> ActionSlotPrereq { get; private set; }
        public static GameTable<ArchiveArticleEntry> ArchiveArticle { get; private set; }
        public static GameTable<ArchiveCategoryEntry> ArchiveCategory { get; private set; }
        public static GameTable<ArchiveEntryEntry> ArchiveEntry { get; private set; }
        public static GameTable<ArchiveEntryUnlockRuleEntry> ArchiveEntryUnlockRule { get; private set; }
        public static GameTable<ArchiveLinkEntry> ArchiveLink { get; private set; }
        public static GameTable<AttributeMilestoneGroupEntry> AttributeMilestoneGroup { get; private set; }
        public static GameTable<AttributeMiniMilestoneGroupEntry> AttributeMiniMilestoneGroup { get; private set; }
        public static GameTable<BindPointEntry> BindPoint { get; private set; }
        public static GameTable<BinkMovieEntry> BinkMovie { get; private set; }
        public static GameTable<BinkMovieSubtitleEntry> BinkMovieSubtitle { get; private set; }
        public static GameTable<BugCategoryEntry> BugCategory { get; private set; }
        public static GameTable<BugSubcategoryEntry> BugSubcategory { get; private set; }
        public static GameTable<CCStateAdditionalDataEntry> CCStateAdditionalData { get; private set; }
        public static GameTable<CCStateDiminishingReturnsEntry> CCStateDiminishingReturns { get; private set; }
        public static GameTable<CCStatesEntry> CCStates { get; private set; }
        public static GameTable<ChallengeEntry> Challenge { get; private set; }
        public static GameTable<ChallengeTierEntry> ChallengeTier { get; private set; }

        [GameData]
        public static GameTable<CharacterCreationEntry> CharacterCreation { get; private set; }

        public static GameTable<CharacterCreationArmorSetEntry> CharacterCreationArmorSet { get; private set; }
        public static GameTable<CharacterCreationPresetEntry> CharacterCreationPreset { get; private set; }

        [GameData]
        public static GameTable<CharacterCustomizationEntry> CharacterCustomization { get; private set; }

        public static GameTable<CharacterCustomizationLabelEntry> CharacterCustomizationLabel { get; private set; }
        public static GameTable<CharacterCustomizationSelectionEntry> CharacterCustomizationSelection { get; private set; }

        [GameData]
        public static GameTable<CharacterTitleEntry> CharacterTitle { get; private set; }

        public static GameTable<CharacterTitleCategoryEntry> CharacterTitleCategory { get; private set; }
        public static GameTable<ChatChannelEntry> ChatChannel { get; private set; }
        public static GameTable<CinematicEntry> Cinematic { get; private set; }
        public static GameTable<CinematicRaceEntry> CinematicRace { get; private set; }
        public static GameTable<CityDirectionEntry> CityDirection { get; private set; }

        [GameData]
        public static GameTable<ClassEntry> Class { get; private set; }

        public static GameTable<ClassSecondaryStatBonusEntry> ClassSecondaryStatBonus { get; private set; }

        [GameData]
        public static GameTable<ClientEventEntry> ClientEvent { get; private set; }

        [GameData]
        public static GameTable<ClientEventActionEntry> ClientEventAction { get; private set; }

        [GameData]
        public static GameTable<ClientSideInteractionEntry> ClientSideInteraction { get; private set; }

        [GameData]
        public static GameTable<ColorShiftEntry> ColorShift { get; private set; }

        [GameData]
        public static GameTable<CombatRewardEntry> CombatReward { get; private set; }

        [GameData]
        public static GameTable<CommunicatorMessagesEntry> CommunicatorMessages { get; private set; }

        [GameData]
        public static GameTable<ComponentRegionEntry> ComponentRegion { get; private set; }

        [GameData]
        public static GameTable<ComponentRegionRectEntry> ComponentRegionRect { get; private set; }

        [GameData]
        public static GameTable<CostumeSpeciesEntry> CostumeSpecies { get; private set; }

        [GameData]
        public static GameTable<Creature2Entry> Creature2 { get; private set; }

        [GameData]
        public static GameTable<Creature2ActionEntry> Creature2Action { get; private set; }

        [GameData]
        public static GameTable<Creature2ActionSetEntry> Creature2ActionSet { get; private set; }

        [GameData]
        public static GameTable<Creature2ActionTextEntry> Creature2ActionText { get; private set; }

        [GameData]
        public static GameTable<Creature2AffiliationEntry> Creature2Affiliation { get; private set; }

        [GameData]
        public static GameTable<Creature2ArcheTypeEntry> Creature2ArcheType { get; private set; }

        [GameData]
        public static GameTable<Creature2DifficultyEntry> Creature2Difficulty { get; private set; }

        [GameData]
        public static GameTable<Creature2DisplayGroupEntryEntry> Creature2DisplayGroupEntry { get; private set; }

        [GameData]
        public static GameTable<Creature2DisplayInfoEntry> Creature2DisplayInfo { get; private set; }

        [GameData]
        public static GameTable<Creature2ModelInfoEntry> Creature2ModelInfo { get; private set; }

        [GameData]
        public static GameTable<Creature2OutfitGroupEntryEntry> Creature2OutfitGroupEntry { get; private set; }

        [GameData]
        public static GameTable<Creature2OutfitInfoEntry> Creature2OutfitInfo { get; private set; }

        [GameData]
        public static GameTable<Creature2OverridePropertiesEntry> Creature2OverrideProperties { get; private set; }

        [GameData]
        public static GameTable<Creature2ResistEntry> Creature2Resist { get; private set; }

        [GameData]
        public static GameTable<Creature2TierEntry> Creature2Tier { get; private set; }

        [GameData]
        public static GameTable<CreatureLevelEntry> CreatureLevel { get; private set; }

        [GameData]
        public static GameTable<CurrencyTypeEntry> CurrencyType { get; private set; }

        public static GameTable<CustomerSurveyEntry> CustomerSurvey { get; private set; }
        public static GameTable<CustomizationParameterEntry> CustomizationParameter { get; private set; }
        public static GameTable<CustomizationParameterMapEntry> CustomizationParameterMap { get; private set; }
        public static GameTable<DailyLoginRewardEntry> DailyLoginReward { get; private set; }

        [GameData]
        public static GameTable<DatacubeEntry> Datacube { get; private set; }

        [GameData]
        public static GameTable<DatacubeVolumeEntry> DatacubeVolume { get; private set; }

        public static GameTable<DistanceDamageModifierEntry> DistanceDamageModifier { get; private set; }

        [GameData]
        public static GameTable<DyeColorRampEntry> DyeColorRamp { get; private set; }

        [GameData]
        public static GameTable<EldanAugmentationEntry> EldanAugmentation { get; private set; }
        public static GameTable<EldanAugmentationCategoryEntry> EldanAugmentationCategory { get; private set; }
        public static GameTable<EmoteSequenceTransitionEntry> EmoteSequenceTransition { get; private set; }

        [GameData]
        public static GameTable<EmotesEntry> Emotes { get; private set; }

        [GameData]
        public static GameTable<EntitlementEntry> Entitlement { get; private set; }

        public static GameTable<EpisodeEntry> Episode { get; private set; }
        public static GameTable<EpisodeQuestEntry> EpisodeQuest { get; private set; }

        [GameData]
        public static GameTable<Faction2Entry> Faction2 { get; private set; }

        public static GameTable<Faction2RelationshipEntry> Faction2Relationship { get; private set; }
        public static GameTable<FinishingMoveDeathVisualEntry> FinishingMoveDeathVisual { get; private set; }
        public static GameTable<FullScreenEffectEntry> FullScreenEffect { get; private set; }

        [GameData]
        public static GameTable<GameFormulaEntry> GameFormula { get; private set; }

        public static GameTable<GenericMapEntry> GenericMap { get; private set; }
        public static GameTable<GenericMapNodeEntry> GenericMapNode { get; private set; }
        public static GameTable<GenericStringGroupsEntry> GenericStringGroups { get; private set; }

        [GameData]
        public static GameTable<GenericUnlockEntryEntry> GenericUnlockEntry { get; private set; }

        public static GameTable<GenericUnlockSetEntry> GenericUnlockSet { get; private set; }
        public static GameTable<GossipEntryEntry> GossipEntry { get; private set; }
        public static GameTable<GossipSetEntry> GossipSet { get; private set; }
        public static GameTable<GuildPerkEntry> GuildPerk { get; private set; }
        public static GameTable<GuildPermissionEntry> GuildPermission { get; private set; }
        public static GameTable<GuildStandardPartEntry> GuildStandardPart { get; private set; }
        public static GameTable<HazardEntry> Hazard { get; private set; }
        public static GameTable<HookAssetEntry> HookAsset { get; private set; }
        public static GameTable<HookTypeEntry> HookType { get; private set; }
        public static GameTable<HousingBuildEntry> HousingBuild { get; private set; }
        public static GameTable<HousingContributionInfoEntry> HousingContributionInfo { get; private set; }
        public static GameTable<HousingContributionTypeEntry> HousingContributionType { get; private set; }

        [GameData]
        public static GameTable<HousingDecorInfoEntry> HousingDecorInfo { get; private set; }

        public static GameTable<HousingDecorLimitCategoryEntry> HousingDecorLimitCategory { get; private set; }
        public static GameTable<HousingDecorTypeEntry> HousingDecorType { get; private set; }
        public static GameTable<HousingMannequinPoseEntry> HousingMannequinPose { get; private set; }
        public static GameTable<HousingMapInfoEntry> HousingMapInfo { get; private set; }
        public static GameTable<HousingNeighborhoodInfoEntry> HousingNeighborhoodInfo { get; private set; }

        [GameData]
        public static GameTable<HousingPlotInfoEntry> HousingPlotInfo { get; private set; }

        public static GameTable<HousingPlotTypeEntry> HousingPlotType { get; private set; }

        [GameData]
        public static GameTable<HousingPlugItemEntry> HousingPlugItem { get; private set; }

        [GameData]
        public static GameTable<HousingPropertyInfoEntry> HousingPropertyInfo { get; private set; }

        [GameData]
        public static GameTable<HousingResidenceInfoEntry> HousingResidenceInfo { get; private set; }

        public static GameTable<HousingResourceEntry> HousingResource { get; private set; }

        [GameData]
        public static GameTable<HousingWallpaperInfoEntry> HousingWallpaperInfo { get; private set; }

        public static GameTable<HousingWarplotBossTokenEntry> HousingWarplotBossToken { get; private set; }
        public static GameTable<HousingWarplotPlugInfoEntry> HousingWarplotPlugInfo { get; private set; }
        public static GameTable<InputActionEntry> InputAction { get; private set; }
        public static GameTable<InputActionCategoryEntry> InputActionCategory { get; private set; }
        public static GameTable<InstancePortalEntry> InstancePortal { get; private set; }

        [GameData("Item2.tbl")]
        public static GameTable<Item2Entry> Item { get; private set; }

        public static GameTable<Item2CategoryEntry> Item2Category { get; private set; }
        public static GameTable<Item2FamilyEntry> Item2Family { get; private set; }

        [GameData("Item2Type.tbl")]
        public static GameTable<Item2TypeEntry> ItemType { get; private set; }

        public static GameTable<ItemBudgetEntry> ItemBudget { get; private set; }
        public static GameTable<ItemColorSetEntry> ItemColorSet { get; private set; }

        [GameData]
        public static GameTable<ItemDisplayEntry> ItemDisplay { get; private set; }

        [GameData]
        public static GameTable<ItemDisplaySourceEntryEntry> ItemDisplaySourceEntry { get; private set; }

        public static GameTable<ItemImbuementEntry> ItemImbuement { get; private set; }
        public static GameTable<ItemImbuementRewardEntry> ItemImbuementReward { get; private set; }
        public static GameTable<ItemProficiencyEntry> ItemProficiency { get; private set; }
        public static GameTable<ItemQualityEntry> ItemQuality { get; private set; }
        public static GameTable<ItemRandomStatEntry> ItemRandomStat { get; private set; }
        public static GameTable<ItemRandomStatGroupEntry> ItemRandomStatGroup { get; private set; }
        public static GameTable<ItemRuneInstanceEntry> ItemRuneInstance { get; private set; }
        public static GameTable<ItemSetEntry> ItemSet { get; private set; }
        public static GameTable<ItemSetBonusEntry> ItemSetBonus { get; private set; }
        public static GameTable<ItemSlotEntry> ItemSlot { get; private set; }

        [GameData]
        public static GameTable<ItemSpecialEntry> ItemSpecial { get; private set; }

        public static GameTable<ItemStatEntry> ItemStat { get; private set; }
        public static GameTable<LanguageEntry> Language { get; private set; }
        public static GameTable<LevelDifferentialAttributeEntry> LevelDifferentialAttribute { get; private set; }
        public static GameTable<LevelUpUnlockEntry> LevelUpUnlock { get; private set; }
        public static GameTable<LevelUpUnlockTypeEntry> LevelUpUnlockType { get; private set; }
        public static GameTable<LiveEventEntry> LiveEvent { get; private set; }
        public static GameTable<LiveEventDisplayItemEntry> LiveEventDisplayItem { get; private set; }
        public static GameTable<LoadingScreenTipEntry> LoadingScreenTip { get; private set; }
        public static GameTable<LocalizedEnumEntry> LocalizedEnum { get; private set; }

        [GameData]
        public static GameTable<LocalizedTextEntry> LocalizedText { get; private set; }

        public static GameTable<LootPinataInfoEntry> LootPinataInfo { get; private set; }
        public static GameTable<LootSpellEntry> LootSpell { get; private set; }
        public static GameTable<LuaEventEntry> LuaEvent { get; private set; }
        public static GameTable<MapContinentEntry> MapContinent { get; private set; }

        [GameData]
        public static GameTable<MapZoneEntry> MapZone { get; private set; }

        public static GameTable<MapZoneHexEntry> MapZoneHex { get; private set; }

        [GameData]
        public static GameTable<MapZoneHexGroupEntry> MapZoneHexGroup { get; private set; }

        [GameData]
        public static GameTable<MapZoneHexGroupEntryEntry> MapZoneHexGroupEntry { get; private set; }
        public static GameTable<MapZoneNemesisRegionEntry> MapZoneNemesisRegion { get; private set; }
        public static GameTable<MapZonePOIEntry> MapZonePOI { get; private set; }
        public static GameTable<MapZoneSpriteEntry> MapZoneSprite { get; private set; }

        [GameData]
        public static GameTable<MapZoneWorldJoinEntry> MapZoneWorldJoin { get; private set; }
        public static GameTable<MatchTypeRewardRotationContentEntry> MatchTypeRewardRotationContent { get; private set; }
        public static GameTable<MatchingGameMapEntry> MatchingGameMap { get; private set; }
        public static GameTable<MatchingGameTypeEntry> MatchingGameType { get; private set; }
        public static GameTable<MatchingRandomRewardEntry> MatchingRandomReward { get; private set; }
        public static GameTable<MaterialDataEntry> MaterialData { get; private set; }
        public static GameTable<MaterialRemapEntry> MaterialRemap { get; private set; }
        public static GameTable<MaterialSetEntry> MaterialSet { get; private set; }
        public static GameTable<MaterialTypeEntry> MaterialType { get; private set; }
        public static GameTable<MiniMapMarkerEntry> MiniMapMarker { get; private set; }
        public static GameTable<MissileRevolverTrackEntry> MissileRevolverTrack { get; private set; }
        public static GameTable<ModelAttachmentEntry> ModelAttachment { get; private set; }
        public static GameTable<ModelBoneEntry> ModelBone { get; private set; }
        public static GameTable<ModelBonePriorityEntry> ModelBonePriority { get; private set; }
        public static GameTable<ModelBoneSetEntry> ModelBoneSet { get; private set; }
        public static GameTable<ModelCameraEntry> ModelCamera { get; private set; }
        public static GameTable<ModelClusterEntry> ModelCluster { get; private set; }
        public static GameTable<ModelEventEntry> ModelEvent { get; private set; }
        public static GameTable<ModelEventVisualJoinEntry> ModelEventVisualJoin { get; private set; }
        public static GameTable<ModelMeshEntry> ModelMesh { get; private set; }
        public static GameTable<ModelPoseEntry> ModelPose { get; private set; }
        public static GameTable<ModelSequenceEntry> ModelSequence { get; private set; }
        public static GameTable<ModelSequenceByModeEntry> ModelSequenceByMode { get; private set; }
        public static GameTable<ModelSequenceBySeatPostureEntry> ModelSequenceBySeatPosture { get; private set; }
        public static GameTable<ModelSequenceByWeaponEntry> ModelSequenceByWeapon { get; private set; }
        public static GameTable<ModelSequenceTransitionEntry> ModelSequenceTransition { get; private set; }
        public static GameTable<ModelSkinFXEntry> ModelSkinFX { get; private set; }
        public static GameTable<PathEpisodeEntry> PathEpisode { get; private set; }
        public static GameTable<PathExplorerActivateEntry> PathExplorerActivate { get; private set; }
        public static GameTable<PathExplorerAreaEntry> PathExplorerArea { get; private set; }
        public static GameTable<PathExplorerDoorEntry> PathExplorerDoor { get; private set; }
        public static GameTable<PathExplorerDoorEntranceEntry> PathExplorerDoorEntrance { get; private set; }
        public static GameTable<PathExplorerNodeEntry> PathExplorerNode { get; private set; }
        public static GameTable<PathExplorerPowerMapEntry> PathExplorerPowerMap { get; private set; }
        public static GameTable<PathExplorerScavengerClueEntry> PathExplorerScavengerClue { get; private set; }
        public static GameTable<PathExplorerScavengerHuntEntry> PathExplorerScavengerHunt { get; private set; }

        [GameData]
        public static GameTable<PathLevelEntry> PathLevel { get; private set; }

        public static GameTable<PathMissionEntry> PathMission { get; private set; }

        [GameData]
        public static GameTable<PathRewardEntry> PathReward { get; private set; }

        public static GameTable<PathScientistCreatureInfoEntry> PathScientistCreatureInfo { get; private set; }
        public static GameTable<PathScientistDatacubeDiscoveryEntry> PathScientistDatacubeDiscovery { get; private set; }
        public static GameTable<PathScientistExperimentationEntry> PathScientistExperimentation { get; private set; }
        public static GameTable<PathScientistExperimentationPatternEntry> PathScientistExperimentationPattern { get; private set; }
        public static GameTable<PathScientistFieldStudyEntry> PathScientistFieldStudy { get; private set; }
        public static GameTable<PathScientistScanBotProfileEntry> PathScientistScanBotProfile { get; private set; }
        public static GameTable<PathScientistSpecimenSurveyEntry> PathScientistSpecimenSurvey { get; private set; }
        public static GameTable<PathSettlerHubEntry> PathSettlerHub { get; private set; }
        public static GameTable<PathSettlerImprovementEntry> PathSettlerImprovement { get; private set; }
        public static GameTable<PathSettlerImprovementGroupEntry> PathSettlerImprovementGroup { get; private set; }
        public static GameTable<PathSettlerInfrastructureEntry> PathSettlerInfrastructure { get; private set; }
        public static GameTable<PathSettlerMayorEntry> PathSettlerMayor { get; private set; }
        public static GameTable<PathSettlerSheriffEntry> PathSettlerSheriff { get; private set; }
        public static GameTable<PathSoldierActivateEntry> PathSoldierActivate { get; private set; }
        public static GameTable<PathSoldierAssassinateEntry> PathSoldierAssassinate { get; private set; }
        public static GameTable<PathSoldierEventEntry> PathSoldierEvent { get; private set; }
        public static GameTable<PathSoldierEventWaveEntry> PathSoldierEventWave { get; private set; }
        public static GameTable<PathSoldierSWATEntry> PathSoldierSWAT { get; private set; }
        public static GameTable<PathSoldierTowerDefenseEntry> PathSoldierTowerDefense { get; private set; }
        public static GameTable<PeriodicQuestGroupEntry> PeriodicQuestGroup { get; private set; }
        public static GameTable<PeriodicQuestSetEntry> PeriodicQuestSet { get; private set; }
        public static GameTable<PeriodicQuestSetCategoryEntry> PeriodicQuestSetCategory { get; private set; }

        [GameData]
        public static GameTable<PetFlairEntry> PetFlair { get; private set; }
        public static GameTable<PlayerNotificationTypeEntry> PlayerNotificationType { get; private set; }
        public static GameTable<PositionalRequirementEntry> PositionalRequirement { get; private set; }

        [GameData]
        public static GameTable<PrerequisiteEntry> Prerequisite { get; private set; }

        public static GameTable<PrerequisiteTypeEntry> PrerequisiteType { get; private set; }
        public static GameTable<PrimalMatrixNodeEntry> PrimalMatrixNode { get; private set; }
        public static GameTable<PrimalMatrixRewardEntry> PrimalMatrixReward { get; private set; }
        public static GameTable<PropAdditionalDetailEntry> PropAdditionalDetail { get; private set; }
        public static GameTable<PublicEventEntry> PublicEvent { get; private set; }
        public static GameTable<PublicEventCustomStatEntry> PublicEventCustomStat { get; private set; }
        public static GameTable<PublicEventDepotEntry> PublicEventDepot { get; private set; }
        public static GameTable<PublicEventObjectiveEntry> PublicEventObjective { get; private set; }
        public static GameTable<PublicEventObjectiveBombDeploymentEntry> PublicEventObjectiveBombDeployment { get; private set; }
        public static GameTable<PublicEventObjectiveGatherResourceEntry> PublicEventObjectiveGatherResource { get; private set; }
        public static GameTable<PublicEventObjectiveStateEntry> PublicEventObjectiveState { get; private set; }
        public static GameTable<PublicEventRewardModifierEntry> PublicEventRewardModifier { get; private set; }
        public static GameTable<PublicEventStatDisplayEntry> PublicEventStatDisplay { get; private set; }
        public static GameTable<PublicEventTeamEntry> PublicEventTeam { get; private set; }
        public static GameTable<PublicEventVirtualItemDepotEntry> PublicEventVirtualItemDepot { get; private set; }
        public static GameTable<PublicEventVoteEntry> PublicEventVote { get; private set; }
        public static GameTable<PvPRatingFloorEntry> PvPRatingFloor { get; private set; }

        [GameData]
        public static GameTable<Quest2Entry> Quest2 { get; private set; }

        [GameData]
        public static GameTable<Quest2DifficultyEntry> Quest2Difficulty { get; private set; }

        public static GameTable<Quest2RandomTextLineJoinEntry> Quest2RandomTextLineJoin { get; private set; }

        [GameData]
        public static GameTable<Quest2RewardEntry> Quest2Reward { get; private set; }
        public static GameTable<QuestCategoryEntry> QuestCategory { get; private set; }
        public static GameTable<QuestDirectionEntry> QuestDirection { get; private set; }
        public static GameTable<QuestDirectionEntryEntry> QuestDirectionEntry { get; private set; }
        public static GameTable<QuestGroupEntry> QuestGroup { get; private set; }
        public static GameTable<QuestHubEntry> QuestHub { get; private set; }

        [GameData]
        public static GameTable<QuestObjectiveEntry> QuestObjective { get; private set; }

        [GameData]
        public static GameTable<RaceEntry> Race { get; private set; }

        public static GameTable<RandomPlayerNameEntry> RandomPlayerName { get; private set; }
        public static GameTable<RandomTextLineEntry> RandomTextLine { get; private set; }
        public static GameTable<RandomTextLineSetEntry> RandomTextLineSet { get; private set; }
        public static GameTable<RealmDataCenterEntry> RealmDataCenter { get; private set; }
        public static GameTable<RedactedEntry> Redacted { get; private set; }
        public static GameTable<ReplaceableMaterialInfoEntry> ReplaceableMaterialInfo { get; private set; }
        public static GameTable<ResourceConversionEntry> ResourceConversion { get; private set; }
        public static GameTable<ResourceConversionGroupEntry> ResourceConversionGroup { get; private set; }
        public static GameTable<RewardPropertyEntry> RewardProperty { get; private set; }
        public static GameTable<RewardPropertyPremiumModifierEntry> RewardPropertyPremiumModifier { get; private set; }
        public static GameTable<RewardRotationContentEntry> RewardRotationContent { get; private set; }
        public static GameTable<RewardRotationEssenceEntry> RewardRotationEssence { get; private set; }
        public static GameTable<RewardRotationItemEntry> RewardRotationItem { get; private set; }
        public static GameTable<RewardRotationModifierEntry> RewardRotationModifier { get; private set; }
        public static GameTable<RewardTrackEntry> RewardTrack { get; private set; }
        public static GameTable<RewardTrackRewardsEntry> RewardTrackRewards { get; private set; }
        public static GameTable<SalvageEntry> Salvage { get; private set; }
        public static GameTable<SalvageExceptionEntry> SalvageException { get; private set; }
        public static GameTable<SkyCloudSetEntry> SkyCloudSet { get; private set; }
        public static GameTable<SkyTrackCloudSetEntry> SkyTrackCloudSet { get; private set; }
        public static GameTable<SoundBankEntry> SoundBank { get; private set; }
        public static GameTable<SoundCombatLoopEntry> SoundCombatLoop { get; private set; }
        public static GameTable<SoundContextEntry> SoundContext { get; private set; }
        public static GameTable<SoundDirectionalAmbienceEntry> SoundDirectionalAmbience { get; private set; }
        public static GameTable<SoundEnvironmentEntry> SoundEnvironment { get; private set; }
        public static GameTable<SoundEventEntry> SoundEvent { get; private set; }
        public static GameTable<SoundImpactEventsEntry> SoundImpactEvents { get; private set; }
        public static GameTable<SoundMusicSetEntry> SoundMusicSet { get; private set; }
        public static GameTable<SoundParameterEntry> SoundParameter { get; private set; }
        public static GameTable<SoundStatesEntry> SoundStates { get; private set; }
        public static GameTable<SoundSwitchEntry> SoundSwitch { get; private set; }
        public static GameTable<SoundUIContextEntry> SoundUIContext { get; private set; }
        public static GameTable<SoundZoneKitEntry> SoundZoneKit { get; private set; }

        [GameData]
        public static GameTable<Spell4Entry> Spell4 { get; private set; }

        [GameData]
        public static GameTable<Spell4AoeTargetConstraintsEntry> Spell4AoeTargetConstraints { get; private set; }

        [GameData]
        public static GameTable<Spell4BaseEntry> Spell4Base { get; private set; }

        [GameData]
        public static GameTable<Spell4CCConditionsEntry> Spell4CCConditions { get; private set; }
        public static GameTable<Spell4CastResultEntry> Spell4CastResult { get; private set; }
        public static GameTable<Spell4ClientMissileEntry> Spell4ClientMissile { get; private set; }

        [GameData]
        public static GameTable<Spell4ConditionsEntry> Spell4Conditions { get; private set; }

        public static GameTable<Spell4EffectGroupListEntry> Spell4EffectGroupList { get; private set; }
        public static GameTable<Spell4EffectModificationEntry> Spell4EffectModification { get; private set; }

        [GameData]
        public static GameTable<Spell4EffectsEntry> Spell4Effects { get; private set; }

        public static GameTable<Spell4GroupListEntry> Spell4GroupList { get; private set; }

        [GameData]
        public static GameTable<Spell4HitResultsEntry> Spell4HitResults { get; private set; }

        public static GameTable<Spell4ModificationEntry> Spell4Modification { get; private set; }

        [GameData]
        public static GameTable<Spell4PrerequisitesEntry> Spell4Prerequisites { get; private set; }
        public static GameTable<Spell4ReagentEntry> Spell4Reagent { get; private set; }
        public static GameTable<Spell4RunnerEntry> Spell4Runner { get; private set; }
        public static GameTable<Spell4ServiceTokenCostEntry> Spell4ServiceTokenCost { get; private set; }

        [GameData]
        public static GameTable<Spell4SpellTypesEntry> Spell4SpellTypes { get; private set; }

        [GameData]
        public static GameTable<Spell4StackGroupEntry> Spell4StackGroup { get; private set; }

        public static GameTable<Spell4TagEntry> Spell4Tag { get; private set; }

        [GameData]
        public static GameTable<Spell4TargetAngleEntry> Spell4TargetAngle { get; private set; }

        [GameData]
        public static GameTable<Spell4TargetMechanicsEntry> Spell4TargetMechanics { get; private set; }

        [GameData]
        public static GameTable<Spell4TelegraphEntry> Spell4Telegraph { get; private set; }

        public static GameTable<Spell4ThresholdsEntry> Spell4Thresholds { get; private set; }
        public static GameTable<Spell4TierRequirementsEntry> Spell4TierRequirements { get; private set; }

        [GameData]
        public static GameTable<Spell4ValidTargetsEntry> Spell4ValidTargets { get; private set; }

        public static GameTable<Spell4VisualEntry> Spell4Visual { get; private set; }
        public static GameTable<Spell4VisualGroupEntry> Spell4VisualGroup { get; private set; }

        [GameData]
        public static GameTable<SpellCoolDownEntry> SpellCoolDown { get; private set; }

        public static GameTable<SpellEffectTypeEntry> SpellEffectType { get; private set; }

        [GameData]
        public static GameTable<SpellLevelEntry> SpellLevel { get; private set; }
        public static GameTable<SpellPhaseEntry> SpellPhase { get; private set; }

        [GameData]
        public static GameTable<Spline2Entry> Spline2 { get; private set; }

        [GameData]
        public static GameTable<Spline2NodeEntry> Spline2Node { get; private set; }

        public static GameTable<StoreDisplayInfoEntry> StoreDisplayInfo { get; private set; }
        public static GameTable<StoreKeywordEntry> StoreKeyword { get; private set; }
        public static GameTable<StoreLinkEntry> StoreLink { get; private set; }
        public static GameTable<StoryPanelEntry> StoryPanel { get; private set; }

        [GameData]
        public static GameTable<TargetGroupEntry> TargetGroup { get; private set; }

        public static GameTable<TargetMarkerEntry> TargetMarker { get; private set; }

        [GameData]
        public static GameTable<TaxiNodeEntry> TaxiNode { get; private set; }
        public static GameTable<TaxiRouteEntry> TaxiRoute { get; private set; }

        [GameData]
        public static GameTable<TelegraphDamageEntry> TelegraphDamage { get; private set; }

        public static GameTable<TicketCategoryEntry> TicketCategory { get; private set; }
        public static GameTable<TicketSubCategoryEntry> TicketSubCategory { get; private set; }
        public static GameTable<TrackingSlotEntry> TrackingSlot { get; private set; }
        public static GameTable<TradeskillEntry> Tradeskill { get; private set; }
        public static GameTable<TradeskillAchievementLayoutEntry> TradeskillAchievementLayout { get; private set; }
        public static GameTable<TradeskillAchievementRewardEntry> TradeskillAchievementReward { get; private set; }
        public static GameTable<TradeskillAdditiveEntry> TradeskillAdditive { get; private set; }
        public static GameTable<TradeskillBonusEntry> TradeskillBonus { get; private set; }
        public static GameTable<TradeskillCatalystEntry> TradeskillCatalyst { get; private set; }
        public static GameTable<TradeskillCatalystOrderingEntry> TradeskillCatalystOrdering { get; private set; }
        public static GameTable<TradeskillHarvestingInfoEntry> TradeskillHarvestingInfo { get; private set; }
        public static GameTable<TradeskillMaterialEntry> TradeskillMaterial { get; private set; }
        public static GameTable<TradeskillMaterialCategoryEntry> TradeskillMaterialCategory { get; private set; }
        public static GameTable<TradeskillProficiencyEntry> TradeskillProficiency { get; private set; }
        public static GameTable<TradeskillSchematic2Entry> TradeskillSchematic2 { get; private set; }
        public static GameTable<TradeskillTalentTierEntry> TradeskillTalentTier { get; private set; }
        public static GameTable<TradeskillTierEntry> TradeskillTier { get; private set; }
        public static GameTable<TutorialEntry> Tutorial { get; private set; }
        public static GameTable<TutorialAnchorEntry> TutorialAnchor { get; private set; }
        public static GameTable<TutorialLayoutEntry> TutorialLayout { get; private set; }
        public static GameTable<TutorialPageEntry> TutorialPage { get; private set; }
        public static GameTable<UnitProperty2Entry> UnitProperty2 { get; private set; }

        [GameData]
        public static GameTable<UnitRaceEntry> UnitRace { get; private set; }

        [GameData]
        public static GameTable<UnitVehicleEntry> UnitVehicle { get; private set; }

        public static GameTable<VeteranTierEntry> VeteranTier { get; private set; }
        public static GameTable<VirtualItemEntry> VirtualItem { get; private set; }
        public static GameTable<VisualEffectEntry> VisualEffect { get; private set; }
        public static GameTable<VitalEntry> Vital { get; private set; }
        public static GameTable<WaterSurfaceEffectEntry> WaterSurfaceEffect { get; private set; }

        [GameData]
        public static GameTable<WindEntry> Wind { get; private set; }

        public static GameTable<WindSpawnEntry> WindSpawn { get; private set; }
        public static GameTable<WordFilterEntry> WordFilter { get; private set; }

        [GameData]
        public static GameTable<WorldEntry> World { get; private set; }

        public static GameTable<WorldClutterEntry> WorldClutter { get; private set; }
        public static GameTable<WorldLayerEntry> WorldLayer { get; private set; }

        [GameData]
        public static GameTable<WorldLocation2Entry> WorldLocation2 { get; private set; }

        public static GameTable<WorldSkyEntry> WorldSky { get; private set; }
        public static GameTable<WorldSocketEntry> WorldSocket { get; private set; }
        public static GameTable<WorldWaterEnvironmentEntry> WorldWaterEnvironment { get; private set; }
        public static GameTable<WorldWaterFogEntry> WorldWaterFog { get; private set; }
        public static GameTable<WorldWaterLayerEntry> WorldWaterLayer { get; private set; }
        public static GameTable<WorldWaterTypeEntry> WorldWaterType { get; private set; }
        public static GameTable<WorldWaterWakeEntry> WorldWaterWake { get; private set; }

        [GameData]
        public static GameTable<WorldZoneEntry> WorldZone { get; private set; }

        [GameData]
        public static GameTable<XpPerLevelEntry> XpPerLevel { get; private set; }

        public static GameTable<ZoneCompletionEntry> ZoneCompletion { get; private set; }

        [GameData("fr-FR.bin")]
        public static TextTable TextFrench { get; private set; }
        [GameData("en-US.bin")]
        public static TextTable TextEnglish { get; private set; }
        [GameData("de-DE.bin")]
        public static TextTable TextGerman { get; private set; }

        public static void Initialise()
        {
            log.Info("Loading GameTables...");

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                LoadGameTablesAsync().GetAwaiter().GetResult();
                Debug.Assert(WorldLocation2 != null);
            }
            catch (Exception exception)
            {
                log.Fatal(exception);
                throw;
            }

            log.Info($"Loaded GameTables in {sw.ElapsedMilliseconds}ms.");
        }

        private static async Task LoadGameTablesAsync()
        {
            List<Exception> exceptions = new List<Exception>();
            int loadCount = Environment.ProcessorCount * 2;
            if (loadCount < minimumThreads)
                loadCount = minimumThreads;
            if (loadCount > maximumThreads)
                loadCount = maximumThreads;

            List<Task> tasks = new List<Task>();
            async Task WaitForNextTaskToFinish()
            {
                Task next = await Task.WhenAny(tasks);
                tasks.Remove(next);
            }

            async Task<bool> ExceptionHandler(Task task)
            {
                try
                {
                    await task;
                    return true;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    return false;
                }
            }

            string GetFilename(PropertyInfo property)
            {
                GameDataAttribute attribute = property.GetCustomAttribute<GameDataAttribute>();
                if (attribute == null)
                    return null;

                string fileName = attribute.FileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = property.Name;
                    if (property.PropertyType == typeof(TextTable))
                        fileName = Path.ChangeExtension(fileName, "bin");
                    else if (property.PropertyType.GetGenericTypeDefinition() == typeof(GameTable<>))
                        fileName = Path.ChangeExtension(fileName, "tbl");
                }

                return fileName;
            }

            List<PropertyInfo> properties = new List<PropertyInfo>();
            // It's done this way so we load the text first, because it's huge.
            foreach (PropertyInfo property in typeof(GameTableManager).GetProperties())
            {
                GameDataAttribute attribute = property.GetCustomAttribute<GameDataAttribute>();
                if (attribute == null)
                    continue;
                if (property.PropertyType == typeof(TextTable))
                    properties.Insert(0, property);
                else
                    properties.Add(property);
            }

            foreach (PropertyInfo property in properties)
            {
                string fileName = GetFilename(property);

                DateTime loadStarted = DateTime.Now;

                tasks.Add(LoadGameTableAsync(property, fileName)
                    .ContinueWith(ExceptionHandler)
                    .Unwrap()
                    .ContinueWith(
                        async task =>
                        {
                            bool result = await task;
                            if (result)
                                log.Info("Completed loading {0} in {1}ms", fileName,
                                    (DateTime.Now - loadStarted).TotalMilliseconds);
                            else
                                log.Error("Failed to load {0} in {1}ms", fileName,
                                    (DateTime.Now - loadStarted).TotalMilliseconds);
                        }).Unwrap());

                if (tasks.Count > loadCount)
                    await WaitForNextTaskToFinish();
            }

            while (tasks.Count > 0)
                await WaitForNextTaskToFinish();

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        private static Task LoadGameTableAsync(PropertyInfo property, string fileName)
        {
            async Task SetPropertyOnCompletion(Task<object> task)
            {
                property.SetValue(null, await task);
            }

            async Task VerifyPropertySetOnCompletion(Task task)
            {
                await task;
                if (property.GetValue(null) == null)
                    throw new InvalidOperationException($"Failed to load game data table {Path.GetFileName(fileName)}");
            }

            if (property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(GameTable<>))
                return Task.Factory.StartNew(() =>
                        GameTableFactory.LoadGameTable(property.PropertyType.GetGenericArguments().Single(), fileName))
                    .ContinueWith(SetPropertyOnCompletion)
                    .Unwrap()
                    .ContinueWith(VerifyPropertySetOnCompletion)
                    .Unwrap();

            if (property.PropertyType == typeof(TextTable))
                return Task.Factory.StartNew<object>(() => GameTableFactory.LoadTextTable(fileName))
                    .ContinueWith(SetPropertyOnCompletion)
                    .Unwrap();

            throw new GameTableException($"Unknown game table type {property.PropertyType}");
        }

        /// <summary>
        /// Return the <see cref="TextTable"/> for the specified <see cref="Static.Language"/>.
        /// </summary>
        public static TextTable GetTextTable(Language language)
        {
            switch (language)
            {
                case Static.Language.English:
                    return TextEnglish;
                case Static.Language.French:
                    return TextFrench;
                case Static.Language.German:
                    return TextGerman;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
