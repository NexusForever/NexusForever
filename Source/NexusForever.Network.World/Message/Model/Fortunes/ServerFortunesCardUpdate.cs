using NexusForever.Game.Static.Fortunes;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortunes
{
    [Message(GameMessageOpcode.ServerFortunesCardUpdate)]
    public class ServerFortunesCardUpdate : IWritable
    {
        public bool Unknown { get; set; } = true; // always needs to be true or the client ignores the rest of the message
        public FortunesOperation Operation { get; set; }   
        public bool[] CardFlipped { get; set; } = new bool[3];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(Operation, 3u);
            for(uint i = 0; i < 3; i++)
            {
                writer.Write(CardFlipped[i]);
            }
        }
    }
}
