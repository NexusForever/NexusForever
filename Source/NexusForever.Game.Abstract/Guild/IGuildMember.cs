using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Guild;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildMember : IDatabaseCharacter, IDatabaseState, INetworkBuildable<GuildMember>
    {
        IGuildBase Guild { get; }
        public Identity PlayerIdentity { get; }
        public ulong CharacterId { get => PlayerIdentity.Id; }
        IGuildRank Rank { get; set; }
        string Note { get; set; }
        int CommunityPlotReservation { get; set; }
    }
}