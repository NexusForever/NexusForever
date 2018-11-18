using System;
using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    
    [Message(GameMessageOpcode.ClientChat, MessageDirection.Client)]
    public class ClientChat : IReadable
    {
        public class ChatLink
        {
            public ChatLinkType Type;
            public ushort StartIndex;
            public ushort EndIndex;
        }

        public class ItemGuidChatLink : ChatLink, IReadable
        {
            public ulong Guid;

            public void Read(GamePacketReader reader)
            {
                Guid = reader.ReadULong();
            }
        }

        public class ItemIdChatLink : ChatLink, IReadable
        {
            public uint ItemId;

            public void Read(GamePacketReader reader)
            {
                ItemId = reader.ReadUInt(18);
            }
        }

        public ChatChannel Channel { get; private set; }
        public ulong Unknown0 { get; set; }
        public string Message { get; private set; }
        public List<ChatLink> LinkItems { get; private set; }

        public void Read(GamePacketReader reader)
        {
            LinkItems = new List<ChatLink>();

            Channel  = reader.ReadEnum<ChatChannel>(14);
            Unknown0 = reader.ReadULong();
            Message  = reader.ReadWideString();

            byte linkedItemCount = reader.ReadByte(5);

            for (int i = 0; i < linkedItemCount; i++)
            {
                ChatLinkType Type = reader.ReadEnum<ChatLinkType>(4);
                ushort StartIndex = reader.ReadUShort(16);
                ushort EndIndex = reader.ReadUShort(16);

                if(Type == ChatLinkType.ItemGuid)
                {
                    ItemGuidChatLink link = new ItemGuidChatLink
                    {
                        Type = Type,
                        StartIndex = StartIndex,
                        EndIndex = EndIndex,
                    };
                    link.Read(reader);
                    LinkItems.Add(link);
                }
                else if(Type == ChatLinkType.ItemItemId)
                {
                    ItemIdChatLink link = new ItemIdChatLink
                    {
                        Type = Type,
                        StartIndex = StartIndex,
                        EndIndex = EndIndex,
                    };
                    link.Read(reader);
                    LinkItems.Add(link);
                }
                else
                {
                    Console.WriteLine("Unkown Type: " + Type);
                }
            }
        }
    }
}
