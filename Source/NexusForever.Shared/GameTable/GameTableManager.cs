using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using NexusForever.Shared.GameTable.Model;
using NLog;
using System.Threading.Tasks;

namespace NexusForever.Shared.GameTable
{
    public static class GameTableManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static GameTable<AccountCurrencyTypeEntry> AccountCurrencyType { get; private set; }
        public static GameTable<AccountItemEntry> AccountItem { get; private set; }
        public static GameTable<AccountItemCooldownGroupEntry> AccountItemCooldownGroup { get; private set; }
        public static GameTable<AchievementEntry> Achievement { get; private set; }
        public static GameTable<AchievementCategoryEntry> AchievementCategory { get; private set; }

        public static GameTable<AchievementChecklistEntry> AchievementChecklist { get; private set; }
        public static GameTable<AchievementGroupEntry> AchievementGroup { get; private set; }
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
        public static GameTable<CharacterCreationEntry> CharacterCreation { get; private set; }
        public static GameTable<CharacterCreationArmorSetEntry> CharacterCreationArmorSet { get; private set; }
        public static GameTable<CharacterCreationPresetEntry> CharacterCreationPreset { get; private set; }
        public static GameTable<CharacterCustomizationEntry> CharacterCustomization { get; private set; }
        public static GameTable<CharacterCustomizationLabelEntry> CharacterCustomizationLabel { get; private set; }
        public static GameTable<CharacterCustomizationSelectionEntry> CharacterCustomizationSelection { get; private set; }
        public static GameTable<CharacterTitleEntry> CharacterTitle { get; private set; }
        public static GameTable<CharacterTitleCategoryEntry> CharacterTitleCategory { get; private set; }
        public static GameTable<ChatChannelEntry> ChatChannel { get; private set; }
        public static GameTable<CinematicEntry> Cinematic { get; private set; }
        public static GameTable<CinematicRaceEntry> CinematicRace { get; private set; }
        public static GameTable<CityDirectionEntry> CityDirection { get; private set; }
        public static GameTable<ClassEntry> Class { get; private set; }
        public static GameTable<ClassSecondaryStatBonusEntry> ClassSecondaryStatBonus { get; private set; }
        public static GameTable<ClientEventEntry> ClientEvent { get; private set; }
        public static GameTable<ClientEventActionEntry> ClientEventAction { get; private set; }
        public static GameTable<ClientSideInteractionEntry> ClientSideInteraction { get; private set; }
        public static GameTable<ColorShiftEntry> ColorShift { get; private set; }
        public static GameTable<CombatRewardEntry> CombatReward { get; private set; }
        public static GameTable<CommunicatorMessagesEntry> CommunicatorMessages { get; private set; }
        public static GameTable<ComponentRegionEntry> ComponentRegion { get; private set; }
        public static GameTable<ComponentRegionRectEntry> ComponentRegionRect { get; private set; }
        public static GameTable<CostumeSpeciesEntry> CostumeSpecies { get; private set; }
        public static GameTable<Creature2Entry> Creature2 { get; private set; }
        public static GameTable<Creature2ActionEntry> Creature2Action { get; private set; }
        public static GameTable<Creature2ActionSetEntry> Creature2ActionSet { get; private set; }
        public static GameTable<Creature2ActionTextEntry> Creature2ActionText { get; private set; }
        public static GameTable<Creature2AffiliationEntry> Creature2Affiliation { get; private set; }
        public static GameTable<Creature2ArcheTypeEntry> Creature2ArcheType { get; private set; }
        public static GameTable<Creature2DifficultyEntry> Creature2Difficulty { get; private set; }
        public static GameTable<Creature2DisplayGroupEntryEntry> Creature2DisplayGroupEntry { get; private set; }
        public static GameTable<Creature2DisplayInfoEntry> Creature2DisplayInfo { get; private set; }
        public static GameTable<Creature2ModelInfoEntry> Creature2ModelInfo { get; private set; }
        public static GameTable<Creature2OutfitGroupEntryEntry> Creature2OutfitGroupEntry { get; private set; }
        public static GameTable<Creature2OutfitInfoEntry> Creature2OutfitInfo { get; private set; }
        public static GameTable<Creature2OverridePropertiesEntry> Creature2OverrideProperties { get; private set; }
        public static GameTable<Creature2ResistEntry> Creature2Resist { get; private set; }
        public static GameTable<Creature2TierEntry> Creature2Tier { get; private set; }
        public static GameTable<CreatureLevelEntry> CreatureLevel { get; private set; }
        public static GameTable<CurrencyTypeEntry> CurrencyType { get; private set; }
        public static GameTable<CustomerSurveyEntry> CustomerSurvey { get; private set; }
        public static GameTable<CustomizationParameterEntry> CustomizationParameter { get; private set; }
        public static GameTable<CustomizationParameterMapEntry> CustomizationParameterMap { get; private set; }
        public static GameTable<DailyLoginRewardEntry> DailyLoginReward { get; private set; }
        public static GameTable<DatacubeEntry> Datacube { get; private set; }
        public static GameTable<DatacubeVolumeEntry> DatacubeVolume { get; private set; }
        public static GameTable<DistanceDamageModifierEntry> DistanceDamageModifier { get; private set; }
        public static GameTable<DyeColorRampEntry> DyeColorRamp { get; private set; }
        public static GameTable<EldanAugmentationEntry> EldanAugmentation { get; private set; }
        public static GameTable<EldanAugmentationCategoryEntry> EldanAugmentationCategory { get; private set; }
        public static GameTable<EmoteSequenceTransitionEntry> EmoteSequenceTransition { get; private set; }
        public static GameTable<EmotesEntry> Emotes { get; private set; }
        public static GameTable<EntitlementEntry> Entitlement { get; private set; }
        public static GameTable<EpisodeEntry> Episode { get; private set; }
        public static GameTable<EpisodeQuestEntry> EpisodeQuest { get; private set; }
        public static GameTable<Faction2Entry> Faction2 { get; private set; }
        public static GameTable<Faction2RelationshipEntry> Faction2Relationship { get; private set; }
        public static GameTable<FinishingMoveDeathVisualEntry> FinishingMoveDeathVisual { get; private set; }
        public static GameTable<FullScreenEffectEntry> FullScreenEffect { get; private set; }
        public static GameTable<GameFormulaEntry> GameFormula { get; private set; }
        public static GameTable<GenericMapEntry> GenericMap { get; private set; }
        public static GameTable<GenericMapNodeEntry> GenericMapNode { get; private set; }
        public static GameTable<GenericStringGroupsEntry> GenericStringGroups { get; private set; }
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
        public static GameTable<HousingDecorInfoEntry> HousingDecorInfo { get; private set; }
        public static GameTable<HousingDecorLimitCategoryEntry> HousingDecorLimitCategory { get; private set; }
        public static GameTable<HousingDecorTypeEntry> HousingDecorType { get; private set; }
        public static GameTable<HousingMannequinPoseEntry> HousingMannequinPose { get; private set; }
        public static GameTable<HousingMapInfoEntry> HousingMapInfo { get; private set; }
        public static GameTable<HousingNeighborhoodInfoEntry> HousingNeighborhoodInfo { get; private set; }
        public static GameTable<HousingPlotInfoEntry> HousingPlotInfo { get; private set; }
        public static GameTable<HousingPlotTypeEntry> HousingPlotType { get; private set; }
        public static GameTable<HousingPlugItemEntry> HousingPlugItem { get; private set; }
        public static GameTable<HousingPropertyInfoEntry> HousingPropertyInfo { get; private set; }
        public static GameTable<HousingResidenceInfoEntry> HousingResidenceInfo { get; private set; }
        public static GameTable<HousingResourceEntry> HousingResource { get; private set; }
        public static GameTable<HousingWallpaperInfoEntry> HousingWallpaperInfo { get; private set; }
        public static GameTable<HousingWarplotBossTokenEntry> HousingWarplotBossToken { get; private set; }
        public static GameTable<HousingWarplotPlugInfoEntry> HousingWarplotPlugInfo { get; private set; }
        public static GameTable<InputActionEntry> InputAction { get; private set; }
        public static GameTable<InputActionCategoryEntry> InputActionCategory { get; private set; }
        public static GameTable<InstancePortalEntry> InstancePortal { get; private set; }
        public static GameTable<Item2Entry> Item { get; private set; }
        public static GameTable<Item2CategoryEntry> Item2Category { get; private set; }
        public static GameTable<Item2FamilyEntry> Item2Family { get; private set; }
        public static GameTable<Item2TypeEntry> ItemType { get; private set; }
        public static GameTable<ItemBudgetEntry> ItemBudget { get; private set; }
        public static GameTable<ItemColorSetEntry> ItemColorSet { get; private set; }
        public static GameTable<ItemDisplayEntry> ItemDisplay { get; private set; }
        public static GameTable<ItemDisplaySourceEntryEntry> ItemDisplaySourceEntry { get; private set; }
        public static GameTable<ItemImbuementEntry> ItemImbuement { get; private set; }
        public static GameTable<ItemImbuementRewardEntry> ItemImbuementReward { get; private set; }
        public static GameTable<ItemProficiencyEntry> ItemProficiency { get; private set; }
        public static GameTable<ItemQualityEntry> ItemQuality { get; private set; }
        public static GameTable<ItemRandomStatEntry> ItemRandomStat { get; private set; }
        public static GameTable<ItemRandomStatGroupEntry> ItemRandomStatGroup { get; private set; }
        public static GameTable<ItemRuneInstanceEntry> ItemRuneInstance { get; private set; }
        //empty public static GameTable<ItemRuneSlotRandomizationEntry> ItemRuneSlotRandomization { get; private set; }
        public static GameTable<ItemSetEntry> ItemSet { get; private set; }
        public static GameTable<ItemSetBonusEntry> ItemSetBonus { get; private set; }
        public static GameTable<ItemSlotEntry> ItemSlot { get; private set; }
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
        public static GameTable<LocalizedTextEntry> LocalizedText { get; private set; }
        public static GameTable<LootPinataInfoEntry> LootPinataInfo { get; private set; }
        public static GameTable<LootSpellEntry> LootSpell { get; private set; }
        public static GameTable<LuaEventEntry> LuaEvent { get; private set; }
        public static GameTable<MapContinentEntry> MapContinent { get; private set; }
        public static GameTable<MapZoneEntry> MapZone { get; private set; }
        public static GameTable<MapZoneHexEntry> MapZoneHex { get; private set; }
        public static GameTable<MapZoneHexGroupEntry> MapZoneHexGroup { get; private set; }
        public static GameTable<MapZoneHexGroupEntryEntry> MapZoneHexGroupEntry { get; private set; }
        //empty public static GameTable<MapZoneLevelBandEntry> MapZoneLevelBand { get; private set; }
        public static GameTable<MapZoneNemesisRegionEntry> MapZoneNemesisRegion { get; private set; }
        public static GameTable<MapZonePOIEntry> MapZonePOI { get; private set; }
        public static GameTable<MapZoneSpriteEntry> MapZoneSprite { get; private set; }
        public static GameTable<MapZoneWorldJoinEntry> MapZoneWorldJoin { get; private set; }
        public static GameTable<MatchTypeRewardRotationContentEntry> MatchTypeRewardRotationContent { get; private set; }
        public static GameTable<MatchingGameMapEntry> MatchingGameMap { get; private set; }
        public static GameTable<MatchingGameTypeEntry> MatchingGameType { get; private set; }
        //empty public static GameTable<MatchingMapPrerequisiteEntry> MatchingMapPrerequisite { get; private set; }
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
        public static GameTable<PathLevelEntry> PathLevel { get; private set; }
        public static GameTable<PathMissionEntry> PathMission { get; private set; }
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
        public static GameTable<PetFlairEntry> PetFlair { get; private set; }
        public static GameTable<PlayerNotificationTypeEntry> PlayerNotificationType { get; private set; }
        public static GameTable<PositionalRequirementEntry> PositionalRequirement { get; private set; }
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
        //empty public static GameTable<PublicEventUnitPropertyModifierEntry> PublicEventUnitPropertyModifier { get; private set; }
        public static GameTable<PublicEventVirtualItemDepotEntry> PublicEventVirtualItemDepot { get; private set; }
        public static GameTable<PublicEventVoteEntry> PublicEventVote { get; private set; }
        public static GameTable<PvPRatingFloorEntry> PvPRatingFloor { get; private set; }
        public static GameTable<Quest2Entry> Quest2 { get; private set; }
        public static GameTable<Quest2DifficultyEntry> Quest2Difficulty { get; private set; }
        public static GameTable<Quest2RandomTextLineJoinEntry> Quest2RandomTextLineJoin { get; private set; }
        public static GameTable<Quest2RewardEntry> Quest2Reward { get; private set; }
        public static GameTable<QuestCategoryEntry> QuestCategory { get; private set; }
        public static GameTable<QuestDirectionEntry> QuestDirection { get; private set; }
        public static GameTable<QuestDirectionEntryEntry> QuestDirectionEntry { get; private set; }
        public static GameTable<QuestGroupEntry> QuestGroup { get; private set; }
        public static GameTable<QuestHubEntry> QuestHub { get; private set; }
        public static GameTable<QuestObjectiveEntry> QuestObjective { get; private set; }
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
        //empty public static GameTable<SoundReplaceEntry> SoundReplace { get; private set; }
        //empty public static GameTable<SoundReplaceDescriptionEntry> SoundReplaceDescription { get; private set; }
        public static GameTable<SoundStatesEntry> SoundStates { get; private set; }
        public static GameTable<SoundSwitchEntry> SoundSwitch { get; private set; }
        public static GameTable<SoundUIContextEntry> SoundUIContext { get; private set; }
        public static GameTable<SoundZoneKitEntry> SoundZoneKit { get; private set; }
        public static GameTable<Spell4Entry> Spell4 { get; private set; }
        public static GameTable<Spell4AoeTargetConstraintsEntry> Spell4AoeTargetConstraints { get; private set; }
        public static GameTable<Spell4BaseEntry> Spell4Base { get; private set; }
        public static GameTable<Spell4CCConditionsEntry> Spell4CCConditions { get; private set; }
        public static GameTable<Spell4CastResultEntry> Spell4CastResult { get; private set; }
        public static GameTable<Spell4ClientMissileEntry> Spell4ClientMissile { get; private set; }
        public static GameTable<Spell4ConditionsEntry> Spell4Conditions { get; private set; }
        public static GameTable<Spell4EffectGroupListEntry> Spell4EffectGroupList { get; private set; }
        public static GameTable<Spell4EffectModificationEntry> Spell4EffectModification { get; private set; }
        public static GameTable<Spell4EffectsEntry> Spell4Effects { get; private set; }
        public static GameTable<Spell4GroupListEntry> Spell4GroupList { get; private set; }
        public static GameTable<Spell4HitResultsEntry> Spell4HitResults { get; private set; }
        public static GameTable<Spell4ModificationEntry> Spell4Modification { get; private set; }
        public static GameTable<Spell4PrerequisitesEntry> Spell4Prerequisites { get; private set; }
        public static GameTable<Spell4ReagentEntry> Spell4Reagent { get; private set; }
        public static GameTable<Spell4RunnerEntry> Spell4Runner { get; private set; }
        public static GameTable<Spell4ServiceTokenCostEntry> Spell4ServiceTokenCost { get; private set; }
        public static GameTable<Spell4SpellTypesEntry> Spell4SpellTypes { get; private set; }
        public static GameTable<Spell4StackGroupEntry> Spell4StackGroup { get; private set; }
        public static GameTable<Spell4TagEntry> Spell4Tag { get; private set; }
        public static GameTable<Spell4TargetAngleEntry> Spell4TargetAngle { get; private set; }
        public static GameTable<Spell4TargetMechanicsEntry> Spell4TargetMechanics { get; private set; }
        public static GameTable<Spell4TelegraphEntry> Spell4Telegraph { get; private set; }
        public static GameTable<Spell4ThresholdsEntry> Spell4Thresholds { get; private set; }
        public static GameTable<Spell4TierRequirementsEntry> Spell4TierRequirements { get; private set; }
        public static GameTable<Spell4ValidTargetsEntry> Spell4ValidTargets { get; private set; }
        public static GameTable<Spell4VisualEntry> Spell4Visual { get; private set; }
        public static GameTable<Spell4VisualGroupEntry> Spell4VisualGroup { get; private set; }
        public static GameTable<SpellCoolDownEntry> SpellCoolDown { get; private set; }
        public static GameTable<SpellEffectTypeEntry> SpellEffectType { get; private set; }
        public static GameTable<SpellLevelEntry> SpellLevel { get; private set; }
        public static GameTable<SpellPhaseEntry> SpellPhase { get; private set; }
        public static GameTable<Spline2Entry> Spline2 { get; private set; }
        public static GameTable<Spline2NodeEntry> Spline2Node { get; private set; }
        public static GameTable<StoreDisplayInfoEntry> StoreDisplayInfo { get; private set; }
        public static GameTable<StoreKeywordEntry> StoreKeyword { get; private set; }
        public static GameTable<StoreLinkEntry> StoreLink { get; private set; }
        public static GameTable<StoryPanelEntry> StoryPanel { get; private set; }
        public static GameTable<TargetGroupEntry> TargetGroup { get; private set; }
        public static GameTable<TargetMarkerEntry> TargetMarker { get; private set; }
        public static GameTable<TaxiNodeEntry> TaxiNode { get; private set; }
        public static GameTable<TaxiRouteEntry> TaxiRoute { get; private set; }
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
        public static GameTable<UnitRaceEntry> UnitRace { get; private set; }
        public static GameTable<UnitVehicleEntry> UnitVehicle { get; private set; }
        public static GameTable<VeteranTierEntry> VeteranTier { get; private set; }
        public static GameTable<VirtualItemEntry> VirtualItem { get; private set; }
        public static GameTable<VisualEffectEntry> VisualEffect { get; private set; }
        public static GameTable<VitalEntry> Vital { get; private set; }
        public static GameTable<WaterSurfaceEffectEntry> WaterSurfaceEffect { get; private set; }
        public static GameTable<WindEntry> Wind { get; private set; }
        public static GameTable<WindSpawnEntry> WindSpawn { get; private set; }
        public static GameTable<WordFilterEntry> WordFilter { get; private set; }
        //empty public static GameTable<WordFilterAltEntry> WordFilterAlt { get; private set; }
        public static GameTable<WorldEntry> World { get; private set; }
        public static GameTable<WorldClutterEntry> WorldClutter { get; private set; }
        public static GameTable<WorldLayerEntry> WorldLayer { get; private set; }
        public static GameTable<WorldLocation2Entry> WorldLocation2 { get; private set; }
        public static GameTable<WorldSkyEntry> WorldSky { get; private set; }
        public static GameTable<WorldSocketEntry> WorldSocket { get; private set; }
        public static GameTable<WorldWaterEnvironmentEntry> WorldWaterEnvironment { get; private set; }
        public static GameTable<WorldWaterFogEntry> WorldWaterFog { get; private set; }
        public static GameTable<WorldWaterLayerEntry> WorldWaterLayer { get; private set; }
        public static GameTable<WorldWaterTypeEntry> WorldWaterType { get; private set; }
        public static GameTable<WorldWaterWakeEntry> WorldWaterWake { get; private set; }
        public static GameTable<WorldZoneEntry> WorldZone { get; private set; }
        public static GameTable<XpPerLevelEntry> XpPerLevel { get; private set; }
        public static GameTable<ZoneCompletionEntry> ZoneCompletion { get; private set; }
        
