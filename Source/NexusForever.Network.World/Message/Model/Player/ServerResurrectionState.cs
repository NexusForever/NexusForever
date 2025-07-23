using NexusForever.Game.Static.Player;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
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
