using System;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.TextFilter;
using NexusForever.WorldServer.Game.TextFilter.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Guild
{
    public partial class Guild
    {
        [GuildOperationHandler(GuildOperation.AdditionalInfo)]
        private GuildResultInfo GuildOperationAdditionalInfo(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildResultInfo GetResult()
            {
                if (member.Rank.Index > 0)
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildName)) 
                    return new GuildResultInfo(GuildResult.InvalidGuildInfo);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo result = GetResult();
            if (result.Result == GuildResult.Success)
            {
                AdditionalInfo = operation.TextValue;

                SendToOnlineUsers(new ServerGuildInfoMessageUpdate
                {
                    RealmId        = WorldServer.RealmId,
                    GuildId        = Id,
                    AdditionalInfo = AdditionalInfo
                });
            }

            return result;
        }

        [GuildOperationHandler(GuildOperation.MessageOfTheDay)]
        private GuildResultInfo GuildOperationMessageOfTheDay(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.MessageOfTheDay))
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildMessageOfTheDay))
                    return new GuildResultInfo(GuildResult.InvalidMessageOfTheDay);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo result = GetResult();
            if (result.Result == GuildResult.Success)
            {
                MessageOfTheDay = operation.TextValue;

                SendToOnlineUsers(new ServerGuildMotdUpdate
                {
                    RealmId         = WorldServer.RealmId,
                    GuildId         = Id,
                    MessageOfTheDay = MessageOfTheDay
                });
            }

            return result;
        }

        [GuildOperationHandler(GuildOperation.TaxUpdate)]
        private GuildResultInfo GuildOperationTaxUpdate(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildResultInfo GetResult()
            {
                if (member.Rank.Index > 0)
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo result = GetResult();
            if (result.Result == GuildResult.Success)
            {
                if (Convert.ToBoolean(operation.Data))
                    SetFlag(GuildFlag.Taxes);
                else
                    RemoveFlag(GuildFlag.Taxes);

                SendToOnlineUsers(new ServerGuildFlagUpdate
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = Id,
                    Value   = (uint)Flags
                });
            }

            return result;
        }
    }
}
