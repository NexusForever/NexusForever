using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterFlagsUpdated)]
    public class ServerCharacterFlagsUpdated : IWritable
    {
        public CharacterFlag Flags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Flags, 32u);
        }
    }
}
