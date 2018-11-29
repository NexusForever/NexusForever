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

        public static GameTable<CharacterCreationEntry> CharacterCreation { get; private set; }
        public static GameTable<CharacterCustomizationEntry> CharacterCustomization { get; private set; }
        public static GameTable<Creature2Entry> Creature2 { get; private set; }
        public static GameTable<Creature2ArcheTypeEntry> Creature2ArcheType { get; private set; }
        public static GameTable<Creature2DifficultyEntry> Creature2Difficulty { get; private set; }
        public static GameTable<Creature2TierEntry> Creature2Tier { get; private set; }
        public static GameTable<CreatureLevelEntry> CreatureLevel { get; private set; }
        public static GameTable<Item2Entry> Item { get; private set; }
        public static GameTable<Item2TypeEntry> ItemType { get; private set; }
        public static GameTable<ItemDisplaySourceEntryEntry> ItemDisplaySource { get; private set; }
        public static GameTable<WorldEntry> World { get; private set; }
        public static GameTable<EmotesEntry> Emotes { get; private set; }

        public static void Initialise()
        {
            log.Info("Loading GameTables...");
            var sw = Stopwatch.StartNew();

            try
            {
                CharacterCreation      = new GameTable<CharacterCreationEntry>("tbl" + Path.DirectorySeparatorChar + "CharacterCreation.tbl");
                CharacterCustomization = new GameTable<CharacterCustomizationEntry>("tbl" + Path.DirectorySeparatorChar + "CharacterCustomization.tbl");
                Creature2              = new GameTable<Creature2Entry>("tbl" + Path.DirectorySeparatorChar + "Creature2.tbl");
                Creature2ArcheType     = new GameTable<Creature2ArcheTypeEntry>("tbl" + Path.DirectorySeparatorChar + "Creature2ArcheType.tbl");
                Creature2Difficulty    = new GameTable<Creature2DifficultyEntry>("tbl" + Path.DirectorySeparatorChar + "Creature2Difficulty.tbl");
                Creature2Tier          = new GameTable<Creature2TierEntry>("tbl" + Path.DirectorySeparatorChar + "Creature2Tier.tbl");
                CreatureLevel          = new GameTable<CreatureLevelEntry>("tbl" + Path.DirectorySeparatorChar + "CreatureLevel.tbl");
                Item                   = new GameTable<Item2Entry>("tbl" + Path.DirectorySeparatorChar + "Item2.tbl");
                ItemType               = new GameTable<Item2TypeEntry>("tbl" + Path.DirectorySeparatorChar + "Item2Type.tbl");
                ItemDisplaySource      = new GameTable<ItemDisplaySourceEntryEntry>("tbl" + Path.DirectorySeparatorChar + "ItemDisplaySourceEntry.tbl");
                World                  = new GameTable<WorldEntry>("tbl" + Path.DirectorySeparatorChar + "World.tbl");
                Emotes                 = new GameTable<EmotesEntry>("tbl" + Path.DirectorySeparatorChar + "Emotes.tbl");
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
