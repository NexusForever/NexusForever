using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class Plug : WorldEntity
    {
        public HousingPlotInfoEntry PlotEntry { get; }
        public HousingPlugItemEntry PlugEntry { get; }

        public Plug(HousingPlotInfoEntry plotEntry, HousingPlugItemEntry plugEntry)
            : base(EntityType.Plug)
        {
            PlotEntry = plotEntry;
            PlugEntry = plugEntry;
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PlugModel
            {
                SocketId  = (ushort)PlotEntry.WorldSocketId,
                PlugId    = (ushort)PlugEntry.WorldIdPlug00,
                PlugFlags = 63
            };
        }
    }
}
