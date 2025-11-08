using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEmoteAndStandState)]
    public class ServerEmoteAndStandState : IWritable
    {
        public uint UnitId { get; set; }
        public StandState StandState { get; set; }
        public uint EmoteId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(StandState, 4); 
            writer.Write(EmoteId, 14);
        }
    }
}
