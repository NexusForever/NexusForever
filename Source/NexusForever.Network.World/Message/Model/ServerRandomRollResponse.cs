using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRandomRollResponse)]
    public class ServerRandomRollResponse : IWritable
    {
        public TargetPlayerIdentity TargetPlayerIdentity { get; set; }
        public uint MinRandom { get; set; }
        public uint MaxRandom { get; set; }
        public int RandomRollResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            TargetPlayerIdentity.Write(writer);
            writer.Write(MinRandom);
            writer.Write(MaxRandom);
            writer.Write(RandomRollResult);
        }
    }
}