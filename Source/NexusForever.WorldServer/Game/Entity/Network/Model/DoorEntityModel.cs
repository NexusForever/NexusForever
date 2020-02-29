using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class DoorEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
        }
    }
}
