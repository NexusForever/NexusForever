using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class InstancePortalEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public uint RemainingTimeMs { get ; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(RemainingTimeMs);
        }
    }
}
