using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // Appears to be gained from using the Ability Tier Point Unlock item (item2d = 91195)
    [Message(GameMessageOpcode.ServerAbilityTierPointsBonus)]
    public class ServerAbilityTierPointsBonus : IWritable
    {
        public uint BonusAbilityTierPoints { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BonusAbilityTierPoints);
        }
    }
}
