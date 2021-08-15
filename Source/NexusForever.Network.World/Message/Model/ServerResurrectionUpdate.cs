using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerResurrectionUpdate)]
    public class ServerResurrectionUpdate : IWritable
    {
        public RezType ShowRezFlags { get; set; } // 8
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ShowRezFlags, 8u);
            writer.Write(Unknown0);
        }
    }
}
