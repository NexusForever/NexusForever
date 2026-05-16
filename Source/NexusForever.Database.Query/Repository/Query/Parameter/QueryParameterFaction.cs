using System.Linq.Expressions;
using NexusForever.Database.Query.Model;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Database.Query.Repository.Query.Parameter
{
    public class QueryParameterFaction : IQueryParameter
    {
        public Faction Faction { get; set; }

        public Expression<Func<CharacterModel, bool>> Express()
        {
            return c => c.Faction == Faction;
        }
    }
}
