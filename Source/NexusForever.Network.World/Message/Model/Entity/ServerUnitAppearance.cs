using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    [Message(GameMessageOpcode.ServerUnitAppearance)]
    public class ServerUnitAppearance : IWritable
    {
        public uint UnitId { get; set; }
        public uint Creature2Id { get; set; }
        public uint DisplayInfoId { get; set; }
        public bool TriggerDefaultBirthSequence { get; set; }
        public bool ShowDefaultName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Creature2Id, 18u);
            writer.Write(DisplayInfoId, 17u);
            writer.Write(TriggerDefaultBirthSequence);
            writer.Write(ShowDefaultName);
        }
    }
}
