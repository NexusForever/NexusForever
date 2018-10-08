using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPlatform)]
    public class SetPlatformCommand : IEntityCommand
    {
        public uint Platform { get; set; }

        public void Read(GamePacketReader reader)
        {
            Platform = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Platform);
        }
    }
}
