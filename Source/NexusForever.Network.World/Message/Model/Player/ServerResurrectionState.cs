using NexusForever.Game.Static.Player;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    // Give a 10 second countdown tick saying "Forbidden Zone"
    // Sets the resurrection options if the player is dead and has the Resurrection dialog
    [Message(GameMessageOpcode.ServerForbiddenZone)]
    public class ServerResurrectionState : IWritable
    {
        public bool Forbidden { get; set; } // triggers the countdown tick, setting to 0 cancels the tick
        public ResurrectionType RezOptions { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Forbidden);
            writer.Write(RezOptions);
        }
    }
}
