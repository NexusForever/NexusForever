using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUpdatePhase)]
    public class ServerUpdatePhase : IWritable
    {
        public uint CanSee { get; set; } = 1;
        public uint CanSeeMe { get; set; } = 1;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CanSee);
            writer.Write(CanSeeMe);
        }
    }
}
