using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityAggroSwitch)]
    public class ServerEntityAggroSwitch : IWritable
    {
        public uint UnitId { get; set; }
        public uint TargetId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(TargetId);
        }
    }
}
