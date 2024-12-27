using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetMove)]
    public class SetMoveCommand : IEntityCommandModel
    {
        public Vector3 Move { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Move  = reader.ReadPackedVector3();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.WritePackedVector3(Move);
            writer.Write(Blend);
        }
    }
}
