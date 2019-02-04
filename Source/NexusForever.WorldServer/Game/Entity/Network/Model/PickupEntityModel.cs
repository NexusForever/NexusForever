using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class PickupEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public uint Unknown0 { get; set; }
        public uint Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(Unknown0);
            writer.Write(Unknown1);
        }
    }
}
