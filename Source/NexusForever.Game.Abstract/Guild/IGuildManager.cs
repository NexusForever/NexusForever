using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuildManager : IDatabaseCharacter, IEnumerable<IGuildBase>
    {
        IGuild Guild { get; }

        /// <summary>
        /// Current <see cref="IGuildBase"/> affiliation.
        /// </summary>
        /// <remarks>
        /// This determines which guild name and type is shown in the nameplate.
        /// </remarks>
        IGuildBase GuildAffiliation { get; set; }

        /// <summary>
        /// Return guild of supplied <see cref="GuildType"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="IPlayer"/> is part of multiple guilds of <see cref="GuildType"/>, the first one is returned.
        /// </remarks>
        T GetGuild<T>(GuildType type) where T : IGuildBase;

        /// <summary>
        /// Send initial packets and trigger login events for any <see cref="IGuildBase"/>'s for <see cref="IPlayer"/>.
        /// </summary>
        void OnLogin();

        /// <summary>
        /// Trigger logout events for any <see cref="IGuildBase"/>'s for <see cref="IPlayer"/>.
        /// </summary>
        void OnLogout();

        /// <summary>
        /// Returns if the supplied <see cref="ClientGuildRegister"/> information are valid to register a new guild.
        /// </summary>
        IGuildResultInfo CanRegisterGuild(ClientGuildRegister guildRegister);

        /// <summary>
        /// Returns if the supplied <see cref="GuildType"/>, name, ranks and standard are valid to register a new guild.
        /// </summary>
        IGuildResultInfo CanRegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, IGuildStandard standard = null);

        /// <summary>
        /// Register a new guild with the supplied <see cref="GuildType"/>, name, ranks and standard. 
        /// </summary>
        /// <remarks>
        /// <see cref="CanRegisterGuild(ClientGuildRegister)"/> should be invoked before invoking this method.
        /// </remarks>
        void RegisterGuild(ClientGuildRegister guildRegister);

        /// <summary>
        /// Register a new guild with the supplied <see cref="GuildType"/>, name, ranks and standard. 
        /// </summary>
        /// <remarks>
        /// <see cref="CanRegisterGuild(GuildType, string, string, string, string, IGuildStandard)"/> should be invoked before invoking this method.
        /// </remarks>
        void RegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, IGuildStandard standard = null);

        /// <summary>
        /// Return if <see cref="IPlayer"/> can be invited to the supplied guild.
        /// </summary>
        IGuildResultInfo CanInviteToGuild(ulong id);

        /// <summary>
        /// Invite <see cref="IPlayer"/> to the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanInviteToGuild(ulong)"/> should be invoked before invoking this method.
        /// </remarks>
        void InviteToGuild(ulong id, IPlayer invitee);

        /// <summary>
        /// Return if <see cref="IPlayer"/> can accept the existing <see cref="IGuildInvite"/>.
        /// </summary>
        IGuildResultInfo CanAcceptInviteToGuild();

        /// <summary>
        /// Accept existing <see cref="IGuildInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanAcceptInviteToGuild"/> should be invoked before invoking this method.
        /// </remarks>
        void AcceptInviteToGuild(bool accepted);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the supplied guild.
        /// </summary>
        IGuildResultInfo CanJoinGuild(ulong id);

        /// <summary>
        /// Adds <see cref="IPlayer"/> the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoinGuild"/> should be invoked before invoking this method.
        /// </remarks>
        void JoinGuild(ulong id);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can leave the supplied guild.
        /// </summary>
        IGuildResultInfo CanLeaveGuild(ulong id);

        /// <summary>
        /// Removes <see cref="IPlayer"/> from the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanLeaveGuild(ulong)"/> should be invoked before invoking this method.
        /// </remarks>
        void LeaveGuild(ulong id, GuildResult reason = GuildResult.MemberQuit);

        /// <summary>
        /// Update current guild affiliation with supplied guild id.
        /// </summary>
        /// <remarks>
        /// If moving to or from a <see cref="IGuild"/>, the Holomark will also be updated or removed.
        /// </remarks>
        void UpdateGuildAffiliation(ulong guildId);

        /// <summary>
        /// Update Holomark positional data.
        /// </summary>
        void UpdateHolomark(bool leftHidden, bool rightHidden, bool backHidden, bool distanceNear);

        /// <summary>
        /// Update Holomark visual and positional data.
        /// </summary>
        void UpdateHolomark();

        /// <summary>
        /// Remove Holomark visual and positional data.
        /// </summary>
        void RemoveHolomark();
    }
}