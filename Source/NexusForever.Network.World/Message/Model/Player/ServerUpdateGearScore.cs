using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerUpdateGearScore)]
    public class ServerUpdateGearScore : IWritable
    {
        public float GearScore { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GearScore);
        }
    }
}
