using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Game.Housing
{
    public class Plot : ISaveCharacter
    {
        public ulong Id { get; }
        public byte Index { get; }
        public HousingPlotInfoEntry PlotEntry { get; }

        public HousingPlugItemEntry PlugEntry
        {
            get => plugEntry;
            set
            {
                plugEntry = value;
                saveMask |= PlotSaveMask.PlugItemId;
            }
        }

        private HousingPlugItemEntry plugEntry;

        public HousingPlugFacing PlugFacing
        {
            get => plugFacing;
            set
            {
                plugFacing = value;
                saveMask |= PlotSaveMask.PlugFacing;
            }
        }

        private HousingPlugFacing plugFacing;

        public byte BuildState
        {
            get => buildState;
            set
            {
                buildState = value;
                saveMask |= PlotSaveMask.BuildState;
            }
        }

        private byte buildState;

        private PlotSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Plot"/> from an existing database model.
        /// </summary>
        public Plot(ResidencePlotModel model)
        {
            Id         = model.Id;
            Index      = model.Index;
            PlotEntry  = GameTableManager.Instance.HousingPlotInfo.GetEntry(model.PlotInfoId);
            PlugEntry  = GameTableManager.Instance.HousingPlugItem.GetEntry(model.PlugItemId);
            plugFacing = (HousingPlugFacing)model.PlugFacing;
            buildState = model.BuildState;
        }

        /// <summary>
        /// Create a new <see cref="Plot"/> from a <see cref="HousingPlotInfoEntry"/>.
        /// </summary>
        public Plot(ulong id, HousingPlotInfoEntry entry)
        {
            Id         = id;
            Index      = (byte)entry.HousingPropertyPlotIndex;
            PlotEntry      = entry;
            plugFacing = HousingPlugFacing.East;

            if (entry.HousingPlugItemIdDefault != 0u)
            {
                // TODO
                // plugItemId = entry.HousingPlugItemIdDefault;
            }

            saveMask = PlotSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == PlotSaveMask.None)
                return;

            if ((saveMask & PlotSaveMask.Create) != 0)
            {
                // plot doesn't exist in database, all infomation must be saved
                context.Add(new ResidencePlotModel
                {
                    Id         = Id,
                    Index      = Index,
                    PlotInfoId = (ushort)PlotEntry.Id,
                    PlugItemId = (ushort)(PlugEntry?.Id ?? 0u),
                    PlugFacing = (byte)PlugFacing,
                    BuildState = BuildState
                });
            }
            else
            {
                // plot already exists in database, save only data that has been modified
                // TODO
            }

            saveMask = PlotSaveMask.None;
        }

        public void SetPlug(ushort plugItemId)
        {
            // TODO
            PlugEntry  = GameTableManager.Instance.HousingPlugItem.GetEntry(plugItemId);
            BuildState = 4;
        }
    }
}
