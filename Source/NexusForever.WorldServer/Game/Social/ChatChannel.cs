using System.Collections.Generic;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Game.Social
{
    public class ChatChannel
    {
        public ChatChannelType Type { get; set; }
        public ulong Id { get; set; }

        private readonly HashSet<ulong> members = new HashSet<ulong>();

        /// <summary>
        /// Create a new <see cref="ChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        public ChatChannel(ChatChannelType type, ulong id)
        {
            Type = type;
            Id   = id;
        }

        /// <summary>
        /// Add a new member to the <see cref="ChatChannel"/> with supplied character id.
        /// </summary>
        public void AddMember(ulong characterId)
        {
            members.Add(characterId);
        }

        /// <summary>
        /// Remove an existing member from <see cref="ChatChannel"/> with supplied character id.
        /// </summary>
        public void RemoveMember(ulong characterId)
        {
            members.Remove(characterId);
        }

        /// <summary>
        /// Returns if supplied character is a member of the <see cref="ChatChannel"/>.
        /// </summary>
        public bool IsMember(ulong characterId)
        {
            return members.Contains(characterId);
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members in the <see cref="ChatChannel"/>.
        /// </summary>
        public void Broadcast(IWritable message)
        {
            foreach (ulong characterId in members)
            {
                Player player = CharacterManager.Instance.GetPlayer(characterId);
                if (player?.Session != null)
                    player.Session.EnqueueMessageEncrypted(message);
            }
        }
    }
}
