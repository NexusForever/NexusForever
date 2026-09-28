using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.PublicEvent;
using NexusForever.Game.Static.PublicEvent;

namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection
{
    public static class HycrestPublicEvent
    {
        public const uint Intro = 418u;
        public const uint Main  = 419u;

        /// <summary>
        /// Join <see cref="IPlayer"/> to the public team of <see cref="IPublicEvent"/>, unless already a member or the event has finished.
        /// </summary>
        /// <remarks>
        /// Joining the same character twice throws, and events created after a player entered the map are not joined automatically.
        /// </remarks>
        public static void JoinPublicTeam(IPublicEvent publicEvent, IPlayer player)
        {
            if (publicEvent == null || publicEvent.HasFinished)
                return;

            bool isMember = publicEvent.GetTeams()
                .SelectMany(t => t.GetMembers())
                .Any(m => m.CharacterId == player.CharacterId);
            if (isMember)
                return;

            publicEvent.JoinEvent(player, PublicEventTeam.PublicTeam);
        }
    }
}
