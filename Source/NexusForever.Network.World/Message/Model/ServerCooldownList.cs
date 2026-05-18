using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCooldownList)]
    public class ServerCooldownList : IWritable
    {
        public List<Cooldown> Cooldowns { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Cooldowns.Count);
            Cooldowns.ForEach(c => c.Write(writer));
        }
    }
}
