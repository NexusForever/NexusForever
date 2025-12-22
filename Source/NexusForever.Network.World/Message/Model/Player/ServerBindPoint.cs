using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerBindPoint)]
    public class ServerBindPoint : IWritable
    {
        public ushort BindPointId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BindPointId);
        }
    }
}
