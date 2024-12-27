using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetPositionProjectile)]
    public class SetPositionProjectileCommand : IEntityCommandModel
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public uint FlightTime { get; set; }
        public float Gravity { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Position   = reader.ReadVector3();
            Rotation   = reader.ReadVector3();
            FlightTime = reader.ReadUInt();
            Gravity    = reader.ReadUInt();
            Offset     = reader.ReadUInt();
            Blend      = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteVector3(Position);
            writer.WriteVector3(Rotation);
            writer.Write(FlightTime);
            writer.Write(Gravity);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
