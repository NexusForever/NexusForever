using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPlug : IWorldEntity
    {
        HousingPlotInfoEntry PlotEntry { get; }
        HousingPlugItemEntry PlugEntry { get; }
    }
}