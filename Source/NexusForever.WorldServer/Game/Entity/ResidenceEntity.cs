using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity
{
    public class ResidenceEntity : WorldEntity
    {
        public HousingPlotInfoEntry PlotEntry { get; }

        public ResidenceEntity(uint creatureId, HousingPlotInfoEntry housingPlotInfoEntry)
            : base(EntityType.Residence)
        {
            PlotEntry = housingPlotInfoEntry;

            CreatureId = creatureId;
            SocketId = (ushort)PlotEntry.WorldSocketId;
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new ResidenceEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
