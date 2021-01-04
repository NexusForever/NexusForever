using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChat)]
    public class ServerChat : IWritable
    {
        public ChatChannelType Channel { get; set; }
        public ulong ChatId { get; set; }

        public bool GM { get; set; }
        public bool Self { get; set; }
        public bool AutoResponse { get; set; }
        public bool CrossFaction { get; set; }        
        public ChatPresenceState PresenceState { get; set; }

        public uint RealmId { get; set; }
        public ulong CharacterId { get; set; }

        public string Name { get; set; }
        public string Realm { get; set; }
        public ulong Guid { get; set; }
        public string Text { get; set; }

        public List<ChatFormat> Formats { get; set; } = new List<ChatFormat>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel, 14u);
            writer.Write(ChatId);

            writer.Write(GM);
            writer.Write(Self);
            writer.Write(AutoResponse);

            writer.Write(RealmId, 14u);
            writer.Write(Guid);

            writer.WriteStringWide(Name);
            writer.WriteStringWide(Realm);
            writer.Write(PresenceState, 3);

            writer.WriteStringWide(Text);
            writer.Write(Formats.Count, 5u);

            Formats.ForEach(f => f.Write(writer));

            writer.Write(CrossFaction);
            writer.Write(0, 16u);

            writer.Write(Guid, 32u); // UnitId?
            writer.Write(0, 8u); // Premium
        }
    }
}
