using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Game.Social
{
    public class ChatChannelMember : ISaveCharacter
    {
        [Flags]
        public enum ChatChannelMemberSaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Delete = 0x02,
            Flags  = 0x04
        }

        public ulong Id { get; set; }
        public ulong CharacterId { get; set; }

        public ChatChannelMemberFlags Flags
        {
            get => flags;
            set
            {
                flags = value;
                saveMask |= ChatChannelMemberSaveMask.Flags;
            }
        }
        private ChatChannelMemberFlags flags;

        public bool IsOnline { get; set; }

        private ChatChannelMemberSaveMask saveMask;

        /// <summary>
        /// Returns if <see cref="ChatChannel"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & ChatChannelMemberSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="ChatChannel"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & ChatChannelMemberSaveMask.Delete) != 0;

        /// <summary>
        /// Enqueue <see cref="ChatChannel"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= ChatChannelMemberSaveMask.Delete;
            else
                saveMask &= ~ChatChannelMemberSaveMask.Delete;
        }

        /// <summary>
        /// Create a new <see cref="ChatChannelMember"/> with supplied character id and <see cref="ChatChannelMemberFlags"/>.
        /// </summary>
        public ChatChannelMember(ulong id, ulong characterId, ChatChannelMemberFlags flags)
        {
            Id          = id;
            CharacterId = characterId;
            Flags       = flags;

            saveMask    = ChatChannelMemberSaveMask.Create;
        }

        /// <summary>
        /// 
        /// </summary>
        public ChatChannelMember(ChatChannelMemberModel model)
        {
            Id          = model.Id;
            CharacterId = model.CharacterId;
            Flags       = (ChatChannelMemberFlags)model.Flags;

            saveMask    = ChatChannelMemberSaveMask.None;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ChatChannelMemberSaveMask.None)
                return;

            var model = new ChatChannelMemberModel
            {
                Id          = Id,
                CharacterId = CharacterId
            };

            if ((saveMask & ChatChannelMemberSaveMask.Create) != 0)
            {
                model.Flags = (byte)flags;
                context.Add(model);
            }
            else if ((saveMask & ChatChannelMemberSaveMask.Delete) != 0)
                context.Remove(model);
            else
            {
                EntityEntry<ChatChannelMemberModel> entry = context.Attach(model);
                if ((saveMask & ChatChannelMemberSaveMask.Flags) != 0)
                {
                    Flags = flags;
                    entry.Property(p => p.Flags).IsModified = true;
                }
            }

            saveMask = ChatChannelMemberSaveMask.None;
        }

        /// <summary>
        /// Returns if supplied <see cref="ChatChannelMemberFlags"/> exists.
        /// </summary>
        public bool HasFlag(ChatChannelMemberFlags flags)
        {
            return (Flags & flags) != 0;
        }

        /// <summary>
        /// Add a new <see cref="ChatChannelMemberFlags"/>.
        /// </summary>
        public void SetFlag(ChatChannelMemberFlags flags)
        {
            Flags |= flags;
        }

        /// <summary>
        /// Remove an existing <see cref="ChatChannelMemberFlags"/>.
        /// </summary>
        public void RemoveFlag(ChatChannelMemberFlags flags)
        {
            Flags &= ~flags;
        }
    }
}
