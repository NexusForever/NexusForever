using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetRotationFaceUnit)]
    public class SetRotationFaceUnitCommand : IEntityCommand
    {
        public uint UnitId { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Blend);
        }
    }
}
