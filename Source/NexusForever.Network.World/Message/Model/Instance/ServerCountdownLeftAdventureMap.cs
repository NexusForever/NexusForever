using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    // Starts the 10 second countdown when a player leaves an adventure map "You left the adventure map!"
    [Message(GameMessageOpcode.ServerCountdownLeftAdventureMap)]
    public class ServerCountdownLeftAdventureMap : IWritable
    {
        public bool LeftMap { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LeftMap);
        }
    }
}
