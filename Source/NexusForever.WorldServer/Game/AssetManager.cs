using System.Collections.Generic;
using System.Collections.Immutable;
using NexusForever.Shared.Database.Character;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game
{
    public static class AssetManager
    {
        /// <summary>
        /// Id to be assigned to the next created character.
        /// </summary>
        public static ulong NextCharacterId { get; set; }

        private static ImmutableDictionary<uint, ImmutableList<CharacterCustomizationEntry>> characterCustomisations;

        public static void Initialise()
        {
            NextCharacterId = CharacterDatabase.GetNextCharacterId() + 1ul;

            CacheCharacterCustomisations();
        }

        private static void CacheCharacterCustomisations()
        {
            var entries = new Dictionary<uint, List<CharacterCustomizationEntry>>();
            foreach (CharacterCustomizationEntry entry in GameTableManager.CharacterCustomization.Entries)
            {
                uint primaryKey = (entry.Value00 << 24) | (entry.CharacterCustomizationLabelId00 << 16) | (entry.Gender << 8) | entry.RaceId;
                if (!entries.ContainsKey(primaryKey))
                    entries.Add(primaryKey, new List<CharacterCustomizationEntry>());

                entries[primaryKey].Add(entry);
            }

            characterCustomisations = entries.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        public static ImmutableList<CharacterCustomizationEntry> GetPrimaryCharacterCustomisation(uint race, uint sex, uint label, uint value)
        {
            uint key = (value << 24) | (label << 16) | (sex << 8) | race;
            return characterCustomisations.TryGetValue(key, out ImmutableList<CharacterCustomizationEntry> entries) ? entries : null;
        }
    }
}
