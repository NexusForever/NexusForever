using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
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
        BuildState BuildState { get; set; }
        
        IPlugEntity PlugEntity { get; set; }

        /// <summary>
        /// Initialise a new <see cref="IPlot"/> from an existing database model.
        /// </summary>
        void Initialise(ResidencePlotModel model);

        /// <summary>
        /// Initialise a new <see cref="IPlot"/> from a <see cref="HousingPlotInfoEntry"/>.
        /// </summary>
        void Initialise(ulong id, HousingPlotInfoEntry entry);

        void SetPlug(ushort plugItemId);
    }
}