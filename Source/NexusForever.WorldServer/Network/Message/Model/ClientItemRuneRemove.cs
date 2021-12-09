using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemRuneRemove)]
    public class ClientItemRuneRemove: IReadable
    {
        public ulong Guid { get; private set; }
        public uint SocketIndex { get; private set; }
        public bool Recover { get; private set; }
        public bool UseServiceTokens { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadULong();
            SocketIndex = reader.ReadUInt();
            Recover = reader.ReadBit();
            UseServiceTokens = reader.ReadBit();
        }
    }
}
