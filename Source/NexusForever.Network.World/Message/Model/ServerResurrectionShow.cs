using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerResurrectionShow)]
    public class ServerResurrectionShow : IWritable
    {
        public uint GhostId { get; set; }
        public uint RezCost { get; set; }

        /// <summary>
        /// Set the amount of time, in milliseconds, 
        /// </summary>
        public uint TimeUntilRezMs { get; set; }
        public bool Dead { get; set; }
        public RezType ShowRezFlags { get; set; } // 8
        public bool Unknown0 { get; set; }
        /// <summary>
        /// This must be set if the player is to be given the option to use Service Tokens.
        /// </summary>
        public uint TimeUntilWakeHereMs { get; set; }
        public uint TimeUntilForceRezMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GhostId);
            writer.Write(RezCost);
            writer.Write(TimeUntilRezMs);
            writer.Write(Dead);
            writer.Write(ShowRezFlags, 8u);
            writer.Write(Unknown0);
            writer.Write(TimeUntilWakeHereMs);
            writer.Write(TimeUntilForceRezMs);
        }
    }
}
