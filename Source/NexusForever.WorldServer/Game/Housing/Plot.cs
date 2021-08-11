using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Game.Housing
{
    public class Plot : ISaveCharacter
    {
        public ulong Id { get; }
        public byte Index { get; }

        public HousingPlotInfoEntry PlotInfoEntry
        {
            get => plotInfoEntry;
            set
            {
                plotInfoEntry = value;
                saveMask |= PlotSaveMask.PlotInfoId;
            }
        }

        private HousingPlotInfoEntry plotInfoEntry;

        public HousingPlugItemEntry PlugItemEntry
        {
            get => plugItemEntry;
            set
            {
                plugItemEntry = value;
                saveMask |= PlotSaveMask.PlugItemId;
            }
        }

        private HousingPlugItemEntry plugItemEntry;

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

        public Plug PlugEntity { get; set; }

        /// <summary>
        /// Create a new <see cref="Plot"/> from an existing database model.
        /// </summary>
        public Plot(ResidencePlotModel model)
        {
            Id            = model.Id;
            Index         = model.Index;
            plotInfoEntry = GameTableManager.Instance.HousingPlotInfo.GetEntry(model.PlotInfoId);
            plugItemEntry = GameTableManager.Instance.HousingPlugItem.GetEntry(model.PlugItemId);
            plugFacing    = (HousingPlugFacing)model.PlugFacing;
            buildState    = model.BuildState;

            saveMask = PlotSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Plot"/> from a <see cref="HousingPlotInfoEntry"/>.
        /// </summary>
        public Plot(ulong id, HousingPlotInfoEntry entry)
        {
            Id            = id;
            Index         = (byte)entry.HousingPropertyPlotIndex;
            plotInfoEntry = entry;
            plugFacing    = HousingPlugFacing.East;

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
                    PlotInfoId = (ushort)PlotInfoEntry.Id,
                    PlugItemId = (ushort)(PlugItemEntry?.Id ?? 0u),
                    PlugFacing = (byte)PlugFacing,
                    BuildState = BuildState
                });
            }
            else
            {
                // plot already exists in database, save only data that has been modified
                var model = new ResidencePlotModel
                {
                    Id    = Id,
                    Index = Index,
                };

                EntityEntry<ResidencePlotModel> entity = context.Attach(model);
                if ((saveMask & PlotSaveMask.PlotInfoId) != 0)
                {
                    model.PlotInfoId = (ushort)PlotInfoEntry.Id;
                    entity.Property(p => p.PlotInfoId).IsModified = true;
                }

                if ((saveMask & PlotSaveMask.PlugItemId) != 0)
                {
                    model.PlugItemId = (ushort)PlugItemEntry.Id;
                    entity.Property(p => p.PlugItemId).IsModified = true;
                }

                if ((saveMask & PlotSaveMask.PlugFacing) != 0)
                {
                    model.PlugFacing = (byte)PlugFacing;
                    entity.Property(p => p.PlugFacing).IsModified = true;
                }

                if ((saveMask & PlotSaveMask.BuildState) != 0)
                {
                    model.BuildState = BuildState;
                    entity.Property(p => p.BuildState).IsModified = true;
                }
            }

            saveMask = PlotSaveMask.None;
        }

        public void SetPlug(ushort plugItemId)
        {
            // TODO
            PlugItemEntry  = GameTableManager.Instance.HousingPlugItem.GetEntry(plugItemId);
            BuildState = 4;
        }
    }
}
