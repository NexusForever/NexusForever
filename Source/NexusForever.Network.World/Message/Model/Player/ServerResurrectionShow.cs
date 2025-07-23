using NexusForever.Game.Static.Player;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerResurrectionShow)]
    public class ServerResurrectionShow : IWritable
    {
        public uint GhostUnitId { get; set; }
        public uint RezCost { get; set; }

        /// <summary>
        /// Set the amount of time, in milliseconds, 
        /// </summary>
        public uint DeathPenaltyLength { get; set; }

        public bool PlayerIsDead { get; set; }
        public ResurrectionType ShowRezFlags { get; set; }
        public bool HasCasterRezRequest { get; set; }

        /// <summary>
        /// This must be set if the player is to be given the option to use Service Tokens.
        /// </summary>
        public uint TimeUntilWakeHereMs { get; set; }

        public uint TimeUntilForceRezMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GhostUnitId);
            writer.Write(RezCost);
            writer.Write(DeathPenaltyLength);
            writer.Write(PlayerIsDead);
            writer.Write(ShowRezFlags, 8u);
            writer.Write(HasCasterRezRequest);
            writer.Write(TimeUntilWakeHereMs);
            writer.Write(TimeUntilForceRezMs);
        }
    }
}
