using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterCreate)]
    public class ServerCharacterCreate : IWritable
    {
        public ulong CharacterId { get; set; }
        public uint WorldId { get; set; }
        public byte Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CharacterId);
            writer.Write(WorldId);
            writer.Write(Result, 3);
        }
    }
}
