using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class PlugEntity : WorldEntity, IPlugEntity
    {
        public override EntityType Type => EntityType.Plug;

        public HousingPlotInfoEntry PlotEntry { get; private set; }
        public HousingPlugItemEntry PlugEntry { get; private set; }

        #region Dependency Injection

        public PlugEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        public void Initialise(HousingPlotInfoEntry plotEntry, HousingPlugItemEntry plugEntry)
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
