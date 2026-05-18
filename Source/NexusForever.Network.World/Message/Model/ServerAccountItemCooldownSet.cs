using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountItemCooldownSet)]
    public class ServerAccountItemCooldownSet : IWritable
    {
        public uint AccountItemCooldownGroup { get; set; }
        public uint CooldownInSeconds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountItemCooldownGroup);
            writer.Write(CooldownInSeconds);
        }
    }
}
