using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetPlatform)]
    public class SetPlatformCommand : IEntityCommandModel
    {
        public uint UnitId { get; set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
        }
    }
}
