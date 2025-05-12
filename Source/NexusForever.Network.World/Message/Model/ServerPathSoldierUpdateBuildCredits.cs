using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierUpdateBuildCredits)]
    public class ServerPathSoldierUpdateBuildCredits : IWritable
    {
        public ushort PathSoldierEventId { get; set; }
        public uint BuildCredits { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(BuildCredits);
        }
    }
}
