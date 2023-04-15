using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Social
{
    public interface IChatMessageBuilder : INetworkBuildable<ServerChat>
    {
        ChatChannelType Type { get; set; }
        ulong ChatId { get; set; }
        bool GM { get; set; }
        bool Self { get; set; }
        bool AutoResponse { get; set; }
        ulong FromCharacterId { get; set; }
        ushort FromCharacterRealmId { get; set; }
        string FromName { get; set; }
        string FromRealm { get; set; }
        ChatPresenceState PresenceState { get; set; }
        string Text { get; set; }
        List<ChatFormat> Formats { get; set; }
        bool CrossFaction { get; set; }
        uint Guid { get; set; }
        byte PremiumTier { get; set; }

        /// <summary>
        /// Append text to the end of the message.
        /// </summary>
        void AppendText(string text);

        /// <summary>
        /// Append an item chat link to the end of the message.
        /// </summary>
        void AppendItem(uint itemId);

        /// <summary>
        /// Append a quest chat link to the end of the message.
        /// </summary>
        void AppendQuest(ushort questId);
    }
}