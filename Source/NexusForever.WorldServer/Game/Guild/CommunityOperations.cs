using NexusForever.Shared;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Guild
{ 
    public partial class Community
    {
        [GuildOperationHandler(GuildOperation.PlotReservation)]
        private GuildResultInfo GuildOperationCommunityPlotReservation(GuildMember member, Player player, ClientGuildOperation operation)
        {
            if (!member.Rank.HasPermission(GuildRankPermission.ReserveCommunityPlot))
                return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

            if (operation.Data.Int32Data < -1 || operation.Data.Int32Data > 4)
                throw new InvalidPacketValueException();

            Residence residence = player.ResidenceManager.Residence;
            if (residence == null)
                throw new InvalidPacketValueException();

            ResidenceChild sourceResidence = Residence.GetChild(member.CharacterId);

            if (operation.Data.Int32Data != -1)
            {
                ResidenceChild targetResidence = Residence.GetChild((PropertyInfoId)(100 + operation.Data.Int32Data));
                if (targetResidence == null)
                {
                    ResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance((PropertyInfoId)(100 + operation.Data.Int32Data));
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

                        player.Rotation = entrance.Rotation.ToEulerDegrees();
                        player.TeleportTo(entrance.Entry, entrance.Position, Residence.Id);
                    }
                    else
                    {
                        // move owner to new instance only if not on the same instance as the residence
                        // otherwise they will be moved to the new instance during the unload
                        if (residence.Map != player.Map)
                        {
                            player.Rotation = entrance.Rotation.ToEulerDegrees();
                            player.TeleportTo(entrance.Entry, entrance.Position, Residence.Id);
                        }

                        // for individual residences remove the entire instance
                        // move any players on the map at the time to the community
                        residence.Map?.Unload(new MapPosition
                        {
                            Info = new MapInfo
                            {
                                Entry      = entrance.Entry,
                                InstanceId = Residence.Id,
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
        private GuildResultInfo GuildOperationCommunityPlotReservationRemoval(GuildMember member, Player player, ClientGuildOperation operation)
        {
            if (!member.Rank.HasPermission(GuildRankPermission.RemoveCommunityPlotReservation))
                return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

            GuildMember targetMember = GetMember(operation.TextValue);
            if (targetMember == null)
                throw new InvalidPacketValueException();

            ResidenceChild child = Residence.GetChild(targetMember.CharacterId);
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
