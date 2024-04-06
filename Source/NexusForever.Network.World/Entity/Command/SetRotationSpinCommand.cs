using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetRotationSpin)]
    public class SetRotationSpinCommand : IEntityCommandModel
    {
        public Vector3 Rotation { get; set; }
        public uint FlightTime { get; set; }
        public float Spin { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Rotation   = reader.ReadVector3();
            FlightTime = reader.ReadUInt();
            Spin       = reader.ReadPackedFloat();
            Offset     = reader.ReadUInt();
            Blend      = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteVector3(Rotation);
            writer.Write(FlightTime);
            writer.WritePackedFloat(Spin);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
