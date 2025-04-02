using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierHoldoutDeath)]
    public class ServerPathSoldierHoldoutDeath : IWritable
    {
        public ushort PathSoldierEventId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
        }
    }
}
