using NexusForever.Game.Static.Player;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerResurrectionUpdate)]
    public class ServerResurrectionUpdate : IWritable
    {
        public ResurrectionType ShowRezFlags { get; set; }
        public bool HasCasterRezRequest { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ShowRezFlags, 8u);
            writer.Write(HasCasterRezRequest);
        }
    }
}
