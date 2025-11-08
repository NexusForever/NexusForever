using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Only used by client when on the CharacterSelect screen
    [Message(GameMessageOpcode.ServerCharacterRenameResult)]
    public class ServerCharacterRenameResult : IWritable
    {
        public CharacterModifyResult Result { get; set; }
        public ulong CharacterId { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 3u);
            writer.Write(CharacterId);
            writer.WriteStringWide(Name);
        }
    }
}
