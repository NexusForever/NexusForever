using Microsoft.Extensions.Options;
using NexusForever.Database.Query.Repository.Query;
using NexusForever.Database.Query.Repository.Query.Parameter;
using NexusForever.Network.Internal.Message.Who;
using NexusForever.Network.Internal.Message.Who.Parameter;
using NexusForever.Server.Character.Configuration;

namespace NexusForever.Server.Character.Game.Who
{
    public class QueryBuilder
    {
        #region Dependency Injection

        private readonly WhoOptions _options;

        public QueryBuilder(
            IOptions<WhoOptions> options)
        {
            _options = options.Value;
        }

        #endregion

        public Query Build(WhoRequestMessage request)
        {
            var query = new Query
            {
                MaxResults = _options.MaxResults
            };

            int offset = 0;
            foreach (int groupOffset in request.ParameterGroupCounts)
            {
                IEnumerable<IWhoParameter> parmeters = request.Parameters
                    .Skip(offset)
                    .Take(groupOffset - offset);

                offset = groupOffset;

                query.Groups.Add(BuildQueryGroup(parmeters));
            }

            IEnumerable<IWhoParameter> finalParameters = request.Parameters.Skip(offset);
            query.Groups.Add(BuildQueryGroup(finalParameters));

            return query;
        }

        private static QueryGroup BuildQueryGroup(IEnumerable<IWhoParameter> parameters)
        {
            var queryGroup = new QueryGroup();

            foreach (IWhoParameter parameter in parameters)
            {
                IQueryParameter queryParameter = parameter switch
                {
                    WhoParameterCombo combo     => combo.ToQueryParameter(),
                    WhoParameterClass @class    => @class.ToQueryParameter(),
                    WhoParameterFaction faction => faction.ToQueryParameter(),
                    WhoParameterGuild guild     => guild.ToQueryParameter(),
                    WhoParameterLevel level     => level.ToQueryParameter(),
                    WhoParameterPath path       => path.ToQueryPararmeter(),
                    WhoParameterPlayer player   => player.ToQueryParameter(),
                    WhoParameterRace race       => race.ToQueryParameter(),
                    WhoParameterZone zone       => zone.ToQueryParameter(),
                    _                           => throw new NotImplementedException()
                };

                queryGroup.Parameters.Add(queryParameter);
            }

            return queryGroup;
        }
    }
}
