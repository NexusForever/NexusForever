using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    [Message(GameMessageOpcode.ServerTitleSet)]
    public class ServerTitleSet : IWritable
    {
        public uint UnitId { get; set; }
        public ushort CharacterTitleId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(CharacterTitleId, 14);
        }
    }
}
