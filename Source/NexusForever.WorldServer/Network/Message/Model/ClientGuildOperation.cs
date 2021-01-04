using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildOperation)]
    public class ClientGuildOperation : IReadable
    {
        public ushort RealmId { get; private set; }
        public ulong GuildId { get; private set; }
        public uint Rank { get; private set; }
        public ulong Data { get; private set; }
        public string TextValue { get; private set; } // Client has this named as "Who", but it is used for Player Names, Rank Names, Notes, MOTD, and other stuff
        public GuildOperation Operation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId   = reader.ReadUShort(14u);
            GuildId   = reader.ReadULong();
            Rank      = reader.ReadUInt();
            Data      = reader.ReadULong();
            TextValue = reader.ReadWideString();
            Operation = reader.ReadEnum<GuildOperation>(6u);
        }
    }
}
