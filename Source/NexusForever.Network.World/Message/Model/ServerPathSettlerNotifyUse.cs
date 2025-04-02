using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerNotifyUse)]
    public class ServerPathSettlerNotifyUse : IWritable
    {
        public ushort PathSettlerImprovementGroupId { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerImprovementGroupId, 14);
            writer.WriteStringWide(Name);
        }
    }
}
