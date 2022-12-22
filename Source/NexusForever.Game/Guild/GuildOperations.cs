using NexusForever.Game.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.TextFilter;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Guild
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

                Broadcast(new ServerGuildInfoMessageUpdate
                {
                    RealmId        = RealmContext.Instance.RealmId,
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

                Broadcast(new ServerGuildMotdUpdate
                {
                    RealmId         = RealmContext.Instance.RealmId,
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
                SetTaxes(Convert.ToBoolean(operation.Data));

            return result;
        }
    }
}
