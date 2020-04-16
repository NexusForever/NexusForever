namespace NexusForever.WorldServer.Game.Quest.Static
{
    // TODO: name more of these
    public enum QuestObjectiveType
    {
        KillCreature                 = 2,  // data = CreatureId, ObjectiveText describes killing creature (Count: 598) (e.g. http://wildstar.mmorpg-life.com/quests/the-mother-of-all-spiders/)
        TalkTo                       = 3,  // data = CreatureId, ObjeciveText describes interacting with creature (Count: 350) (e.g. http://wildstar.mmorpg-life.com/quests/search-party/)
        CollectItem                  = 4,  // data = Item2Id, ObjectiveText describes collecting items (Count: 327)
        ActivateEntity               = 5,  // data = CreatureId, ObjectiveText describes interacting with creature (Count: 1556)
        KillTargetGroups             = 8,  // data = TargetGroupId, ObjectiveText describes killing creatures of a type, TargetGroups contain other TargetGroup IDs that contain creature IDs (Count: 498)
        ActivateEntity2              = 9,  // data = CreatureId, ObjectiveText describes interacting with Creature (Count: 18)
        Unknown10                    = 10, // data = unknown, ObjectiveTexts are pretty varied: killing, completing challenges or dungeons on certain difficult - maybe scripted updates (Count: 52)
        SpellSuccess                 = 11, // data = Spell4Id, ObjectiveText describes casting spell that interacts with creature or location (Count: 128)
        SucceedCSI                   = 12, // data = CreatureId, ObjectiveText describes interacting with object after succeeding a ClientSideInteraction event (Count: 1187)
        SpellSuccess2                = 13, // data = Spell4Id, ObjectiveText describes casting spell (Count: 3)
        ActivateTargetGroupChecklist = 14, // data = TargetGroupId, ObjectiveText describes using a creature, Creature casts spell which has an Activate effect, Creature IDs are part of TargetGroup entry (Effect ID 7) (Count: 426)
        Unknown15                    = 15, // data = unknown, ObjectiveText describes killing a single or number of named creatures (Count: 26)
        KillTargetGroup              = 16, // data = TargetGroupId, ObjectiveText describes killing creatures, Creature IDs are part of TargetGroup entry (Count: 55)
        EnterZone                    = 17, // data = ZoneId, ObjectiveText describes entering an area (Count: 199)
        TalkToTargetGroup            = 18, // data = TargetGroupId, ObjectiveText describes talking with creatures, Creature IDs are part of TargetGroup entry (Count: 35)
        UNUSED19                     = 19, // data = 0 on all entries, ObjectiveText describes completing a checklist but looks to have been only used in testing (Count: 2)
        Unknown20                    = 20, // data = unknown, ObjectiveText describes completing a cooking recipe (Count: 11)
        GatheResource                = 21, // data = CreatureId, ObjectiveText describes harvesting/destroying an object (Count: 10)
        EnterArea                    = 22, // data = unknown, ObjectiveText describes finding creatures or areas in locations (Count: 511)
        ActivateTargetGroup          = 23, // data = TargetGroupId, ObjectiveText describes interacting with objects (Count: 96)
        CompleteQuest                = 24, // data = QuestId, ObjectiveText describes either completing quests by name, or completing objectives of other quests (Count: 39)
        CompleteEvent                = 25, // data = unknown, ObjectiveText describes completing a mini event (killing a bunch of zombies like in http://wildstar.mmorpg-life.com/quests/here-they-come-exile/) (Count: 152)
        Unknown27                    = 27, // data = unknown, ObjectiveText not in localisation files (Count: 1)
        Unknown28                    = 28, // data = unknown, ObjectiveText describes rescuing creatures (Count: 5)
        Unknown29                    = 29, // data = unknown, ObjectiveText not in localisation files (Count: 2)
        Unknown31                    = 31, // data = unknown, ObjectiveText are all empty (Count: 260)
        VirtualCollect               = 32, // data = VirtualItemId, ObjectiveText describes collecting items and lots of $v(vitem=xxx) macros (Count: 709)
        CraftSchematic               = 33, // data = TradeskillSchematicId, ObjectiveText describes crafting items (Count: 215)
        SpellSuccess3                = 35, // data = Spell4Id, ObjectiveText describes using an item/spell to destroy something (Count: 10)
        SpellSuccess4                = 36, // data = Spell4Id, ObjectiveText describes using an item/spell on something (Count: 6)
        LearnTradeskill              = 37, // data = 0 on all entries, ObjectiveText describes learning tradeskills from creature (Count: 4)
        ObtainSchematic              = 38, // data = TradeskillSchematicId, ObjectiveText describes obtaining schematic result (Count: 2520)
        Unknown39                    = 39, // data = 0 on all entries, ObjectiveText describes completing quests (Count: 2)
        Unknown40                    = 40, // data = unknown, ObjectiveText describes participating in group content (Count: 2)
        PvPKills                     = 41, // data = unknown, ObjectiveText describes killing players in PvP content (Count: 2)
        EarnCurrency                 = 42, // data = CurrencyId, ObjectiveText describes collecting an amount of certain currency (Count: 2)
        CombatMomentum               = 44, // data = CombatMomentumId, ObjectiveText describes completing an amount of combat momentum actions (Count: 7)
        KillCreature2                = 46, // data = CreatureDifficulty (maybe), ObjectiveText describes killing an amount of creatures of certain difficulty or higher (Count: 1)
        CompleteChallenge            = 47, // data = 0 on all entries, ObjectiveText describes completing challenges anywhere in Nexus (Count: 2),
        BeginMatrix                  = 48, // data = 0 on all entries, ObjectiveText describes starting to use the Primal Matrix (Count: 2)
    }

    // Observations: When data = 0, it appears to be completed by script or use the targetGroupIdRewardPane Value which is a TargetGroup. Touring Tremor Ridge (Quest ID: 9164) has 2 objectives which have data values of 0 - Interacting with Taxi Stand, and Interacting with Mailbox.
}
