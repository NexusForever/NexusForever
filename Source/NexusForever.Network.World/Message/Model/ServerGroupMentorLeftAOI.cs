using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fired whenever a pair of players using mentoring get too far apart, when the players move within range again, and when the timer to move within range runs out.
    [Message(GameMessageOpcode.ServerGroupMentorLeftAOI)]
    public class ServerGroupMentorLeftAOI : IWritable
    { 
        public uint TimeUntilCancelledMS { get; set; } // How long the players have to return to eachother's Area of Interest before the Mentoring status is canceled.
        public bool TimerStop { get; set; } // Whether or not the countdown timer has stopped.  This can be because the players have moved within range of eachother or the timer has expired.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TimeUntilCancelledMS);
            writer.Write(TimerStop);
        }
    }
}
