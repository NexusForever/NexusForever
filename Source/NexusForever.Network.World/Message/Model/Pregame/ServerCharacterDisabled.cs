using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    [Message(GameMessageOpcode.ServerCharacterDisabled)]
    public class ServerCharacterDisabled : IWritable
    {
        public ulong CharacterId { get; set; }
        public bool Disabled { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CharacterId);
            writer.Write(Disabled);
        }
    }
}
