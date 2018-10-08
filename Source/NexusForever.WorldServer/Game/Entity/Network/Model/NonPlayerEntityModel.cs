using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class NonPlayerEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public byte Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write(Unknown0);
        }
    }
}
