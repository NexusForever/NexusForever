using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpell0x143)]
    public class ServerSpell0x143 : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero byte message
        }
    }
}
