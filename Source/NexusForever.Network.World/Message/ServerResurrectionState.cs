using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message
{
    [Message(GameMessageOpcode.ServerResurrectionState)]
    public class ServerResurrectionState : IWritable
    {
        public bool Forbidden { get; set; }
        public ResurrectionType RezType { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Forbidden);
            writer.Write(RezType);
        }
    }
}
