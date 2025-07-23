using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerPlayerChanged)]
    public class ServerPlayerChanged : IWritable
    {
        public uint UnitId { get; set; }
        public uint Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Unused);
        }
    }
}
