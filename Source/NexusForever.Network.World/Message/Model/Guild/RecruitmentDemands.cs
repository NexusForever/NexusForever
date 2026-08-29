using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    public class RecruitmentDemands : IWritable
    {
        public RecruitmendDemand WarriorAssault { get; set; }
        public RecruitmendDemand WarriorSupport { get; set; }
        public RecruitmendDemand EngineerAssault { get; set; }
        public RecruitmendDemand EngineerSupport { get; set; }
        public RecruitmendDemand EsperAssault { get; set; }
        public RecruitmendDemand EsperSupport { get; set; }
        public RecruitmendDemand MedicAssault { get; set; }
        public RecruitmendDemand MedicSupport { get; set; }
        public RecruitmendDemand StalkedAssault { get; set; }
        public RecruitmendDemand StalkerSupport { get; set; }
        public RecruitmendDemand SpellslingerAssault { get; set; }
        public RecruitmendDemand SpellslingerSupport { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(WarriorAssault, 8u);
            writer.Write(WarriorSupport, 8u);
            writer.Write(EngineerAssault, 8u);
            writer.Write(EngineerSupport, 8u);
            writer.Write(EsperAssault, 8u);
            writer.Write(EsperSupport, 8u);
            writer.Write(MedicAssault, 8u);
            writer.Write(MedicSupport, 8u);
            writer.Write(StalkedAssault, 8u);
            writer.Write(StalkerSupport, 8u);
            writer.Write(SpellslingerAssault, 8u);
            writer.Write(SpellslingerSupport, 8u);
        }
    }
}
