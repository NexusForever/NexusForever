using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using NexusForever.Game.Abstract.GalacticArchive;
using NexusForever.Game.Static.GalacticArchive;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.GalacticArchive
{
    public sealed class GlobalGalacticArchiveManager : Singleton<GlobalGalacticArchiveManager>, IGlobalGalacticArchiveManager
    {
        /// <summary>
        /// The slot an entry is in for a single article.
        /// </summary>
        private readonly record struct ArticleSlot(ushort ArticleId, int Slot);

        // rule is only satisfied once every object is complete, no rule in the game table sets it
        private const uint UnlockRuleFlagAllObjects = 0x01;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private FrozenDictionary<ushort, uint> entryMasks;
        private FrozenDictionary<ushort, uint> initialEntryMasks;
        private FrozenDictionary<(ArchiveEntryUnlockRuleType Type, uint ObjectId), ImmutableArray<(ushort ArticleId, uint EntryMask)>> unlockRules;

        /// <summary>
        /// Initialise <see cref="IGlobalGalacticArchiveManager"/> and build the unlock rule index from the game tables.
        /// </summary>
        public void Initialise()
        {
            long start = Stopwatch.GetTimestamp();

            Dictionary<uint, List<ArticleSlot>> entrySlots = InitialiseArticles();
            int ruleCount = InitialiseUnlockRules(entrySlots);

            TimeSpan span = Stopwatch.GetElapsedTime(start);
            log.Info($"Initialised {entryMasks.Count} archive articles and {ruleCount} archive entry unlock rules in {span.TotalMilliseconds}ms.");
        }

        /// <summary>
        /// Build the slot masks for every article, returning the slots each entry is in.
        /// </summary>
        /// <remarks>
        /// An entry can be in a slot of more than one article.
        /// </remarks>
        private Dictionary<uint, List<ArticleSlot>> InitialiseArticles()
        {
            var masks        = new Dictionary<ushort, uint>();
            var initialMasks = new Dictionary<ushort, uint>();
            var entrySlots   = new Dictionary<uint, List<ArticleSlot>>();

            foreach (ArchiveArticleEntry article in GameTableManager.Instance.ArchiveArticle.Entries)
            {
                ushort articleId      = (ushort)article.Id;
                uint entryMask        = 0u;
                uint initialEntryMask = 0u;

                uint[] entryIds = GetEntryIds(article);
                for (int slot = 0; slot < entryIds.Length; slot++)
                {
                    ArchiveEntryEntry entry = GameTableManager.Instance.ArchiveEntry.GetEntry(entryIds[slot]);
                    if (entry is null)
                        continue;

                    entryMask |= 1u << slot;
                    if (((ArchiveEntryFlags)entry.ArchiveEntryFlags).HasFlag(ArchiveEntryFlags.UnlockWithArticle))
                        initialEntryMask |= 1u << slot;

                    if (!entrySlots.TryGetValue(entry.Id, out List<ArticleSlot> slots))
                    {
                        slots = [];
                        entrySlots.Add(entry.Id, slots);
                    }

                    slots.Add(new ArticleSlot(articleId, slot));
                }

                masks.Add(articleId, entryMask);
                initialMasks.Add(articleId, initialEntryMask);
            }

            entryMasks        = masks.ToFrozenDictionary();
            initialEntryMasks = initialMasks.ToFrozenDictionary();

            return entrySlots;
        }

        /// <summary>
        /// Build the slot masks unlocked by every object with an unlock rule, returning the number of rules used.
        /// </summary>
        private int InitialiseUnlockRules(Dictionary<uint, List<ArticleSlot>> entrySlots)
        {
            var rules = new Dictionary<(ArchiveEntryUnlockRuleType Type, uint ObjectId), Dictionary<ushort, uint>>();

            int ruleCount     = 0;
            int unplacedCount = 0;
            foreach (ArchiveEntryUnlockRuleEntry rule in GameTableManager.Instance.ArchiveEntryUnlockRule.Entries)
            {
                if (!entrySlots.TryGetValue(rule.ArchiveEntryId, out List<ArticleSlot> slots))
                {
                    unplacedCount++;
                    continue;
                }

                if ((rule.ArchiveEntryUnlockRuleFlags & UnlockRuleFlagAllObjects) != 0u)
                {
                    log.Warn($"Archive entry unlock rule {rule.Id} requires all objects to be complete which is not supported, skipping.");
                    continue;
                }

                var type = (ArchiveEntryUnlockRuleType)rule.ArchiveEntryUnlockRuleEnum;
                foreach (uint objectId in GetObjectIds(rule))
                {
                    if (objectId == 0u)
                        continue;

                    if (!rules.TryGetValue((type, objectId), out Dictionary<ushort, uint> articleMasks))
                    {
                        articleMasks = [];
                        rules.Add((type, objectId), articleMasks);
                    }

                    foreach (ArticleSlot slot in slots)
                        articleMasks[slot.ArticleId] = articleMasks.GetValueOrDefault(slot.ArticleId) | 1u << slot.Slot;
                }

                ruleCount++;
            }

            unlockRules = rules.ToFrozenDictionary(
                r => r.Key,
                r => r.Value
                    .Select(a => (ArticleId: a.Key, EntryMask: a.Value))
                    .ToImmutableArray());

            if (unplacedCount != 0)
                log.Debug($"Skipped {unplacedCount} archive entry unlock rules for entries that aren't in any article.");

            return ruleCount;
        }

        private static uint[] GetEntryIds(ArchiveArticleEntry article)
        {
            return
            [
                article.ArchiveEntryId00,
                article.ArchiveEntryId01,
                article.ArchiveEntryId02,
                article.ArchiveEntryId03,
                article.ArchiveEntryId04,
                article.ArchiveEntryId05,
                article.ArchiveEntryId06,
                article.ArchiveEntryId07,
                article.ArchiveEntryId08,
                article.ArchiveEntryId09,
                article.ArchiveEntryId10,
                article.ArchiveEntryId11,
                article.ArchiveEntryId12,
                article.ArchiveEntryId13,
                article.ArchiveEntryId14,
                article.ArchiveEntryId15
            ];
        }

        private static uint[] GetObjectIds(ArchiveEntryUnlockRuleEntry rule)
        {
            return
            [
                rule.Object00,
                rule.Object01,
                rule.Object02,
                rule.Object03,
                rule.Object04,
                rule.Object05
            ];
        }

        /// <summary>
        /// Return a mask of the slots that contain an entry for the article with supplied id.
        /// </summary>
        public uint GetEntryMask(ushort articleId)
        {
            return entryMasks.GetValueOrDefault(articleId);
        }

        /// <summary>
        /// Return a mask of the slots that contain an entry that is unlocked together with the article with supplied id.
        /// </summary>
        public uint GetInitialEntryMask(ushort articleId)
        {
            return initialEntryMasks.GetValueOrDefault(articleId);
        }

        /// <summary>
        /// Return each article and mask of slots that contain an entry with an unlock rule of <see cref="ArchiveEntryUnlockRuleType"/> for the supplied object id.
        /// </summary>
        public ImmutableArray<(ushort ArticleId, uint EntryMask)> GetUnlockedEntries(ArchiveEntryUnlockRuleType type, uint objectId)
        {
            return unlockRules.TryGetValue((type, objectId), out ImmutableArray<(ushort ArticleId, uint EntryMask)> articles) ? articles : [];
        }
    }
}
