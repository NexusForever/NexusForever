using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerTitleUpdate)]
    public class ServerTitleUpdate : IWritable
    {
        public ushort CharacterTitleId { get; set; }
        public bool AlreadyOwned { get; set; }
        public bool Revoked { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CharacterTitleId, 14u);
            writer.Write(AlreadyOwned);
            writer.Write(Revoked);
        }
    }
}
