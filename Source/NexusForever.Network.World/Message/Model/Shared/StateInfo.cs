using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class StateInfo : IWritable
    {
        public PvpGameState State { get; set; }
        public uint TimeElapsed { get; set; } // in milliseconds

        public void Write(GamePacketWriter writer)
        {
            writer.Write(State, 3u);
            writer.Write(TimeElapsed);
        }
    }
}
