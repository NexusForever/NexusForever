namespace NexusForever.Game.Static.Prerequisite
{
    public enum PrerequisiteType
    {
        None                        = 0,
        Level                       = 1, // Level requirement not met
        Race                        = 2, // Race requirement not met
        Class                       = 3, // Class requirement not met
        Faction                     = 4, // Faction requirement not met
        Reputation                  = 5, // Reputation requirement not met
        QuestState                  = 6, // Quest requirement not met
        AchievementState            = 7, // Achievement requirement not met
        ItemProficiency             = 8, // Item proficiency requirement not met
        EpisodeState                = 9, // Episode requirement not met
        Gender                      = 10, // Gender requirement not met
        OtherPrerequisite           = 11, // Other requirement not met
        DeadState                   = 12, // Player death state not correct
        ItemEquipped                = 13, // Item equipment requirement not met
        ItemOnCharacter             = 14, // Inventory requirement not met
        UnderSpell                  = 15, // Spell requirements not met
        DistanceToWorldLocation     = 16, // Distance requirements not met
        // 17 is unused in PrerequisiteType.tbl
        ScheduledEvent              = 18, // You cannot do that at this time
        TradeSkillProfession        = 19, // You must have the correct tradeskill tier.
        GroupSize                   = 20, // Incorrect party size
        TradeSkillSchematicLevel    = 21, // Incorrect schematic level
        QuestObjective0             = 22, // Quest objective requirements not met
        QuestObjective1             = 23, // Quest objective requirements not met
        InPhase                     = 24, // You cannot do that
        CanSeePhase                 = 25, // You cannot do that
        InSubZone                   = 26, // Incorrect zone
        InTriggerVolume             = 27, // You are out of bounds
        InCombat                    = 28, // You must be in combat
        TimeOfDay                   = 29, // Incorrect time of day
        FacilityWithinDistance      = 30, // Distance requirement not met
        CreatureWithinDistance      = 31, // Distance requirement not met
        // 32 is unused in PrerequisiteType.tbl
        QuestObjective2             = 33, // Quest objective requirements not met
        QuestObjective3             = 34, // Quest objective requirements not met
        QuestObjective4             = 35, // Quest objective requirements not met
        QuestObjective5             = 36, // Quest objective requirements not met
        RandomPercent               = 37, // Random check: Failed
        IsCreature                  = 38, // Incorrect creature
        IsPlayer                    = 39, // Incorrect player
        Health                      = 40, // Incorrect health
        ZoneExplored                = 41, // Zone exploration criteria not met
        IsGroupLeader               = 42, // You cannot do that
        IsObjectiveActive           = 43, // You cannot do that right now
        InTargetGroup               = 44, // You cannot do that right now
        // 45 is unused in PrerequisiteType.tbl
        Waypoint                    = 46, // Incorrect waypoint direction
        Unknown47                   = 47, // You cannot do that right now
        Unknown48                   = 48, // You cannot do that right now
        Schedule                    = 49, // That schedule is unavailable
        Unknown50                   = 50, // You cannot do that now
        ItemQuantity                = 51, // You do not have the correct number of items
        Path                        = 52, // You do not meet the player path requirement
        PathEpisode                 = 53, // You do not meet the player path episode requirement
        PlayerPathMission           = 54, // You do not meet the player path mission requirement
        Unknown55                   = 55, // Requirements not met
        Unknown56                   = 56, // Requirements not met
        // 57 is unused in PrerequisiteType.tbl
        // 58 is unused in PrerequisiteType.tbl
        Spell59                     = 59, // Spell requirements not met
        Spell60                     = 60, // Spell requirements not met
        PathMissionCount            = 61, // Path mission count is incorrect 
        ScanCreature                = 62, // Unable to scan this creature - Probably HasScannedCreature
        Unknown63                   = 63, // Requirements not met
        Unknown64                   = 64, // Requirements not met
        Deprecated65                = 65, // Marked as DEPRECATED
        Deprecated66                = 66, // Marked as DEPRECATED
        PathMissionRequirement      = 67, // You do not meet the path mission requirement
        QuestObjective              = 68, // Quest objective requirement not met
        ChallengeRequirement        = 69, // Challenge requirement not met
        ChallengeTier               = 70, // Challenge tier requirement not met
        Unknown71                   = 71, // Requirements not met
        // 72 is unused in PrerequisiteType.tbl
        Vital                       = 73, // Requirements not met - Part of Vital
        ChallengeLocked             = 74, // The specified challenge is locked
        // 75 is unused in PrerequisiteType.tbl
        Unknown76                   = 76, // Requirements not met
        Unknown77                   = 77, // Requirements not met
        MovementMode                = 78, // Invalid movement mode
        UnderForcedMovement         = 79, // Not under forced movement
        HealthRequirement           = 80, // You do not meet the health requirements
        WrongSpellMechanic          = 81, // Wrong spell mechanic
        Unknown82                   = 82, // Requirements not met
        Vehicle                     = 83, // Vehicle conditions not met
        Mount                       = 84, // Mount conditions not met
        Taxi                        = 85, // Taxi conditions not met
        Jump                        = 86, // Jump requirement not met
        Moving                      = 87, // Moving requirement not met
        Pet                         = 88, // Pet requirements not met
        Unknown89                   = 89, // Requirements not met
        Unknown90                   = 90, // Requirements not met
        PathLevel                   = 91, // Incorrect path level
        SpellCondition92            = 92, // Spell conditions not met
        SpellCondition93            = 93, // Spell conditions not met
        QuestObjective6             = 94, // Quest objective requirements not met
        Distance                    = 95, // Distance requirements not met
        Unknown96                   = 96, // Requirements not met
        Unknown97                   = 97, // Requirements not met
        Unknown98                   = 98, // Requirements not met
        Unknown99                   = 99, // Requirements not met
        Unknown100                  = 100, // Requirements not met
        Unknown101                  = 101, // Requirements not met
        SpellEffect102              = 102, // Spell effect requirements not met
        SpellEffect103              = 103, // Spell effect requirements not met
        SpellEffect104              = 104, // Spell effect requirements not met
        SpellEffect105              = 105, // Spell effect requirements not met
        Unknown106                  = 106, // Requirements not met
        QuestObjective7             = 107, // Quest object requirement not met
        /// <summary>
        /// Checks to see if a PositionalRequirement Entry is met.
        /// </summary>
        PositionalRequirement       = 108, // Requirements not met
        WorldRequirement            = 109, // World requirement not met
        PlayerPathMissionCount      = 110, // Player path mission count not correct
        HazardProperty111           = 111, // Hazard property requirement not met
        HazardProperty112           = 112, // Hazard property requirement not met
        HazardProperty113           = 113, // Hazard property requirement not met
        Vital114                    = 114, // Vital requirement not met
        Unit                        = 115, // Unit requirement not met
        Stealth                     = 116, // Stealth property requirement not met
        PublicEvent117              = 117, // Public event requirement not met
        PublicEventObjective188     = 118, // Public event objective requirement not met 
        PublicEventObjectiveObject  = 119, // Public event objective object requirement not met
        PublicEvent120              = 120, // Public event requirement not met
        PublicEvent121              = 121, // Public event requirement not met.
        PublicEventObjective122     = 122, // Public event objective not objective spawn
        Unknown123                  = 123, // Public event objective not objective spawn
        ChallengeCompletionCount    = 124, // Invalid challenge completion count
        Challenge125                = 125, // Challenge requirement not met
        PathHoldout                 = 126, // Path holdout requirement not met
        PathHoldoutPlayer           = 127, // Path holdout requirement for player not met
        Faction128                  = 128, // Faction requirement not met
        SpellObj                    = 129, // Spell requirement not met
        Spell130                    = 130, // Spell requirement not met
        Level131                    = 131, // Level requirement not met
        // 132 is unused in PrerequisiteType.tbl
        Item133                     = 133, // Item requirement not met
        Item134                     = 134, // Item requirement not met
        Item135                     = 135, // Item requirement not met
        Item136                     = 136, // Item requirement not met
        ItemLevel                   = 137, // Item level requirement not met
        ItemSpecial                 = 138, // Item special requirement not met
        ItemMicrochip               = 139, // Item microchip requirement not met
        Item140                     = 140, // Item requirement not met
        IsOutOfBounds               = 141, // You are out of bounds
        Unknown142                  = 142, // Spell requirement not met
        Difficulty                  = 143, // Difficulty requirement not met
        Unknown144                  = 144, // Exist in PrerequisiteType.tbl but does not have a description
        ItemIsSelfCraftedWeapon     = 145, // Self-crafted weapon requirement not met
        ItemTradeSkillLevel         = 146, // Item tradeskill level requirement not met
        PlayersInWorld              = 147, // There are no players in the world who meet the requirement
        InfrastructureState         = 148, // The infrastructure state requirement is not met
        State                       = 149, // State requirement not met
        HubEconomyProgress          = 150, // Hub economy progress requirement not met
        HubQualityOfLife            = 151, // Hub quality of life progress requirement not met
        HubSecurity                 = 152, // Hub security progress requirement not met
        HoldoutWave                 = 153, // The specified holdout wave requirement not met
        Holdout                     = 154, // Soldier holdout requirement not met
        PathMission155              = 155, // Path mission requirement not met
        // 156 is unused in PrerequisiteType.tbl
        // 157 is unused in PrerequisiteType.tbl
        // 158 is unused in PrerequisiteType.tbl
        Unknown159                  = 159, // Requirements not met
        Challenge160                = 160, // Challenge requirement not met
        // 161 is unused in PrerequisiteType.tbl
        PublicEventObjective2       = 162, // Public event objective requirement not met
        PublicEventParticipantCount = 163, // Public event participant count requirement not met
        PublicEventParticipant164   = 164, // Public event participant requirement not met
        // 165 is unused in PrerequisiteType.tbl
        PublicEventParticipant166   = 166, // Public event participant requirement not met
        LiveEvent167                = 167, // Live event requirement not met
        LiveEventCount              = 168, // Live event count requirement not correct
        LiveEvent169                = 169, // Live event requirement not met
        Unknown170                  = 170, // Requirements not met
        Unknown171                  = 171, // Requirements not met
        Unknown172                  = 172, // Requirements not met
        ItemTradeSkill              = 173, // Item tradeskill requirement not met
        Item6                       = 174, // Item requirement not met
        TradeSkill                  = 175, // Tradeskill requirement not met
        ChallengeObject             = 176, // Challenge object requirement not met
        TrueLevel                   = 177, // True level requirement not met
        Item7                       = 178, // Item of type Equipped?
        Unknown179                  = 179, // Requirements not met
        Unknown180                  = 180, // Requirements not met
        // 181 is unused in PrerequisiteType.tbl
        HouseOwnership              = 182, // Housing ownership requirement not met
        Guild                       = 183, // Guild requirement not met
        Guild2                      = 184, // Guild requirement not met
        GuildPerk                   = 185, // Guild perk requirement not met
        Unknown186                  = 186, // Requirements not met
        WarplotPlugUpgrade          = 187, // Warplot plug upgrade tier requirement not met
        WarplotPermission           = 188, // Warplot permission requirement not met
        Unknown189                  = 189, // Exist in PrerequisiteType.tbl but does not have a description
        PetFlair                    = 190, // Pet flair requirement not met
        Spell191                    = 191, // Spell requirement not met
        Unknown192                  = 192, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown193                  = 193, // Requirement not met
        MountUsage                  = 194, // Ground mounts cannot be used in this area
        HoverboardUsage             = 195, // Hoverboard mounts cannot be used in this area
        Racial                      = 196, // Racial requirement not met
        GliderUsage                 = 197, // Gliders cannot be used in this area
        SpaceshipUsage              = 198, // Spaceships cannot be used in this area
        PublicEvent199              = 199, // Public event requirement not met
        PublicEventObjective200     = 200, // Public event objective requirement not met
        AchievementObject           = 201, // Achievement object requirement not met
        // 202 is unused in PrerequisiteType.tbl
        Unknown203                  = 203, // Exist in PrerequisiteType.tbl but does not have a description
        Dueling                     = 204, // Dueling requirement not met
        Personal2V2ArenaRating205   = 205, // Personal arena 2v2 rating requirement not met
        Personal3V3ArenaRating206   = 206, // Personal arena 3v3 rating requirement not met
        Personal5V5ArenaRating207   = 207, // Personal arena 5v5 rating requirement not met
        Battlegrounds208            = 208, // Battlegrounds requirement not met 
        Warplots209                 = 209, // Warplots requirement not met
        Personal2V2ArenaRating210   = 210, // Personal arena 2v2 rating requirement not met
        Personal3V3ArenaRating211   = 211, // Personal arena 3v3 rating requirement not met
        Personal5V5ArenaRating212   = 212, // Personal arena 5v5 rating requirement not met
        PersonalWarplotRating       = 213, // Personal warplots rating requirement not met
        SpellBaseId                 = 214, // Spell requirement not met
        Shield215                   = 215, // Shield requirement not met
        Shield216                   = 216, // Shield requirement not met
        PvpFlag                     = 217, // Target is not flagged for PvP.
        Datacube                    = 218, // Datacube requirement not met
        Volume                      = 219, // Volume requirement not met
        Unknown220                  = 220, // Exist in PrerequisiteType.tbl but does not have a description
        Spell221                    = 221, // Spell requirement not met, Could be SpellBase
        Unknown222                  = 222, // Requirements not met
        Unknown223                  = 223, // Requirements not met
        LevelGrantAbilityTierPoints = 224, // Level does not grant any ability tier points
        LevelGrantAmpPoints         = 225, // Level does not grant any AMP points
        Entitlement                 = 226, // Entitlement requirement not met
        EldanAugmentation227        = 227, // Eldan augmentation requirement not met
        TradeSkill228               = 228, // Tradeskill requirement not met
        TradeSkill229               = 229, // Tradeskill requirement not met
        WarplotRating               = 230, // Warplot rating requirement not met
        EldanAugmentation231        = 231, // Eldan augmentation requirement not met
        Plane                       = 232, // Plane requirement not met
        Spell233                    = 233, // Spell requirement not met
        WarplotUpgrade              = 234, // Warplot upgrade requirement not met
        BonusAbilityTierPoints      = 235, // Bonus ability tier point requirement not met
        AbilityTierPoints           = 236, // Ability tier points requirement not met
        BonusPower                  = 237, // Bonus power requirement not met
        Power                       = 238, // Power requirement not met
        ActionBar                   = 239, // Action bar does not meet requirements 
        Unknown240                  = 240, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown241                  = 241, // Exist in PrerequisiteType.tbl but does not have a description
        LiveEvent242                = 242, // Live event not complete
        Faction243                  = 243, // Faction requirement not met
        Unknown244                  = 244, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown245                  = 245, // Exist in PrerequisiteType.tbl but does not have a description
        Inventory                   = 246, // Inventory requirement not met - AccountItemClaimed?
        // 247 is unused in PrerequisiteType.tbl
        Unknown248                  = 248, // Exist in PrerequisiteType.tbl but does not have a description
        // 249 is unused in PrerequisiteType.tbl
        BaseFaction                 = 250, // Base faction requirement not met
        Personal2V2ArenaRating251   = 251, // Personal arena 2v2 rating requirement not met
        Personal3V3ArenaRating252   = 252, // Personal arena 3v3 rating requirement not met
        Personal5V5ArenaRating253   = 253, // Personal arena 5v5 rating requirement not met
        Battlegrounds254            = 254, // Battlegrounds requirement not met
        Warplots255                 = 255, // Warplots requirement not met
        WarplotRating256            = 256, // Warplot rating requirement not met
        // 257 is unused in PrerequisiteType.tbl
        // 258 is unused in PrerequisiteType.tbl
        Unknown259                  = 259, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown260                  = 260, // Requirements not met
        Personal2V2ArenaRating261   = 261, // Personal arena 2v2 rating requirement not met
        Personal3V3ArenaRating262   = 262, // Personal arena 3v3 rating requirement not met
        Personal5V5ArenaRating263   = 263, // Personal arena 5v5 rating requirement not met
        Battlegrounds264            = 264, // Battlegrounds requirement not met
        Warplots265                 = 265, // Warplots requirement not met
        Unknown266                  = 266, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown267                  = 267, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown268                  = 268, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown269                  = 269, // Exist in PrerequisiteType.tbl but does not have a description
        LoyaltyRewards              = 270, // Loyalty requirement not met
        Unknown271                  = 271, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown272                  = 272, // Exist in PrerequisiteType.tbl but does not have a description
        EntitlementCount            = 273, // Entitlement count requirement not met
        // 274 is unused in PrerequisiteType.tbl
        Unknown275                  = 275, // Exist in PrerequisiteType.tbl but does not have a description
        OutOfBounds                 = 276, // You're out of bounds
        Unknown277                  = 277, // You cannot do that right now
        Unknown278                  = 278, // Requirements not met
        Spell279                    = 279, // Spell requirement not met
        Spell280                    = 280, // Spell requirement not met
        Falling                     = 281, // Falling requirement not met
        // 282 is unused in PrerequisiteType.tbl
        Unknown283                  = 283, // Marked as N/A
        // 284 is unused in PrerequisiteType.tbl
        Unknown285                  = 285, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown286                  = 286, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown287                  = 287, // Exist in PrerequisiteType.tbl but does not have a description
        PurchasedTitle              = 288, // Character title requirement not met
        Unknown289                  = 289, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown290                  = 290, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown291                  = 291, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown292                  = 292, // Exist in PrerequisiteType.tbl but does not have a description - Primal Matrix ?
        Unknown293                  = 293, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown294                  = 294, // Exist in PrerequisiteType.tbl but does not have a description
        Unknown295                  = 295  // Requirements not met           
    }
}
