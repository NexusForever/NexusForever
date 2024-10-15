using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Map.Lock;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Guild
{
    public partial class Community
    {
        [GuildOperationHandler(GuildOperation.PlotReservation)]
        private IGuildResultInfo GuildOperationCommunityPlotReservation(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            if (!member.Rank.HasPermission(GuildRankPermission.ReserveCommunityPlot))
                return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

            if (operation.Data.Int32Data < -1 || operation.Data.Int32Data > 4)
                throw new InvalidPacketValueException();

            IResidence residence = player.ResidenceManager.Residence;
            if (residence == null)
                throw new InvalidPacketValueException();

            IResidenceChild sourceResidence = Residence.GetChild(member.CharacterId);

            if (operation.Data.Int32Data != -1)
            {
                IResidenceChild targetResidence = Residence.GetChild((PropertyInfoId)(100 + operation.Data.Int32Data));
                if (targetResidence == null)
                {
                    IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance((PropertyInfoId)(100 + operation.Data.Int32Data));
                    if (entrance == null)
                        throw new InvalidPacketValueException();

                    if (sourceResidence != null)
                    {
                        // current plot reservation must be temporary to reserve a new plot
                        if (!sourceResidence.IsTemporary)
                            throw new InvalidPacketValueException();

                        // for residences on a community just remove the residence
                        // any players on the map at the time can stay in the instance
                        if (residence.Map != null)
                            residence.Map.RemoveChild(residence);
                        else
                            residence.Parent.RemoveChild(residence);

                        IMapLock mapLock = MapLockManager.Instance.GetResidenceLock(Residence);

                        player.Rotation = entrance.Rotation.ToEuler();
                        player.TeleportTo(entrance.Entry, entrance.Position, mapLock);
                    }
                    else
                    {
                        IMapLock mapLock = MapLockManager.Instance.GetResidenceLock(Residence);

                        // move owner to new instance only if not on the same instance as the residence
                        // otherwise they will be moved to the new instance during the unload
                        if (residence.Map != player.Map)
                        {
                            player.Rotation = entrance.Rotation.ToEuler();
                            player.TeleportTo(entrance.Entry, entrance.Position, mapLock);
                        }

                        // for individual residences remove the entire instance
                        // move any players on the map at the time to the community
                        residence.Map?.Unload(new MapPosition
                        {
                            Info = new MapInfo
                            {
                                Entry   = entrance.Entry,
                                MapLock = mapLock,
                            },
                            Position = entrance.Position
                        });
                    }

                    // update residence with new plot location and add to community
                    residence.PropertyInfoId = (PropertyInfoId)(100 + operation.Data.Int32Data);

                    if (Residence.Map != null)
                        Residence.Map.AddChild(residence, false);
                    else
                        Residence.AddChild(residence, false);
                }
                else
                {
                    // can only remove reservation if one already exists
                    if (targetResidence != sourceResidence)
                        throw new InvalidPacketValueException();

                    targetResidence.IsTemporary = false;
                }
            }
            else
            {
                // can only remove reservation if one already exists
                if (sourceResidence == null)
                    throw new InvalidPacketValueException();

                // removing the reservation does not remove the plot only removes the permanent status
                sourceResidence.IsTemporary = true;
            }

            member.CommunityPlotReservation = operation.Data.Int32Data;
            AnnounceGuildMemberChange(member);

            return new GuildResultInfo(GuildResult.Success);
        }

        // This operation is slightly different to the removal above, it is used when removing the reservation of another player 
        [GuildOperationHandler(GuildOperation.PlotReservationRemoval)]
        private IGuildResultInfo GuildOperationCommunityPlotReservationRemoval(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            if (!member.Rank.HasPermission(GuildRankPermission.RemoveCommunityPlotReservation))
                return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

            IGuildMember targetMember = GetMember(operation.TextValue);
            if (targetMember == null)
                throw new InvalidPacketValueException();

            IResidenceChild child = Residence.GetChild(targetMember.CharacterId);
            if (child == null)
                throw new InvalidPacketValueException();

            // removing the reservation does not remove the plot only removes the permanent status
            child.IsTemporary = true;

            targetMember.CommunityPlotReservation = -1;
            AnnounceGuildMemberChange(targetMember);

            return new GuildResultInfo(GuildResult.Success);
        }
    }
}
