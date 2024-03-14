using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Entity;
using NexusForever.Game.Housing;
using NexusForever.Game.Static.Housing;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Map
{
    public class ResidenceMapInstance : MapInstance, IResidenceMapInstance
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // housing maps have unlimited vision range.
        public override float? VisionRange { get; protected set; } = null;

        private readonly Dictionary<ulong, IResidence> residences = new();

        /// <summary>
        /// Initialise <see cref="IResidenceMapInstance"/> with <see cref="IResidence"/>.
        /// </summary>
        public void Initialise(IResidence residence)
        {
            AddResidence(residence);
            foreach (IResidenceChild childResidence in residence.GetChildren())
                AddResidence(childResidence.Residence);
        }

        private void AddResidence(IResidence residence)
        {
            residences.Add(residence.Id, residence);
            residence.Map = this;

            foreach (IPlot plot in residence.GetPlots()
                .Where(p => p.PlugItemEntry != null))
                AddPlugEntity(plot);
        }

        private void AddPlugEntity(IPlot plot)
        {
            var plug = new Plug(plot.PlotInfoEntry, plot.PlugItemEntry);
            plot.PlugEntity = plug;

            EnqueueAdd(plug, new MapPosition
            {
                Position = Vector3.Zero
            });
        }

        private void RemoveResidence(IResidence residence)
        {
            residences.Remove(residence.Id);
            residence.Map = null;

            foreach (IPlot plot in residence.GetPlots()
                .Where(p => p.PlugItemEntry != null))
                plot?.PlugEntity.RemoveFromMap();
        }

        protected override IMapPosition GetPlayerReturnLocation(IPlayer player)
        {
            // if the residence is unloaded return player to their own residence
            IResidence returnResidence = GlobalResidenceManager.Instance.GetResidenceByOwner(player.Name);
            returnResidence ??= GlobalResidenceManager.Instance.CreateResidence(player);
            IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(returnResidence.PropertyInfoId);

            return new MapPosition
            {
                Info   = new MapInfo
                {
                    Entry      = entrance.Entry,
                    InstanceId = returnResidence.Id
                },
                Position = entrance.Position
            };
        }

        protected override void AddEntity(IGridEntity entity, Vector3 vector)
        {
            base.AddEntity(entity, vector);
            if (entity is not IPlayer player)
                return;

            SendResidences(player);
            SendResidencePlots(player);
            SendResidenceDecor(player);

            // this shows the housing toolbar, might need to move this to a more generic place in the future
            player.Session.EnqueueMessageEncrypted(new ServerShowActionBar
            {
                ShortcutSet            = ShortcutSet.FloatingSpellBar,
                ActionBarShortcutSetId = 1553,
                Guid                   = player.Guid
            });
        }

        protected override void OnUnload()
        {
            foreach (IResidence residence in residences.Values.ToList())
                RemoveResidence(residence);
        }

        private void SendResidences(IPlayer player = null)
        {
            var housingProperties = new ServerHousingProperties();
            foreach (IResidence residence in residences.Values)
                housingProperties.Residences.Add(residence.Build());

            if (player != null)
                player.Session.EnqueueMessageEncrypted(housingProperties);
            else
                EnqueueToAll(housingProperties);
        }

        private void SendResidence(IResidence residence, IPlayer player = null)
        {
            var housingProperties = new ServerHousingProperties();
            housingProperties.Residences.Add(residence.Build());

            if (player != null)
                player.Session.EnqueueMessageEncrypted(housingProperties);
            else
                EnqueueToAll(housingProperties);
        }

        private void SendResidenceRemoved(IResidence residence, IPlayer player = null)
        {
            var housingProperties = new ServerHousingProperties();

            ServerHousingProperties.Residence residenceInfo = residence.Build();
            residenceInfo.ResidenceDeleted = true;
            housingProperties.Residences.Add(residenceInfo);

            if (player != null)
                player.Session.EnqueueMessageEncrypted(housingProperties);
            else
                EnqueueToAll(housingProperties);
        }

        private void SendResidencePlots(IPlayer player = null)
        {
            foreach (IResidence residence in residences.Values)
                SendResidencePlots(residence, player);
        }

        private void SendResidencePlots(IResidence residence, IPlayer player = null)
        {
            var housingPlots = new ServerHousingPlots
            {
                RealmId     = RealmContext.Instance.RealmId,
                ResidenceId = residence.Id
            };

            foreach (IPlot plot in residence.GetPlots())
            {
                housingPlots.Plots.Add(new ServerHousingPlots.Plot
                {
                    PlotPropertyIndex = plot.Index,
                    PlotInfoId        = plot.PlotInfoEntry.Id,
                    PlugFacing        = plot.PlugFacing,
                    PlugItemId        = plot.PlugItemEntry?.Id ?? 0u,
                    BuildState        = plot.BuildState
                });
            }

            if (player != null)
                player.Session.EnqueueMessageEncrypted(housingPlots);
            else
                EnqueueToAll(housingPlots);
        }

        private void SendResidenceDecor(IPlayer player = null)
        {
            // a separate ServerHousingResidenceDecor has to be used for each residence
            // the client uses the residence id from the first decor it receives as the storage for the rest as well
            // no idea why it was implemented like this...
            foreach (IResidence residence in residences.Values)
                SendResidenceDecor(residence, player);
        }

        private void SendResidenceDecor(IResidence residence, IPlayer player = null)
        {
            var residenceDecor = new ServerHousingResidenceDecor
            {
                Operation = 0
            };

            IDecor[] decors = residence.GetDecor().ToArray();
            for (uint i = 0u; i < decors.Length; i++)
            {
                IDecor decor = decors[i];
                residenceDecor.DecorData.Add(decor.Build());

                // client freaks out if too much decor is sent in a single message, limit to 100
                if (i == decors.Length - 1 || i != 0u && i % 100u == 0u)
                {
                    if (player != null)
                        player.Session.EnqueueMessageEncrypted(residenceDecor);
                    else
                        EnqueueToAll(residenceDecor);

                    residenceDecor.DecorData.Clear();
                }
            }
        }

        /// <summary>
        /// Add child <see cref="IResidence"/> to parent <see cref="IResidence"/>.
        /// </summary>
        public void AddChild(IResidence residence, bool temporary)
        {
            IResidence community = residences.Values.SingleOrDefault(r => r.IsCommunityResidence);
            if (community == null)
                throw new InvalidOperationException("Can't add child residence to a map that isn't a community!");

            community.AddChild(residence, temporary);
            AddResidence(residence);

            SendResidence(residence);
            SendResidencePlots(residence);
            SendResidenceDecor(residence);
        }

        /// <summary>
        /// Remove child <see cref="IResidence"/> to parent <see cref="IResidence"/>.
        /// </summary>
        public void RemoveChild(IResidence residence)
        {
            IResidence community = residences.Values.SingleOrDefault(r => r.IsCommunityResidence);
            if (community == null)
                throw new InvalidOperationException("Can't remove child residence from a map that isn't a community!");

            community.RemoveChild(residence);
            RemoveResidence(residence);

            SendResidenceRemoved(residence);
        }

        /// <summary>
        /// Crate all placed <see cref="IDecor"/>.
        /// </summary>
        public void CrateAllDecor(TargetResidence targetResidence, IPlayer player)
        {
            if (!residences.TryGetValue(targetResidence.ResidenceId, out IResidence residence)
                || !residence.CanModifyResidence(player))
                throw new InvalidPacketValueException();

            var housingResidenceDecor = new ServerHousingResidenceDecor();
            foreach (IDecor decor in residence.GetPlacedDecor())
            {
                decor.Crate();
                housingResidenceDecor.DecorData.Add(decor.Build());
            }

            EnqueueToAll(housingResidenceDecor);
        }

        /// <summary>
        /// Handle <see cref="IDecor"/> update (create, move or delete).
        /// </summary>
        public void DecorUpdate(IPlayer player, ClientHousingDecorUpdate housingDecorUpdate)
        {
            foreach (DecorInfo update in housingDecorUpdate.DecorUpdates)
            {
                if (!residences.TryGetValue(update.TargetResidence.ResidenceId, out IResidence residence)
                    || !residence.CanModifyResidence(player))
                    throw new InvalidPacketValueException();

                switch (housingDecorUpdate.Operation)
                {
                    case DecorUpdateOperation.Create:
                        DecorCreate(residence, player, update);
                        break;
                    case DecorUpdateOperation.Move:
                        DecorMove(residence, player, update);
                        break;
                    case DecorUpdateOperation.Delete:
                        DecorDelete(residence, update);
                        break;
                    default:
                        throw new InvalidPacketValueException();
                }
            }
        }

        /// <summary>
        /// Create and add <see cref="IDecor"/> from supplied <see cref="HousingDecorInfoEntry"/> to your crate.
        /// </summary>
        public void DecorCreate(IResidence residence, HousingDecorInfoEntry entry, uint quantity)
        {
            var residenceDecor = new ServerHousingResidenceDecor();
            for (uint i = 0u; i < quantity; i++)
            {
                IDecor decor = residence.DecorCreate(entry);
                residenceDecor.DecorData.Add(decor.Build());
            }

            EnqueueToAll(residenceDecor);
        }

        private void DecorCreate(IResidence residence, IPlayer player, DecorInfo update)
        {
            HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(update.DecorInfoId);
            if (entry == null)
                throw new InvalidPacketValueException();

            if (entry.CostCurrencyTypeId != 0u && entry.Cost != 0u)
            {
                /*if (!player.CurrencyManager.CanAfford((byte)entry.CostCurrencyTypeId, entry.Cost))
                {
                    // TODO: show error
                    return;
                }

                player.CurrencyManager.CurrencySubtractAmount((byte)entry.CostCurrencyTypeId, entry.Cost);*/
            }

            IDecor decor = residence.DecorCreate(entry);
            decor.Type = update.DecorType;
            decor.PlotIndex = update.PlotIndex;

            if (update.ColourShiftId != decor.ColourShiftId)
            {
                if (update.ColourShiftId != 0u)
                {
                    ColorShiftEntry colourEntry = GameTableManager.Instance.ColorShift.GetEntry(update.ColourShiftId);
                    if (colourEntry == null)
                        throw new InvalidPacketValueException();
                }

                decor.ColourShiftId = update.ColourShiftId;
            }

            if (update.DecorType != DecorType.Crate)
            {
                if (update.Scale < 0f)
                    throw new InvalidPacketValueException();

                // new decor is being placed directly in the world
                decor.Position = update.Position;
                decor.Rotation = update.Rotation;
                decor.Scale    = update.Scale;
            }

            EnqueueToAll(new ServerHousingResidenceDecor
            {
                Operation = 0,
                DecorData = new List<ServerHousingResidenceDecor.Decor>
                {
                     decor.Build()
                }
            });
        }

        private void DecorMove(IResidence residence, IPlayer player, DecorInfo update)
        {
            IDecor decor = residence.GetDecor(update.DecorId);
            if (decor == null)
                throw new InvalidPacketValueException();

            HousingResult GetResult()
            {
                if (!IsValidPlotForPosition(update))
                    return HousingResult.Decor_InvalidPosition;

                return HousingResult.Success;
            }

            HousingResult result = GetResult();
            if (result == HousingResult.Success)
            {
                if (update.PlotIndex != decor.PlotIndex)
                {
                    decor.PlotIndex = update.PlotIndex;
                }

                if (update.ColourShiftId != decor.ColourShiftId)
                {
                    if (update.ColourShiftId != 0u)
                    {
                        ColorShiftEntry colourEntry = GameTableManager.Instance.ColorShift.GetEntry(update.ColourShiftId);
                        if (colourEntry == null)
                            throw new InvalidPacketValueException();
                    }

                    decor.ColourShiftId = update.ColourShiftId;
                }

                if (decor.Type == DecorType.Crate)
                {
                    if (decor.Entry.Creature2IdActiveProp != 0u)
                    {
                        // TODO: used for decor that have an associated entity
                    }

                    // crate->world
                    decor.Move(update.DecorType, update.Position, update.Rotation, update.Scale, update.PlotIndex);
                }
                else
                {
                    if (update.DecorType == DecorType.Crate)
                        decor.Crate();
                    else
                    {
                        // world->world
                        decor.Move(update.DecorType, update.Position, update.Rotation, update.Scale, update.PlotIndex);
                        decor.DecorParentId = update.ParentDecorId;
                    }
                }
            }
            else
            {
                player.Session.EnqueueMessageEncrypted(new ServerHousingResult
                {
                    RealmId     = RealmContext.Instance.RealmId,
                    ResidenceId = residence.Id,
                    PlayerName  = player.Name,
                    Result      = result
                });
            }

            EnqueueToAll(new ServerHousingResidenceDecor
            {
                Operation = 0,
                DecorData = new List<ServerHousingResidenceDecor.Decor>
                {
                    decor.Build()
                }
            });
        }

        private void DecorDelete(IResidence residence, DecorInfo update)
        {
            IDecor decor = residence.GetDecor(update.DecorId);
            if (decor == null)
                throw new InvalidPacketValueException();

            if (decor.Position != Vector3.Zero)
                throw new InvalidOperationException();

            DecorDelete(residence, decor);
        }

        /// <summary>
        /// Remove an existing <see cref="IDecor"/> from <see cref="IResidence"/>.
        /// </summary>
        public void DecorDelete(IResidence residence, IDecor decor)
        {
            if (decor.PendingCreate)
                residence.DecorRemove(decor);
            else
                decor.EnqueueDelete(true);

            var residenceDecor = new ServerHousingResidenceDecor();
            residenceDecor.DecorData.Add(new ServerHousingResidenceDecor.Decor
            {
                RealmId     = RealmContext.Instance.RealmId,
                ResidenceId = residence.Id,
                DecorId     = decor.DecorId,
                DecorInfoId = 0
            });

            EnqueueToAll(residenceDecor);
        }

        /// <summary>
        /// Create a new <see cref="IDecor"/> from an existing <see cref="IDecor"/> for <see cref="IResidence"/>.
        /// </summary>
        /// <remarks>
        /// Copies all data from the source <see cref="IDecor"/> with a new id.
        /// </remarks>
        public void DecorCopy(IResidence residence, IDecor decor)
        {
            IDecor newDecor = residence.DecorCopy(decor);

            var residenceDecor = new ServerHousingResidenceDecor();
            residenceDecor.DecorData.Add(newDecor.Build());
            EnqueueToAll(residenceDecor);
        }

        /// <summary>
        /// Used to confirm the position and PlotIndex are valid together when placing Decor
        /// </summary>
        private bool IsValidPlotForPosition(DecorInfo update)
        {
            return true;

            /*if (update.PlotIndex == int.MaxValue)
                return true;

            WorldSocketEntry worldSocketEntry = GameTableManager.Instance.WorldSocket.GetEntry(residenceOld.GetPlot((byte)update.PlotIndex).PlotEntry.WorldSocketId);

            // TODO: Calculate position based on individual maps on Community & Warplot residences
            var worldPosition = new Vector3(1472f + update.Position.X, update.Position.Y, 1440f + update.Position.Z);

            (uint gridX, uint gridZ) = MapGrid.GetGridCoord(worldPosition);
            (uint localCellX, uint localCellZ) = MapCell.GetCellCoord(worldPosition);
            (uint globalCellX, uint globalCellZ) = (gridX * MapDefines.GridCellCount + localCellX, gridZ * MapDefines.GridCellCount + localCellZ);

            // TODO: Investigate need for offset.
            // Offset added due to calculation being +/- 1 sometimes when placing very close to plots. They were valid placements in the client, though.
            uint maxBound = worldSocketEntry.BoundIds.Max() + 1;
            uint minBound = worldSocketEntry.BoundIds.Min() - 1;

            log.Debug($"IsValidPlotForPosition - PlotIndex: {update.PlotIndex}, Range: {minBound}-{maxBound}, Coords: {globalCellX}, {globalCellZ}");

            return !(globalCellX >= minBound && globalCellX <= maxBound && globalCellZ >= minBound && globalCellZ <= maxBound);*/
        }

        /// <summary>
        /// Rename <see cref="IResidence"/> with supplied name.
        /// </summary>
        public void RenameResidence(IPlayer player, TargetResidence targetResidence, string name)
        {
            if (!residences.TryGetValue(targetResidence.ResidenceId, out IResidence residence)
                || !residence.CanModifyResidence(player))
                throw new InvalidPacketValueException();

            RenameResidence(residence, name);
        }

        /// <summary>
        /// Rename <see cref="IResidence"/> with supplied name.
        /// </summary>
        public void RenameResidence(IResidence residence, string name)
        {
            residence.Name = name;
            SendResidence(residence);
        }

        /// <summary>
        /// Remodel <see cref="IResidence"/>.
        /// </summary>
        public void Remodel(TargetResidence targetResidence, IPlayer player, ClientHousingRemodel housingRemodel)
        {
            if (!residences.TryGetValue(targetResidence.ResidenceId, out IResidence residence)
                || !residence.CanModifyResidence(player))
                throw new InvalidPacketValueException();

            if (housingRemodel.RoofDecorInfoId != 0u)
                residence.Roof = (ushort)housingRemodel.RoofDecorInfoId;
            if (housingRemodel.WallpaperId != 0u)
                residence.Wallpaper = (ushort)housingRemodel.WallpaperId;
            if (housingRemodel.EntrywayDecorInfoId != 0u)
                residence.Entryway = (ushort)housingRemodel.EntrywayDecorInfoId;
            if (housingRemodel.DoorDecorInfoId != 0u)
                residence.Door = (ushort)housingRemodel.DoorDecorInfoId;
            if (housingRemodel.SkyWallpaperId != 0u)
                residence.Sky = (ushort)housingRemodel.SkyWallpaperId;
            if (housingRemodel.MusicId != 0u)
                residence.Music = (ushort)housingRemodel.MusicId;
            if (housingRemodel.GroundWallpaperId != 0u)
                residence.Ground = (ushort)housingRemodel.GroundWallpaperId;

            SendResidences();
        }

        /// <summary>
        /// UpdateResidenceFlags <see cref="IResidence"/>.
        /// </summary>
        public void UpdateResidenceFlags(TargetResidence targetResidence, IPlayer player, ClientHousingFlagsUpdate flagsUpdate)
        {
            if (!residences.TryGetValue(targetResidence.ResidenceId, out IResidence residence)
                || !residence.CanModifyResidence(player))
                throw new InvalidPacketValueException();

            residence.Flags           = flagsUpdate.Flags;
            residence.ResourceSharing = flagsUpdate.ResourceSharing;
            residence.GardenSharing   = flagsUpdate.GardenSharing;

            SendResidences();
        }
    }
}
