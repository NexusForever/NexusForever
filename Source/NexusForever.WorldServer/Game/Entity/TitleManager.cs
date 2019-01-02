using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class TitleManager : IUpdate, ISaveCharacter, IEnumerable<Title>
    {
        public ushort ActiveTitleId
        {
            get => activeTitleId;
            set
            {
                if (value != 0 && (!titles.ContainsKey(value) || activeTitleId == value))
                    return;

                activeTitleId = value;
                activeSaved   = false;

                player.EnqueueToVisible(new ServerTitleSet
                {
                    Guid  = player.Guid,
                    Title = ActiveTitleId
                }, true);
            }
        }

        private ushort activeTitleId;
        private bool activeSaved = true;

        private readonly Player player;
        private readonly Dictionary<ushort, Title> titles = new Dictionary<ushort, Title>();
        private readonly Dictionary<ushort, Title> removedTitles = new Dictionary<ushort, Title>();

        /// <summary>
        /// Create a new <see cref="TitleManager"/> from existing <see cref="Character"/> database model.
        /// </summary>
        public TitleManager(Player owner, Character model)
        {
            player = owner;
            activeTitleId = model.Title;

            foreach (CharacterTitle titleModel in model.CharacterTitle)
                titles.Add(titleModel.Title, new Title(titleModel));

            EnsureActiveTitleIsOwned();
        }

        public void Update(double lastTick)
        {
            foreach (Title title in titles.Values.ToArray())
            {
                title.Update(lastTick);
                if (title.TimeRemaining != null && title.TimeRemaining <= 0d)
                {
                    // title has expired
                    RemoveTitle((ushort)title.Entry.Id);
                }
            }
        }

        public void Save(CharacterContext context)
        {
            if (!activeSaved)
            {
                var model = new Character
                {
                    Id    = player.CharacterId,
                    Title = activeTitleId
                };

                EntityEntry<Character> entity = context.Attach(model);
                entity.Property(p => p.Title).IsModified = true;

                activeSaved = true;
            }

            // must delete before adding new titles
            foreach (Title title in removedTitles.Values)
                title.Delete(context);

            foreach (Title title in titles.Values)
                title.Save(context);
        }

        /// <summary>
        /// Add new <see cref="Title"/> with supplied title id, if suppress is true <see cref="ServerTitleUpdate"/> won't be sent.
        /// </summary>
        public void AddTitle(ushort titleId, bool suppress = false)
        {
            CharacterTitleEntry entry = GameTableManager.CharacterTitle.GetEntry(titleId);
            if (entry == null)
                throw new InvalidPacketValueException();

            if (titles.ContainsKey(titleId))
            {
                player.Session.EnqueueMessageEncrypted(new ServerTitleUpdate
                {
                    TitleId      = titleId,
                    Alreadyowned = true
                });

                return;
            }

            if (removedTitles.Remove(titleId, out Title title))
                titles.Add(titleId, title);
            else
                titles.Add(titleId, new Title(player.CharacterId, entry));

            if (!suppress)
            {
                player.Session.EnqueueMessageEncrypted(new ServerTitleUpdate
                {
                    TitleId = titleId
                });
            }
        }

        /// <summary>
        /// Remove <see cref="Title"/> with supplied title id, if suppress is true <see cref="ServerTitleUpdate"/> won't be sent.
        /// </summary>
        public void RemoveTitle(ushort titleId, bool suppress = false)
        {
            if (GameTableManager.CharacterTitle.GetEntry(titleId) == null)
                throw new InvalidPacketValueException();

            if (!titles.Remove(titleId, out Title title))
                return;

            if (title.SavedToDatabase)
                removedTitles.Add((ushort)title.Entry.Id, title);

            if (!suppress)
            {
                player.Session.EnqueueMessageEncrypted(new ServerTitleUpdate
                {
                    TitleId = (ushort)title.Entry.Id,
                    Revoked = true
                });
            }

            EnsureActiveTitleIsOwned();
        }

        /// <summary>
        /// Send <see cref="ServerTitles"/> to owner <see cref="Player"/>.
        /// </summary>
        public void SendTitles()
        {
            player.Session.EnqueueMessageEncrypted(new ServerTitles
            {
                Titles = titles.Values.Select(t => new ServerTitles.Title
                {
                    TitleId       = (ushort)t.Entry.Id,
                    TimeRemaining = (uint)(t.TimeRemaining ?? 0d)
                }).ToList()
            });
        }

        /// <summary>
        /// This is only used debug/command purposes.
        /// </summary>
        public void AddAllTitles()
        {
            ushort[] titleIds = GameTableManager.CharacterTitle.Entries
                .Select(entry => (ushort)entry.Id)
                .ToArray();

            foreach (ushort titleId in titleIds)
                AddTitle(titleId, true);

            // client crashes if too many single title updates are sent, send as bulk
            SendTitles();
        }

        /// <summary>
        /// This is only used debug/command purposes.
        /// </summary>
        public void RemoveAllTitles()
        {
            ushort[] titleIds = titles.Values
                .Select(entry => (ushort)entry.Entry.Id)
                .ToArray();

            foreach (ushort titleId in titleIds)
                RemoveTitle(titleId, true);

            player.Session.EnqueueMessageEncrypted(new ServerTitles
            {
                Titles = titleIds.Select(t => new ServerTitles.Title
                {
                    TitleId = t,
                    Revoked = true
                }).ToList()
            });
        }

        private void EnsureActiveTitleIsOwned()
        {
            if (activeTitleId != 0 && !titles.ContainsKey(activeTitleId))
                ActiveTitleId = 0;
        }

        public IEnumerator<Title> GetEnumerator()
        {
            return titles.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
