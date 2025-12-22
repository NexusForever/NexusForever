using NexusForever.Game.Static.Player;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerCharacterAppearanceResult)]
    public class ServerCharacterAppearanceResult : IWritable
    {
        public CharacterRecustomisationResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 3u);   
        }
    }
}
