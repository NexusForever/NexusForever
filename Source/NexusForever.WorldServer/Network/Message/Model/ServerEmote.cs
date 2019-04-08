using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEmote)]
    class ServerEmote : IWritable
    {
        public uint Guid { get; set; }
        public uint StandState { get; set; }
        public uint EmoteId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid, 32);
            writer.Write(StandState, 4); 
            writer.Write(EmoteId, 14);
        }
    }
}
