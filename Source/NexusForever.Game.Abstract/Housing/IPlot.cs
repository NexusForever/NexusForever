using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Housing;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Housing
{
    public interface IPlot : IDatabaseCharacter
    {
        ulong Id { get; }
        byte Index { get; }
        HousingPlotInfoEntry PlotInfoEntry { get; set; }
        HousingPlugItemEntry PlugItemEntry { get; set; }
        HousingPlugFacing PlugFacing { get; set; }
        byte BuildState { get; set; }
        
        IPlugEntity PlugEntity { get; set; }
        
        void SetPlug(ushort plugItemId);
    }
}