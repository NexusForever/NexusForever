using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEmote)]
    public class ServerEmote : IWritable
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
