using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.TextFilter;
using NexusForever.WorldServer.Game.TextFilter.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using NetworkGuildMember = NexusForever.WorldServer.Network.Message.Model.Shared.GuildMember;

namespace NexusForever.WorldServer.Game.Guild
{
    public class GuildManager : ISaveCharacter, IEnumerable<GuildBase>
    {
        [Flags]
        public enum SaveMask
        {
            None        = 0x00,
            Affiliation = 0x01
        }

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns the maximum number of guilds a <see cref="Player"/> can be in by <see cref="GuildType"/>.
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
        /// Returns the <see cref="GuildResult"/> when <see cref="Player"/> is at maximum number of guilds by <see cref="GuildType"/>.
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

        public Guild Guild { get; private set; }

        /// <summary>
        /// Current <see cref="GuildBase"/> affiliation.
        /// </summary>
        /// <remarks>
        /// This determines which guild name and type is shown in the nameplate.
        /// </remarks>
        public GuildBase GuildAffiliation
        {
            get => guildAffiliation;
            set
            {
                guildAffiliation = value;
                saveMask |= SaveMask.Affiliation;
            }
        }
        private GuildBase guildAffiliation;

        private SaveMask saveMask;

        private readonly Player owner;

        private readonly Dictionary<ulong, GuildBase> guilds = new Dictionary<ulong, GuildBase>();
        private GuildInvite pendingInvite;

