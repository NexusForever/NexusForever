using System;
using System.Collections.Generic;
using System.IO;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    public class ServerLinkedItemId : IWritable
    {
        public ushort StartIndex;
        public ushort EndIndex;
        public uint ItemId;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChatLinkType.ItemItemId, 4);
            writer.Write(StartIndex, 16);
            writer.Write(EndIndex, 16);
            writer.Write(ItemId, 18);
        }
    }

    [Message(GameMessageOpcode.ServerChat, MessageDirection.Server)]
    class ServerChat : IWritable
    {
        public ChatChannel Channel { get; set; }
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

        public List<ServerLinkedItemId> LinkedItems { get; set; } = new List<ServerLinkedItemId>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel, 14u);
            writer.Write((ulong)ChatId, 64);

            writer.Write(GM);
            writer.Write(Self);
            writer.Write(AutoResponse);

            writer.Write(RealmId, 14);
            writer.Write(Guid, 64);

            writer.WriteStringWide(Name);
            writer.WriteStringWide(Realm);
            writer.Write(PresenceState, 3);

            writer.WriteStringWide(Text);
            writer.Write(LinkedItems.Count, 5);

            LinkedItems.ForEach((linkedItem) =>
            {
                linkedItem.Write(writer);
            });

            writer.Write(CrossFaction);
            writer.Write(0, 16); 

            writer.Write(Guid, 32); // UnitId?
            writer.Write(0, 8); // Premium
        }
    }
}
