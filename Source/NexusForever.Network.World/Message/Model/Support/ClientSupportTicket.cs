using NexusForever.Game.Static;
using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Support
{
    [Message(GameMessageOpcode.ClientSupportTicket)]
    public class ClientSupportTicket : IReadable
    {
        public ushort TicketCategoryId { get; private set; }
        public ushort TicketSubCategoryId { get; private set; }
        public Vector3 Position { get; private set; } // player unit's current commanded position
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public Language LanguageId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TicketCategoryId = reader.ReadUShort();
            TicketSubCategoryId = reader.ReadUShort();
            Position = reader.ReadVector3();
            Subject = reader.ReadWideString();
            Body = reader.ReadWideString();
            LanguageId = reader.ReadEnum<Language>(32u);
        }
    }
}
