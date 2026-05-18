using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
