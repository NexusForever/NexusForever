using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerCreddRedeemResult)]
    public class ServerCreddRedeemResult : IWritable
    {
        public CharacterModifyResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 6u);
        }
    }
}
