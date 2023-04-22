using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAbilityPoints)]
    public class ServerAbilityPoints : IWritable
    {
        public uint AbilityPoints { get; set; }
        public uint TotalAbilityPoints { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AbilityPoints);
            writer.Write(TotalAbilityPoints);
        }
    }
}
