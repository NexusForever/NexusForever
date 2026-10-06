using NexusForever.Game.Static.AccountInventory;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountPrivilegeRestrictionUpdate)]
    public class ServerAccountPrivilegeRestrictionUpdate : IWritable
    {
        public AccountPrivilegeRestriction Restriction { get; set; }
        public float PenaltyDays { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Restriction, 3u);
            writer.Write(PenaltyDays);
        }
    }
}
