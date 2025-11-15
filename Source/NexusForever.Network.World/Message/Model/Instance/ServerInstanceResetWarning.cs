using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    // Sends the warning "This instance is scheduled to reset soon. You will be removed and progress reset in $+(time)."
    [Message(GameMessageOpcode.ServerInstanceResetWarning)]
    public class ServerInstanceResetWarning : IWritable
    {
        public uint SecondsUntilReset { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SecondsUntilReset);
        }
    }
}