        /// <summary>
        /// Create a new <see cref="GuildManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public GuildManager(Player player, CharacterModel model)
        {
            owner = player;

            foreach (GuildBase guild in GlobalGuildManager.Instance.GetCharacterGuilds(owner.CharacterId))
            {
                if (guild.Type == GuildType.Guild)
                    Guild = guild as Guild;

                guilds.Add(guild.Id, guild);
            }

            log.Trace($"Loaded {guilds.Count} guild(s) for character {owner.CharacterId}.");

            // check that the player is allowed to be affiliated with this guild
            // validation can fail if the player is removed from the guild or the guild is disbanded while offline
            if (model.GuildAffiliation != null)
                GuildAffiliation = guilds.TryGetValue(model.GuildAffiliation.Value, out GuildBase guild) ? guild : guilds.Values.FirstOrDefault();
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
        /// Send initial packets and trigger login events for any <see cref="GuildBase"/>'s for <see cref="Player"/>.
        /// </summary>
        public void OnLogin()
        {
            SendGuildInitialise();
            if (Guild != null)
                UpdateHolomark();

            foreach (GuildBase guild in guilds.Values)
                guild.OnPlayerLogin(owner);
        }

        private void SendGuildInitialise()
        {
            var guildInit = new ServerGuildInit();

            uint index = 0u;
            foreach (GuildBase guild in guilds.Values)
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
        /// Trigger logout events for any <see cref="GuildBase"/>'s for <see cref="Player"/>.
        /// </summary>
        public void OnLogout()
        {
            foreach (GuildBase guild in guilds.Values)
                guild.OnPlayerLogout(owner);
        }

        /// <summary>
        /// Returns if the supplied <see cref="ClientGuildRegister"/> information are valid to register a new guild.
        /// </summary>
        public GuildResultInfo CanRegisterGuild(ClientGuildRegister guildRegister)
        {
            GuildStandard standard = null;
            if (guildRegister.GuildType == GuildType.Guild)
                standard = new GuildStandard(guildRegister.GuildStandard);

            return CanRegisterGuild(guildRegister.GuildType, guildRegister.GuildName, guildRegister.MasterTitle,
                guildRegister.CouncilTitle, guildRegister.MasterTitle, standard);
        }

        /// <summary>
        /// Returns if the supplied <see cref="GuildType"/>, name, ranks and standard are valid to register a new guild.
        /// </summary>
        public GuildResultInfo CanRegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, GuildStandard standard = null)
        {
            if (!CanStoreGuildType(type))
                return new GuildResultInfo(GetMaximumGuildTypeError(type));

            if (!TextFilterManager.Instance.IsTextValid(name) || !TextFilterManager.Instance.IsTextValid(name, UserText.GuildName))
                return new GuildResultInfo(GuildResult.InvalidGuildName, referenceString: name);

            if (GlobalGuildManager.Instance.GetGuild(name) != null)
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
        /// Returns if a <see cref="GuildBase"/> of the supplied <see cref="GuildType"/> can be stored.
        /// </summary>
        /// <remarks>
        /// This will return false if the owner is at cap for the supplied <see cref="GuildType"/>.
        /// </remarks>
        private bool CanStoreGuildType(GuildType type)
        {
            uint count = (uint)guilds.Count(g => g.Value.Type == type);
            return GetMaximumGuildTypeCount(type) < count;
        }

        /// <summary>
        /// Register a new guild with the supplied <see cref="GuildType"/>, name, ranks and standard. 
        /// </summary>
        /// <remarks>
        /// <see cref="CanRegisterGuild(ClientGuildRegister)"/> should be invoked before invoking this method.
        /// </remarks>
        public void RegisterGuild(ClientGuildRegister guildRegister)
        {
            GuildStandard standard = null;
            if (guildRegister.GuildType == GuildType.Guild)
                standard = new GuildStandard(guildRegister.GuildStandard);

            RegisterGuild(guildRegister.GuildType, guildRegister.GuildName, guildRegister.MasterTitle,
                guildRegister.CouncilTitle, guildRegister.MemberTitle, standard);
        }

        /// <summary>
        /// Register a new guild with the supplied <see cref="GuildType"/>, name, ranks and standard. 
        /// </summary>
        /// <remarks>
        /// <see cref="CanRegisterGuild(GuildType, string, string, string, string, GuildStandard)"/> should be invoked before invoking this method.
        /// </remarks>
        public void RegisterGuild(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName, GuildStandard standard = null)
        {
            GuildBase guild = GlobalGuildManager.Instance.RegisterGuild(type, name, leaderRankName, councilRankName, memberRankName, standard);
            JoinGuild(guild);
        }

        /// <summary>
        /// Return if <see cref="Player"/> can be invited to the supplied guild.
        /// </summary>
        public GuildResultInfo CanInviteToGuild(ulong id)
        {
            GuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            if (CanStoreGuildType(guild.Type))
                return new GuildResultInfo(GuildResult.CharacterCannotJoinMoreGuilds, referenceString: owner.Name);

            if (pendingInvite != null)
                return new GuildResultInfo(GuildResult.CharacterAlreadyHasAGuildInvite, referenceString: owner.Name);

            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Invite <see cref="Player"/> to the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanInviteToGuild(ulong)"/> should be invoked before invoking this method.
        /// </remarks>
        public void InviteToGuild(ulong id, Player invitee)
        {
            GuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
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
            });;

            log.Trace($"Invited character {owner.CharacterId} to guild {id}.");
        }

        /// <summary>
        /// Return if <see cref="Player"/> can accept the existing <see cref="GuildInvite"/>.
        /// </summary>
        public GuildResultInfo CanAcceptInviteToGuild()
        {
            if (pendingInvite == null)
                return new GuildResultInfo(GuildResult.NoPendingInvites);

            // TODO: check for expiry

            GuildBase guild = GlobalGuildManager.Instance.GetGuild(pendingInvite.GuildId);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            return guild.CanJoinGuild(owner);
        }

        /// <summary>
        /// Accept existing <see cref="GuildInvite"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanAcceptInviteToGuild"/> should be invoked before invoking this method.
        /// </remarks>
        public void AcceptInviteToGuild(bool accepted)
        {
            if (pendingInvite == null)
                throw new InvalidOperationException($"Invalid guild invite for {owner.CharacterId}!");

            Player invitee = CharacterManager.Instance.GetPlayer(pendingInvite.InviteeId);
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
        /// Returns if <see cref="Player"/> can join the supplied guild.
        /// </summary>
        public GuildResultInfo CanJoinGuild(ulong id)
        {
            GuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            if (!CanStoreGuildType(guild.Type))
                return new GuildResultInfo(GetMaximumGuildTypeError(guild.Type));

            return guild.CanJoinGuild(owner);
        }

        /// <summary>
        /// Adds <see cref="Player"/> the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoinGuild"/> should be invoked before invoking this method.
        /// </remarks>
        public void JoinGuild(ulong id)
        {
            GuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                throw new ArgumentException($"Invalid guild {id}!");

            JoinGuild(guild);
        }

        private void JoinGuild(GuildBase guild)
        {
            guilds.Add(guild.Id, guild);
            if (guild.Type == GuildType.Guild)
                Guild = guild as Guild;

            if (GuildAffiliation == null)
                UpdateGuildAffiliation(guild.Id);

            guild.JoinGuild(owner);

            GlobalGuildManager.Instance.TrackCharacterGuild(owner.CharacterId, guild.Id);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can leave the supplied guild.
        /// </summary>
        public GuildResultInfo CanLeaveGuild(ulong id)
        {
            GuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                return new GuildResultInfo(GuildResult.NotAGuild);

            return guild.CanLeaveGuild(owner);
        }

        /// <summary>
        /// Removes <see cref="Player"/> from the supplied guild.
        /// </summary>
        /// <remarks>
        /// <see cref="CanLeaveGuild(ulong)"/> should be invoked before invoking this method.
        /// </remarks>
        public void LeaveGuild(ulong id, GuildResult reason = GuildResult.MemberQuit)
        {
            GuildBase guild = GlobalGuildManager.Instance.GetGuild(id);
            if (guild == null)
                throw new ArgumentException($"Invalid guild {id}!");

            guild.LeaveGuild(owner, reason);

            guilds.Remove(guild.Id);
            if (guild.Type == GuildType.Guild)
                Guild = null;

            if (GuildAffiliation?.Id == guild.Id)
            {
                GuildBase newAffiliation = guilds.Values.FirstOrDefault();
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
        /// If moving to or from a <see cref="Game.Guild.Guild"/>, the Holomark will also be updated or removed.
        /// </remarks>
        public void UpdateGuildAffiliation(ulong guildId)
        {
            if (!guilds.TryGetValue(guildId, out GuildBase guild))
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
        /// If existing affiliation is a <see cref="Game.Guild.Guild"/>, the Holomark will also be removed.
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
            var itemVisuals    = new List<ItemVisual>();

            if (backHidden)
            {
                mask |= CharacterFlag.HolomarkHideBack;
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardBack, DisplayId = 0 });
            }
            else
            {
                mask &= ~CharacterFlag.HolomarkHideBack;
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardBack, DisplayId = (ushort)(distanceNear ? 7163 : 5580) });
            }

            if (leftHidden)
            {
                mask |= CharacterFlag.HolomarkHideLeft;
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardShoulderL, DisplayId = 0 });
            }
            else
            {
                mask &= ~CharacterFlag.HolomarkHideLeft;
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardShoulderL, DisplayId = (ushort)(distanceNear ? 7164 : 5581) });
            }

            if (rightHidden)
            {
                mask |= CharacterFlag.HolomarkHideRight;
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardShoulderR, DisplayId = 0 });
            }
            else
            {
                mask &= ~CharacterFlag.HolomarkHideRight;
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardShoulderR, DisplayId = (ushort)(distanceNear ? 7165 : 5582) });
            }

            if (distanceNear)
                mask |= CharacterFlag.HolomarkNear;
            else
                mask &= ~CharacterFlag.HolomarkNear;

            // set flags directly and manually send message to prevent multiple messages through SetFlag and RemoveFlag
            owner.Flags = mask;
            owner.SendCharacterFlagsUpdated();

            SendPlayerHolomarkChange(itemVisuals);
        }

        /// <summary>
        /// Update Holomark visual and positional data.
        /// </summary>
        public void UpdateHolomark()
        {
            if (Guild == null)
               throw new InvalidOperationException($"Failed to update Holomark visual data for character {owner.CharacterId}!");

            var itemVisuals = new List<ItemVisual>
            {
                new ItemVisual { Slot = ItemSlot.GuildStandardScanLines,      DisplayId = (ushort)Guild.Standard.ScanLines.GuildStandardPartEntry.ItemDisplayIdStandard },
                new ItemVisual { Slot = ItemSlot.GuildStandardBackgroundIcon, DisplayId = (ushort)Guild.Standard.BackgroundIcon.GuildStandardPartEntry.ItemDisplayIdStandard },
                new ItemVisual { Slot = ItemSlot.GuildStandardForegroundIcon, DisplayId = (ushort)Guild.Standard.ForegroundIcon.GuildStandardPartEntry.ItemDisplayIdStandard },
                new ItemVisual { Slot = ItemSlot.GuildStandardChest,          DisplayId = 5411 }
            };

            bool isNear = owner.HasFlag(CharacterFlag.HolomarkNear);
            if (!owner.HasFlag(CharacterFlag.HolomarkHideBack))
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardBack, DisplayId = (ushort)(isNear ? 7163 : 5580) });
            if (!owner.HasFlag(CharacterFlag.HolomarkHideLeft))
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardShoulderL, DisplayId = (ushort)(isNear ? 7164 : 5581) });
            if (!owner.HasFlag(CharacterFlag.HolomarkHideRight))
                itemVisuals.Add(new ItemVisual { Slot = ItemSlot.GuildStandardShoulderR, DisplayId = (ushort)(isNear ? 7165 : 5582) });

            SendPlayerHolomarkChange(itemVisuals);
        }

        /// <summary>
        /// Remove Holomark visual and positional data.
        /// </summary>
        public void RemoveHolomark()
        {
            var itemVisuals = new List<ItemVisual>
            {
                new ItemVisual { Slot = ItemSlot.GuildStandardScanLines,      DisplayId = 0 },
                new ItemVisual { Slot = ItemSlot.GuildStandardBackgroundIcon, DisplayId = 0 },
                new ItemVisual { Slot = ItemSlot.GuildStandardForegroundIcon, DisplayId = 0 },
                new ItemVisual { Slot = ItemSlot.GuildStandardChest,          DisplayId = 0 },
                new ItemVisual { Slot = ItemSlot.GuildStandardBack,           DisplayId = 0 },
                new ItemVisual { Slot = ItemSlot.GuildStandardShoulderL,      DisplayId = 0 },
                new ItemVisual { Slot = ItemSlot.GuildStandardShoulderR,      DisplayId = 0 }
            };

            SendPlayerHolomarkChange(itemVisuals);
        }

        /// <summary>
        /// Update local clients with necessary Holomark visual changes.
        /// </summary>
        private void SendPlayerHolomarkChange(List<ItemVisual> itemVisuals)
        {
            owner.SetAppearance(itemVisuals);
            owner.EnqueueToVisible(new ServerItemVisualUpdate
            {
                Guid        = owner.Guid,
                ItemVisuals = itemVisuals
            }, true);
        }

        public IEnumerator<GuildBase> GetEnumerator()
        {
            return guilds.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
