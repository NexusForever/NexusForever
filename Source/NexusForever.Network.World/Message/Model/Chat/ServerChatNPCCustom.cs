using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChatNPCCustom)]
    public class ServerChatNPCCustom : IWritable
    {
        public Channel Channel { get; set; }
        public uint UnitNameLocalizedTextId { get; set; }
        public string CustomChatText { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);
            writer.Write(UnitNameLocalizedTextId, 21u);
            writer.WriteStringWide(CustomChatText);
        }
    }
}
