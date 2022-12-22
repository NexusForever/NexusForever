using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterCreate)]
    public class ServerCharacterCreate : IWritable
    {
        public ulong CharacterId { get; set; }
        public uint WorldId { get; set; }
        public CharacterModifyResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CharacterId);
            writer.Write(WorldId);
            writer.Write(Result, 3);
        }
    }
}
