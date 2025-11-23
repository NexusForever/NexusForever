using NexusForever.Game.Static.Fortunes;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortunes
{
    // Generally end all 0 after ClientFortunesNotifyGame/Storefront and then send with values after ClientFortunesStart
    // Fortune games can be partially played and resumed so this message is not always sent with 0's after ClientFortunesNotifyGame/Storefront
    [Message(GameMessageOpcode.ServerFortunesCards)]
    public class ServerFortunesCards : IWritable
    {
        public FortunesOperation Operation { get; set; } 
        public RewardRarity[] Rarity { get; set; } = new RewardRarity[3]; 
        public uint[] AccountItemId { get; set; } = new uint[3]; 
        public bool[] CardFlipped { get; set; } = new bool[3]; 

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Operation, 3u);
            for (uint i = 0; i < 3; i++)
            {
                writer.Write(Rarity[i], 2u);
            }
            for (uint i = 0; i < 3; i++)
            {
                writer.Write(AccountItemId[i]);
            }
            for (uint i = 0; i < 3; i++)
            {
                writer.Write(CardFlipped[i]);
            }
        }
    }
}
