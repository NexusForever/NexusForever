using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ClientCraftingAbandon)]
    public class ClientCraftingAbandon : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
