using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierTriggerOrdersTextOnUnit)]
    public class ServerPathSoldierTriggerOrdersTextOnUnit : IWritable
    {
        public ushort PathMissionId { get; set; }
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 15);
            writer.Write(UnitId);
        }
    }
}
