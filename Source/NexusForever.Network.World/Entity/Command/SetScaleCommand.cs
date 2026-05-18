using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetScale)]
    public class SetScaleCommand : IEntityCommandModel
    {
        public float Scale { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Scale = reader.ReadPackedFloat();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.WritePackedFloat(Scale);
            writer.Write(Blend);
        }
    }
}
