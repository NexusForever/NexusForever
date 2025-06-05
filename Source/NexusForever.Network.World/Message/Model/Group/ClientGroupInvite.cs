using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupInvite)]
    public class ClientGroupInvite : IReadable
    {
        public string InviteeName { get; private set; }
        public string Unused { get; private set; } // Always filled with empty string

        public void Read(GamePacketReader reader)
        {
            InviteeName     = reader.ReadWideString();
            Unused          = reader.ReadWideString();
        }
    }
}
