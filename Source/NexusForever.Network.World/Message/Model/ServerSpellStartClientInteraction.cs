using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    // Used by CastMethod: 5
    [Message(GameMessageOpcode.ServerSpellStartClientInteraction)]
    public class ServerSpellStartClientInteraction : IWritable
    {
        public uint ClientUniqueId { get; set; }
        public uint CastingId { get; set; }
        public uint CasterId { get; set; }
        public Position Position { get; set; } = new Position();
        public uint Yaw { get; set; }

        public List<InitialPosition> InitialPositionData { get; set; } = new List<InitialPosition>();
        public List<TelegraphPosition> TelegraphPositionData { get; set; } = new List<TelegraphPosition>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ClientUniqueId);
            writer.Write(CastingId);
            writer.Write(CasterId);
            Position.Write(writer);
            writer.Write(Yaw);

            writer.Write(InitialPositionData.Count, 8u);
            InitialPositionData.ForEach(u => u.Write(writer));

            writer.Write(TelegraphPositionData.Count, 8u);
            TelegraphPositionData.ForEach(u => u.Write(writer));
        }
    }
}