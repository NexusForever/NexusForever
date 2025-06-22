using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fires at regular intervals if the player is AFK in a PvP match. 
    // This is triggered when the player has 2 minutes, 1 minute, 30 seconds, and 5 second intervals before they are removed from a match
    [Message(GameMessageOpcode.ServerMatchingPvpInactivityAlert)]
    public class ServerMatchingPvpInactivityAlert : IWritable
    {
        public uint RemainingTimeMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RemainingTimeMs);
        }
    }
}