        private static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = 
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }

        private static PropertyInfo GetProperty(Expression<Func<object>> expression)
        {
            return GetMemberInfo(expression).Member as PropertyInfo;
        }

        private static Task LoadGameTableAsync<T>(Expression<Func<object>> propertyExpression, string fileName)
        {
            // TODO
            return Task.CompletedTask;
        }


        public static TextTable Text { get; private set; }

        public static void Initialise()
        {
            log.Info("Loading GameTables...");
            var sw = Stopwatch.StartNew();
            Task<TextTable> textLoadTask = null;
            try
            {
                textLoadTask = Task.Run(() =>       GameTableFactory.LoadText("en-US.bin"));
                CharacterCreation                 = GameTableFactory.Load<CharacterCreationEntry>("CharacterCreation.tbl");
                CharacterCustomization            = GameTableFactory.Load<CharacterCustomizationEntry>("CharacterCustomization.tbl");
                Creature2                         = GameTableFactory.Load<Creature2Entry>("Creature2.tbl");
                Creature2ArcheType                = GameTableFactory.Load<Creature2ArcheTypeEntry>("Creature2ArcheType.tbl");
                Creature2Difficulty               = GameTableFactory.Load<Creature2DifficultyEntry>("Creature2Difficulty.tbl");
                Creature2DisplayGroupEntry        = GameTableFactory.Load<Creature2DisplayGroupEntryEntry>("Creature2DisplayGroupEntry.tbl");
                Creature2ModelInfo                = GameTableFactory.Load<Creature2ModelInfoEntry>("Creature2ModelInfo.tbl");
                Creature2OutfitGroupEntry         = GameTableFactory.Load<Creature2OutfitGroupEntryEntry>("Creature2OutfitGroupEntry.tbl");
                Creature2Tier                     = GameTableFactory.Load<Creature2TierEntry>("Creature2Tier.tbl");
                CreatureLevel                     = GameTableFactory.Load<CreatureLevelEntry>("CreatureLevel.tbl");
                Emotes                            = GameTableFactory.Load<EmotesEntry>("Emotes.tbl");
                Faction2                          = GameTableFactory.Load<Faction2Entry>("Faction2.tbl");
                Item                              = GameTableFactory.Load<Item2Entry>("Item2.tbl");
                ItemType                          = GameTableFactory.Load<Item2TypeEntry>("Item2Type.tbl");
                ItemDisplaySourceEntry            = GameTableFactory.Load<ItemDisplaySourceEntryEntry>("ItemDisplaySourceEntry.tbl");
                MapZone                           = GameTableFactory.Load<MapZoneEntry>("MapZone.tbl");
                UnitRace                          = GameTableFactory.Load<UnitRaceEntry>("UnitRace.tbl");
                World                             = GameTableFactory.Load<WorldEntry>("World.tbl");
                WorldLocation2                    = GameTableFactory.Load<WorldLocation2Entry>("WorldLocation2.tbl");
                LocalizedText                     = GameTableFactory.Load<LocalizedTextEntry>("LocalizedText.tbl");
                WorldZone                         = GameTableFactory.Load<WorldZoneEntry>("WorldZone.tbl");
                Text                              = textLoadTask.GetAwaiter().GetResult();
                log.Info("Indexing names to zones for teleport by name support");
                ZoneLookupTable = CreateTeleportLookups();
            }
            catch (Exception exception)
            {
                log.Fatal(exception);
                throw;
            } 
            finally 
            {
                try
                {
                    textLoadTask?.Wait();
                }
                catch
                {

                }
            }
            log.Info($"Loaded GameTables in {sw.ElapsedMilliseconds}ms.");
        }

        public static IEnumerable<WorldLocation2Entry> LookupZonesByName(string zoneName)
        {
            ZoneLookupTable.TryGetValue(zoneName, out List<WorldLocation2Entry> list);
            return list ?? Enumerable.Empty<WorldLocation2Entry>();
        }

        private static Dictionary<string, List<WorldLocation2Entry>> CreateTeleportLookups()
        {
            Dictionary<string, List<WorldLocation2Entry>> index = new Dictionary<string, List<WorldLocation2Entry>>(StringComparer.OrdinalIgnoreCase);
            
            foreach (WorldLocation2Entry zone in WorldLocation2.Entries)
            {
                WorldZoneEntry worldZone = WorldZone.GetEntry(zone.WorldZoneId);
                WorldEntry world = World.GetEntry(zone.WorldId);
                if(worldZone == null && world == null) continue;
                uint textId = worldZone?.LocalizedTextIdName ?? world.LocalizedTextIdName;
                if(textId < 1) continue;
                string text = Text.GetText(textId);
                if(string.IsNullOrWhiteSpace(text)) continue;
                AddObjectToKey(index, text, zone);
            }

            return index;
        }

        private static void AddObjectToKey(Dictionary<string, List<WorldLocation2Entry>> dictionary, string key, WorldLocation2Entry obj)
        {
            List<WorldLocation2Entry> entries;
            if (!dictionary.TryGetValue(key, out entries))
            {
                dictionary.Add(key, entries = new List<WorldLocation2Entry>());
            }

            entries.Add(obj);
        }

        private static Dictionary<string, List<WorldLocation2Entry>> ZoneLookupTable {get; set;}

        private static string LookupText(uint id)
        {
            return Text.Entries.FirstOrDefault(i => i.Id == id)?.Text;
        }
    }
}

