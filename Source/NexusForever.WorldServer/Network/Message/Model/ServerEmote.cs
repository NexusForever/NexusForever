using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEmote)]
    class ServerEmote : IWritable
    {
        public uint Guid { get; set; }
        public StandState StandState { get; set; }
        public uint EmoteId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(StandState, 4); 
            writer.Write(EmoteId, 14);
        }
    }
}
