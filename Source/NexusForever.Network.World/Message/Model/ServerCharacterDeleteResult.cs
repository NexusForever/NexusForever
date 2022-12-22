using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterDeleteResult)]
    public class ServerCharacterDeleteResult : IWritable
    {
        public CharacterModifyResult Result { get; set; } // 6
        public uint Data { get; set; } // used to select type of guild to show in error when result = DeleteFailed_GuildMaster

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 6u);
            writer.Write(Data);
        }
    }
}
