using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerPathActivateResult)]
    public class ServerPathActivateResult : IWritable
    {
        public GenericError Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 8);
        }
    }
}
