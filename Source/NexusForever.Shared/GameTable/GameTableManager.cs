using System;
using System.IO;
using System.Diagnostics;
using NexusForever.Shared.GameTable.Model;
using NLog;

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
	

        public static void Initialise()
        {
            log.Info("Loading GameTables...");
            var sw = Stopwatch.StartNew();

            try
            {
                AccountCurrencyType = new GameTable<AccountCurrencyTypeEntry>("tbl/AccountCurrencyType.tbl");
                AccountItem = new GameTable<AccountItemEntry>("tbl/AccountItem.tbl");
                AccountItemCooldownGroup = new GameTable<AccountItemCooldownGroupEntry>("tbl/AccountItemCooldownGroup.tbl");
                Achievement = new GameTable<AchievementEntry>("tbl/Achievement.tbl");
                AchievementCategory = new GameTable<AchievementCategoryEntry>("tbl/AchievementCategory.tbl");
                AchievementChecklist = new GameTable<AchievementChecklistEntry>("tbl/AchievementChecklist.tbl");
                AchievementGroup = new GameTable<AchievementGroupEntry>("tbl/AchievementGroup.tbl");
                AchievementSubGroup = new GameTable<AchievementSubGroupEntry>("tbl/AchievementSubGroup.tbl");
                AchievementText = new GameTable<AchievementTextEntry>("tbl/AchievementText.tbl");
                ActionBarShortcutSet = new GameTable<ActionBarShortcutSetEntry>("tbl/ActionBarShortcutSet.tbl");
                ActionSlotPrereq = new GameTable<ActionSlotPrereqEntry>("tbl/ActionSlotPrereq.tbl");
                ArchiveArticle = new GameTable<ArchiveArticleEntry>("tbl/ArchiveArticle.tbl");
                ArchiveCategory = new GameTable<ArchiveCategoryEntry>("tbl/ArchiveCategory.tbl");
                ArchiveEntry = new GameTable<ArchiveEntryEntry>("tbl/ArchiveEntry.tbl");
                ArchiveEntryUnlockRule = new GameTable<ArchiveEntryUnlockRuleEntry>("tbl/ArchiveEntryUnlockRule.tbl");
                ArchiveLink = new GameTable<ArchiveLinkEntry>("tbl/ArchiveLink.tbl");
                AttributeMilestoneGroup = new GameTable<AttributeMilestoneGroupEntry>("tbl/AttributeMilestoneGroup.tbl");
                AttributeMiniMilestoneGroup = new GameTable<AttributeMiniMilestoneGroupEntry>("tbl/AttributeMiniMilestoneGroup.tbl");
                BindPoint = new GameTable<BindPointEntry>("tbl/BindPoint.tbl");
                BinkMovie = new GameTable<BinkMovieEntry>("tbl/BinkMovie.tbl");
                BinkMovieSubtitle = new GameTable<BinkMovieSubtitleEntry>("tbl/BinkMovieSubtitle.tbl");
                BugCategory = new GameTable<BugCategoryEntry>("tbl/BugCategory.tbl");
                BugSubcategory = new GameTable<BugSubcategoryEntry>("tbl/BugSubcategory.tbl");
                CCStateAdditionalData = new GameTable<CCStateAdditionalDataEntry>("tbl/CCStateAdditionalData.tbl");
                CCStateDiminishingReturns = new GameTable<CCStateDiminishingReturnsEntry>("tbl/CCStateDiminishingReturns.tbl");
                CCStates = new GameTable<CCStatesEntry>("tbl/CCStates.tbl");
                Challenge = new GameTable<ChallengeEntry>("tbl/Challenge.tbl");
                ChallengeTier = new GameTable<ChallengeTierEntry>("tbl/ChallengeTier.tbl");
                CharacterCreation = new GameTable<CharacterCreationEntry>("tbl/CharacterCreation.tbl");
                CharacterCreationArmorSet = new GameTable<CharacterCreationArmorSetEntry>("tbl/CharacterCreationArmorSet.tbl");
                CharacterCreationPreset = new GameTable<CharacterCreationPresetEntry>("tbl/CharacterCreationPreset.tbl");
                CharacterCustomization = new GameTable<CharacterCustomizationEntry>("tbl/CharacterCustomization.tbl");
                CharacterCustomizationLabel = new GameTable<CharacterCustomizationLabelEntry>("tbl/CharacterCustomizationLabel.tbl");
                CharacterCustomizationSelection = new GameTable<CharacterCustomizationSelectionEntry>("tbl/CharacterCustomizationSelection.tbl");
                CharacterTitle = new GameTable<CharacterTitleEntry>("tbl/CharacterTitle.tbl");
                CharacterTitleCategory = new GameTable<CharacterTitleCategoryEntry>("tbl/CharacterTitleCategory.tbl");
                ChatChannel = new GameTable<ChatChannelEntry>("tbl/ChatChannel.tbl");
                Cinematic = new GameTable<CinematicEntry>("tbl/Cinematic.tbl");
                CinematicRace = new GameTable<CinematicRaceEntry>("tbl/CinematicRace.tbl");
                CityDirection = new GameTable<CityDirectionEntry>("tbl/CityDirection.tbl");
                Class = new GameTable<ClassEntry>("tbl/Class.tbl");
                ClassSecondaryStatBonus = new GameTable<ClassSecondaryStatBonusEntry>("tbl/ClassSecondaryStatBonus.tbl");
                ClientEvent = new GameTable<ClientEventEntry>("tbl/ClientEvent.tbl");
                ClientEventAction = new GameTable<ClientEventActionEntry>("tbl/ClientEventAction.tbl");
                ClientSideInteraction = new GameTable<ClientSideInteractionEntry>("tbl/ClientSideInteraction.tbl");
                //broken ColorShift = new GameTable<ColorShiftEntry>("tbl/ColorShift.tbl");
                CombatReward = new GameTable<CombatRewardEntry>("tbl/CombatReward.tbl");
                CommunicatorMessages = new GameTable<CommunicatorMessagesEntry>("tbl/CommunicatorMessages.tbl");
                ComponentRegion = new GameTable<ComponentRegionEntry>("tbl/ComponentRegion.tbl");
                ComponentRegionRect = new GameTable<ComponentRegionRectEntry>("tbl/ComponentRegionRect.tbl");
                CostumeSpecies = new GameTable<CostumeSpeciesEntry>("tbl/CostumeSpecies.tbl");
                Creature2 = new GameTable<Creature2Entry>("tbl/Creature2.tbl");
                Creature2Action = new GameTable<Creature2ActionEntry>("tbl/Creature2Action.tbl");
                Creature2ActionSet = new GameTable<Creature2ActionSetEntry>("tbl/Creature2ActionSet.tbl");
                Creature2ActionText = new GameTable<Creature2ActionTextEntry>("tbl/Creature2ActionText.tbl");
                Creature2Affiliation = new GameTable<Creature2AffiliationEntry>("tbl/Creature2Affiliation.tbl");
                Creature2ArcheType = new GameTable<Creature2ArcheTypeEntry>("tbl/Creature2ArcheType.tbl");
                Creature2Difficulty = new GameTable<Creature2DifficultyEntry>("tbl/Creature2Difficulty.tbl");
                Creature2DisplayGroupEntry = new GameTable<Creature2DisplayGroupEntryEntry>("tbl/Creature2DisplayGroupEntry.tbl");
                Creature2DisplayInfo = new GameTable<Creature2DisplayInfoEntry>("tbl/Creature2DisplayInfo.tbl");
                Creature2ModelInfo = new GameTable<Creature2ModelInfoEntry>("tbl/Creature2ModelInfo.tbl");
                Creature2OutfitGroupEntry = new GameTable<Creature2OutfitGroupEntryEntry>("tbl/Creature2OutfitGroupEntry.tbl");
                Creature2OutfitInfo = new GameTable<Creature2OutfitInfoEntry>("tbl/Creature2OutfitInfo.tbl");
                Creature2OverrideProperties = new GameTable<Creature2OverridePropertiesEntry>("tbl/Creature2OverrideProperties.tbl");
                Creature2Resist = new GameTable<Creature2ResistEntry>("tbl/Creature2Resist.tbl");
                Creature2Tier = new GameTable<Creature2TierEntry>("tbl/Creature2Tier.tbl");
                CreatureLevel = new GameTable<CreatureLevelEntry>("tbl/CreatureLevel.tbl");
                //broken CurrencyType = new GameTable<CurrencyTypeEntry>("tbl/CurrencyType.tbl");
                CustomerSurvey = new GameTable<CustomerSurveyEntry>("tbl/CustomerSurvey.tbl");
                CustomizationParameter = new GameTable<CustomizationParameterEntry>("tbl/CustomizationParameter.tbl");
                CustomizationParameterMap = new GameTable<CustomizationParameterMapEntry>("tbl/CustomizationParameterMap.tbl");
                DailyLoginReward = new GameTable<DailyLoginRewardEntry>("tbl/DailyLoginReward.tbl");
                Datacube = new GameTable<DatacubeEntry>("tbl/Datacube.tbl");
                DatacubeVolume = new GameTable<DatacubeVolumeEntry>("tbl/DatacubeVolume.tbl");
                DistanceDamageModifier = new GameTable<DistanceDamageModifierEntry>("tbl/DistanceDamageModifier.tbl");
                DyeColorRamp = new GameTable<DyeColorRampEntry>("tbl/DyeColorRamp.tbl");
                EldanAugmentation = new GameTable<EldanAugmentationEntry>("tbl/EldanAugmentation.tbl");
                EldanAugmentationCategory = new GameTable<EldanAugmentationCategoryEntry>("tbl/EldanAugmentationCategory.tbl");
                EmoteSequenceTransition = new GameTable<EmoteSequenceTransitionEntry>("tbl/EmoteSequenceTransition.tbl");
                Emotes = new GameTable<EmotesEntry>("tbl/Emotes.tbl");
                Entitlement = new GameTable<EntitlementEntry>("tbl/Entitlement.tbl");
                Episode = new GameTable<EpisodeEntry>("tbl/Episode.tbl");
                EpisodeQuest = new GameTable<EpisodeQuestEntry>("tbl/EpisodeQuest.tbl");
                Faction2 = new GameTable<Faction2Entry>("tbl/Faction2.tbl");
                Faction2Relationship = new GameTable<Faction2RelationshipEntry>("tbl/Faction2Relationship.tbl");
                FinishingMoveDeathVisual = new GameTable<FinishingMoveDeathVisualEntry>("tbl/FinishingMoveDeathVisual.tbl");
                FullScreenEffect = new GameTable<FullScreenEffectEntry>("tbl/FullScreenEffect.tbl");
                GameFormula = new GameTable<GameFormulaEntry>("tbl/GameFormula.tbl");
                GenericMap = new GameTable<GenericMapEntry>("tbl/GenericMap.tbl");
                GenericMapNode = new GameTable<GenericMapNodeEntry>("tbl/GenericMapNode.tbl");
                GenericStringGroups = new GameTable<GenericStringGroupsEntry>("tbl/GenericStringGroups.tbl");
                GenericUnlockEntry = new GameTable<GenericUnlockEntryEntry>("tbl/GenericUnlockEntry.tbl");
                GenericUnlockSet = new GameTable<GenericUnlockSetEntry>("tbl/GenericUnlockSet.tbl");
                GossipEntry = new GameTable<GossipEntryEntry>("tbl/GossipEntry.tbl");
                GossipSet = new GameTable<GossipSetEntry>("tbl/GossipSet.tbl");
                GuildPerk = new GameTable<GuildPerkEntry>("tbl/GuildPerk.tbl");
                GuildPermission = new GameTable<GuildPermissionEntry>("tbl/GuildPermission.tbl");
                GuildStandardPart = new GameTable<GuildStandardPartEntry>("tbl/GuildStandardPart.tbl");
                Hazard = new GameTable<HazardEntry>("tbl/Hazard.tbl");
                HookAsset = new GameTable<HookAssetEntry>("tbl/HookAsset.tbl");
                HookType = new GameTable<HookTypeEntry>("tbl/HookType.tbl");
                HousingBuild = new GameTable<HousingBuildEntry>("tbl/HousingBuild.tbl");
                HousingContributionInfo = new GameTable<HousingContributionInfoEntry>("tbl/HousingContributionInfo.tbl");
                HousingContributionType = new GameTable<HousingContributionTypeEntry>("tbl/HousingContributionType.tbl");
                HousingDecorInfo = new GameTable<HousingDecorInfoEntry>("tbl/HousingDecorInfo.tbl");
                HousingDecorLimitCategory = new GameTable<HousingDecorLimitCategoryEntry>("tbl/HousingDecorLimitCategory.tbl");
                HousingDecorType = new GameTable<HousingDecorTypeEntry>("tbl/HousingDecorType.tbl");
                HousingMannequinPose = new GameTable<HousingMannequinPoseEntry>("tbl/HousingMannequinPose.tbl");
                HousingMapInfo = new GameTable<HousingMapInfoEntry>("tbl/HousingMapInfo.tbl");
                HousingNeighborhoodInfo = new GameTable<HousingNeighborhoodInfoEntry>("tbl/HousingNeighborhoodInfo.tbl");
                HousingPlotInfo = new GameTable<HousingPlotInfoEntry>("tbl/HousingPlotInfo.tbl");
                HousingPlotType = new GameTable<HousingPlotTypeEntry>("tbl/HousingPlotType.tbl");
                HousingPlugItem = new GameTable<HousingPlugItemEntry>("tbl/HousingPlugItem.tbl");
                HousingPropertyInfo = new GameTable<HousingPropertyInfoEntry>("tbl/HousingPropertyInfo.tbl");
                HousingResidenceInfo = new GameTable<HousingResidenceInfoEntry>("tbl/HousingResidenceInfo.tbl");
                HousingResource = new GameTable<HousingResourceEntry>("tbl/HousingResource.tbl");
                HousingWallpaperInfo = new GameTable<HousingWallpaperInfoEntry>("tbl/HousingWallpaperInfo.tbl");
                HousingWarplotBossToken = new GameTable<HousingWarplotBossTokenEntry>("tbl/HousingWarplotBossToken.tbl");
                HousingWarplotPlugInfo = new GameTable<HousingWarplotPlugInfoEntry>("tbl/HousingWarplotPlugInfo.tbl");
                InputAction = new GameTable<InputActionEntry>("tbl/InputAction.tbl");
                InputActionCategory = new GameTable<InputActionCategoryEntry>("tbl/InputActionCategory.tbl");
                InstancePortal = new GameTable<InstancePortalEntry>("tbl/InstancePortal.tbl");
                Item = new GameTable<Item2Entry>("tbl/Item2.tbl");
                Item2Category = new GameTable<Item2CategoryEntry>("tbl/Item2Category.tbl");
                Item2Family = new GameTable<Item2FamilyEntry>("tbl/Item2Family.tbl");
                ItemType = new GameTable<Item2TypeEntry>("tbl/Item2Type.tbl");
                ItemBudget = new GameTable<ItemBudgetEntry>("tbl/ItemBudget.tbl");
                ItemColorSet = new GameTable<ItemColorSetEntry>("tbl/ItemColorSet.tbl");
                //broken ItemDisplay = new GameTable<ItemDisplayEntry>("tbl/ItemDisplay.tbl");
                ItemDisplaySourceEntry = new GameTable<ItemDisplaySourceEntryEntry>("tbl/ItemDisplaySourceEntry.tbl");
                ItemImbuement = new GameTable<ItemImbuementEntry>("tbl/ItemImbuement.tbl");
                ItemImbuementReward = new GameTable<ItemImbuementRewardEntry>("tbl/ItemImbuementReward.tbl");
                ItemProficiency = new GameTable<ItemProficiencyEntry>("tbl/ItemProficiency.tbl");
                ItemQuality = new GameTable<ItemQualityEntry>("tbl/ItemQuality.tbl");
                ItemRandomStat = new GameTable<ItemRandomStatEntry>("tbl/ItemRandomStat.tbl");
                ItemRandomStatGroup = new GameTable<ItemRandomStatGroupEntry>("tbl/ItemRandomStatGroup.tbl");
                ItemRuneInstance = new GameTable<ItemRuneInstanceEntry>("tbl/ItemRuneInstance.tbl");
                //empty ItemRuneSlotRandomization = new GameTable<ItemRuneSlotRandomizationEntry>("tbl/ItemRuneSlotRandomization.tbl");
                ItemSet = new GameTable<ItemSetEntry>("tbl/ItemSet.tbl");
                ItemSetBonus = new GameTable<ItemSetBonusEntry>("tbl/ItemSetBonus.tbl");
                ItemSlot = new GameTable<ItemSlotEntry>("tbl/ItemSlot.tbl");
                ItemSpecial = new GameTable<ItemSpecialEntry>("tbl/ItemSpecial.tbl");
                ItemStat = new GameTable<ItemStatEntry>("tbl/ItemStat.tbl");
                Language = new GameTable<LanguageEntry>("tbl/Language.tbl");
                LevelDifferentialAttribute = new GameTable<LevelDifferentialAttributeEntry>("tbl/LevelDifferentialAttribute.tbl");
                LevelUpUnlock = new GameTable<LevelUpUnlockEntry>("tbl/LevelUpUnlock.tbl");
                LevelUpUnlockType = new GameTable<LevelUpUnlockTypeEntry>("tbl/LevelUpUnlockType.tbl");
                LiveEvent = new GameTable<LiveEventEntry>("tbl/LiveEvent.tbl");
                LiveEventDisplayItem = new GameTable<LiveEventDisplayItemEntry>("tbl/LiveEventDisplayItem.tbl");
                LoadingScreenTip = new GameTable<LoadingScreenTipEntry>("tbl/LoadingScreenTip.tbl");
                LocalizedEnum = new GameTable<LocalizedEnumEntry>("tbl/LocalizedEnum.tbl");
                LocalizedText = new GameTable<LocalizedTextEntry>("tbl/LocalizedText.tbl");
                LootPinataInfo = new GameTable<LootPinataInfoEntry>("tbl/LootPinataInfo.tbl");
                LootSpell = new GameTable<LootSpellEntry>("tbl/LootSpell.tbl");
                LuaEvent = new GameTable<LuaEventEntry>("tbl/LuaEvent.tbl");
                MapContinent = new GameTable<MapContinentEntry>("tbl/MapContinent.tbl");
                MapZone = new GameTable<MapZoneEntry>("tbl/MapZone.tbl");
                MapZoneHex = new GameTable<MapZoneHexEntry>("tbl/MapZoneHex.tbl");
                MapZoneHexGroup = new GameTable<MapZoneHexGroupEntry>("tbl/MapZoneHexGroup.tbl");
                MapZoneHexGroupEntry = new GameTable<MapZoneHexGroupEntryEntry>("tbl/MapZoneHexGroupEntry.tbl");
                //empty MapZoneLevelBand = new GameTable<MapZoneLevelBandEntry>("tbl/MapZoneLevelBand.tbl");
                MapZoneNemesisRegion = new GameTable<MapZoneNemesisRegionEntry>("tbl/MapZoneNemesisRegion.tbl");
                MapZonePOI = new GameTable<MapZonePOIEntry>("tbl/MapZonePOI.tbl");
                MapZoneSprite = new GameTable<MapZoneSpriteEntry>("tbl/MapZoneSprite.tbl");
                MapZoneWorldJoin = new GameTable<MapZoneWorldJoinEntry>("tbl/MapZoneWorldJoin.tbl");
                MatchTypeRewardRotationContent = new GameTable<MatchTypeRewardRotationContentEntry>("tbl/MatchTypeRewardRotationContent.tbl");
                MatchingGameMap = new GameTable<MatchingGameMapEntry>("tbl/MatchingGameMap.tbl");
                MatchingGameType = new GameTable<MatchingGameTypeEntry>("tbl/MatchingGameType.tbl");
                //empty MatchingMapPrerequisite = new GameTable<MatchingMapPrerequisiteEntry>("tbl/MatchingMapPrerequisite.tbl");
                MatchingRandomReward = new GameTable<MatchingRandomRewardEntry>("tbl/MatchingRandomReward.tbl");
                MaterialData = new GameTable<MaterialDataEntry>("tbl/MaterialData.tbl");
                MaterialRemap = new GameTable<MaterialRemapEntry>("tbl/MaterialRemap.tbl");
                MaterialSet = new GameTable<MaterialSetEntry>("tbl/MaterialSet.tbl");
                MaterialType = new GameTable<MaterialTypeEntry>("tbl/MaterialType.tbl");
                MiniMapMarker = new GameTable<MiniMapMarkerEntry>("tbl/MiniMapMarker.tbl");
                MissileRevolverTrack = new GameTable<MissileRevolverTrackEntry>("tbl/MissileRevolverTrack.tbl");
                ModelAttachment = new GameTable<ModelAttachmentEntry>("tbl/ModelAttachment.tbl");
                ModelBone = new GameTable<ModelBoneEntry>("tbl/ModelBone.tbl");
                ModelBonePriority = new GameTable<ModelBonePriorityEntry>("tbl/ModelBonePriority.tbl");
                ModelBoneSet = new GameTable<ModelBoneSetEntry>("tbl/ModelBoneSet.tbl");
                ModelCamera = new GameTable<ModelCameraEntry>("tbl/ModelCamera.tbl");
                ModelCluster = new GameTable<ModelClusterEntry>("tbl/ModelCluster.tbl");
                ModelEvent = new GameTable<ModelEventEntry>("tbl/ModelEvent.tbl");
                ModelEventVisualJoin = new GameTable<ModelEventVisualJoinEntry>("tbl/ModelEventVisualJoin.tbl");
                ModelMesh = new GameTable<ModelMeshEntry>("tbl/ModelMesh.tbl");
                ModelPose = new GameTable<ModelPoseEntry>("tbl/ModelPose.tbl");
                ModelSequence = new GameTable<ModelSequenceEntry>("tbl/ModelSequence.tbl");
                ModelSequenceByMode = new GameTable<ModelSequenceByModeEntry>("tbl/ModelSequenceByMode.tbl");
                ModelSequenceBySeatPosture = new GameTable<ModelSequenceBySeatPostureEntry>("tbl/ModelSequenceBySeatPosture.tbl");
                ModelSequenceByWeapon = new GameTable<ModelSequenceByWeaponEntry>("tbl/ModelSequenceByWeapon.tbl");
                ModelSequenceTransition = new GameTable<ModelSequenceTransitionEntry>("tbl/ModelSequenceTransition.tbl");
                ModelSkinFX = new GameTable<ModelSkinFXEntry>("tbl/ModelSkinFX.tbl");
                PathEpisode = new GameTable<PathEpisodeEntry>("tbl/PathEpisode.tbl");
                PathExplorerActivate = new GameTable<PathExplorerActivateEntry>("tbl/PathExplorerActivate.tbl");
                PathExplorerArea = new GameTable<PathExplorerAreaEntry>("tbl/PathExplorerArea.tbl");
                PathExplorerDoor = new GameTable<PathExplorerDoorEntry>("tbl/PathExplorerDoor.tbl");
                PathExplorerDoorEntrance = new GameTable<PathExplorerDoorEntranceEntry>("tbl/PathExplorerDoorEntrance.tbl");
                PathExplorerNode = new GameTable<PathExplorerNodeEntry>("tbl/PathExplorerNode.tbl");
                PathExplorerPowerMap = new GameTable<PathExplorerPowerMapEntry>("tbl/PathExplorerPowerMap.tbl");
                PathExplorerScavengerClue = new GameTable<PathExplorerScavengerClueEntry>("tbl/PathExplorerScavengerClue.tbl");
                PathExplorerScavengerHunt = new GameTable<PathExplorerScavengerHuntEntry>("tbl/PathExplorerScavengerHunt.tbl");
                PathLevel = new GameTable<PathLevelEntry>("tbl/PathLevel.tbl");
                PathMission = new GameTable<PathMissionEntry>("tbl/PathMission.tbl");
                PathReward = new GameTable<PathRewardEntry>("tbl/PathReward.tbl");
                PathScientistCreatureInfo = new GameTable<PathScientistCreatureInfoEntry>("tbl/PathScientistCreatureInfo.tbl");
                PathScientistDatacubeDiscovery = new GameTable<PathScientistDatacubeDiscoveryEntry>("tbl/PathScientistDatacubeDiscovery.tbl");
                PathScientistExperimentation = new GameTable<PathScientistExperimentationEntry>("tbl/PathScientistExperimentation.tbl");
                PathScientistExperimentationPattern = new GameTable<PathScientistExperimentationPatternEntry>("tbl/PathScientistExperimentationPattern.tbl");
                PathScientistFieldStudy = new GameTable<PathScientistFieldStudyEntry>("tbl/PathScientistFieldStudy.tbl");
                PathScientistScanBotProfile = new GameTable<PathScientistScanBotProfileEntry>("tbl/PathScientistScanBotProfile.tbl");
                PathScientistSpecimenSurvey = new GameTable<PathScientistSpecimenSurveyEntry>("tbl/PathScientistSpecimenSurvey.tbl");
                PathSettlerHub = new GameTable<PathSettlerHubEntry>("tbl/PathSettlerHub.tbl");
                PathSettlerImprovement = new GameTable<PathSettlerImprovementEntry>("tbl/PathSettlerImprovement.tbl");
                PathSettlerImprovementGroup = new GameTable<PathSettlerImprovementGroupEntry>("tbl/PathSettlerImprovementGroup.tbl");
                PathSettlerInfrastructure = new GameTable<PathSettlerInfrastructureEntry>("tbl/PathSettlerInfrastructure.tbl");
                PathSettlerMayor = new GameTable<PathSettlerMayorEntry>("tbl/PathSettlerMayor.tbl");
                PathSettlerSheriff = new GameTable<PathSettlerSheriffEntry>("tbl/PathSettlerSheriff.tbl");
                PathSoldierActivate = new GameTable<PathSoldierActivateEntry>("tbl/PathSoldierActivate.tbl");
                PathSoldierAssassinate = new GameTable<PathSoldierAssassinateEntry>("tbl/PathSoldierAssassinate.tbl");
                PathSoldierEvent = new GameTable<PathSoldierEventEntry>("tbl/PathSoldierEvent.tbl");
                PathSoldierEventWave = new GameTable<PathSoldierEventWaveEntry>("tbl/PathSoldierEventWave.tbl");
                PathSoldierSWAT = new GameTable<PathSoldierSWATEntry>("tbl/PathSoldierSWAT.tbl");
                PathSoldierTowerDefense = new GameTable<PathSoldierTowerDefenseEntry>("tbl/PathSoldierTowerDefense.tbl");
                PeriodicQuestGroup = new GameTable<PeriodicQuestGroupEntry>("tbl/PeriodicQuestGroup.tbl");
                PeriodicQuestSet = new GameTable<PeriodicQuestSetEntry>("tbl/PeriodicQuestSet.tbl");
                PeriodicQuestSetCategory = new GameTable<PeriodicQuestSetCategoryEntry>("tbl/PeriodicQuestSetCategory.tbl");
                PetFlair = new GameTable<PetFlairEntry>("tbl/PetFlair.tbl");
                PlayerNotificationType = new GameTable<PlayerNotificationTypeEntry>("tbl/PlayerNotificationType.tbl");
                PositionalRequirement = new GameTable<PositionalRequirementEntry>("tbl/PositionalRequirement.tbl");
                Prerequisite = new GameTable<PrerequisiteEntry>("tbl/Prerequisite.tbl");
                PrerequisiteType = new GameTable<PrerequisiteTypeEntry>("tbl/PrerequisiteType.tbl");
                PrimalMatrixNode = new GameTable<PrimalMatrixNodeEntry>("tbl/PrimalMatrixNode.tbl");
                PrimalMatrixReward = new GameTable<PrimalMatrixRewardEntry>("tbl/PrimalMatrixReward.tbl");
                PropAdditionalDetail = new GameTable<PropAdditionalDetailEntry>("tbl/PropAdditionalDetail.tbl");
                PublicEvent = new GameTable<PublicEventEntry>("tbl/PublicEvent.tbl");
                PublicEventCustomStat = new GameTable<PublicEventCustomStatEntry>("tbl/PublicEventCustomStat.tbl");
                PublicEventDepot = new GameTable<PublicEventDepotEntry>("tbl/PublicEventDepot.tbl");
                PublicEventObjective = new GameTable<PublicEventObjectiveEntry>("tbl/PublicEventObjective.tbl");
                PublicEventObjectiveBombDeployment = new GameTable<PublicEventObjectiveBombDeploymentEntry>("tbl/PublicEventObjectiveBombDeployment.tbl");
                PublicEventObjectiveGatherResource = new GameTable<PublicEventObjectiveGatherResourceEntry>("tbl/PublicEventObjectiveGatherResource.tbl");
                PublicEventObjectiveState = new GameTable<PublicEventObjectiveStateEntry>("tbl/PublicEventObjectiveState.tbl");
                PublicEventRewardModifier = new GameTable<PublicEventRewardModifierEntry>("tbl/PublicEventRewardModifier.tbl");
                PublicEventStatDisplay = new GameTable<PublicEventStatDisplayEntry>("tbl/PublicEventStatDisplay.tbl");
                PublicEventTeam = new GameTable<PublicEventTeamEntry>("tbl/PublicEventTeam.tbl");
                //empty PublicEventUnitPropertyModifier = new GameTable<PublicEventUnitPropertyModifierEntry>("tbl/PublicEventUnitPropertyModifier.tbl");
                PublicEventVirtualItemDepot = new GameTable<PublicEventVirtualItemDepotEntry>("tbl/PublicEventVirtualItemDepot.tbl");
                PublicEventVote = new GameTable<PublicEventVoteEntry>("tbl/PublicEventVote.tbl");
                PvPRatingFloor = new GameTable<PvPRatingFloorEntry>("tbl/PvPRatingFloor.tbl");
                Quest2 = new GameTable<Quest2Entry>("tbl/Quest2.tbl");
                Quest2Difficulty = new GameTable<Quest2DifficultyEntry>("tbl/Quest2Difficulty.tbl");
                Quest2RandomTextLineJoin = new GameTable<Quest2RandomTextLineJoinEntry>("tbl/Quest2RandomTextLineJoin.tbl");
                Quest2Reward = new GameTable<Quest2RewardEntry>("tbl/Quest2Reward.tbl");
                QuestCategory = new GameTable<QuestCategoryEntry>("tbl/QuestCategory.tbl");
                QuestDirection = new GameTable<QuestDirectionEntry>("tbl/QuestDirection.tbl");
                QuestDirectionEntry = new GameTable<QuestDirectionEntryEntry>("tbl/QuestDirectionEntry.tbl");
                QuestGroup = new GameTable<QuestGroupEntry>("tbl/QuestGroup.tbl");
                QuestHub = new GameTable<QuestHubEntry>("tbl/QuestHub.tbl");
                QuestObjective = new GameTable<QuestObjectiveEntry>("tbl/QuestObjective.tbl");
                //broken Race = new GameTable<RaceEntry>("tbl/Race.tbl");
                RandomPlayerName = new GameTable<RandomPlayerNameEntry>("tbl/RandomPlayerName.tbl");
                RandomTextLine = new GameTable<RandomTextLineEntry>("tbl/RandomTextLine.tbl");
                RandomTextLineSet = new GameTable<RandomTextLineSetEntry>("tbl/RandomTextLineSet.tbl");
                RealmDataCenter = new GameTable<RealmDataCenterEntry>("tbl/RealmDataCenter.tbl");
                Redacted = new GameTable<RedactedEntry>("tbl/Redacted.tbl");
                ReplaceableMaterialInfo = new GameTable<ReplaceableMaterialInfoEntry>("tbl/ReplaceableMaterialInfo.tbl");
                ResourceConversion = new GameTable<ResourceConversionEntry>("tbl/ResourceConversion.tbl");
                ResourceConversionGroup = new GameTable<ResourceConversionGroupEntry>("tbl/ResourceConversionGroup.tbl");
                RewardProperty = new GameTable<RewardPropertyEntry>("tbl/RewardProperty.tbl");
                RewardPropertyPremiumModifier = new GameTable<RewardPropertyPremiumModifierEntry>("tbl/RewardPropertyPremiumModifier.tbl");
                RewardRotationContent = new GameTable<RewardRotationContentEntry>("tbl/RewardRotationContent.tbl");
                RewardRotationEssence = new GameTable<RewardRotationEssenceEntry>("tbl/RewardRotationEssence.tbl");
                RewardRotationItem = new GameTable<RewardRotationItemEntry>("tbl/RewardRotationItem.tbl");
                RewardRotationModifier = new GameTable<RewardRotationModifierEntry>("tbl/RewardRotationModifier.tbl");
                RewardTrack = new GameTable<RewardTrackEntry>("tbl/RewardTrack.tbl");
                RewardTrackRewards = new GameTable<RewardTrackRewardsEntry>("tbl/RewardTrackRewards.tbl");
                Salvage = new GameTable<SalvageEntry>("tbl/Salvage.tbl");
                SalvageException = new GameTable<SalvageExceptionEntry>("tbl/SalvageException.tbl");
                SkyCloudSet = new GameTable<SkyCloudSetEntry>("tbl/SkyCloudSet.tbl");
                SkyTrackCloudSet = new GameTable<SkyTrackCloudSetEntry>("tbl/SkyTrackCloudSet.tbl");
                SoundBank = new GameTable<SoundBankEntry>("tbl/SoundBank.tbl");
                SoundCombatLoop = new GameTable<SoundCombatLoopEntry>("tbl/SoundCombatLoop.tbl");
                SoundContext = new GameTable<SoundContextEntry>("tbl/SoundContext.tbl");
                SoundDirectionalAmbience = new GameTable<SoundDirectionalAmbienceEntry>("tbl/SoundDirectionalAmbience.tbl");
                SoundEnvironment = new GameTable<SoundEnvironmentEntry>("tbl/SoundEnvironment.tbl");
                SoundEvent = new GameTable<SoundEventEntry>("tbl/SoundEvent.tbl");
                SoundImpactEvents = new GameTable<SoundImpactEventsEntry>("tbl/SoundImpactEvents.tbl");
                SoundMusicSet = new GameTable<SoundMusicSetEntry>("tbl/SoundMusicSet.tbl");
                SoundParameter = new GameTable<SoundParameterEntry>("tbl/SoundParameter.tbl");
                //empty SoundReplace = new GameTable<SoundReplaceEntry>("tbl/SoundReplace.tbl");
                //empty SoundReplaceDescription = new GameTable<SoundReplaceDescriptionEntry>("tbl/SoundReplaceDescription.tbl");
                SoundStates = new GameTable<SoundStatesEntry>("tbl/SoundStates.tbl");
                SoundSwitch = new GameTable<SoundSwitchEntry>("tbl/SoundSwitch.tbl");
                SoundUIContext = new GameTable<SoundUIContextEntry>("tbl/SoundUIContext.tbl");
                SoundZoneKit = new GameTable<SoundZoneKitEntry>("tbl/SoundZoneKit.tbl");
                Spell4 = new GameTable<Spell4Entry>("tbl/Spell4.tbl");
                Spell4AoeTargetConstraints = new GameTable<Spell4AoeTargetConstraintsEntry>("tbl/Spell4AoeTargetConstraints.tbl");
                Spell4Base = new GameTable<Spell4BaseEntry>("tbl/Spell4Base.tbl");
                Spell4CCConditions = new GameTable<Spell4CCConditionsEntry>("tbl/Spell4CCConditions.tbl");
                Spell4CastResult = new GameTable<Spell4CastResultEntry>("tbl/Spell4CastResult.tbl");
                Spell4ClientMissile = new GameTable<Spell4ClientMissileEntry>("tbl/Spell4ClientMissile.tbl");
                Spell4Conditions = new GameTable<Spell4ConditionsEntry>("tbl/Spell4Conditions.tbl");
                Spell4EffectGroupList = new GameTable<Spell4EffectGroupListEntry>("tbl/Spell4EffectGroupList.tbl");
                Spell4EffectModification = new GameTable<Spell4EffectModificationEntry>("tbl/Spell4EffectModification.tbl");
                Spell4Effects = new GameTable<Spell4EffectsEntry>("tbl/Spell4Effects.tbl");
                Spell4GroupList = new GameTable<Spell4GroupListEntry>("tbl/Spell4GroupList.tbl");
                Spell4HitResults = new GameTable<Spell4HitResultsEntry>("tbl/Spell4HitResults.tbl");
                Spell4Modification = new GameTable<Spell4ModificationEntry>("tbl/Spell4Modification.tbl");
                Spell4Prerequisites = new GameTable<Spell4PrerequisitesEntry>("tbl/Spell4Prerequisites.tbl");
                Spell4Reagent = new GameTable<Spell4ReagentEntry>("tbl/Spell4Reagent.tbl");
                Spell4Runner = new GameTable<Spell4RunnerEntry>("tbl/Spell4Runner.tbl");
                Spell4ServiceTokenCost = new GameTable<Spell4ServiceTokenCostEntry>("tbl/Spell4ServiceTokenCost.tbl");
                Spell4SpellTypes = new GameTable<Spell4SpellTypesEntry>("tbl/Spell4SpellTypes.tbl");
                Spell4StackGroup = new GameTable<Spell4StackGroupEntry>("tbl/Spell4StackGroup.tbl");
                Spell4Tag = new GameTable<Spell4TagEntry>("tbl/Spell4Tag.tbl");
                Spell4TargetAngle = new GameTable<Spell4TargetAngleEntry>("tbl/Spell4TargetAngle.tbl");
                Spell4TargetMechanics = new GameTable<Spell4TargetMechanicsEntry>("tbl/Spell4TargetMechanics.tbl");
                Spell4Telegraph = new GameTable<Spell4TelegraphEntry>("tbl/Spell4Telegraph.tbl");
                Spell4Thresholds = new GameTable<Spell4ThresholdsEntry>("tbl/Spell4Thresholds.tbl");
                Spell4TierRequirements = new GameTable<Spell4TierRequirementsEntry>("tbl/Spell4TierRequirements.tbl");
                Spell4ValidTargets = new GameTable<Spell4ValidTargetsEntry>("tbl/Spell4ValidTargets.tbl");
                Spell4Visual = new GameTable<Spell4VisualEntry>("tbl/Spell4Visual.tbl");
                Spell4VisualGroup = new GameTable<Spell4VisualGroupEntry>("tbl/Spell4VisualGroup.tbl");
                SpellCoolDown = new GameTable<SpellCoolDownEntry>("tbl/SpellCoolDown.tbl");
                SpellEffectType = new GameTable<SpellEffectTypeEntry>("tbl/SpellEffectType.tbl");
                SpellLevel = new GameTable<SpellLevelEntry>("tbl/SpellLevel.tbl");
                SpellPhase = new GameTable<SpellPhaseEntry>("tbl/SpellPhase.tbl");
                Spline2 = new GameTable<Spline2Entry>("tbl/Spline2.tbl");
                Spline2Node = new GameTable<Spline2NodeEntry>("tbl/Spline2Node.tbl");
                StoreDisplayInfo = new GameTable<StoreDisplayInfoEntry>("tbl/StoreDisplayInfo.tbl");
                StoreKeyword = new GameTable<StoreKeywordEntry>("tbl/StoreKeyword.tbl");
                StoreLink = new GameTable<StoreLinkEntry>("tbl/StoreLink.tbl");
                StoryPanel = new GameTable<StoryPanelEntry>("tbl/StoryPanel.tbl");
                TargetGroup = new GameTable<TargetGroupEntry>("tbl/TargetGroup.tbl");
                TargetMarker = new GameTable<TargetMarkerEntry>("tbl/TargetMarker.tbl");
                TaxiNode = new GameTable<TaxiNodeEntry>("tbl/TaxiNode.tbl");
                TaxiRoute = new GameTable<TaxiRouteEntry>("tbl/TaxiRoute.tbl");
                TelegraphDamage = new GameTable<TelegraphDamageEntry>("tbl/TelegraphDamage.tbl");
                TicketCategory = new GameTable<TicketCategoryEntry>("tbl/TicketCategory.tbl");
                TicketSubCategory = new GameTable<TicketSubCategoryEntry>("tbl/TicketSubCategory.tbl");
                TrackingSlot = new GameTable<TrackingSlotEntry>("tbl/TrackingSlot.tbl");
                Tradeskill = new GameTable<TradeskillEntry>("tbl/Tradeskill.tbl");
                TradeskillAchievementLayout = new GameTable<TradeskillAchievementLayoutEntry>("tbl/TradeskillAchievementLayout.tbl");
                TradeskillAchievementReward = new GameTable<TradeskillAchievementRewardEntry>("tbl/TradeskillAchievementReward.tbl");
                TradeskillAdditive = new GameTable<TradeskillAdditiveEntry>("tbl/TradeskillAdditive.tbl");
                TradeskillBonus = new GameTable<TradeskillBonusEntry>("tbl/TradeskillBonus.tbl");
                TradeskillCatalyst = new GameTable<TradeskillCatalystEntry>("tbl/TradeskillCatalyst.tbl");
                TradeskillCatalystOrdering = new GameTable<TradeskillCatalystOrderingEntry>("tbl/TradeskillCatalystOrdering.tbl");
                TradeskillHarvestingInfo = new GameTable<TradeskillHarvestingInfoEntry>("tbl/TradeskillHarvestingInfo.tbl");
                TradeskillMaterial = new GameTable<TradeskillMaterialEntry>("tbl/TradeskillMaterial.tbl");
                TradeskillMaterialCategory = new GameTable<TradeskillMaterialCategoryEntry>("tbl/TradeskillMaterialCategory.tbl");
                TradeskillProficiency = new GameTable<TradeskillProficiencyEntry>("tbl/TradeskillProficiency.tbl");
                TradeskillSchematic2 = new GameTable<TradeskillSchematic2Entry>("tbl/TradeskillSchematic2.tbl");
                TradeskillTalentTier = new GameTable<TradeskillTalentTierEntry>("tbl/TradeskillTalentTier.tbl");
                TradeskillTier = new GameTable<TradeskillTierEntry>("tbl/TradeskillTier.tbl");
                Tutorial = new GameTable<TutorialEntry>("tbl/Tutorial.tbl");
                TutorialAnchor = new GameTable<TutorialAnchorEntry>("tbl/TutorialAnchor.tbl");
                TutorialLayout = new GameTable<TutorialLayoutEntry>("tbl/TutorialLayout.tbl");
                TutorialPage = new GameTable<TutorialPageEntry>("tbl/TutorialPage.tbl");
                UnitProperty2 = new GameTable<UnitProperty2Entry>("tbl/UnitProperty2.tbl");
                UnitRace = new GameTable<UnitRaceEntry>("tbl/UnitRace.tbl");
                UnitVehicle = new GameTable<UnitVehicleEntry>("tbl/UnitVehicle.tbl");
                VeteranTier = new GameTable<VeteranTierEntry>("tbl/VeteranTier.tbl");
                VirtualItem = new GameTable<VirtualItemEntry>("tbl/VirtualItem.tbl");
                VisualEffect = new GameTable<VisualEffectEntry>("tbl/VisualEffect.tbl");
                Vital = new GameTable<VitalEntry>("tbl/Vital.tbl");
                WaterSurfaceEffect = new GameTable<WaterSurfaceEffectEntry>("tbl/WaterSurfaceEffect.tbl");
                Wind = new GameTable<WindEntry>("tbl/Wind.tbl");
                WindSpawn = new GameTable<WindSpawnEntry>("tbl/WindSpawn.tbl");
                WordFilter = new GameTable<WordFilterEntry>("tbl/WordFilter.tbl");
                //empty WordFilterAlt = new GameTable<WordFilterAltEntry>("tbl/WordFilterAlt.tbl");
                World = new GameTable<WorldEntry>("tbl/World.tbl");
                WorldClutter = new GameTable<WorldClutterEntry>("tbl/WorldClutter.tbl");
                //broken WorldLayer = new GameTable<WorldLayerEntry>("tbl/WorldLayer.tbl");
                WorldLocation2 = new GameTable<WorldLocation2Entry>("tbl/WorldLocation2.tbl");
                WorldSky = new GameTable<WorldSkyEntry>("tbl/WorldSky.tbl");
                WorldSocket = new GameTable<WorldSocketEntry>("tbl/WorldSocket.tbl");
                WorldWaterEnvironment = new GameTable<WorldWaterEnvironmentEntry>("tbl/WorldWaterEnvironment.tbl");
                WorldWaterFog = new GameTable<WorldWaterFogEntry>("tbl/WorldWaterFog.tbl");
                WorldWaterLayer = new GameTable<WorldWaterLayerEntry>("tbl/WorldWaterLayer.tbl");
                WorldWaterType = new GameTable<WorldWaterTypeEntry>("tbl/WorldWaterType.tbl");
                WorldWaterWake = new GameTable<WorldWaterWakeEntry>("tbl/WorldWaterWake.tbl");
                WorldZone = new GameTable<WorldZoneEntry>("tbl/WorldZone.tbl");
                XpPerLevel = new GameTable<XpPerLevelEntry>("tbl/XpPerLevel.tbl");
                ZoneCompletion = new GameTable<ZoneCompletionEntry>("tbl/ZoneCompletion.tbl");
            }
            catch (Exception exception)
            {
                log.Fatal(exception);
                throw;
            }

            log.Info($"Loaded GameTables in {sw.ElapsedMilliseconds}ms.");
        }
    }
}
