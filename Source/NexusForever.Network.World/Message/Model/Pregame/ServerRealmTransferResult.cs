using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Client only processes message when on CharacterSelect screen.
    [Message(GameMessageOpcode.ServerRealmTransferResult)]
    public class ServerRealmTransferResult : IWritable
    {
        public CharacterModifyResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 6u);
        }
    }
}
