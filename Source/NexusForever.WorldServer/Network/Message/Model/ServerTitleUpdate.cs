using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerTitleUpdate)]
    public class ServerTitleUpdate : IWritable
    {
        public ushort TitleId { get; set; }
        public bool Alreadyowned { get; set; }
        public bool Revoked { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TitleId, 14u);
            writer.Write(Alreadyowned);
            writer.Write(Revoked);
        }
    }
}
