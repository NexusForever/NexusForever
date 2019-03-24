using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetScale)]
    public class SetScaleCommand : IEntityCommand
    {
        public ushort Scale { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            Scale = reader.ReadUShort();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Scale);
            writer.Write(Blend);
        }
    }
}
