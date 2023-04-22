using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class TitleManager : ITitleManager
    {
        public ushort ActiveTitleId
        {
            get => activeTitleId;
            set
            {
                if (value != 0 && (!titles.TryGetValue(value, out ITitle title) || title.Revoked || activeTitleId == value))
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

        private readonly IPlayer player;
        private readonly Dictionary<ushort, ITitle> titles = new();

        /// <summary>
        /// Create a new <see cref="ITitleManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public TitleManager(IPlayer owner, CharacterModel model)
        {
            player = owner;
            activeTitleId = model.Title;

            foreach (CharacterTitleModel titleModel in model.CharacterTitle)
                titles.Add(titleModel.Title, new Title(titleModel));

            EnsureActiveTitleIsOwned();
        }

        public void Update(double lastTick)
        {
            foreach (ITitle title in titles.Values)
            {
                title.Update(lastTick);
                if (title.TimeRemaining != null && title.TimeRemaining <= 0d)
                {
                    // title has expired
                    RevokeTitle((ushort)title.Entry.Id);
                }
            }
        }

        public void Save(CharacterContext context)
        {
            if (!activeSaved)
            {
                // character is attached in Player::Save, this will only be local lookup
                CharacterModel character = context.Character.Find(player.CharacterId);
                character.Title = activeTitleId;

                EntityEntry<CharacterModel> entity = context.Entry(character);
                entity.Property(p => p.Title).IsModified = true;

                activeSaved = true;
            }

            foreach (ITitle title in titles.Values)
                title.Save(context);
        }

        /// <summary>
        /// Add new <see cref="ITitle"/> with supplied title id.
        /// </summary>
        /// <remarks>
        /// If suppress if true, update won't be sent to client.
        /// </remarks>
        public void AddTitle(ushort titleId, bool suppress = false)
        {
            CharacterTitleEntry entry = GameTableManager.Instance.CharacterTitle.GetEntry(titleId);
            if (entry == null)
                throw new InvalidPacketValueException();

            if (titles.TryGetValue(titleId, out ITitle title))
            {
                if (title.Revoked)
                    title.Revoked = false;
                else
                {
                    player.Session.EnqueueMessageEncrypted(new ServerTitleUpdate
                    {
                        TitleId      = titleId,
                        Alreadyowned = true
                    });

                    return;
                }
            }
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
        /// Revoke <see cref="ITitle"/> with supplied title id.
        /// </summary>
        /// <remarks>
        /// If suppress if true, update won't be sent to client.
        /// </remarks>
        public void RevokeTitle(ushort titleId, bool suppress = false)
        {
            if (GameTableManager.Instance.CharacterTitle.GetEntry(titleId) == null)
                throw new InvalidPacketValueException();

            if (!titles.TryGetValue(titleId, out ITitle title))
                throw new InvalidPacketValueException();

            title.Revoked = true;

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
        /// Send all owned titles to client.
        /// </summary>
        public void SendTitles()
        {
            player.Session.EnqueueMessageEncrypted(new ServerTitles
            {
                Titles = titles.Values.Select(t => new ServerTitles.Title
                {
                    TitleId       = (ushort)t.Entry.Id,
                    Revoked       = t.Revoked,
                    TimeRemaining = (uint)(t.TimeRemaining ?? 0d)
                }).ToList()
            });
        }

        /// <summary>
        /// Add all available titles.
        /// </summary>
        /// <remarks>
        /// This is only used debug/command purposes.
        /// </remarks>
        public void AddAllTitles()
        {
            ushort[] titleIds = GameTableManager.Instance.CharacterTitle.Entries
                .Select(entry => (ushort)entry.Id)
                .ToArray();

            foreach (ushort titleId in titleIds)
                AddTitle(titleId, true);

            // client crashes if too many single title updates are sent, send as bulk
            SendTitles();
        }

        /// <summary>
        /// Remove all available titles.
        /// </summary>
        /// <remarks>
        /// This is only used debug/command purposes.
        /// </remarks>
        public void RevokeAllTitles()
        {
            ushort[] titleIds = titles.Values
                .Select(entry => (ushort)entry.Entry.Id)
                .ToArray();

            foreach (ushort titleId in titleIds)
                RevokeTitle(titleId, true);

            // client crashes if too many single title updates are sent, send as bulk
            SendTitles();
        }

        private void EnsureActiveTitleIsOwned()
        {
            if (activeTitleId != 0 && (!titles.TryGetValue(activeTitleId, out ITitle title) || title.Revoked))
                ActiveTitleId = 0;
        }

        /// <summary>
        /// Returns if title with supplied id is owned.
        /// </summary>
        public bool HasTitle(ushort id)
        {
            return titles.TryGetValue(id, out ITitle title) && !title.Revoked;
        }

        public IEnumerator<ITitle> GetEnumerator()
        {
            return titles.Values
                .Where(t => !t.Revoked)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
