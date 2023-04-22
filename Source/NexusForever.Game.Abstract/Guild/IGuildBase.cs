using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildBase : IDatabaseCharacter, INetworkBuildable<GuildData>, IEnumerable<IGuildMember>
    {
        ulong Id { get; }
        GuildType Type { get; }
        DateTime CreateTime { get; }
        string Name { get; set; }
        ulong? LeaderId { get; set; }
        GuildFlag Flags { get; set; }

        uint MemberCount { get; }

        /// <summary>
        /// Maximum number of <see cref="IGuildMember"/>'s allowed in the guild.
        /// </summary>
        uint MaxMembers { get; }

        /// <summary>
        /// Add a new <see cref="GuildFlag"/>.
        /// </summary>
        void SetFlag(GuildFlag flags);

        /// <summary>
        /// Remove an existing <see cref="GuildFlag"/>.
        /// </summary>
        void RemoveFlag(GuildFlag flags);

        /// <summary>
        /// Returns if supplied <see cref="GuildFlag"/> exists.
        /// </summary>
        bool HasFlags(GuildFlag flags);

        /// <summary>
        /// Trigger login events for <see cref="IPlayer"/> for <see cref="IGuildBase"/>.
        /// </summary>
        void OnPlayerLogin(IPlayer player);

        /// <summary>
        /// Trigger logout events for <see cref="IPlayer"/> for <see cref="IGuildBase"/>.
        /// </summary>
        void OnPlayerLogout(IPlayer player);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the <see cref="IGuildBase"/>.
        /// </summary>
        IGuildResultInfo CanJoinGuild(IPlayer player);

        /// <summary>
        /// Add a new <see cref="IPlayer"/> to the <see cref="IGuildBase"/>.
        /// </summary>
        void JoinGuild(IPlayer player);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can leave the <see cref="IGuildBase"/>.
        /// </summary>
        IGuildResultInfo CanLeaveGuild(IPlayer player);

        /// <summary>
        /// Remove an existing <see cref="IPlayer"/> from the <see cref="IGuildBase"/>.
        /// </summary>
        void LeaveGuild(IPlayer player, GuildResult reason);

        /// <summary>
        /// Disband <see cref="IGuildBase"/>.
        /// </summary>
        void DisbandGuild();

        /// <summary>
        /// Return <see cref="IGuildMember"/> with supplied character id.
        /// </summary>
        IGuildMember GetMember(ulong characterId);

        /// <summary>
        /// Return <see cref="IGuildMember"/> with supplied character name.
        /// </summary>
        IGuildMember GetMember(string memberName);

        /// <summary>
        /// Send <see cref="IWritable"/> to all online members.
        /// </summary>
        void Broadcast(IWritable writable);

        /// <summary>
        /// Rename <see cref="IGuildBase"/> with supplied name.
        /// </summary>
        void RenameGuild(string name);   
    }
}