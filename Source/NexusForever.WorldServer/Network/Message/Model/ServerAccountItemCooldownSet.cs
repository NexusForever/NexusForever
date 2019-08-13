using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
