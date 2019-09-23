using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Game.Housing
{
    public class Plot : ISaveCharacter
    {
        public ulong Id { get; }
        public byte Index { get; }
        public Plug PlugEntity { get; private set; }
        public HousingPlotInfoEntry PlotEntry { get; }
        public DateTime BuildStartTime { get; private set; } = new DateTime(2018, 12, 1);

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
        public Plot(ResidencePlot model)
        {
            Id         = model.Id;
            Index      = model.Index;
            PlotEntry  = GameTableManager.HousingPlotInfo.GetEntry(model.PlotInfoId);
            plugEntry  = GameTableManager.HousingPlugItem.GetEntry(model.PlugItemId);
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
                context.Add(new ResidencePlot
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
                var model = new ResidencePlot
                {
                    Id = Id,
                    Index = Index
                };

                // could probably clean this up with reflection, works for the time being
                EntityEntry<ResidencePlot> entity = context.Attach(model);
                if ((saveMask & PlotSaveMask.PlugItemId) != 0)
                {
                    model.PlugItemId = (ushort)(PlugEntry?.Id ?? 0u);
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

        public void SetPlug(uint plugItemId, HousingPlugFacing plugFacing = HousingPlugFacing.East)
        {
            // TODO
            PlugEntry  = GameTableManager.HousingPlugItem.GetEntry(plugItemId);
            PlugFacing = plugFacing;

            // BuildState needs to be cleared to get rid of the plug entity properly
            BuildState = 0;
            BuildStartTime = DateTime.UtcNow;
        }

        public void RemovePlug()
        {
            PlugEntry = null;
            PlugEntity = null;
            PlugFacing = HousingPlugFacing.East;
            BuildState = 0;
        }

        public void SetPlugEntity(Plug plugEntity)
        {
            PlugEntity = plugEntity;
        }
    }
}
