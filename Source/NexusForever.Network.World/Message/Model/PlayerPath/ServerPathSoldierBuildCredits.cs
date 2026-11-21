using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathSoldierBuildCredits)]
    public class ServerPathSoldierBuildCredits : IWritable
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
