using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.GalacticArchive;
using NexusForever.Network.World.Message.Model.GalacticArchive;

namespace NexusForever.Game.Entity
{
    public class ArchiveArticle : IArchiveArticle
    {
        [Flags]
        public enum ArchiveArticleSaveMask
        {
            None          = 0x0000,
            Create        = 0x0001,
            UnlockedFlags = 0x0002,
            ViewedFlags   = 0x0004
        }

        public ushort Id { get; }

        public uint UnlockedFlags
        {
            get;
            private set
            {
                field     = value;
                saveMask |= ArchiveArticleSaveMask.UnlockedFlags;
            }
        }

        public uint ViewedFlags
        {
            get;
            private set
            {
                field     = value;
                saveMask |= ArchiveArticleSaveMask.ViewedFlags;
            }
        }

        /// <summary>
        /// Returns if the article itself is unlocked.
        /// </summary>
        public bool IsUnlocked => (UnlockedFlags & (uint)ArchiveArticleUnlockFlags.Unlocked) != 0u;

        private ArchiveArticleSaveMask saveMask;

        private readonly IPlayer player;

        /// <summary>
        /// Create a new <see cref="IArchiveArticle"/> from the supplied article id.
        /// </summary>
        public ArchiveArticle(IPlayer player, ushort id)
        {
            this.player = player;

            Id       = id;

            saveMask = ArchiveArticleSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="IArchiveArticle"/> from an existing database model.
        /// </summary>
        public ArchiveArticle(IPlayer player, CharacterArchiveArticleModel model)
        {
            this.player = player;

            Id            = model.ArticleId;
            UnlockedFlags = model.UnlockedFlags;
            ViewedFlags   = model.ViewedFlags;

            // assigning the flags marks them to be saved, an article loaded from the database has nothing to save
            saveMask      = ArchiveArticleSaveMask.None;
        }

        /// <summary>
        /// Save any changes to the article to the <see cref="CharacterContext"/>.
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (saveMask == ArchiveArticleSaveMask.None)
                return;

            if (saveMask.HasFlag(ArchiveArticleSaveMask.Create))
            {
                // article doesn't exist in database, all information must be saved
                context.Add(new CharacterArchiveArticleModel
                {
                    Id            = player.CharacterId,
                    ArticleId     = Id,
                    UnlockedFlags = UnlockedFlags,
                    ViewedFlags   = ViewedFlags
                });
            }
            else
            {
                // article already exists in database, save only data that has been modified
                var model = new CharacterArchiveArticleModel
                {
                    Id        = player.CharacterId,
                    ArticleId = Id
                };

                EntityEntry<CharacterArchiveArticleModel> entity = context.Attach(model);
                if (saveMask.HasFlag(ArchiveArticleSaveMask.UnlockedFlags))
                {
                    model.UnlockedFlags = UnlockedFlags;
                    entity.Property(p => p.UnlockedFlags).IsModified = true;
                }

                if (saveMask.HasFlag(ArchiveArticleSaveMask.ViewedFlags))
                {
                    model.ViewedFlags = ViewedFlags;
                    entity.Property(p => p.ViewedFlags).IsModified = true;
                }
            }

            saveMask = ArchiveArticleSaveMask.None;
        }

        /// <summary>
        /// Returns if every entry in the supplied slot mask is unlocked.
        /// </summary>
        public bool HasEntries(uint entryMask)
        {
            return (UnlockedFlags & entryMask) == entryMask;
        }

        /// <summary>
        /// Unlock the article and every entry in the supplied slot mask.
        /// </summary>
        /// <remarks>
        /// Returns false if the article and every supplied entry were already unlocked.
        /// </remarks>
        public bool Unlock(uint entryMask)
        {
            uint flags = UnlockedFlags
                | (uint)ArchiveArticleUnlockFlags.Unlocked
                | (entryMask & (uint)ArchiveArticleUnlockFlags.Entries);
            if (flags == UnlockedFlags)
                return false;

            UnlockedFlags = flags;
            return true;
        }

        /// <summary>
        /// Mark the article and every unlocked entry as viewed.
        /// </summary>
        public void View()
        {
            // must match what the client applies before it sends ClientGalacticArchiveViewed, otherwise the client will send it again
            uint viewed = ViewedFlags | UnlockedFlags | (uint)ArchiveArticleUnlockFlags.Viewed;
            if (viewed != ViewedFlags)
                ViewedFlags = viewed;

            uint unlocked = UnlockedFlags | (uint)ArchiveArticleUnlockFlags.Viewed;
            if (unlocked != UnlockedFlags)
                UnlockedFlags = unlocked;
        }

        /// <summary>
        /// Build a <see cref="ServerGalacticArchiveUpdate"/> holding the current state of the article.
        /// </summary>
        public ServerGalacticArchiveUpdate Build()
        {
            return new ServerGalacticArchiveUpdate
            {
                ArchiveArticleId = Id,
                UnlockedFlags    = UnlockedFlags,
                ViewedFlags      = ViewedFlags
            };
        }
    }
}
