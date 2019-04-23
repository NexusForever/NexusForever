using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
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
