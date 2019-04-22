using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPositionProjectile)]
    public class SetPositionProjectileCommand : IEntityCommandModel
    {
        public Position Position { get; set; } 
        public Position Rotation { get; set; }

        public uint FlightTime { get; set; }
        public float Gravity { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Position = new Position();
            Rotation = new Position();

            Position.Read(reader);
            Rotation.Read(reader);
            FlightTime = reader.ReadUInt();
            Gravity = reader.ReadUInt();
            Offset = reader.ReadUInt();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            Position.Write(writer);
            Rotation.Write(writer);
            writer.Write(FlightTime);
            writer.Write(Gravity);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
