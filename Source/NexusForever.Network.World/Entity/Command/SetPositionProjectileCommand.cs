using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetPositionProjectile)]
    public class SetPositionProjectileCommand : IEntityCommandModel
    {
        public Vector3 End { get; set; }
        public Vector3 Start { get; set; }
        public uint FlightTime { get; set; }
        public float Gravity { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            End        = reader.ReadVector3();
            Start      = reader.ReadVector3();
            FlightTime = reader.ReadUInt();
            Gravity    = reader.ReadSingle();
            Offset     = reader.ReadUInt();
            Blend      = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteVector3(End);
            writer.WriteVector3(Start);
            writer.Write(FlightTime);
            writer.Write(Gravity);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
