using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGenericError)]
    public class ServerGenericError : IWritable
    {
        public GenericError Error { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Error, 8u);
        }
    }
}
