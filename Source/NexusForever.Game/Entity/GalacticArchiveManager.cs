using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.GalacticArchive;
using NexusForever.Game.Static.GalacticArchive;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;

namespace NexusForever.Game.Entity
{
    public class GalacticArchiveManager : IGalacticArchiveManager
    {
        private IPlayer player;
        private readonly Dictionary<ushort, IArchiveArticle> articles = [];

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;
        private readonly IGlobalGalacticArchiveManager globalArchiveManager;

        public GalacticArchiveManager(
            IGameTableManager gameTableManager,
            IGlobalGalacticArchiveManager globalArchiveManager)
        {
            this.gameTableManager     = gameTableManager;
            this.globalArchiveManager = globalArchiveManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IGalacticArchiveManager"/> from <see cref="CharacterModel"/> database model.
        /// </summary>
        public void Initialise(IPlayer owner, CharacterModel model)
        {
            if (player != null)
                throw new InvalidOperationException("GalacticArchiveManager has already been initialised!");

            player = owner;

            foreach (CharacterArchiveArticleModel articleModel in model.ArchiveArticle)
                articles.Add(articleModel.ArticleId, new ArchiveArticle(player, articleModel));
        }

        /// <summary>
        /// Save any changes to the articles to the <see cref="CharacterContext"/>.
        /// </summary>
        public void Save(CharacterContext context)
        {
            foreach (IArchiveArticle article in articles.Values)
                article.Save(context);
        }

        /// <summary>
        /// Return <see cref="IArchiveArticle"/> with supplied id.
        /// </summary>
        public IArchiveArticle GetArticle(ushort id)
        {
            return articles.GetValueOrDefault(id);
        }

        /// <summary>
        /// Unlock article with supplied id at the request of the client.
        /// </summary>
        public void UnlockArticle(ushort id)
        {
            ArchiveArticleEntry entry = gameTableManager.ArchiveArticle.GetEntry(id);
            if (entry is null)
                throw new InvalidPacketValueException();

            if (((ArchiveArticleFlags)entry.ArchiveArticleFlags).HasFlag(ArchiveArticleFlags.NoClientUnlock))
                return;

            if (GetArticle(id) is { IsUnlocked: true })
                return;

            // client applies the unlock before it sends ClientGalacticArchiveUnlock, it doesn't need to be told
            Unlock(id, 0u, sendUpdate: false);
        }

        /// <summary>
        /// Mark article with supplied id and every unlocked entry in it as viewed at the request of the client.
        /// </summary>
        public void ViewArticle(ushort id)
        {
            if (GetArticle(id) is not { IsUnlocked: true } article)
                return;

            // client applies the view before it sends ClientGalacticArchiveViewed, it doesn't need to be told
            article.View();
        }

        /// <summary>
        /// Unlock every entry with an unlock rule of <see cref="ArchiveEntryUnlockRuleType"/> for the supplied object id.
        /// </summary>
        /// <remarks>
        /// Must only be invoked once the object is complete, for a quest this is when the quest is handed in.
        /// </remarks>
        public void UpdateUnlockRule(ArchiveEntryUnlockRuleType type, uint objectId)
        {
            foreach ((ushort articleId, uint entryMask) in globalArchiveManager.GetUnlockedEntries(type, objectId))
                Unlock(articleId, entryMask, sendUpdate: true);
        }

        /// <summary>
        /// Send every article to the owner <see cref="IPlayer"/> after entering a world.
        /// </summary>
        public void SendInitialPackets()
        {
            // client discards all articles when it changes world, each must be sent again
            foreach (IArchiveArticle article in articles.Values)
                player.Session.EnqueueMessageEncrypted(article.Build());
        }

        private void Unlock(ushort id, uint entryMask, bool sendUpdate)
        {
            if (!articles.TryGetValue(id, out IArchiveArticle article))
            {
                article = new ArchiveArticle(player, id);
                articles.Add(id, article);
            }

            uint articleEntryMask = globalArchiveManager.GetEntryMask(id);
            bool wasComplete      = article.IsUnlocked && article.HasEntries(articleEntryMask);

            if (!article.IsUnlocked)
                entryMask |= globalArchiveManager.GetInitialEntryMask(id);

            if (!article.Unlock(entryMask))
                return;

            if (sendUpdate)
                player.Session.EnqueueMessageEncrypted(article.Build());

            if (!wasComplete && article.HasEntries(articleEntryMask))
            {
                ArchiveArticleEntry entry = gameTableManager.ArchiveArticle.GetEntry(id);
                if (entry.CharacterTitleIdReward != 0u)
                    player.TitleManager.AddTitle((ushort)entry.CharacterTitleIdReward);
            }
        }
    }
}
