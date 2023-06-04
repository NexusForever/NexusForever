using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityBoneUpdate)]
    public class ServerEntityBoneUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public List<float> Bones { get; set; } = new List<float>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Bones.Count);
            Bones.ForEach(u => writer.Write(u));
        }
    }
}
