using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;


namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientEmote)]
    public class ClientEmote : IReadable
    {
        public uint EmoteId { get; private set; }
        public uint Unknown0 { get; set; }

        public void Read(GamePacketReader reader)
        {
            EmoteId = reader.ReadUInt(14);
            Unknown0 = reader.ReadUInt(32);
        }
    }
}
