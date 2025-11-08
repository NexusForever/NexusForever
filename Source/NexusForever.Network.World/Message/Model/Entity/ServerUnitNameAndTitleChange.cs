using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    [Message(GameMessageOpcode.ServerUnitNameAndTitleChange)]
    public class ServerUnitNameAndTitleChange : IWritable
    {
        public uint UnitId { get; set; }
        public uint TitleId { get; set; }
        public uint Creature2Id { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(TitleId, 14u);
            writer.Write(Creature2Id, 18u);
            writer.WriteStringWide(Name);
        }
    }
}
