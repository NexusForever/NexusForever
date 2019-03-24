using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class PlatformEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public byte QuestChecklistIdx { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write(QuestChecklistIdx);
        }
    }
}
