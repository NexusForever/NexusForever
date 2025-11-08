using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCreatureFlagsUpdate)]
    public class ServerCreatureFlagsUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public uint Flags { get; set; } // TODO: investigate flags
        public uint UiFlags { get; set; } // TODO: investigate flags

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Flags);
            writer.Write(UiFlags);
        }
    }
}
