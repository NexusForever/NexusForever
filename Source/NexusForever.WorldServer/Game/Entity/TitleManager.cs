using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    // TODO we might want to put this at a more generic place!
    internal enum SaveStatus
    {
        New,
        Saved,
        Deleted
    }

    public class TitleManager : ISaveCharacter
    {
        private readonly Player player;

        private Dictionary<ulong, SaveStatus> owned = new Dictionary<ulong, SaveStatus>();

        public List<ulong> Owned
        {
            get => owned.Where(entry => entry.Value != SaveStatus.Deleted).Select(entry => entry.Key).ToList();
            set
            {
                foreach (ulong title in value)
                {
                    if (!owned.ContainsKey(title))
                        owned.Add(title, SaveStatus.New);
                    else if (owned[title] == SaveStatus.Deleted)
                        owned[title] = SaveStatus.Saved;
                }

                foreach (ulong title in owned.Keys.ToList())
                {
                    if (value.Contains(title))
                        continue;

                    if (owned[title] == SaveStatus.Saved)
                        owned[title] = SaveStatus.Deleted;
                    else
                        owned.Remove(title);
                }
                
                Update();

                EnsureActiveTitleIsOwned();
            }
        }

        private ulong active;
        private bool activeSaved = true;

        public ulong Active
        {
            get => active;
            set
            {
                if ((value != 0 && (!owned.ContainsKey(value) || owned[value] == SaveStatus.Deleted)) || active == value)
                    return;

                active = value;

                player.EnqueueToVisible(new ServerTitleSet
                {
                    Guid = player.Guid,
                    Title = Active
                }, true);

                activeSaved = false;
            }
        }

        public TitleManager(Player owner, Character model)
        {
            player = owner;
            active = model.Title;

            foreach (CharacterTitle characterTitle in model.CharacterTitle)
                owned.Add(characterTitle.Title, SaveStatus.Saved);
        }

        public void Add(ulong title)
        {
            if (owned.ContainsKey(title))
            {
                if (owned[title] == SaveStatus.Deleted)
                    owned[title] = SaveStatus.Saved;
                else
                    return;
            }
            else
            {
                if (GameTableManager.CharacterTitle.GetEntry(title) == null)
                    return;

                owned.Add(title, SaveStatus.New);
            }

            player.Session.EnqueueMessageEncrypted(new ServerTitleAdd
            {
                Title = title
            });
        }

        public void Remove(ulong title)
        {
            if (!owned.ContainsKey(title) || owned[title] == SaveStatus.Deleted)
                return;

            if (owned[title] == SaveStatus.New)
                owned.Remove(title);
            else
                owned[title] = SaveStatus.Deleted;

            Update();

            EnsureActiveTitleIsOwned();
        }

        private void EnsureActiveTitleIsOwned()
        {
            if (owned.ContainsKey(active))
                return;

            Active = 0;
        }

        public void Update()
        {
            player.Session.EnqueueMessageEncrypted(new ServerTitlesUpdate
            {
                Titles = Owned
            });
        }

        public void Save(CharacterContext context)
        {
            if (!activeSaved)
            {
                Character model = new Character
                {
                    Id = player.CharacterId
                };

                EntityEntry<Character> entity = context.Attach(model);
                model.Title = active;
                entity.Property(p => p.Title).IsModified = true;

                activeSaved = true;
            }

            foreach (ulong title in owned.Where(entry => entry.Value != SaveStatus.Saved).Select(entry => entry.Key).ToList())
            {
                CharacterTitle model = new CharacterTitle
                {
                    Id = player.CharacterId,
                    Title = (uint) title
                };

                if (owned[title] == SaveStatus.Deleted)
                {
                    context.Entry(model).State = EntityState.Deleted;
                    owned.Remove(title);
                }
                else if (owned[title] == SaveStatus.New)
                    context.Entry(model).State = EntityState.Added;

            }
        }
    }
}
