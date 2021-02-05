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
    public sealed class GameTableManager : Singleton<GameTableManager>
    {
        private const int minimumThreads = 2;
        private const int maximumThreads = 16;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [GameData]
        public GameTable<AccountCurrencyTypeEntry> AccountCurrencyType { get; private set; }

        [GameData]
        public GameTable<AccountItemEntry> AccountItem { get; private set; }

        [GameData]
        public GameTable<AccountItemCooldownGroupEntry> AccountItemCooldownGroup { get; private set; }

        [GameData]
        public GameTable<AchievementEntry> Achievement { get; private set; }

        [GameData]
        public GameTable<AchievementCategoryEntry> AchievementCategory { get; private set; }

        [GameData]
        public GameTable<AchievementChecklistEntry> AchievementChecklist { get; private set; }

        [GameData]
        public GameTable<AchievementGroupEntry> AchievementGroup { get; private set; }

        [GameData]
        public GameTable<AchievementSubGroupEntry> AchievementSubGroup { get; private set; }

        public GameTable<AchievementTextEntry> AchievementText { get; private set; }
        public GameTable<ActionBarShortcutSetEntry> ActionBarShortcutSet { get; private set; }
        public GameTable<ActionSlotPrereqEntry> ActionSlotPrereq { get; private set; }
        public GameTable<ArchiveArticleEntry> ArchiveArticle { get; private set; }
        public GameTable<ArchiveCategoryEntry> ArchiveCategory { get; private set; }
        public GameTable<ArchiveEntryEntry> ArchiveEntry { get; private set; }
        public GameTable<ArchiveEntryUnlockRuleEntry> ArchiveEntryUnlockRule { get; private set; }
        public GameTable<ArchiveLinkEntry> ArchiveLink { get; private set; }
        public GameTable<AttributeMilestoneGroupEntry> AttributeMilestoneGroup { get; private set; }
        public GameTable<AttributeMiniMilestoneGroupEntry> AttributeMiniMilestoneGroup { get; private set; }
        public GameTable<BindPointEntry> BindPoint { get; private set; }
        public GameTable<BinkMovieEntry> BinkMovie { get; private set; }
        public GameTable<BinkMovieSubtitleEntry> BinkMovieSubtitle { get; private set; }
        public GameTable<BugCategoryEntry> BugCategory { get; private set; }
        public GameTable<BugSubcategoryEntry> BugSubcategory { get; private set; }
        public GameTable<CCStateAdditionalDataEntry> CCStateAdditionalData { get; private set; }
        public GameTable<CCStateDiminishingReturnsEntry> CCStateDiminishingReturns { get; private set; }
        public GameTable<CCStatesEntry> CCStates { get; private set; }
        public GameTable<ChallengeEntry> Challenge { get; private set; }
        public GameTable<ChallengeTierEntry> ChallengeTier { get; private set; }

        [GameData]
        public GameTable<CharacterCreationEntry> CharacterCreation { get; private set; }

        public GameTable<CharacterCreationArmorSetEntry> CharacterCreationArmorSet { get; private set; }
        public GameTable<CharacterCreationPresetEntry> CharacterCreationPreset { get; private set; }

        [GameData]
        public GameTable<CharacterCustomizationEntry> CharacterCustomization { get; private set; }

        public GameTable<CharacterCustomizationLabelEntry> CharacterCustomizationLabel { get; private set; }
        public GameTable<CharacterCustomizationSelectionEntry> CharacterCustomizationSelection { get; private set; }

        [GameData]
        public GameTable<CharacterTitleEntry> CharacterTitle { get; private set; }

        public GameTable<CharacterTitleCategoryEntry> CharacterTitleCategory { get; private set; }
        public GameTable<ChatChannelEntry> ChatChannel { get; private set; }
        public GameTable<CinematicEntry> Cinematic { get; private set; }
        public GameTable<CinematicRaceEntry> CinematicRace { get; private set; }
        public GameTable<CityDirectionEntry> CityDirection { get; private set; }

        [GameData]
        public GameTable<ClassEntry> Class { get; private set; }

        public GameTable<ClassSecondaryStatBonusEntry> ClassSecondaryStatBonus { get; private set; }

        [GameData]
        public GameTable<ClientEventEntry> ClientEvent { get; private set; }

        [GameData]
        public GameTable<ClientEventActionEntry> ClientEventAction { get; private set; }

        [GameData]
        public GameTable<ClientSideInteractionEntry> ClientSideInteraction { get; private set; }

        [GameData]
        public GameTable<ColorShiftEntry> ColorShift { get; private set; }

        [GameData]
        public GameTable<CombatRewardEntry> CombatReward { get; private set; }

        [GameData]
        public GameTable<CommunicatorMessagesEntry> CommunicatorMessages { get; private set; }

        [GameData]
        public GameTable<ComponentRegionEntry> ComponentRegion { get; private set; }

        [GameData]
        public GameTable<ComponentRegionRectEntry> ComponentRegionRect { get; private set; }

        [GameData]
        public GameTable<CostumeSpeciesEntry> CostumeSpecies { get; private set; }

        [GameData]
        public GameTable<Creature2Entry> Creature2 { get; private set; }

        [GameData]
        public GameTable<Creature2ActionEntry> Creature2Action { get; private set; }

        [GameData]
        public GameTable<Creature2ActionSetEntry> Creature2ActionSet { get; private set; }

        [GameData]
        public GameTable<Creature2ActionTextEntry> Creature2ActionText { get; private set; }

        [GameData]
        public GameTable<Creature2AffiliationEntry> Creature2Affiliation { get; private set; }

        [GameData]
        public GameTable<Creature2ArcheTypeEntry> Creature2ArcheType { get; private set; }

        [GameData]
        public GameTable<Creature2DifficultyEntry> Creature2Difficulty { get; private set; }

        [GameData]
        public GameTable<Creature2DisplayGroupEntryEntry> Creature2DisplayGroupEntry { get; private set; }

        [GameData]
        public GameTable<Creature2DisplayInfoEntry> Creature2DisplayInfo { get; private set; }

        [GameData]
        public GameTable<Creature2ModelInfoEntry> Creature2ModelInfo { get; private set; }

        [GameData]
        public GameTable<Creature2OutfitGroupEntryEntry> Creature2OutfitGroupEntry { get; private set; }

        [GameData]
        public GameTable<Creature2OutfitInfoEntry> Creature2OutfitInfo { get; private set; }

        [GameData]
        public GameTable<Creature2OverridePropertiesEntry> Creature2OverrideProperties { get; private set; }

        [GameData]
        public GameTable<Creature2ResistEntry> Creature2Resist { get; private set; }

        [GameData]
        public GameTable<Creature2TierEntry> Creature2Tier { get; private set; }

        [GameData]
        public GameTable<CreatureLevelEntry> CreatureLevel { get; private set; }

        [GameData]
        public GameTable<CurrencyTypeEntry> CurrencyType { get; private set; }

        public GameTable<CustomerSurveyEntry> CustomerSurvey { get; private set; }
        public GameTable<CustomizationParameterEntry> CustomizationParameter { get; private set; }
        public GameTable<CustomizationParameterMapEntry> CustomizationParameterMap { get; private set; }
        public GameTable<DailyLoginRewardEntry> DailyLoginReward { get; private set; }

        [GameData]
        public GameTable<DatacubeEntry> Datacube { get; private set; }

        [GameData]
        public GameTable<DatacubeVolumeEntry> DatacubeVolume { get; private set; }

        public GameTable<DistanceDamageModifierEntry> DistanceDamageModifier { get; private set; }

        [GameData]
        public GameTable<DyeColorRampEntry> DyeColorRamp { get; private set; }

        [GameData]
        public GameTable<EldanAugmentationEntry> EldanAugmentation { get; private set; }
        public GameTable<EldanAugmentationCategoryEntry> EldanAugmentationCategory { get; private set; }
        public GameTable<EmoteSequenceTransitionEntry> EmoteSequenceTransition { get; private set; }

        [GameData]
        public GameTable<EmotesEntry> Emotes { get; private set; }

        [GameData]
        public GameTable<EntitlementEntry> Entitlement { get; private set; }

        public GameTable<EpisodeEntry> Episode { get; private set; }
        public GameTable<EpisodeQuestEntry> EpisodeQuest { get; private set; }

        [GameData]
        public GameTable<Faction2Entry> Faction2 { get; private set; }

        [GameData]
        public GameTable<Faction2RelationshipEntry> Faction2Relationship { get; private set; }
        
        public GameTable<FinishingMoveDeathVisualEntry> FinishingMoveDeathVisual { get; private set; }
        public GameTable<FullScreenEffectEntry> FullScreenEffect { get; private set; }

        [GameData]
        public GameTable<GameFormulaEntry> GameFormula { get; private set; }

        public GameTable<GenericMapEntry> GenericMap { get; private set; }
        public GameTable<GenericMapNodeEntry> GenericMapNode { get; private set; }
        public GameTable<GenericStringGroupsEntry> GenericStringGroups { get; private set; }

        [GameData]
        public GameTable<GenericUnlockEntryEntry> GenericUnlockEntry { get; private set; }

        public GameTable<GenericUnlockSetEntry> GenericUnlockSet { get; private set; }
        public GameTable<GossipEntryEntry> GossipEntry { get; private set; }
        public GameTable<GossipSetEntry> GossipSet { get; private set; }
        public GameTable<GuildPerkEntry> GuildPerk { get; private set; }
        public GameTable<GuildPermissionEntry> GuildPermission { get; private set; }

        [GameData]
        public GameTable<GuildStandardPartEntry> GuildStandardPart { get; private set; }
        
        public GameTable<HazardEntry> Hazard { get; private set; }
        public GameTable<HookAssetEntry> HookAsset { get; private set; }
        public GameTable<HookTypeEntry> HookType { get; private set; }
        public GameTable<HousingBuildEntry> HousingBuild { get; private set; }
        public GameTable<HousingContributionInfoEntry> HousingContributionInfo { get; private set; }
        public GameTable<HousingContributionTypeEntry> HousingContributionType { get; private set; }

        [GameData]
        public GameTable<HousingDecorInfoEntry> HousingDecorInfo { get; private set; }

        public GameTable<HousingDecorLimitCategoryEntry> HousingDecorLimitCategory { get; private set; }
        public GameTable<HousingDecorTypeEntry> HousingDecorType { get; private set; }
        public GameTable<HousingMannequinPoseEntry> HousingMannequinPose { get; private set; }
        public GameTable<HousingMapInfoEntry> HousingMapInfo { get; private set; }
        public GameTable<HousingNeighborhoodInfoEntry> HousingNeighborhoodInfo { get; private set; }

        [GameData]
        public GameTable<HousingPlotInfoEntry> HousingPlotInfo { get; private set; }

        public GameTable<HousingPlotTypeEntry> HousingPlotType { get; private set; }

        [GameData]
        public GameTable<HousingPlugItemEntry> HousingPlugItem { get; private set; }

        [GameData]
        public GameTable<HousingPropertyInfoEntry> HousingPropertyInfo { get; private set; }

        public GameTable<HousingResidenceInfoEntry> HousingResidenceInfo { get; private set; }
        public GameTable<HousingResourceEntry> HousingResource { get; private set; }

        [GameData]
        public GameTable<HousingWallpaperInfoEntry> HousingWallpaperInfo { get; private set; }

        public GameTable<HousingWarplotBossTokenEntry> HousingWarplotBossToken { get; private set; }
        public GameTable<HousingWarplotPlugInfoEntry> HousingWarplotPlugInfo { get; private set; }
        public GameTable<InputActionEntry> InputAction { get; private set; }
        public GameTable<InputActionCategoryEntry> InputActionCategory { get; private set; }
        public GameTable<InstancePortalEntry> InstancePortal { get; private set; }

        [GameData("Item2.tbl")]
        public GameTable<Item2Entry> Item { get; private set; }

        public GameTable<Item2CategoryEntry> Item2Category { get; private set; }
        public GameTable<Item2FamilyEntry> Item2Family { get; private set; }

        [GameData("Item2Type.tbl")]
        public GameTable<Item2TypeEntry> ItemType { get; private set; }

        public GameTable<ItemBudgetEntry> ItemBudget { get; private set; }
        public GameTable<ItemColorSetEntry> ItemColorSet { get; private set; }

        [GameData]
        public GameTable<ItemDisplayEntry> ItemDisplay { get; private set; }

        [GameData]
        public GameTable<ItemDisplaySourceEntryEntry> ItemDisplaySourceEntry { get; private set; }

        public GameTable<ItemImbuementEntry> ItemImbuement { get; private set; }
        public GameTable<ItemImbuementRewardEntry> ItemImbuementReward { get; private set; }
        public GameTable<ItemProficiencyEntry> ItemProficiency { get; private set; }
        public GameTable<ItemQualityEntry> ItemQuality { get; private set; }
        public GameTable<ItemRandomStatEntry> ItemRandomStat { get; private set; }
        public GameTable<ItemRandomStatGroupEntry> ItemRandomStatGroup { get; private set; }
        public GameTable<ItemRuneInstanceEntry> ItemRuneInstance { get; private set; }
        public GameTable<ItemSetEntry> ItemSet { get; private set; }
        public GameTable<ItemSetBonusEntry> ItemSetBonus { get; private set; }
        public GameTable<ItemSlotEntry> ItemSlot { get; private set; }

        [GameData]
        public GameTable<ItemSpecialEntry> ItemSpecial { get; private set; }

        public GameTable<ItemStatEntry> ItemStat { get; private set; }
        public GameTable<LanguageEntry> Language { get; private set; }
        public GameTable<LevelDifferentialAttributeEntry> LevelDifferentialAttribute { get; private set; }
        
        [GameData]
        public GameTable<LevelUpUnlockEntry> LevelUpUnlock { get; private set; }
        
        [GameData]
        public GameTable<LevelUpUnlockTypeEntry> LevelUpUnlockType { get; private set; }
        
        public GameTable<LiveEventEntry> LiveEvent { get; private set; }
        public GameTable<LiveEventDisplayItemEntry> LiveEventDisplayItem { get; private set; }
        public GameTable<LoadingScreenTipEntry> LoadingScreenTip { get; private set; }
        public GameTable<LocalizedEnumEntry> LocalizedEnum { get; private set; }

        [GameData]
        public GameTable<LocalizedTextEntry> LocalizedText { get; private set; }

        public GameTable<LootPinataInfoEntry> LootPinataInfo { get; private set; }
        public GameTable<LootSpellEntry> LootSpell { get; private set; }
        public GameTable<LuaEventEntry> LuaEvent { get; private set; }
        public GameTable<MapContinentEntry> MapContinent { get; private set; }

        [GameData]
        public GameTable<MapZoneEntry> MapZone { get; private set; }

        public GameTable<MapZoneHexEntry> MapZoneHex { get; private set; }

        [GameData]
        public GameTable<MapZoneHexGroupEntry> MapZoneHexGroup { get; private set; }

        [GameData]
        public GameTable<MapZoneHexGroupEntryEntry> MapZoneHexGroupEntry { get; private set; }
        public GameTable<MapZoneNemesisRegionEntry> MapZoneNemesisRegion { get; private set; }
        public GameTable<MapZonePOIEntry> MapZonePOI { get; private set; }
        public GameTable<MapZoneSpriteEntry> MapZoneSprite { get; private set; }

        [GameData]
        public GameTable<MapZoneWorldJoinEntry> MapZoneWorldJoin { get; private set; }
        public GameTable<MatchTypeRewardRotationContentEntry> MatchTypeRewardRotationContent { get; private set; }
        public GameTable<MatchingGameMapEntry> MatchingGameMap { get; private set; }
        public GameTable<MatchingGameTypeEntry> MatchingGameType { get; private set; }
        public GameTable<MatchingRandomRewardEntry> MatchingRandomReward { get; private set; }
        public GameTable<MaterialDataEntry> MaterialData { get; private set; }
        public GameTable<MaterialRemapEntry> MaterialRemap { get; private set; }
        public GameTable<MaterialSetEntry> MaterialSet { get; private set; }
        public GameTable<MaterialTypeEntry> MaterialType { get; private set; }
        public GameTable<MiniMapMarkerEntry> MiniMapMarker { get; private set; }
        public GameTable<MissileRevolverTrackEntry> MissileRevolverTrack { get; private set; }
        public GameTable<ModelAttachmentEntry> ModelAttachment { get; private set; }
        public GameTable<ModelBoneEntry> ModelBone { get; private set; }
        public GameTable<ModelBonePriorityEntry> ModelBonePriority { get; private set; }
        public GameTable<ModelBoneSetEntry> ModelBoneSet { get; private set; }
        public GameTable<ModelCameraEntry> ModelCamera { get; private set; }
        public GameTable<ModelClusterEntry> ModelCluster { get; private set; }
        public GameTable<ModelEventEntry> ModelEvent { get; private set; }
        public GameTable<ModelEventVisualJoinEntry> ModelEventVisualJoin { get; private set; }
        public GameTable<ModelMeshEntry> ModelMesh { get; private set; }
        public GameTable<ModelPoseEntry> ModelPose { get; private set; }
        public GameTable<ModelSequenceEntry> ModelSequence { get; private set; }
        public GameTable<ModelSequenceByModeEntry> ModelSequenceByMode { get; private set; }
        public GameTable<ModelSequenceBySeatPostureEntry> ModelSequenceBySeatPosture { get; private set; }
        public GameTable<ModelSequenceByWeaponEntry> ModelSequenceByWeapon { get; private set; }
        public GameTable<ModelSequenceTransitionEntry> ModelSequenceTransition { get; private set; }
        public GameTable<ModelSkinFXEntry> ModelSkinFX { get; private set; }
        public GameTable<PathEpisodeEntry> PathEpisode { get; private set; }
        public GameTable<PathExplorerActivateEntry> PathExplorerActivate { get; private set; }
        public GameTable<PathExplorerAreaEntry> PathExplorerArea { get; private set; }
        public GameTable<PathExplorerDoorEntry> PathExplorerDoor { get; private set; }
        public GameTable<PathExplorerDoorEntranceEntry> PathExplorerDoorEntrance { get; private set; }
        public GameTable<PathExplorerNodeEntry> PathExplorerNode { get; private set; }
        public GameTable<PathExplorerPowerMapEntry> PathExplorerPowerMap { get; private set; }
        public GameTable<PathExplorerScavengerClueEntry> PathExplorerScavengerClue { get; private set; }
        public GameTable<PathExplorerScavengerHuntEntry> PathExplorerScavengerHunt { get; private set; }

        [GameData]
        public GameTable<PathLevelEntry> PathLevel { get; private set; }

        public GameTable<PathMissionEntry> PathMission { get; private set; }

        [GameData]
        public GameTable<PathRewardEntry> PathReward { get; private set; }

        public GameTable<PathScientistCreatureInfoEntry> PathScientistCreatureInfo { get; private set; }
        public GameTable<PathScientistDatacubeDiscoveryEntry> PathScientistDatacubeDiscovery { get; private set; }
        public GameTable<PathScientistExperimentationEntry> PathScientistExperimentation { get; private set; }
        public GameTable<PathScientistExperimentationPatternEntry> PathScientistExperimentationPattern { get; private set; }
        public GameTable<PathScientistFieldStudyEntry> PathScientistFieldStudy { get; private set; }
        public GameTable<PathScientistScanBotProfileEntry> PathScientistScanBotProfile { get; private set; }
        public GameTable<PathScientistSpecimenSurveyEntry> PathScientistSpecimenSurvey { get; private set; }
        public GameTable<PathSettlerHubEntry> PathSettlerHub { get; private set; }
        public GameTable<PathSettlerImprovementEntry> PathSettlerImprovement { get; private set; }
        public GameTable<PathSettlerImprovementGroupEntry> PathSettlerImprovementGroup { get; private set; }
        public GameTable<PathSettlerInfrastructureEntry> PathSettlerInfrastructure { get; private set; }
        public GameTable<PathSettlerMayorEntry> PathSettlerMayor { get; private set; }
        public GameTable<PathSettlerSheriffEntry> PathSettlerSheriff { get; private set; }
        public GameTable<PathSoldierActivateEntry> PathSoldierActivate { get; private set; }
        public GameTable<PathSoldierAssassinateEntry> PathSoldierAssassinate { get; private set; }
        public GameTable<PathSoldierEventEntry> PathSoldierEvent { get; private set; }
        public GameTable<PathSoldierEventWaveEntry> PathSoldierEventWave { get; private set; }
        public GameTable<PathSoldierSWATEntry> PathSoldierSWAT { get; private set; }
        public GameTable<PathSoldierTowerDefenseEntry> PathSoldierTowerDefense { get; private set; }
        public GameTable<PeriodicQuestGroupEntry> PeriodicQuestGroup { get; private set; }
        public GameTable<PeriodicQuestSetEntry> PeriodicQuestSet { get; private set; }
        public GameTable<PeriodicQuestSetCategoryEntry> PeriodicQuestSetCategory { get; private set; }

        [GameData]
        public GameTable<PetFlairEntry> PetFlair { get; private set; }
        public GameTable<PlayerNotificationTypeEntry> PlayerNotificationType { get; private set; }
        public GameTable<PositionalRequirementEntry> PositionalRequirement { get; private set; }

        [GameData]
        public GameTable<PrerequisiteEntry> Prerequisite { get; private set; }

        public GameTable<PrerequisiteTypeEntry> PrerequisiteType { get; private set; }
        public GameTable<PrimalMatrixNodeEntry> PrimalMatrixNode { get; private set; }
        public GameTable<PrimalMatrixRewardEntry> PrimalMatrixReward { get; private set; }
        public GameTable<PropAdditionalDetailEntry> PropAdditionalDetail { get; private set; }
        public GameTable<PublicEventEntry> PublicEvent { get; private set; }
        public GameTable<PublicEventCustomStatEntry> PublicEventCustomStat { get; private set; }
        public GameTable<PublicEventDepotEntry> PublicEventDepot { get; private set; }
        public GameTable<PublicEventObjectiveEntry> PublicEventObjective { get; private set; }
        public GameTable<PublicEventObjectiveBombDeploymentEntry> PublicEventObjectiveBombDeployment { get; private set; }
        public GameTable<PublicEventObjectiveGatherResourceEntry> PublicEventObjectiveGatherResource { get; private set; }
        public GameTable<PublicEventObjectiveStateEntry> PublicEventObjectiveState { get; private set; }
        public GameTable<PublicEventRewardModifierEntry> PublicEventRewardModifier { get; private set; }
        public GameTable<PublicEventStatDisplayEntry> PublicEventStatDisplay { get; private set; }
        public GameTable<PublicEventTeamEntry> PublicEventTeam { get; private set; }
        public GameTable<PublicEventVirtualItemDepotEntry> PublicEventVirtualItemDepot { get; private set; }
        public GameTable<PublicEventVoteEntry> PublicEventVote { get; private set; }
        public GameTable<PvPRatingFloorEntry> PvPRatingFloor { get; private set; }

        [GameData]
        public GameTable<Quest2Entry> Quest2 { get; private set; }

        [GameData]
        public GameTable<Quest2DifficultyEntry> Quest2Difficulty { get; private set; }

        public GameTable<Quest2RandomTextLineJoinEntry> Quest2RandomTextLineJoin { get; private set; }

        [GameData]
        public GameTable<Quest2RewardEntry> Quest2Reward { get; private set; }
        public GameTable<QuestCategoryEntry> QuestCategory { get; private set; }
        public GameTable<QuestDirectionEntry> QuestDirection { get; private set; }
        public GameTable<QuestDirectionEntryEntry> QuestDirectionEntry { get; private set; }
        public GameTable<QuestGroupEntry> QuestGroup { get; private set; }
        public GameTable<QuestHubEntry> QuestHub { get; private set; }

        [GameData]
        public GameTable<QuestObjectiveEntry> QuestObjective { get; private set; }

        [GameData]
        public GameTable<RaceEntry> Race { get; private set; }

        public GameTable<RandomPlayerNameEntry> RandomPlayerName { get; private set; }
        public GameTable<RandomTextLineEntry> RandomTextLine { get; private set; }
        public GameTable<RandomTextLineSetEntry> RandomTextLineSet { get; private set; }
        public GameTable<RealmDataCenterEntry> RealmDataCenter { get; private set; }
        public GameTable<RedactedEntry> Redacted { get; private set; }
        public GameTable<ReplaceableMaterialInfoEntry> ReplaceableMaterialInfo { get; private set; }
        public GameTable<ResourceConversionEntry> ResourceConversion { get; private set; }
        public GameTable<ResourceConversionGroupEntry> ResourceConversionGroup { get; private set; }

        [GameData]
        public GameTable<RewardPropertyEntry> RewardProperty { get; private set; }

        [GameData]
        public GameTable<RewardPropertyPremiumModifierEntry> RewardPropertyPremiumModifier { get; private set; }

        public GameTable<RewardRotationContentEntry> RewardRotationContent { get; private set; }
        public GameTable<RewardRotationEssenceEntry> RewardRotationEssence { get; private set; }
        public GameTable<RewardRotationItemEntry> RewardRotationItem { get; private set; }
        public GameTable<RewardRotationModifierEntry> RewardRotationModifier { get; private set; }
        public GameTable<RewardTrackEntry> RewardTrack { get; private set; }
        public GameTable<RewardTrackRewardsEntry> RewardTrackRewards { get; private set; }
        public GameTable<SalvageEntry> Salvage { get; private set; }
        public GameTable<SalvageExceptionEntry> SalvageException { get; private set; }
        public GameTable<SkyCloudSetEntry> SkyCloudSet { get; private set; }
        public GameTable<SkyTrackCloudSetEntry> SkyTrackCloudSet { get; private set; }
        public GameTable<SoundBankEntry> SoundBank { get; private set; }
        public GameTable<SoundCombatLoopEntry> SoundCombatLoop { get; private set; }
        public GameTable<SoundContextEntry> SoundContext { get; private set; }
        public GameTable<SoundDirectionalAmbienceEntry> SoundDirectionalAmbience { get; private set; }
        public GameTable<SoundEnvironmentEntry> SoundEnvironment { get; private set; }
        public GameTable<SoundEventEntry> SoundEvent { get; private set; }
        public GameTable<SoundImpactEventsEntry> SoundImpactEvents { get; private set; }
        public GameTable<SoundMusicSetEntry> SoundMusicSet { get; private set; }
        public GameTable<SoundParameterEntry> SoundParameter { get; private set; }
        public GameTable<SoundStatesEntry> SoundStates { get; private set; }
        public GameTable<SoundSwitchEntry> SoundSwitch { get; private set; }
        public GameTable<SoundUIContextEntry> SoundUIContext { get; private set; }
        public GameTable<SoundZoneKitEntry> SoundZoneKit { get; private set; }

        [GameData]
        public GameTable<Spell4Entry> Spell4 { get; private set; }

        [GameData]
        public GameTable<Spell4AoeTargetConstraintsEntry> Spell4AoeTargetConstraints { get; private set; }

        [GameData]
        public GameTable<Spell4BaseEntry> Spell4Base { get; private set; }

        [GameData]
        public GameTable<Spell4CCConditionsEntry> Spell4CCConditions { get; private set; }
        public GameTable<Spell4CastResultEntry> Spell4CastResult { get; private set; }
        public GameTable<Spell4ClientMissileEntry> Spell4ClientMissile { get; private set; }

        [GameData]
        public GameTable<Spell4ConditionsEntry> Spell4Conditions { get; private set; }

        public GameTable<Spell4EffectGroupListEntry> Spell4EffectGroupList { get; private set; }
        public GameTable<Spell4EffectModificationEntry> Spell4EffectModification { get; private set; }

        [GameData]
        public GameTable<Spell4EffectsEntry> Spell4Effects { get; private set; }

        public GameTable<Spell4GroupListEntry> Spell4GroupList { get; private set; }

        [GameData]
        public GameTable<Spell4HitResultsEntry> Spell4HitResults { get; private set; }

        public GameTable<Spell4ModificationEntry> Spell4Modification { get; private set; }

        [GameData]
        public GameTable<Spell4PrerequisitesEntry> Spell4Prerequisites { get; private set; }
        public GameTable<Spell4ReagentEntry> Spell4Reagent { get; private set; }
        public GameTable<Spell4RunnerEntry> Spell4Runner { get; private set; }
        public GameTable<Spell4ServiceTokenCostEntry> Spell4ServiceTokenCost { get; private set; }

        [GameData]
        public GameTable<Spell4SpellTypesEntry> Spell4SpellTypes { get; private set; }

        [GameData]
        public GameTable<Spell4StackGroupEntry> Spell4StackGroup { get; private set; }

        public GameTable<Spell4TagEntry> Spell4Tag { get; private set; }

        [GameData]
        public GameTable<Spell4TargetAngleEntry> Spell4TargetAngle { get; private set; }

        [GameData]
        public GameTable<Spell4TargetMechanicsEntry> Spell4TargetMechanics { get; private set; }

        [GameData]
        public GameTable<Spell4TelegraphEntry> Spell4Telegraph { get; private set; }

        public GameTable<Spell4ThresholdsEntry> Spell4Thresholds { get; private set; }
        public GameTable<Spell4TierRequirementsEntry> Spell4TierRequirements { get; private set; }

        [GameData]
        public GameTable<Spell4ValidTargetsEntry> Spell4ValidTargets { get; private set; }

        public GameTable<Spell4VisualEntry> Spell4Visual { get; private set; }
        public GameTable<Spell4VisualGroupEntry> Spell4VisualGroup { get; private set; }

        [GameData]
        public GameTable<SpellCoolDownEntry> SpellCoolDown { get; private set; }

        public GameTable<SpellEffectTypeEntry> SpellEffectType { get; private set; }

        [GameData]
        public GameTable<SpellLevelEntry> SpellLevel { get; private set; }
        public GameTable<SpellPhaseEntry> SpellPhase { get; private set; }

        [GameData]
        public GameTable<Spline2Entry> Spline2 { get; private set; }

        [GameData]
        public GameTable<Spline2NodeEntry> Spline2Node { get; private set; }

        public GameTable<StoreDisplayInfoEntry> StoreDisplayInfo { get; private set; }
        public GameTable<StoreKeywordEntry> StoreKeyword { get; private set; }
        public GameTable<StoreLinkEntry> StoreLink { get; private set; }

        [GameData]
        public GameTable<StoryPanelEntry> StoryPanel { get; private set; }

        [GameData]
        public GameTable<TargetGroupEntry> TargetGroup { get; private set; }

        public GameTable<TargetMarkerEntry> TargetMarker { get; private set; }

        [GameData]
        public GameTable<TaxiNodeEntry> TaxiNode { get; private set; }
        [GameData]
        public GameTable<TaxiRouteEntry> TaxiRoute { get; private set; }

        [GameData]
        public GameTable<TelegraphDamageEntry> TelegraphDamage { get; private set; }

        public GameTable<TicketCategoryEntry> TicketCategory { get; private set; }
        public GameTable<TicketSubCategoryEntry> TicketSubCategory { get; private set; }
        public GameTable<TrackingSlotEntry> TrackingSlot { get; private set; }
        public GameTable<TradeskillEntry> Tradeskill { get; private set; }
        public GameTable<TradeskillAchievementLayoutEntry> TradeskillAchievementLayout { get; private set; }
        public GameTable<TradeskillAchievementRewardEntry> TradeskillAchievementReward { get; private set; }
        public GameTable<TradeskillAdditiveEntry> TradeskillAdditive { get; private set; }
        public GameTable<TradeskillBonusEntry> TradeskillBonus { get; private set; }
        public GameTable<TradeskillCatalystEntry> TradeskillCatalyst { get; private set; }
        public GameTable<TradeskillCatalystOrderingEntry> TradeskillCatalystOrdering { get; private set; }
        public GameTable<TradeskillHarvestingInfoEntry> TradeskillHarvestingInfo { get; private set; }

        [GameData]
        public GameTable<TradeskillMaterialEntry> TradeskillMaterial { get; private set; }

        public GameTable<TradeskillMaterialCategoryEntry> TradeskillMaterialCategory { get; private set; }
        public GameTable<TradeskillProficiencyEntry> TradeskillProficiency { get; private set; }
        public GameTable<TradeskillSchematic2Entry> TradeskillSchematic2 { get; private set; }
        public GameTable<TradeskillTalentTierEntry> TradeskillTalentTier { get; private set; }
        public GameTable<TradeskillTierEntry> TradeskillTier { get; private set; }
        public GameTable<TutorialEntry> Tutorial { get; private set; }
        public GameTable<TutorialAnchorEntry> TutorialAnchor { get; private set; }
        public GameTable<TutorialLayoutEntry> TutorialLayout { get; private set; }
        public GameTable<TutorialPageEntry> TutorialPage { get; private set; }
        public GameTable<UnitProperty2Entry> UnitProperty2 { get; private set; }

        [GameData]
        public GameTable<UnitRaceEntry> UnitRace { get; private set; }

        [GameData]
        public GameTable<UnitVehicleEntry> UnitVehicle { get; private set; }

        public GameTable<VeteranTierEntry> VeteranTier { get; private set; }
        public GameTable<VirtualItemEntry> VirtualItem { get; private set; }
        public GameTable<VisualEffectEntry> VisualEffect { get; private set; }
        public GameTable<VitalEntry> Vital { get; private set; }
        public GameTable<WaterSurfaceEffectEntry> WaterSurfaceEffect { get; private set; }

        [GameData]
        public GameTable<WindEntry> Wind { get; private set; }

        public GameTable<WindSpawnEntry> WindSpawn { get; private set; }

        [GameData]
        public GameTable<WordFilterEntry> WordFilter { get; private set; }

        [GameData]
        public GameTable<WorldEntry> World { get; private set; }

        public GameTable<WorldClutterEntry> WorldClutter { get; private set; }
        public GameTable<WorldLayerEntry> WorldLayer { get; private set; }

        [GameData]
        public GameTable<WorldLocation2Entry> WorldLocation2 { get; private set; }

        [GameData]
        public GameTable<WorldSkyEntry> WorldSky { get; private set; }

        public GameTable<WorldSocketEntry> WorldSocket { get; private set; }
        public GameTable<WorldWaterEnvironmentEntry> WorldWaterEnvironment { get; private set; }
        public GameTable<WorldWaterFogEntry> WorldWaterFog { get; private set; }
        public GameTable<WorldWaterLayerEntry> WorldWaterLayer { get; private set; }
        public GameTable<WorldWaterTypeEntry> WorldWaterType { get; private set; }
        public GameTable<WorldWaterWakeEntry> WorldWaterWake { get; private set; }

        [GameData]
        public GameTable<WorldZoneEntry> WorldZone { get; private set; }

        [GameData]
        public GameTable<XpPerLevelEntry> XpPerLevel { get; private set; }

        public GameTable<ZoneCompletionEntry> ZoneCompletion { get; private set; }

        [GameData("fr-FR.bin")]
        public TextTable TextFrench { get; private set; }
        [GameData("en-US.bin")]
        public TextTable TextEnglish { get; private set; }
        [GameData("de-DE.bin")]
        public TextTable TextGerman { get; private set; }

        private GameTableManager()
        {
        }

        public void Initialise()
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

        private async Task LoadGameTablesAsync()
        {
            var exceptions = new List<Exception>();
            int loadCount = Environment.ProcessorCount * 2;
            if (loadCount < minimumThreads)
                loadCount = minimumThreads;
            if (loadCount > maximumThreads)
                loadCount = maximumThreads;

            var tasks = new List<Task>();
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

            var properties = new List<PropertyInfo>();
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

        private Task LoadGameTableAsync(PropertyInfo property, string fileName)
        {
            async Task SetPropertyOnCompletion(Task<object> task)
            {
                property.SetValue(this, await task);
            }

            async Task VerifyPropertySetOnCompletion(Task task)
            {
                await task;
                if (property.GetValue(this) == null)
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
        public TextTable GetTextTable(Language language)
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
