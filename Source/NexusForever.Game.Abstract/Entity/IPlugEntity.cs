using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPlugEntity : IWorldEntity
    {
        HousingPlotInfoEntry PlotEntry { get; }
        HousingPlugItemEntry PlugEntry { get; }

        void Initialise(HousingPlotInfoEntry plotEntry, HousingPlugItemEntry plugEntry);
    }
}
