using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Game.Social.Static;
using System;

namespace NexusForever.WorldServer.Game.Guild
{
    public partial class Community : GuildChat
    {
        public override uint MaxMembers => 20u;

        public Residence Residence { get; set; }

        /// <summary>
        /// Create a new <see cref="Community"/> using <see cref="GuildModel"/>
        /// </summary>
        public Community(GuildModel baseModel) 
            : base(baseModel)
        {
            InitialiseChatChannels(ChatChannelType.Community, null);
        }

        /// <summary>
        /// Create a new <see cref="Community"/> using the supplied parameters.
        /// </summary>
        public Community(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.Community, name, leaderRankName, councilRankName, memberRankName)
        {
            InitialiseChatChannels(ChatChannelType.Community, null);
        }

        /// <summary>
        /// Set <see cref="Community"/> privacy level.
        /// </summary>
        public void SetCommunityPrivate(bool enabled)
        {
            if (enabled)
                SetFlag(GuildFlag.CommunityPrivate);
            else
                RemoveFlag(GuildFlag.CommunityPrivate);

            SendGuildFlagUpdate();
        }

        public override void DisbandGuild()
        {
            // Complete Disband First so that all Residences are removed before we Remove Community.
            base.DisbandGuild();

            // TODO: Kick everybody from Residence Map?
            GlobalResidenceManager.Instance.RemoveCommunity(this);
        }

        protected override void RemoveMember(ulong characterId, bool disband)
        {
            if (!members.TryGetValue(characterId, out GuildMember member))
                throw new ArgumentException($"Invalid member {characterId} for guild {Id}.");

            ICharacter character = CharacterManager.Instance.GetCharacterInfo(characterId);
            if (character == null)
                throw new InvalidOperationException("Character must exist to remove from Community!");

            // Remove Player's Residence before you remove member to prevent any lockup with the player's Residence
            RemoveChildResidence(character);

            base.RemoveMember(characterId, disband);
        }

        /// <summary>
        /// Removes a Player's <see cref="Residence"/> from this <see cref="Community"/> plot.
        /// </summary>
        public void RemoveChildResidence(ICharacter character)
        {
            ResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(PropertyInfoId.Residence);
            if (entrance == null)
                throw new InvalidOperationException();

            ResidenceChild child = Residence.GetChild(character.CharacterId);
            if (child == null)
                return;

            if (child.Residence.Map != null)
                child.Residence.Map.RemoveChild(child.Residence);
            else
                child.Residence.Parent.RemoveChild(child.Residence);

            child.Residence.PropertyInfoId = PropertyInfoId.Residence;

            if (character is not Player player)
                return;

            if (Residence.Map == null)
                return;

            if (player.Map != Residence.Map)
                return;

            // shouldn't need to check for existing instance
            // individual residence instances are unloaded when transfered to a community
            // if for some reason the instance is still unloading the residence will be initalised again after
            player.Rotation = entrance.Rotation.ToEulerDegrees();
            player.TeleportTo(entrance.Entry, entrance.Position, child.Residence.Id);
        }
    }
}
