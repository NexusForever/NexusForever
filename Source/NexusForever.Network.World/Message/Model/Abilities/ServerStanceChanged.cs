using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerStanceChanged)]
    public class ServerStanceChanged : IWritable
    {
        public byte InnateIndex { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(InnateIndex, 2u);
        }
    }
}