/*
                move to enable - provided for convenience
                World                             
                AccountCurrencyType               = GameTableFactory.Load<AccountCurrencyTypeEntry>("AccountCurrencyType.tbl");
                AccountItem                       = GameTableFactory.Load<AccountItemEntry>("AccountItem.tbl");
                AccountItemCooldownGroup          = GameTableFactory.Load<AccountItemCooldownGroupEntry>("AccountItemCooldownGroup.tbl");
                Achievement                       = GameTableFactory.Load<AchievementEntry>("Achievement.tbl");
                AchievementCategory               = GameTableFactory.Load<AchievementCategoryEntry>("AchievementCategory.tbl");
                AchievementChecklist              = GameTableFactory.Load<AchievementChecklistEntry>("AchievementChecklist.tbl");
                AchievementGroup                  = GameTableFactory.Load<AchievementGroupEntry>("AchievementGroup.tbl");
                AchievementSubGroup               = GameTableFactory.Load<AchievementSubGroupEntry>("AchievementSubGroup.tbl");
                AchievementText                   = GameTableFactory.Load<AchievementTextEntry>("AchievementText.tbl");
                ActionBarShortcutSet              = GameTableFactory.Load<ActionBarShortcutSetEntry>("ActionBarShortcutSet.tbl");
                ActionSlotPrereq                  = GameTableFactory.Load<ActionSlotPrereqEntry>("ActionSlotPrereq.tbl");
                ArchiveArticle                    = GameTableFactory.Load<ArchiveArticleEntry>("ArchiveArticle.tbl");
                ArchiveCategory                   = GameTableFactory.Load<ArchiveCategoryEntry>("ArchiveCategory.tbl");
                ArchiveEntry                      = GameTableFactory.Load<ArchiveEntryEntry>("ArchiveEntry.tbl");
                ArchiveEntryUnlockRule            = GameTableFactory.Load<ArchiveEntryUnlockRuleEntry>("ArchiveEntryUnlockRule.tbl");
                ArchiveLink                       = GameTableFactory.Load<ArchiveLinkEntry>("ArchiveLink.tbl");
                AttributeMilestoneGroup           = GameTableFactory.Load<AttributeMilestoneGroupEntry>("AttributeMilestoneGroup.tbl");
                AttributeMiniMilestoneGroup       = GameTableFactory.Load<AttributeMiniMilestoneGroupEntry>("AttributeMiniMilestoneGroup.tbl");
                BindPoint                         = GameTableFactory.Load<BindPointEntry>("BindPoint.tbl");
                BinkMovie                         = GameTableFactory.Load<BinkMovieEntry>("BinkMovie.tbl");
                BinkMovieSubtitle                 = GameTableFactory.Load<BinkMovieSubtitleEntry>("BinkMovieSubtitle.tbl");
                BugCategory                       = GameTableFactory.Load<BugCategoryEntry>("BugCategory.tbl");
                BugSubcategory                    = GameTableFactory.Load<BugSubcategoryEntry>("BugSubcategory.tbl");
                CCStateAdditionalData             = GameTableFactory.Load<CCStateAdditionalDataEntry>("CCStateAdditionalData.tbl");
                CCStateDiminishingReturns         = GameTableFactory.Load<CCStateDiminishingReturnsEntry>("CCStateDiminishingReturns.tbl");
                CCStates                          = GameTableFactory.Load<CCStatesEntry>("CCStates.tbl");
                Challenge                         = GameTableFactory.Load<ChallengeEntry>("Challenge.tbl");
                ChallengeTier                     = GameTableFactory.Load<ChallengeTierEntry>("ChallengeTier.tbl");
                CharacterCreationArmorSet         = GameTableFactory.Load<CharacterCreationArmorSetEntry>("CharacterCreationArmorSet.tbl");
                CharacterCreationPreset           = GameTableFactory.Load<CharacterCreationPresetEntry>("CharacterCreationPreset.tbl");
                CharacterCustomizationLabel       = GameTableFactory.Load<CharacterCustomizationLabelEntry>("CharacterCustomizationLabel.tbl");
                CharacterCustomizationSelection   = GameTableFactory.Load<CharacterCustomizationSelectionEntry>("CharacterCustomizationSelection.tbl");
                CharacterTitle                    = GameTableFactory.Load<CharacterTitleEntry>("CharacterTitle.tbl");
                CharacterTitleCategory            = GameTableFactory.Load<CharacterTitleCategoryEntry>("CharacterTitleCategory.tbl");
                ChatChannel                       = GameTableFactory.Load<ChatChannelEntry>("ChatChannel.tbl");
                Cinematic                         = GameTableFactory.Load<CinematicEntry>("Cinematic.tbl");
                CinematicRace                     = GameTableFactory.Load<CinematicRaceEntry>("CinematicRace.tbl");
                CityDirection                     = GameTableFactory.Load<CityDirectionEntry>("CityDirection.tbl");
                Class                             = GameTableFactory.Load<ClassEntry>("Class.tbl");
                ClassSecondaryStatBonus           = GameTableFactory.Load<ClassSecondaryStatBonusEntry>("ClassSecondaryStatBonus.tbl");
                ClientEvent                       = GameTableFactory.Load<ClientEventEntry>("ClientEvent.tbl");
                ClientEventAction                 = GameTableFactory.Load<ClientEventActionEntry>("ClientEventAction.tbl");
                ClientSideInteraction             = GameTableFactory.Load<ClientSideInteractionEntry>("ClientSideInteraction.tbl");
                //broken ColorShift               = GameTableFactory.Load<ColorShiftEntry>("ColorShift.tbl");
                CombatReward                      = GameTableFactory.Load<CombatRewardEntry>("CombatReward.tbl");
                CommunicatorMessages              = GameTableFactory.Load<CommunicatorMessagesEntry>("CommunicatorMessages.tbl");
                ComponentRegion                   = GameTableFactory.Load<ComponentRegionEntry>("ComponentRegion.tbl");
                ComponentRegionRect               = GameTableFactory.Load<ComponentRegionRectEntry>("ComponentRegionRect.tbl");
                CostumeSpecies                    = GameTableFactory.Load<CostumeSpeciesEntry>("CostumeSpecies.tbl");
                Creature2Action                   = GameTableFactory.Load<Creature2ActionEntry>("Creature2Action.tbl");
                Creature2ActionSet                = GameTableFactory.Load<Creature2ActionSetEntry>("Creature2ActionSet.tbl");
                Creature2ActionText               = GameTableFactory.Load<Creature2ActionTextEntry>("Creature2ActionText.tbl");
                Creature2Affiliation              = GameTableFactory.Load<Creature2AffiliationEntry>("Creature2Affiliation.tbl");
                Creature2DisplayInfo              = GameTableFactory.Load<Creature2DisplayInfoEntry>("Creature2DisplayInfo.tbl");
                Creature2OutfitInfo               = GameTableFactory.Load<Creature2OutfitInfoEntry>("Creature2OutfitInfo.tbl");
                Creature2OverrideProperties       = GameTableFactory.Load<Creature2OverridePropertiesEntry>("Creature2OverrideProperties.tbl");
                Creature2Resist                   = GameTableFactory.Load<Creature2ResistEntry>("Creature2Resist.tbl");
                //broken CurrencyType             = GameTableFactory.Load<CurrencyTypeEntry>("CurrencyType.tbl");
                CustomerSurvey                    = GameTableFactory.Load<CustomerSurveyEntry>("CustomerSurvey.tbl");
                CustomizationParameter            = GameTableFactory.Load<CustomizationParameterEntry>("CustomizationParameter.tbl");
                CustomizationParameterMap         = GameTableFactory.Load<CustomizationParameterMapEntry>("CustomizationParameterMap.tbl");
                DailyLoginReward                  = GameTableFactory.Load<DailyLoginRewardEntry>("DailyLoginReward.tbl");
                Datacube                          = GameTableFactory.Load<DatacubeEntry>("Datacube.tbl");
                DatacubeVolume                    = GameTableFactory.Load<DatacubeVolumeEntry>("DatacubeVolume.tbl");
                DistanceDamageModifier            = GameTableFactory.Load<DistanceDamageModifierEntry>("DistanceDamageModifier.tbl");
                DyeColorRamp                      = GameTableFactory.Load<DyeColorRampEntry>("DyeColorRamp.tbl");
                EldanAugmentation                 = GameTableFactory.Load<EldanAugmentationEntry>("EldanAugmentation.tbl");
                EldanAugmentationCategory         = GameTableFactory.Load<EldanAugmentationCategoryEntry>("EldanAugmentationCategory.tbl");
                EmoteSequenceTransition           = GameTableFactory.Load<EmoteSequenceTransitionEntry>("EmoteSequenceTransition.tbl");
                Entitlement                       = GameTableFactory.Load<EntitlementEntry>("Entitlement.tbl");
                Episode                           = GameTableFactory.Load<EpisodeEntry>("Episode.tbl");
                EpisodeQuest                      = GameTableFactory.Load<EpisodeQuestEntry>("EpisodeQuest.tbl");
                Faction2Relationship              = GameTableFactory.Load<Faction2RelationshipEntry>("Faction2Relationship.tbl");
                FinishingMoveDeathVisual          = GameTableFactory.Load<FinishingMoveDeathVisualEntry>("FinishingMoveDeathVisual.tbl");
                FullScreenEffect                  = GameTableFactory.Load<FullScreenEffectEntry>("FullScreenEffect.tbl");
                GameFormula                       = GameTableFactory.Load<GameFormulaEntry>("GameFormula.tbl");
                GenericMap                        = GameTableFactory.Load<GenericMapEntry>("GenericMap.tbl");
                GenericMapNode                    = GameTableFactory.Load<GenericMapNodeEntry>("GenericMapNode.tbl");
                GenericStringGroups               = GameTableFactory.Load<GenericStringGroupsEntry>("GenericStringGroups.tbl");
                GenericUnlockEntry                = GameTableFactory.Load<GenericUnlockEntryEntry>("GenericUnlockEntry.tbl");
                GenericUnlockSet                  = GameTableFactory.Load<GenericUnlockSetEntry>("GenericUnlockSet.tbl");
                GossipEntry                       = GameTableFactory.Load<GossipEntryEntry>("GossipEntry.tbl");
                GossipSet                         = GameTableFactory.Load<GossipSetEntry>("GossipSet.tbl");
                GuildPerk                         = GameTableFactory.Load<GuildPerkEntry>("GuildPerk.tbl");
                GuildPermission                   = GameTableFactory.Load<GuildPermissionEntry>("GuildPermission.tbl");
                GuildStandardPart                 = GameTableFactory.Load<GuildStandardPartEntry>("GuildStandardPart.tbl");
                Hazard                            = GameTableFactory.Load<HazardEntry>("Hazard.tbl");
                HookAsset                         = GameTableFactory.Load<HookAssetEntry>("HookAsset.tbl");
                HookType                          = GameTableFactory.Load<HookTypeEntry>("HookType.tbl");
                HousingBuild                      = GameTableFactory.Load<HousingBuildEntry>("HousingBuild.tbl");
                HousingContributionInfo           = GameTableFactory.Load<HousingContributionInfoEntry>("HousingContributionInfo.tbl");
                HousingContributionType           = GameTableFactory.Load<HousingContributionTypeEntry>("HousingContributionType.tbl");
                HousingDecorInfo                  = GameTableFactory.Load<HousingDecorInfoEntry>("HousingDecorInfo.tbl");
                HousingDecorLimitCategory         = GameTableFactory.Load<HousingDecorLimitCategoryEntry>("HousingDecorLimitCategory.tbl");
                HousingDecorType                  = GameTableFactory.Load<HousingDecorTypeEntry>("HousingDecorType.tbl");
                HousingMannequinPose              = GameTableFactory.Load<HousingMannequinPoseEntry>("HousingMannequinPose.tbl");
                HousingMapInfo                    = GameTableFactory.Load<HousingMapInfoEntry>("HousingMapInfo.tbl");
                HousingNeighborhoodInfo           = GameTableFactory.Load<HousingNeighborhoodInfoEntry>("HousingNeighborhoodInfo.tbl");
                HousingPlotInfo                   = GameTableFactory.Load<HousingPlotInfoEntry>("HousingPlotInfo.tbl");
                HousingPlotType                   = GameTableFactory.Load<HousingPlotTypeEntry>("HousingPlotType.tbl");
                HousingPlugItem                   = GameTableFactory.Load<HousingPlugItemEntry>("HousingPlugItem.tbl");
                HousingPropertyInfo               = GameTableFactory.Load<HousingPropertyInfoEntry>("HousingPropertyInfo.tbl");
                HousingResidenceInfo              = GameTableFactory.Load<HousingResidenceInfoEntry>("HousingResidenceInfo.tbl");
                HousingResource                   = GameTableFactory.Load<HousingResourceEntry>("HousingResource.tbl");
                HousingWallpaperInfo              = GameTableFactory.Load<HousingWallpaperInfoEntry>("HousingWallpaperInfo.tbl");
                HousingWarplotBossToken           = GameTableFactory.Load<HousingWarplotBossTokenEntry>("HousingWarplotBossToken.tbl");
                HousingWarplotPlugInfo            = GameTableFactory.Load<HousingWarplotPlugInfoEntry>("HousingWarplotPlugInfo.tbl");
                InputAction                       = GameTableFactory.Load<InputActionEntry>("InputAction.tbl");
                InputActionCategory               = GameTableFactory.Load<InputActionCategoryEntry>("InputActionCategory.tbl");
                InstancePortal                    = GameTableFactory.Load<InstancePortalEntry>("InstancePortal.tbl");
                Item2Category                     = GameTableFactory.Load<Item2CategoryEntry>("Item2Category.tbl");
                Item2Family                       = GameTableFactory.Load<Item2FamilyEntry>("Item2Family.tbl");
                ItemBudget                        = GameTableFactory.Load<ItemBudgetEntry>("ItemBudget.tbl");
                ItemColorSet                      = GameTableFactory.Load<ItemColorSetEntry>("ItemColorSet.tbl");
                //broken ItemDisplay              = GameTableFactory.Load<ItemDisplayEntry>("ItemDisplay.tbl");
                ItemImbuement                     = GameTableFactory.Load<ItemImbuementEntry>("ItemImbuement.tbl");
                ItemImbuementReward               = GameTableFactory.Load<ItemImbuementRewardEntry>("ItemImbuementReward.tbl");
                ItemProficiency                   = GameTableFactory.Load<ItemProficiencyEntry>("ItemProficiency.tbl");
                ItemQuality                       = GameTableFactory.Load<ItemQualityEntry>("ItemQuality.tbl");
                ItemRandomStat                    = GameTableFactory.Load<ItemRandomStatEntry>("ItemRandomStat.tbl");
                ItemRandomStatGroup               = GameTableFactory.Load<ItemRandomStatGroupEntry>("ItemRandomStatGroup.tbl");
                ItemRuneInstance                  = GameTableFactory.Load<ItemRuneInstanceEntry>("ItemRuneInstance.tbl");
                //empty ItemRuneSlotRandomization = GameTableFactory.Load<ItemRuneSlotRandomizationEntry>("ItemRuneSlotRandomization.tbl");
                ItemSet                           = GameTableFactory.Load<ItemSetEntry>("ItemSet.tbl");
                ItemSetBonus                      = GameTableFactory.Load<ItemSetBonusEntry>("ItemSetBonus.tbl");
                ItemSlot                          = GameTableFactory.Load<ItemSlotEntry>("ItemSlot.tbl");
                ItemSpecial                       = GameTableFactory.Load<ItemSpecialEntry>("ItemSpecial.tbl");
                ItemStat                          = GameTableFactory.Load<ItemStatEntry>("ItemStat.tbl");
                Language                          = GameTableFactory.Load<LanguageEntry>("Language.tbl");
                LevelDifferentialAttribute        = GameTableFactory.Load<LevelDifferentialAttributeEntry>("LevelDifferentialAttribute.tbl");
                LevelUpUnlock                     = GameTableFactory.Load<LevelUpUnlockEntry>("LevelUpUnlock.tbl");
                LevelUpUnlockType                 = GameTableFactory.Load<LevelUpUnlockTypeEntry>("LevelUpUnlockType.tbl");
                LiveEvent                         = GameTableFactory.Load<LiveEventEntry>("LiveEvent.tbl");
                LiveEventDisplayItem              = GameTableFactory.Load<LiveEventDisplayItemEntry>("LiveEventDisplayItem.tbl");
                LoadingScreenTip                  = GameTableFactory.Load<LoadingScreenTipEntry>("LoadingScreenTip.tbl");
                LocalizedEnum                     = GameTableFactory.Load<LocalizedEnumEntry>("LocalizedEnum.tbl");

                LootPinataInfo                    = GameTableFactory.Load<LootPinataInfoEntry>("LootPinataInfo.tbl");
                LootSpell                         = GameTableFactory.Load<LootSpellEntry>("LootSpell.tbl");
                LuaEvent                          = GameTableFactory.Load<LuaEventEntry>("LuaEvent.tbl");
                MapContinent                      = GameTableFactory.Load<MapContinentEntry>("MapContinent.tbl");
                MapZone                           = GameTableFactory.Load<MapZoneEntry>("MapZone.tbl");
                MapZoneHex                        = GameTableFactory.Load<MapZoneHexEntry>("MapZoneHex.tbl");
                MapZoneHexGroup                   = GameTableFactory.Load<MapZoneHexGroupEntry>("MapZoneHexGroup.tbl");
                MapZoneHexGroupEntry              = GameTableFactory.Load<MapZoneHexGroupEntryEntry>("MapZoneHexGroupEntry.tbl");
                //empty MapZoneLevelBand          = GameTableFactory.Load<MapZoneLevelBandEntry>("MapZoneLevelBand.tbl");
                MapZoneNemesisRegion              = GameTableFactory.Load<MapZoneNemesisRegionEntry>("MapZoneNemesisRegion.tbl");
                MapZonePOI                        = GameTableFactory.Load<MapZonePOIEntry>("MapZonePOI.tbl");
                MapZoneSprite                     = GameTableFactory.Load<MapZoneSpriteEntry>("MapZoneSprite.tbl");
                MapZoneWorldJoin                  = GameTableFactory.Load<MapZoneWorldJoinEntry>("MapZoneWorldJoin.tbl");
                MatchTypeRewardRotationContent    = GameTableFactory.Load<MatchTypeRewardRotationContentEntry>("MatchTypeRewardRotationContent.tbl");
                MatchingGameMap                   = GameTableFactory.Load<MatchingGameMapEntry>("MatchingGameMap.tbl");
                MatchingGameType                  = GameTableFactory.Load<MatchingGameTypeEntry>("MatchingGameType.tbl");
                //empty MatchingMapPrerequisite   = GameTableFactory.Load<MatchingMapPrerequisiteEntry>("MatchingMapPrerequisite.tbl");
                MatchingRandomReward              = GameTableFactory.Load<MatchingRandomRewardEntry>("MatchingRandomReward.tbl");
                MaterialData                      = GameTableFactory.Load<MaterialDataEntry>("MaterialData.tbl");
                MaterialRemap                     = GameTableFactory.Load<MaterialRemapEntry>("MaterialRemap.tbl");
                MaterialSet                       = GameTableFactory.Load<MaterialSetEntry>("MaterialSet.tbl");
                MaterialType                      = GameTableFactory.Load<MaterialTypeEntry>("MaterialType.tbl");
                MiniMapMarker                     = GameTableFactory.Load<MiniMapMarkerEntry>("MiniMapMarker.tbl");
                MissileRevolverTrack              = GameTableFactory.Load<MissileRevolverTrackEntry>("MissileRevolverTrack.tbl");
                ModelAttachment                 = GameTableFactory.Load<ModelAttachmentEntry>("ModelAttachment.tbl");
                ModelBone                         = GameTableFactory.Load<ModelBoneEntry>("ModelBone.tbl");
                ModelBonePriority                 = GameTableFactory.Load<ModelBonePriorityEntry>("ModelBonePriority.tbl");
                ModelBoneSet                      = GameTableFactory.Load<ModelBoneSetEntry>("ModelBoneSet.tbl");
                ModelCamera                       = GameTableFactory.Load<ModelCameraEntry>("ModelCamera.tbl");
                ModelCluster                      = GameTableFactory.Load<ModelClusterEntry>("ModelCluster.tbl");
                ModelEvent                        = GameTableFactory.Load<ModelEventEntry>("ModelEvent.tbl");
                ModelEventVisualJoin              = GameTableFactory.Load<ModelEventVisualJoinEntry>("ModelEventVisualJoin.tbl");
                ModelMesh                         = GameTableFactory.Load<ModelMeshEntry>("ModelMesh.tbl");
                ModelPose                         = GameTableFactory.Load<ModelPoseEntry>("ModelPose.tbl");
                ModelSequence                     = GameTableFactory.Load<ModelSequenceEntry>("ModelSequence.tbl");
                ModelSequenceByMode               = GameTableFactory.Load<ModelSequenceByModeEntry>("ModelSequenceByMode.tbl");
                ModelSequenceBySeatPosture        = GameTableFactory.Load<ModelSequenceBySeatPostureEntry>("ModelSequenceBySeatPosture.tbl");
                ModelSequenceByWeapon             = GameTableFactory.Load<ModelSequenceByWeaponEntry>("ModelSequenceByWeapon.tbl");
                ModelSequenceTransition           = GameTableFactory.Load<ModelSequenceTransitionEntry>("ModelSequenceTransition.tbl");
                ModelSkinFX                       = GameTableFactory.Load<ModelSkinFXEntry>("ModelSkinFX.tbl");
                PathEpisode                       = GameTableFactory.Load<PathEpisodeEntry>("PathEpisode.tbl");
                PathExplorerActivate              = GameTableFactory.Load<PathExplorerActivateEntry>("PathExplorerActivate.tbl");
                PathExplorerArea                  = GameTableFactory.Load<PathExplorerAreaEntry>("PathExplorerArea.tbl");
                PathExplorerDoor                  = GameTableFactory.Load<PathExplorerDoorEntry>("PathExplorerDoor.tbl");
                PathExplorerDoorEntrance          = GameTableFactory.Load<PathExplorerDoorEntranceEntry>("PathExplorerDoorEntrance.tbl");
                PathExplorerNode                  = GameTableFactory.Load<PathExplorerNodeEntry>("PathExplorerNode.tbl");
                PathExplorerPowerMap              = GameTableFactory.Load<PathExplorerPowerMapEntry>("PathExplorerPowerMap.tbl");
                PathExplorerScavengerClue         = GameTableFactory.Load<PathExplorerScavengerClueEntry>("PathExplorerScavengerClue.tbl");
                PathExplorerScavengerHunt         = GameTableFactory.Load<PathExplorerScavengerHuntEntry>("PathExplorerScavengerHunt.tbl");
                PathLevel                         = GameTableFactory.Load<PathLevelEntry>("PathLevel.tbl");
                PathMission                       = GameTableFactory.Load<PathMissionEntry>("PathMission.tbl");
                PathReward                        = GameTableFactory.Load<PathRewardEntry>("PathReward.tbl");
                PathScientistCreatureInfo         = GameTableFactory.Load<PathScientistCreatureInfoEntry>("PathScientistCreatureInfo.tbl");
                PathScientistDatacubeDiscovery    = GameTableFactory.Load<PathScientistDatacubeDiscoveryEntry>("PathScientistDatacubeDiscovery.tbl");
                PathScientistExperimentation      = GameTableFactory.Load<PathScientistExperimentationEntry>("PathScientistExperimentation.tbl");
                PathScientistExperimentationPattern = GameTableFactory.Load<PathScientistExperimentationPatternEntry>("PathScientistExperimentationPattern.tbl");
                PathScientistFieldStudy           = GameTableFactory.Load<PathScientistFieldStudyEntry>("PathScientistFieldStudy.tbl");
                PathScientistScanBotProfile       = GameTableFactory.Load<PathScientistScanBotProfileEntry>("PathScientistScanBotProfile.tbl");
                PathScientistSpecimenSurvey       = GameTableFactory.Load<PathScientistSpecimenSurveyEntry>("PathScientistSpecimenSurvey.tbl");
                PathSettlerHub                    = GameTableFactory.Load<PathSettlerHubEntry>("PathSettlerHub.tbl");
                PathSettlerImprovement            = GameTableFactory.Load<PathSettlerImprovementEntry>("PathSettlerImprovement.tbl");
                PathSettlerImprovementGroup       = GameTableFactory.Load<PathSettlerImprovementGroupEntry>("PathSettlerImprovementGroup.tbl");
                PathSettlerInfrastructure         = GameTableFactory.Load<PathSettlerInfrastructureEntry>("PathSettlerInfrastructure.tbl");
                PathSettlerMayor                  = GameTableFactory.Load<PathSettlerMayorEntry>("PathSettlerMayor.tbl");
                PathSettlerSheriff                = GameTableFactory.Load<PathSettlerSheriffEntry>("PathSettlerSheriff.tbl");
                PathSoldierActivate               = GameTableFactory.Load<PathSoldierActivateEntry>("PathSoldierActivate.tbl");
                PathSoldierAssassinate            = GameTableFactory.Load<PathSoldierAssassinateEntry>("PathSoldierAssassinate.tbl");
                PathSoldierEvent                  = GameTableFactory.Load<PathSoldierEventEntry>("PathSoldierEvent.tbl");
                PathSoldierEventWave              = GameTableFactory.Load<PathSoldierEventWaveEntry>("PathSoldierEventWave.tbl");
                PathSoldierSWAT                   = GameTableFactory.Load<PathSoldierSWATEntry>("PathSoldierSWAT.tbl");
                PathSoldierTowerDefense           = GameTableFactory.Load<PathSoldierTowerDefenseEntry>("PathSoldierTowerDefense.tbl");
                PeriodicQuestGroup                = GameTableFactory.Load<PeriodicQuestGroupEntry>("PeriodicQuestGroup.tbl");
                PeriodicQuestSet                  = GameTableFactory.Load<PeriodicQuestSetEntry>("PeriodicQuestSet.tbl");
                PeriodicQuestSetCategory          = GameTableFactory.Load<PeriodicQuestSetCategoryEntry>("PeriodicQuestSetCategory.tbl");
                PetFlair                          = GameTableFactory.Load<PetFlairEntry>("PetFlair.tbl");
                PlayerNotificationType            = GameTableFactory.Load<PlayerNotificationTypeEntry>("PlayerNotificationType.tbl");
                PositionalRequirement             = GameTableFactory.Load<PositionalRequirementEntry>("PositionalRequirement.tbl");
                Prerequisite                      = GameTableFactory.Load<PrerequisiteEntry>("Prerequisite.tbl");
                PrerequisiteType                  = GameTableFactory.Load<PrerequisiteTypeEntry>("PrerequisiteType.tbl");
                PrimalMatrixNode                  = GameTableFactory.Load<PrimalMatrixNodeEntry>("PrimalMatrixNode.tbl");
                PrimalMatrixReward                = GameTableFactory.Load<PrimalMatrixRewardEntry>("PrimalMatrixReward.tbl");
                PropAdditionalDetail              = GameTableFactory.Load<PropAdditionalDetailEntry>("PropAdditionalDetail.tbl");
                PublicEvent                       = GameTableFactory.Load<PublicEventEntry>("PublicEvent.tbl");
                PublicEventCustomStat             = GameTableFactory.Load<PublicEventCustomStatEntry>("PublicEventCustomStat.tbl");
                PublicEventDepot                  = GameTableFactory.Load<PublicEventDepotEntry>("PublicEventDepot.tbl");
                PublicEventObjective              = GameTableFactory.Load<PublicEventObjectiveEntry>("PublicEventObjective.tbl");
                PublicEventObjectiveBombDeployment = GameTableFactory.Load<PublicEventObjectiveBombDeploymentEntry>("PublicEventObjectiveBombDeployment.tbl");
                PublicEventObjectiveGatherResource = GameTableFactory.Load<PublicEventObjectiveGatherResourceEntry>("PublicEventObjectiveGatherResource.tbl");
                PublicEventObjectiveState         = GameTableFactory.Load<PublicEventObjectiveStateEntry>("PublicEventObjectiveState.tbl");
                PublicEventRewardModifier         = GameTableFactory.Load<PublicEventRewardModifierEntry>("PublicEventRewardModifier.tbl");
                PublicEventStatDisplay            = GameTableFactory.Load<PublicEventStatDisplayEntry>("PublicEventStatDisplay.tbl");
                PublicEventTeam                   = GameTableFactory.Load<PublicEventTeamEntry>("PublicEventTeam.tbl");
                //empty PublicEventUnitPropertyModifier = GameTableFactory.Load<PublicEventUnitPropertyModifierEntry>("PublicEventUnitPropertyModifier.tbl");
                PublicEventVirtualItemDepot       = GameTableFactory.Load<PublicEventVirtualItemDepotEntry>("PublicEventVirtualItemDepot.tbl");
                PublicEventVote                   = GameTableFactory.Load<PublicEventVoteEntry>("PublicEventVote.tbl");
                PvPRatingFloor                    = GameTableFactory.Load<PvPRatingFloorEntry>("PvPRatingFloor.tbl");
                Quest2                            = GameTableFactory.Load<Quest2Entry>("Quest2.tbl");
                Quest2Difficulty                  = GameTableFactory.Load<Quest2DifficultyEntry>("Quest2Difficulty.tbl");
                Quest2RandomTextLineJoin          = GameTableFactory.Load<Quest2RandomTextLineJoinEntry>("Quest2RandomTextLineJoin.tbl");
                Quest2Reward                      = GameTableFactory.Load<Quest2RewardEntry>("Quest2Reward.tbl");
                QuestCategory                     = GameTableFactory.Load<QuestCategoryEntry>("QuestCategory.tbl");
                QuestDirection                    = GameTableFactory.Load<QuestDirectionEntry>("QuestDirection.tbl");
                QuestDirectionEntry               = GameTableFactory.Load<QuestDirectionEntryEntry>("QuestDirectionEntry.tbl");
                QuestGroup                        = GameTableFactory.Load<QuestGroupEntry>("QuestGroup.tbl");
                QuestHub                          = GameTableFactory.Load<QuestHubEntry>("QuestHub.tbl");
                QuestObjective                    = GameTableFactory.Load<QuestObjectiveEntry>("QuestObjective.tbl");
                //broken Race                     = GameTableFactory.Load<RaceEntry>("Race.tbl");
                RandomPlayerName                  = GameTableFactory.Load<RandomPlayerNameEntry>("RandomPlayerName.tbl");
                RandomTextLine                    = GameTableFactory.Load<RandomTextLineEntry>("RandomTextLine.tbl");
                RandomTextLineSet                 = GameTableFactory.Load<RandomTextLineSetEntry>("RandomTextLineSet.tbl");
                RealmDataCenter                   = GameTableFactory.Load<RealmDataCenterEntry>("RealmDataCenter.tbl");
                Redacted                          = GameTableFactory.Load<RedactedEntry>("Redacted.tbl");
                ReplaceableMaterialInfo           = GameTableFactory.Load<ReplaceableMaterialInfoEntry>("ReplaceableMaterialInfo.tbl");
                ResourceConversion                = GameTableFactory.Load<ResourceConversionEntry>("ResourceConversion.tbl");
                ResourceConversionGroup           = GameTableFactory.Load<ResourceConversionGroupEntry>("ResourceConversionGroup.tbl");
                RewardProperty                    = GameTableFactory.Load<RewardPropertyEntry>("RewardProperty.tbl");
                RewardPropertyPremiumModifier     = GameTableFactory.Load<RewardPropertyPremiumModifierEntry>("RewardPropertyPremiumModifier.tbl");
                RewardRotationContent             = GameTableFactory.Load<RewardRotationContentEntry>("RewardRotationContent.tbl");
                RewardRotationEssence             = GameTableFactory.Load<RewardRotationEssenceEntry>("RewardRotationEssence.tbl");
                RewardRotationItem                = GameTableFactory.Load<RewardRotationItemEntry>("RewardRotationItem.tbl");
                RewardRotationModifier            = GameTableFactory.Load<RewardRotationModifierEntry>("RewardRotationModifier.tbl");
                RewardTrack                       = GameTableFactory.Load<RewardTrackEntry>("RewardTrack.tbl");
                RewardTrackRewards                = GameTableFactory.Load<RewardTrackRewardsEntry>("RewardTrackRewards.tbl");
                Salvage                           = GameTableFactory.Load<SalvageEntry>("Salvage.tbl");
                SalvageException                  = GameTableFactory.Load<SalvageExceptionEntry>("SalvageException.tbl");
                SkyCloudSet                       = GameTableFactory.Load<SkyCloudSetEntry>("SkyCloudSet.tbl");
                SkyTrackCloudSet                  = GameTableFactory.Load<SkyTrackCloudSetEntry>("SkyTrackCloudSet.tbl");
                SoundBank                         = GameTableFactory.Load<SoundBankEntry>("SoundBank.tbl");
                SoundCombatLoop                   = GameTableFactory.Load<SoundCombatLoopEntry>("SoundCombatLoop.tbl");
                SoundContext                      = GameTableFactory.Load<SoundContextEntry>("SoundContext.tbl");
                SoundDirectionalAmbience          = GameTableFactory.Load<SoundDirectionalAmbienceEntry>("SoundDirectionalAmbience.tbl");
                SoundEnvironment                  = GameTableFactory.Load<SoundEnvironmentEntry>("SoundEnvironment.tbl");
                SoundEvent                        = GameTableFactory.Load<SoundEventEntry>("SoundEvent.tbl");
                SoundImpactEvents                 = GameTableFactory.Load<SoundImpactEventsEntry>("SoundImpactEvents.tbl");
                SoundMusicSet                     = GameTableFactory.Load<SoundMusicSetEntry>("SoundMusicSet.tbl");
                SoundParameter                    = GameTableFactory.Load<SoundParameterEntry>("SoundParameter.tbl");
                //empty SoundReplace              = GameTableFactory.Load<SoundReplaceEntry>("SoundReplace.tbl");
                //empty SoundReplaceDescription   = GameTableFactory.Load<SoundReplaceDescriptionEntry>("SoundReplaceDescription.tbl");
                SoundStates                       = GameTableFactory.Load<SoundStatesEntry>("SoundStates.tbl");
                SoundSwitch                       = GameTableFactory.Load<SoundSwitchEntry>("SoundSwitch.tbl");
                SoundUIContext                    = GameTableFactory.Load<SoundUIContextEntry>("SoundUIContext.tbl");
                SoundZoneKit                      = GameTableFactory.Load<SoundZoneKitEntry>("SoundZoneKit.tbl");
                Spell4                            = GameTableFactory.Load<Spell4Entry>("Spell4.tbl");
                Spell4AoeTargetConstraints        = GameTableFactory.Load<Spell4AoeTargetConstraintsEntry>("Spell4AoeTargetConstraints.tbl");
                Spell4Base                        = GameTableFactory.Load<Spell4BaseEntry>("Spell4Base.tbl");
                Spell4CCConditions                = GameTableFactory.Load<Spell4CCConditionsEntry>("Spell4CCConditions.tbl");
                Spell4CastResult                  = GameTableFactory.Load<Spell4CastResultEntry>("Spell4CastResult.tbl");
                Spell4ClientMissile               = GameTableFactory.Load<Spell4ClientMissileEntry>("Spell4ClientMissile.tbl");
                Spell4Conditions                  = GameTableFactory.Load<Spell4ConditionsEntry>("Spell4Conditions.tbl");
                Spell4EffectGroupList             = GameTableFactory.Load<Spell4EffectGroupListEntry>("Spell4EffectGroupList.tbl");
                Spell4EffectModification          = GameTableFactory.Load<Spell4EffectModificationEntry>("Spell4EffectModification.tbl");
                Spell4Effects                     = GameTableFactory.Load<Spell4EffectsEntry>("Spell4Effects.tbl");
                Spell4GroupList                   = GameTableFactory.Load<Spell4GroupListEntry>("Spell4GroupList.tbl");
                Spell4HitResults                  = GameTableFactory.Load<Spell4HitResultsEntry>("Spell4HitResults.tbl");
                Spell4Modification                = GameTableFactory.Load<Spell4ModificationEntry>("Spell4Modification.tbl");
                Spell4Prerequisites               = GameTableFactory.Load<Spell4PrerequisitesEntry>("Spell4Prerequisites.tbl");
                Spell4Reagent                     = GameTableFactory.Load<Spell4ReagentEntry>("Spell4Reagent.tbl");
                Spell4Runner                      = GameTableFactory.Load<Spell4RunnerEntry>("Spell4Runner.tbl");
                Spell4ServiceTokenCost            = GameTableFactory.Load<Spell4ServiceTokenCostEntry>("Spell4ServiceTokenCost.tbl");
                Spell4SpellTypes                  = GameTableFactory.Load<Spell4SpellTypesEntry>("Spell4SpellTypes.tbl");
                Spell4StackGroup                  = GameTableFactory.Load<Spell4StackGroupEntry>("Spell4StackGroup.tbl");
                Spell4Tag                         = GameTableFactory.Load<Spell4TagEntry>("Spell4Tag.tbl");
                Spell4TargetAngle                 = GameTableFactory.Load<Spell4TargetAngleEntry>("Spell4TargetAngle.tbl");
                Spell4TargetMechanics             = GameTableFactory.Load<Spell4TargetMechanicsEntry>("Spell4TargetMechanics.tbl");
                Spell4Telegraph                   = GameTableFactory.Load<Spell4TelegraphEntry>("Spell4Telegraph.tbl");
                Spell4Thresholds                  = GameTableFactory.Load<Spell4ThresholdsEntry>("Spell4Thresholds.tbl");
                Spell4TierRequirements            = GameTableFactory.Load<Spell4TierRequirementsEntry>("Spell4TierRequirements.tbl");
                Spell4ValidTargets                = GameTableFactory.Load<Spell4ValidTargetsEntry>("Spell4ValidTargets.tbl");
                Spell4Visual                      = GameTableFactory.Load<Spell4VisualEntry>("Spell4Visual.tbl");
                Spell4VisualGroup                 = GameTableFactory.Load<Spell4VisualGroupEntry>("Spell4VisualGroup.tbl");
                SpellCoolDown                     = GameTableFactory.Load<SpellCoolDownEntry>("SpellCoolDown.tbl");
                SpellEffectType                   = GameTableFactory.Load<SpellEffectTypeEntry>("SpellEffectType.tbl");
                SpellLevel                        = GameTableFactory.Load<SpellLevelEntry>("SpellLevel.tbl");
                SpellPhase                        = GameTableFactory.Load<SpellPhaseEntry>("SpellPhase.tbl");
                Spline2                           = GameTableFactory.Load<Spline2Entry>("Spline2.tbl");
                Spline2Node                       = GameTableFactory.Load<Spline2NodeEntry>("Spline2Node.tbl");
                StoreDisplayInfo                  = GameTableFactory.Load<StoreDisplayInfoEntry>("StoreDisplayInfo.tbl");
                StoreKeyword                      = GameTableFactory.Load<StoreKeywordEntry>("StoreKeyword.tbl");
                StoreLink                         = GameTableFactory.Load<StoreLinkEntry>("StoreLink.tbl");
                StoryPanel                        = GameTableFactory.Load<StoryPanelEntry>("StoryPanel.tbl");
                TargetGroup                       = GameTableFactory.Load<TargetGroupEntry>("TargetGroup.tbl");
                TargetMarker                      = GameTableFactory.Load<TargetMarkerEntry>("TargetMarker.tbl");
                TaxiNode                          = GameTableFactory.Load<TaxiNodeEntry>("TaxiNode.tbl");
                TaxiRoute                         = GameTableFactory.Load<TaxiRouteEntry>("TaxiRoute.tbl");
                TelegraphDamage                   = GameTableFactory.Load<TelegraphDamageEntry>("TelegraphDamage.tbl");
                TicketCategory                    = GameTableFactory.Load<TicketCategoryEntry>("TicketCategory.tbl");
                TicketSubCategory                 = GameTableFactory.Load<TicketSubCategoryEntry>("TicketSubCategory.tbl");
                TrackingSlot                      = GameTableFactory.Load<TrackingSlotEntry>("TrackingSlot.tbl");
                Tradeskill                        = GameTableFactory.Load<TradeskillEntry>("Tradeskill.tbl");
                TradeskillAchievementLayout       = GameTableFactory.Load<TradeskillAchievementLayoutEntry>("TradeskillAchievementLayout.tbl");
                TradeskillAchievementReward       = GameTableFactory.Load<TradeskillAchievementRewardEntry>("TradeskillAchievementReward.tbl");
                TradeskillAdditive                = GameTableFactory.Load<TradeskillAdditiveEntry>("TradeskillAdditive.tbl");
                TradeskillBonus                   = GameTableFactory.Load<TradeskillBonusEntry>("TradeskillBonus.tbl");
                TradeskillCatalyst                = GameTableFactory.Load<TradeskillCatalystEntry>("TradeskillCatalyst.tbl");
                TradeskillCatalystOrdering        = GameTableFactory.Load<TradeskillCatalystOrderingEntry>("TradeskillCatalystOrdering.tbl");
                TradeskillHarvestingInfo          = GameTableFactory.Load<TradeskillHarvestingInfoEntry>("TradeskillHarvestingInfo.tbl");
                TradeskillMaterial                = GameTableFactory.Load<TradeskillMaterialEntry>("TradeskillMaterial.tbl");
                TradeskillMaterialCategory        = GameTableFactory.Load<TradeskillMaterialCategoryEntry>("TradeskillMaterialCategory.tbl");
                TradeskillProficiency             = GameTableFactory.Load<TradeskillProficiencyEntry>("TradeskillProficiency.tbl");
                TradeskillSchematic2              = GameTableFactory.Load<TradeskillSchematic2Entry>("TradeskillSchematic2.tbl");
                TradeskillTalentTier              = GameTableFactory.Load<TradeskillTalentTierEntry>("TradeskillTalentTier.tbl");
                TradeskillTier                    = GameTableFactory.Load<TradeskillTierEntry>("TradeskillTier.tbl");
                Tutorial                          = GameTableFactory.Load<TutorialEntry>("Tutorial.tbl");
                TutorialAnchor                    = GameTableFactory.Load<TutorialAnchorEntry>("TutorialAnchor.tbl");
                TutorialLayout                    = GameTableFactory.Load<TutorialLayoutEntry>("TutorialLayout.tbl");
                TutorialPage                      = GameTableFactory.Load<TutorialPageEntry>("TutorialPage.tbl");
                UnitProperty2                     = GameTableFactory.Load<UnitProperty2Entry>("UnitProperty2.tbl");
                UnitVehicle                       = GameTableFactory.Load<UnitVehicleEntry>("UnitVehicle.tbl");
                VeteranTier                       = GameTableFactory.Load<VeteranTierEntry>("VeteranTier.tbl");
                VirtualItem                       = GameTableFactory.Load<VirtualItemEntry>("VirtualItem.tbl");
                VisualEffect                      = GameTableFactory.Load<VisualEffectEntry>("VisualEffect.tbl");
                Vital                             = GameTableFactory.Load<VitalEntry>("Vital.tbl");
                WaterSurfaceEffect                = GameTableFactory.Load<WaterSurfaceEffectEntry>("WaterSurfaceEffect.tbl");
                Wind                              = GameTableFactory.Load<WindEntry>("Wind.tbl");
                WindSpawn                         = GameTableFactory.Load<WindSpawnEntry>("WindSpawn.tbl");
                WordFilter                        = GameTableFactory.Load<WordFilterEntry>("WordFilter.tbl");
                //empty WordFilterAlt             = GameTableFactory.Load<WordFilterAltEntry>("WordFilterAlt.tbl");
                WorldClutter                      = GameTableFactory.Load<WorldClutterEntry>("WorldClutter.tbl");
                //broken WorldLayer               = GameTableFactory.Load<WorldLayerEntry>("WorldLayer.tbl");
                WorldLocation2                    = GameTableFactory.Load<WorldLocation2Entry>("WorldLocation2.tbl");
                WorldSky                          = GameTableFactory.Load<WorldSkyEntry>("WorldSky.tbl");
                WorldSocket                       = GameTableFactory.Load<WorldSocketEntry>("WorldSocket.tbl");
                WorldWaterEnvironment             = GameTableFactory.Load<WorldWaterEnvironmentEntry>("WorldWaterEnvironment.tbl");
                WorldWaterFog                     = GameTableFactory.Load<WorldWaterFogEntry>("WorldWaterFog.tbl");
                WorldWaterLayer                   = GameTableFactory.Load<WorldWaterLayerEntry>("WorldWaterLayer.tbl");
                WorldWaterType                    = GameTableFactory.Load<WorldWaterTypeEntry>("WorldWaterType.tbl");
                WorldWaterWake                    = GameTableFactory.Load<WorldWaterWakeEntry>("WorldWaterWake.tbl");
                WorldZone                         = GameTableFactory.Load<WorldZoneEntry>("WorldZone.tbl");
                XpPerLevel                        = GameTableFactory.Load<XpPerLevelEntry>("XpPerLevel.tbl");
                ZoneCompletion                    = GameTableFactory.Load<ZoneCompletionEntry>("ZoneCompletion.tbl");
                */
