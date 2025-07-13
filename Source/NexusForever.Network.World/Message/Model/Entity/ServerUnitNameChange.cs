using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitNameChange)]
    public class ServerUnitNameChange : IWritable
    {
        public uint UnitId { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.WriteStringWide(Name);
        }
    }
}
