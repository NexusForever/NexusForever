using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Guild
{
    public partial class Guild
    {
        [GuildOperationHandler(GuildOperation.AdditionalInfo)]
        private IGuildResultInfo GuildOperationAdditionalInfo(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildResultInfo GetResult()
            {
                if (member.Rank.Index > 0)
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildName)) 
                    return new GuildResultInfo(GuildResult.InvalidGuildInfo);

                return new GuildResultInfo(GuildResult.Success);
            }

            IGuildResultInfo result = GetResult();
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
        private IGuildResultInfo GuildOperationMessageOfTheDay(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.MessageOfTheDay))
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildMessageOfTheDay))
                    return new GuildResultInfo(GuildResult.InvalidMessageOfTheDay);

                return new GuildResultInfo(GuildResult.Success);
            }

            IGuildResultInfo result = GetResult();
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
        private IGuildResultInfo GuildOperationTaxUpdate(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildResultInfo GetResult()
            {
                if (member.Rank.Index > 0)
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                return new GuildResultInfo(GuildResult.Success);
            }

            IGuildResultInfo result = GetResult();
            if (result.Result == GuildResult.Success)
                SetTaxes(Convert.ToBoolean(operation.Data));

            return result;
        }
    }
}
