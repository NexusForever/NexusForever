using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildRank : IDatabaseCharacter, IDatabaseState, INetworkBuildable<GuildRank>, IEnumerable<IGuildMember>
    {
        ulong GuildId { get; }
        byte Index { get; }
        string Name { get; set; }
        GuildRankPermission Permissions { get; set; }
        ulong BankPermissions { get; set; }
        ulong BankMoneyWithdrawlLimits { get; set; }
        ulong RepairLimit { get; set; }

        uint MemberCount { get; }

        /// <summary>
        /// Add a new <see cref="GuildRankPermission"/>.
        /// </summary>
        void AddPermission(GuildRankPermission guildRankPermission);

        /// <summary>
        /// Remove an existing <see cref="GuildRankPermission"/>.
        /// </summary>
        void RemovePermission(GuildRankPermission guildRankPermission);

        /// <summary>
        /// Returns if supplied <see cref="GuildRankPermission"/> exists.
        /// </summary>
        bool HasPermission(GuildRankPermission guildRankPermission);

        /// <summary>
        /// Add a new <see cref="IGuildMember"/>.
        /// </summary>
        void AddMember(IGuildMember member);

        /// <summary>
        /// Remove an existing <see cref="IGuildMember"/>
        /// </summary>
        void RemoveMember(IGuildMember member);
    }
}