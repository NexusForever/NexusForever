using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChatWhisperFail)]
    class ServerChatWhisperFail : IWritable
    {
        public string CharacterTo { get; set; }
        public bool IsAccountWhisper { get; set; }
        public ushort Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(CharacterTo);
            writer.Write(IsAccountWhisper);
            writer.Write(Unknown1); // Result?
        }
    }
}
