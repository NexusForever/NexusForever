using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetRotationFacePosition)]
    public class SetRotationFacePositionCommand : IEntityCommandModel
    {
        public Vector3 Position { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Position = reader.ReadVector3();
            Blend    = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteVector3(Position);
            writer.Write(Blend);
        }
    }
}
