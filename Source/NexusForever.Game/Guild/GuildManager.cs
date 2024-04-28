using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NLog;
using NetworkGuildMember = NexusForever.Network.World.Message.Model.Shared.GuildMember;

namespace NexusForever.Game.Guild
{
    public class GuildManager : IGuildManager
    {
        [Flags]
        public enum SaveMask
        {
            None        = 0x00,
            Affiliation = 0x01
        }

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns the maximum number of guilds a <see cref="IPlayer"/> can be in by <see cref="GuildType"/>.
        /// </summary>
        private static uint GetMaximumGuildTypeCount(GuildType type)
        {
            // move to configuration file?
            switch (type)
            {
                case GuildType.Guild:
                case GuildType.WarParty:
                case GuildType.ArenaTeam2v2:
                case GuildType.ArenaTeam3v3:
                case GuildType.ArenaTeam5v5:
                case GuildType.Community:
                    return 1u;
                case GuildType.Circle:
                    return 5u;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns the <see cref="GuildResult"/> when <see cref="IPlayer"/> is at maximum number of guilds by <see cref="GuildType"/>.
        /// </summary>
        private static GuildResult GetMaximumGuildTypeError(GuildType type)
        {
            switch (type)
            {
                case GuildType.Guild:
                    return GuildResult.AtMaxGuildCount;
                case GuildType.WarParty:
                    return GuildResult.MaxWarPartyCount;
                case GuildType.ArenaTeam2v2:
                case GuildType.ArenaTeam3v3:
                case GuildType.ArenaTeam5v5:
                    return GuildResult.MaxArenaTeamCount;
                case GuildType.Community:
                    return GuildResult.AtMaxCommunityCount;
                case GuildType.Circle:
                    return GuildResult.AtMaxCircleCount;
                default:
                    throw new ArgumentException();
            }
        }

        public IGuild Guild { get; private set; }

        /// <summary>
        /// Current <see cref="IGuildBase"/> affiliation.
        /// </summary>
        /// <remarks>
        /// This determines which guild name and type is shown in the nameplate.
        /// </remarks>
        public IGuildBase GuildAffiliation
        {
            get => guildAffiliation;
            set
            {
                guildAffiliation = value;
                saveMask |= SaveMask.Affiliation;
            }
        }
        private IGuildBase guildAffiliation;

        private SaveMask saveMask;

        private readonly IPlayer owner;

        private readonly Dictionary<ulong, IGuildBase> guilds = new();
        private IGuildInvite pendingInvite;

        /// <summary>
        /// Create a new <see cref="IGuildManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public GuildManager(IPlayer player, CharacterModel model)
        {
            owner = player;

            foreach (IGuildBase guild in GlobalGuildManager.Instance.GetCharacterGuilds(owner.CharacterId))
            {
                if (guild.Type == GuildType.Guild)
                    Guild = guild as IGuild;

                guilds.Add(guild.Id, guild);
            }

            log.Trace($"Loaded {guilds.Count} guild(s) for character {owner.CharacterId}.");

            // check that the player is allowed to be affiliated with this guild
            // validation can fail if the player is removed from the guild or the guild is disbanded while offline
            if (model.GuildAffiliation != null)
                GuildAffiliation = guilds.TryGetValue(model.GuildAffiliation.Value, out IGuildBase guild) ? guild : guilds.Values.FirstOrDefault();
            else if (model.GuildAffiliation == null && guilds.Count > 0)
                GuildAffiliation = guilds.Values.FirstOrDefault();
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            // character is attached in Player::Save, this will only be local lookup
            CharacterModel character = context.Character.Find(owner.CharacterId);
            EntityEntry<CharacterModel> entity = context.Entry(character);

            if ((saveMask & SaveMask.Affiliation) != 0)
            {
                character.GuildAffiliation = guildAffiliation?.Id;
                entity.Property(p => p.GuildAffiliation).IsModified = true;
            }

            saveMask = SaveMask.None;
        }

        /// <summary>
        /// Return guild of supplied <see cref="GuildType"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="IPlayer"/> is part of multiple guilds of <see cref="GuildType"/>, the first one is returned.
        /// </remarks>
        public T GetGuild<T>(GuildType type) where T : IGuildBase
        {
            return (T)guilds.FirstOrDefault(g => g.Value.Type == type).Value;
        }

        /// <summary>
        /// Send initial packets and trigger login events for any <see cref="IGuildBase"/>'s for <see cref="IPlayer"/>.
        /// </summary>
        public void OnLogin()
        {
            SendGuildInitialise();
            if (Guild != null)
                UpdateHolomark();

            foreach (IGuildBase guild in guilds.Values)
                guild.OnPlayerLogin(owner);
        }

        private void SendGuildInitialise()
        {
            var guildInit = new ServerGuildInit();

            uint index = 0u;
            foreach (IGuildBase guild in guilds.Values)
            {
                NetworkGuildMember member = guild.GetMember(owner.CharacterId).Build();
                if (guildAffiliation?.Id == guild.Id)
                {
                    guildInit.NameplateIndex = index;
                    member.Unknown10 = 1; // TODO: research this
                }

                guildInit.Self.Add(member);
                guildInit.SelfPrivate.Add(new GuildPlayerLimits());
                guildInit.Guilds.Add(guild.Build());

                index++;
            }

            owner.Session.EnqueueMessageEncrypted(guildInit);
        }

        /// <summary>
        /// Trigger logout events for any <see cref="IGuildBase"/>'s for <see cref="IPlayer"/>.
        /// </summary>
        public void OnLogout()
        {
            foreach (IGuildBase guild in guilds.Values)
                guild.OnPlayerLogout(owner);
        }

        /// <summary>
        /// Returns if the supplied <see cref="ClientGuildRegister"/> information are valid to register a new guild.
        /// </summary>
        public IGuildResultInfo CanRegisterGuild(ClientGuildRegister guildRegister)
        {
            IGuildStandard standard = null;
            if (guildRegister.GuildType == GuildType.Guild)
                standard = new GuildStandard(guildRegister.GuildStandard);

            return CanRegisterGuild(guildRegister.GuildType, guildRegister.GuildName, guildRegister.MasterTitle,
                guildRegister.CouncilTitle, guildRegister.MasterTitle, standard);
        }

        /// <summary>
        /// Returns if the supplied <see cref="GuildType"/>, name, ranks and standard are valid to register a new guild.
        /// </summary>
        public IGuildResultInfo CanRegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, IGuildStandard standard = null)
        {
            if (!CanStoreGuildType(type))
                return new GuildResultInfo(GetMaximumGuildTypeError(type));

            if (!TextFilterManager.Instance.IsTextValid(name) || !TextFilterManager.Instance.IsTextValid(name, UserText.GuildName))
                return new GuildResultInfo(GuildResult.InvalidGuildName, referenceString: name);

            if (GlobalGuildManager.Instance.GetGuild(type, name) != null)
                return new GuildResultInfo(GuildResult.GuildNameUnavailable, referenceString: name);

            var rankNames = new List<string> { leaderRankName, councilRankName, memberRankName };
            foreach (string rankName in rankNames) 
                if (!TextFilterManager.Instance.IsTextValid(rankName) || !TextFilterManager.Instance.IsTextValid(rankName, UserText.GuildRankName))
                    return new GuildResultInfo(GuildResult.InvalidGuildName, referenceString: rankName);

            if (standard != null && !standard.Validate())
                return new GuildResultInfo(GuildResult.InvalidStandard);

            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Returns if a <see cref="IGuildBase"/> of the supplied <see cref="GuildType"/> can be stored.
        /// </summary>
        /// <remarks>
        /// This will return false if the owner is at cap for the supplied <see cref="GuildType"/>.
        /// </remarks>
        private bool CanStoreGuildType(GuildType type)
        {
            uint count = (uint)guilds.Count(g => g.Value.Type == type);
            return count < GetMaximumGuildTypeCount(type);
        }

        /// <summary>
        /// Register a new guild with the supplied <see cref="GuildType"/>, name, ranks and standard. 
        /// </summary>
        /// <remarks>
        /// <see cref="CanRegisterGuild(ClientGuildRegister)"/> should be invoked before invoking this method.
        /// </remarks>
        public void RegisterGuild(ClientGuildRegister guildRegister)
        {
            IGuildStandard standard = null;
            if (guildRegister.GuildType == GuildType.Guild)
                standard = new GuildStandard(guildRegister.GuildStandard);

            RegisterGuild(guildRegister.GuildType, guildRegister.GuildName, guildRegister.MasterTitle,
                guildRegister.CouncilTitle, guildRegister.MemberTitle, standard);
        }

        /// <summary>
        /// Register a new guild with the supplied <see cref="GuildType"/>, name, ranks and standard. 
        /// </summary>
        /// <remarks>
        /// <see cref="CanRegisterGuild(GuildType, string, string, string, string, IGuildStandard)"/> should be invoked before invoking this method.
        /// </remarks>
        public void RegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, IGuildStandard standard = null)
        {
            IGuildBase guild = GlobalGuildManager.Instance.RegisterGuild(type, name, leaderRankName, councilRankName, memberRankName, standard);
            JoinGuild(guild);
        }

        /// <summary>
        /// Return if <see cref="IPlayer"/> can be invited to the supplied guild.
        /// </summary>
        public IGuildResultInfo CanInviteToGuild(ulong id)
        {
            IGuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            if (!CanStoreGuildType(guild.Type))
                return new GuildResultInfo(GuildResult.CharacterCannotJoinMoreGuilds, referenceString: owner.Name);

            if (pendingInvite != null)
                return new GuildResultInfo(GuildResult.CharacterAlreadyHasAGuildInvite, referenceString: owner.Name);

            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Invite <see cref="IPlayer"/> to the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanInviteToGuild(ulong)"/> should be invoked before invoking this method.
        /// </remarks>
        public void InviteToGuild(ulong id, IPlayer invitee)
        {
            IGuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                throw new ArgumentException($"Invalid guild {id}!");

            pendingInvite = new GuildInvite
            {
                GuildId   = id,
                InviteeId = invitee.CharacterId
            };

            owner.Session.EnqueueMessageEncrypted(new ServerGuildInvite
            {
                GuildName  = guild.Name,
                GuildType  = guild.Type,
                PlayerName = invitee.Name,
                Flags      = (uint)guild.Flags
            });

            log.Trace($"Invited character {owner.CharacterId} to guild {id}.");
        }

        /// <summary>
        /// Return if <see cref="IPlayer"/> can accept the existing <see cref="IGuildInvite"/>.
        /// </summary>
        public IGuildResultInfo CanAcceptInviteToGuild()
        {
            if (pendingInvite == null)
                return new GuildResultInfo(GuildResult.NoPendingInvites);

            // TODO: check for expiry

            IGuildBase guild = GlobalGuildManager.Instance.GetGuild(pendingInvite.GuildId);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            return guild.CanJoinGuild(owner);
        }

        /// <summary>
        /// Accept existing <see cref="IGuildInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanAcceptInviteToGuild"/> should be invoked before invoking this method.
        /// </remarks>
        public void AcceptInviteToGuild(bool accepted)
        {
            if (pendingInvite == null)
                throw new InvalidOperationException($"Invalid guild invite for {owner.CharacterId}!");

            IPlayer invitee = PlayerManager.Instance.GetPlayer(pendingInvite.InviteeId);
            if (accepted)
            {
                if (invitee?.Session != null)
                    GuildBase.SendGuildResult(invitee.Session, GuildResult.InviteAccepted, referenceText: owner.Name);
                JoinGuild(pendingInvite.GuildId);
            }
            else
            {
                if (invitee?.Session != null)
                    GuildBase.SendGuildResult(invitee.Session, GuildResult.InviteDeclined, referenceText: owner.Name);
            }

            pendingInvite = null;
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the supplied guild.
        /// </summary>
        public IGuildResultInfo CanJoinGuild(ulong id)
        {
            IGuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            if (!CanStoreGuildType(guild.Type))
                return new GuildResultInfo(GetMaximumGuildTypeError(guild.Type));

            return guild.CanJoinGuild(owner);
        }

        /// <summary>
        /// Adds <see cref="IPlayer"/> the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoinGuild"/> should be invoked before invoking this method.
        /// </remarks>
        public void JoinGuild(ulong id)
        {
            IGuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                throw new ArgumentException($"Invalid guild {id}!");

            JoinGuild(guild);
        }

        private void JoinGuild(IGuildBase guild)
        {
            guilds.Add(guild.Id, guild);
            if (guild.Type == GuildType.Guild)
                Guild = guild as IGuild;

            if (GuildAffiliation == null)
                UpdateGuildAffiliation(guild.Id);

            guild.JoinGuild(owner);

            GlobalGuildManager.Instance.TrackCharacterGuild(owner.CharacterId, guild.Id);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can leave the supplied guild.
        /// </summary>
        public IGuildResultInfo CanLeaveGuild(ulong id)
        {
            IGuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            return guild.CanLeaveGuild(owner);
        }

        /// <summary>
        /// Removes <see cref="IPlayer"/> from the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanLeaveGuild(ulong)"/> should be invoked before invoking this method.
        /// </remarks>
        public void LeaveGuild(ulong id, GuildResult reason = GuildResult.MemberQuit)
        {
            IGuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                throw new ArgumentException($"Invalid guild {id}!");

            guild.LeaveGuild(owner, reason);

            guilds.Remove(guild.Id);
            if (guild.Type == GuildType.Guild)
                Guild = null;

            if (GuildAffiliation?.Id == guild.Id)
            {
                IGuildBase newAffiliation = guilds.Values.FirstOrDefault();
                if (newAffiliation != null)
                    UpdateGuildAffiliation(newAffiliation.Id);
                else
                    RemoveGuildAffiliation();
            }
        }

        /// <summary>
        /// Update current guild affiliation with supplied guild id.
        /// </summary>
        /// <remarks>
        /// If moving to or from a <see cref="IGuild"/>, the Holomark will also be updated or removed.
        /// </remarks>
        public void UpdateGuildAffiliation(ulong guildId)
        {
            if (!guilds.TryGetValue(guildId, out IGuildBase guild))
                throw new ArgumentException($"Invalid guild id {guildId} for character {owner.CharacterId}!");

            // update Holomark if our new affiliation is a guild
            if (guild.Type == GuildType.Guild)
                UpdateHolomark();
            // otherwise remove Holomark if the previous affiliation was a guild
            else if (GuildAffiliation?.Type == GuildType.Guild)
                RemoveHolomark();

            owner.EnqueueToVisible(new ServerEntityGuildAffiliation
            {
                UnitId    = owner.Guid,
                GuildName = guild.Name,
                GuildType = guild.Type
            }, true);

            GuildAffiliation = guild;
        }

        /// <summary>
        /// Remove existing guild affiliation.
        /// </summary>
        /// <remarks>
        /// If existing affiliation is a <see cref="IGuild"/>, the Holomark will also be removed.
        /// </remarks>
        private void RemoveGuildAffiliation()
        {
            if (GuildAffiliation == null)
                throw new InvalidOperationException($"Unable to remove guild affilation for character {owner.CharacterId}, no existing affilation!");

            if (GuildAffiliation.Type == GuildType.Guild)
                RemoveHolomark();

            owner.EnqueueToVisible(new ServerEntityGuildAffiliation
            {
                UnitId    = owner.Guid,
                GuildName = "",
                GuildType = GuildType.None
            }, true);

            GuildAffiliation = null;
        }

        /// <summary>
        /// Update Holomark positional data.
        /// </summary>
        public void UpdateHolomark(bool leftHidden, bool rightHidden, bool backHidden, bool distanceNear)
        {
            if (Guild == null)
                throw new InvalidOperationException($"Failed to update Holomark positional data for character {owner.CharacterId}!");

            CharacterFlag mask = owner.Flags;

            if (backHidden)
            {
                mask |= CharacterFlag.HolomarkHideBack;
                owner.RemoveVisual(ItemSlot.GuildStandardBack);
            }
            else
            {
                mask &= ~CharacterFlag.HolomarkHideBack;
                owner.AddVisual(ItemSlot.GuildStandardBack, (ushort)(distanceNear ? 7163 : 5580));
            }

            if (leftHidden)
            {
                mask |= CharacterFlag.HolomarkHideLeft;
                owner.RemoveVisual(ItemSlot.GuildStandardShoulderL);
            }
            else
            {
                mask &= ~CharacterFlag.HolomarkHideLeft;
                owner.AddVisual(ItemSlot.GuildStandardShoulderL, (ushort)(distanceNear ? 7164 : 5581));
            }

            if (rightHidden)
            {
                mask |= CharacterFlag.HolomarkHideRight;
                owner.RemoveVisual(ItemSlot.GuildStandardShoulderR);
            }
            else
            {
                mask &= ~CharacterFlag.HolomarkHideRight;
                owner.AddVisual(ItemSlot.GuildStandardShoulderR, (ushort)(distanceNear ? 7165 : 5582));
            }

            if (distanceNear)
                mask |= CharacterFlag.HolomarkNear;
            else
                mask &= ~CharacterFlag.HolomarkNear;

            // set flags directly and manually send message to prevent multiple messages through SetFlag and RemoveFlag
            owner.Flags = mask;
            owner.SendCharacterFlagsUpdated();
        }

        /// <summary>
        /// Update Holomark visual and positional data.
        /// </summary>
        public void UpdateHolomark()
        {
            if (Guild == null)
               throw new InvalidOperationException($"Failed to update Holomark visual data for character {owner.CharacterId}!");

            owner.AddVisual(ItemSlot.GuildStandardScanLines,      (ushort)Guild.Standard.ScanLines.GuildStandardPartEntry.ItemDisplayIdStandard);
            owner.AddVisual(ItemSlot.GuildStandardBackgroundIcon, (ushort)Guild.Standard.BackgroundIcon.GuildStandardPartEntry.ItemDisplayIdStandard);
            owner.AddVisual(ItemSlot.GuildStandardForegroundIcon, (ushort)Guild.Standard.ForegroundIcon.GuildStandardPartEntry.ItemDisplayIdStandard);
            owner.AddVisual(ItemSlot.GuildStandardChest,          5411);

            bool isNear = owner.HasFlag(CharacterFlag.HolomarkNear);
            if (!owner.HasFlag(CharacterFlag.HolomarkHideBack))
                owner.AddVisual(ItemSlot.GuildStandardBack, (ushort)(isNear ? 7163 : 5580));
            if (!owner.HasFlag(CharacterFlag.HolomarkHideLeft))
                owner.AddVisual(ItemSlot.GuildStandardShoulderL, (ushort)(isNear ? 7164 : 5581));
            if (!owner.HasFlag(CharacterFlag.HolomarkHideRight))
                owner.AddVisual(ItemSlot.GuildStandardShoulderR, (ushort)(isNear ? 7165 : 5582));
        }

        /// <summary>
        /// Remove Holomark visual and positional data.
        /// </summary>
        public void RemoveHolomark()
        {
            owner.RemoveVisual(ItemSlot.GuildStandardScanLines);
            owner.RemoveVisual(ItemSlot.GuildStandardBackgroundIcon);
            owner.RemoveVisual(ItemSlot.GuildStandardForegroundIcon);
            owner.RemoveVisual(ItemSlot.GuildStandardChest);
            owner.RemoveVisual(ItemSlot.GuildStandardBack);
            owner.RemoveVisual(ItemSlot.GuildStandardShoulderL);
            owner.RemoveVisual(ItemSlot.GuildStandardShoulderR);
        }

        public IEnumerator<IGuildBase> GetEnumerator()
        {
            return guilds.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
