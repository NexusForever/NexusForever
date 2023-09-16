using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerResurrectionUpdate)]
    public class ServerResurrectionUpdate : IWritable
    {
        public ResurrectionType ShowRezFlags { get; set; } // 8
        public bool HasCasterRezRequest { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ShowRezFlags, 8u);
            writer.Write(HasCasterRezRequest);
        }
    }
}
