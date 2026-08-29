using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Static.Housing;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Housing
{
    public class Plot : IPlot
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IPlot"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum PlotSaveMask
        {
            None       = 0x0000,
            Create     = 0x0001,
            PlugItemId = 0x0002,
            PlugFacing = 0x0004,
            BuildState = 0x0008,
            PlotInfoId = 0x0010
        }

        public ulong Id { get; private set; }
        public byte Index { get; private set; }

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

        public BuildState BuildState
        {
            get => buildState;
            set
            {
                buildState = value;
                saveMask |= PlotSaveMask.BuildState;
            }
        }

        private BuildState buildState;

        private PlotSaveMask saveMask;

        public IPlugEntity PlugEntity { get; set; }

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public Plot(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Initialise a new <see cref="IPlot"/> from an existing database model.
        /// </summary>
        public void Initialise(ResidencePlotModel model)
        {
            Id            = model.Id;
            Index         = model.Index;
            plotInfoEntry = gameTableManager.HousingPlotInfo.GetEntry(model.PlotInfoId);
            plugItemEntry = gameTableManager.HousingPlugItem.GetEntry(model.PlugItemId);
            plugFacing    = model.PlugFacing;
            buildState    = model.BuildState;

            saveMask = PlotSaveMask.None;
        }

        /// <summary>
        /// Initialise a new <see cref="IPlot"/> from a <see cref="HousingPlotInfoEntry"/>.
        /// </summary>
        public void Initialise(ulong id, HousingPlotInfoEntry entry)
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
                    PlugFacing = PlugFacing,
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
                    model.PlugFacing = PlugFacing;
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
            BuildState = BuildState.InProgress;
        }
    }
}
