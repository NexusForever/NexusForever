using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Group.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Group;
using NexusForever.Server.GroupServer.Network.Internal;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Server.GroupServer.Character
{
    public class Character : IWrappedModel<CharacterModel>
    {
        public CharacterModel Model { get; private set; }

        public Identity Identity
        {
            get => new Identity
            {
                Id      = Model.CharacterId,
                RealmId = Model.RealmId
            };
        }

        public IdentityName IdentityName
        {
            get => new IdentityName
            {
                Name      = Model.Name,
                RealmName = Model.RealmName
            };
        }

        public Sex Sex
        {
            get => Model.Sex;
            set => Model.Sex = value;
        }

        public Race Race
        {
            get => Model.Race;
            set => Model.Race = value;
        }

        public Class Class
        {
            get => Model.Class;
            set => Model.Class = value;
        }

        public Path Path
        {
            get => Model.Path;
            set => Model.Path = value;
        }

        public byte Level
        {
            get => (byte)(GetStat(Stat.Level) ?? 0u);
            set => SetStat(Stat.Level, value);
        }

        public byte EffectiveLevel
        {
            get => (byte)(GetStat(Stat.MentorLevel) ?? 0u);
            set => SetStat(Stat.MentorLevel, value);
        }

        public Faction Faction
        {
            get => Model.Faction;
            set => Model.Faction = value;
        }

        public float Health
        {
            get => GetStat(Stat.Health) ?? 0f;
            set => SetStat(Stat.Health, value);
        }

        public float MaxHealth
        {
            get => GetProperty(Property.BaseHealth) ?? 0f;
            set => SetProperty(Property.BaseHealth, value);
        }

        public float Shield
        {
            get => GetStat(Stat.Shield) ?? 0f;
            set => SetStat(Stat.Shield, value);
        }

        public float MaxShield
        {
            get => GetProperty(Property.ShieldCapacityMax) ?? 0f;
            set => SetProperty(Property.ShieldCapacityMax, value);
        }

        public float InterruptArmour
        {
            get => GetStat(Stat.InterruptArmour) ?? 0f;
            set => SetStat(Stat.InterruptArmour, value);
        }

        public float MaxInterruptArmour
        {
            get => GetProperty(Property.InterruptArmorThreshold) ?? 0f;
            set => SetProperty(Property.InterruptArmorThreshold, value);
        }

        // TODO
        public float Absorption { get; set; }
        public float MaxAbsorption { get; set; }

        public float Focus
        {
            get => GetStat(Stat.Focus) ?? 0f;
            set => SetStat(Stat.Focus, value);
        }

        public float MaxFocus
        {
            get => GetProperty(Property.BaseFocusPool) ?? 0f;
            set => SetProperty(Property.BaseFocusPool, value);
        }

        // TODO
        public float HealingAbsorb { get; set; }
        public float MaxHealingAbsorb { get; set; }

        public ushort CurrentRealmId
        {
            get => Model.CurrentRealm;
            set => Model.CurrentRealm = value;
        }

        public ushort WorldZoneId
        {
            get => Model.WorldZoneId;
            set => Model.WorldZoneId = value;
        }

        public uint WorldId
        {
            get => Model.MapId;
            set => Model.MapId = value;
        }

        public Vector3 Position
        {
            get => new Vector3(Model.PositionX, Model.PositionY, Model.PositionZ);
            set
            {
                Model.PositionX = value.X;
                Model.PositionY = value.Y;
                Model.PositionZ = value.Z;
            }
        }

        // TODO
        public uint PhaseFlags1 { get; } = 1;
        public uint PhaseFlags2 { get; } = 1;

        public bool StatsDirty
        {
            get => Model.StatsDirty;
            set => Model.StatsDirty = value;
        }

        public bool RealmDirty
        {
            get => Model.RealmDirty;
            set => Model.RealmDirty = value;
        }

        public CharacterGroup PrimaryGroup
        {
            get => _groups[0];
            private set
            {
                _groups[0] = value;
                if (_groups[0] == null)
                    return;

                _groups[0].Index = 0;
            }
        }

        public CharacterGroup SecondaryGroup
        {
            get => _groups[1];
            private set
            {
                _groups[1] = value;
                if (_groups[1] == null)
                    return;

                _groups[1].Index = 1;
            }
        }

        public GroupInvite GroupInvite { get; private set; }

        private readonly CharacterGroup[] _groups = new CharacterGroup[2];
        private readonly Dictionary<Stat, CharacterStat> _stats = [];
        private readonly Dictionary<Property, CharacterProperty> _properties = [];

        #region Dependency Injection

        private readonly GroupManager _groupManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInternalMessagePublisher _messagePublisher;

        public Character(
            GroupManager groupManager,
            IServiceProvider serviceProvider,
            OutboxMessagePublisher messagePublisher)
        {
            _groupManager = groupManager;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Initialise group with a model.
        /// </summary>
        /// <param name="characterModel">Model to initialise character.</param>
        public void Initialise(CharacterModel characterModel)
        {
            Model = characterModel;

            if (characterModel.Invite != null)
            {
                GroupInvite = new GroupInvite();
                GroupInvite.Initialise(characterModel.Invite);
            }

            foreach (CharacterGroupModel groupModel in characterModel.Groups)
            {
                var characterGroup = _serviceProvider.GetRequiredService<CharacterGroup>();
                characterGroup.Initialise(groupModel);
                _groups[groupModel.Index] = characterGroup;
            }

            foreach (CharacterStatModel statModel in characterModel.Stats)
            {
                var stat = new CharacterStat();
                stat.Initialise(statModel);
                _stats.Add(stat.Stat, stat);
            }

            foreach (CharacterPropertyModel propertyModel in characterModel.Properties)
            {
                var property = new CharacterProperty();
                property.Initialise(propertyModel);
                _properties.Add(property.Property, property);
            }
        }

        /// <summary>
        /// Return all groups this character is associated with.
        /// </summary>
        public IEnumerable<CharacterGroup> GetGroups()
        {
            foreach (CharacterGroup group in _groups)
                if (group != null)
                    yield return group;
        }

        /// <summary>
        /// Get a group that the character is a member of by id.
        /// </summary>
        /// <param name="groupId">The id of the group to return.</param>
        public CharacterGroup GetGroup(ulong groupId)
        {
            foreach (CharacterGroup group in _groups)
                if (group != null && group.GroupId == groupId)
                    return group;

            return null;
        }

        /// <summary>
        /// Add a group to the character.
        /// </summary>
        /// <param name="group">A group which the character is a member of.</param>
        /// <returns></returns>
        public async Task AddGroupAsync(Group.Group group)
        {
            GroupMember member = group.GetMember(Identity);
            if (member == null)
                return;

            if (SecondaryGroup != null)
                throw new InvalidOperationException("Character cannot be a member of more than two groups.");

            if (PrimaryGroup != null)
                SecondaryGroup = PrimaryGroup;

            var characterGroup = _serviceProvider.GetRequiredService<CharacterGroup>();
            characterGroup.Initialise();
            characterGroup.GroupId = group.Id;

            PrimaryGroup = characterGroup;
            Model.Groups.Add(characterGroup.Model);

            await _messagePublisher.PublishAsync(new GroupMemberJoinedMessage
            {
                Group       = await group.ToInternalGroup(),
                AddedMember = await member.ToInternalGroupMember()
            });

            await SendGroupAssociationUpdated(group);
        }

        /// <summary>
        /// Remove a group from the character.
        /// </summary>
        /// <param name="group">A group which the character is a member of to be removed from.</param>
        /// <param name="removeReason">Reason for the removal from the group.</param>
        public async Task RemoveGroupAsync(Group.Group group, RemoveReason removeReason)
        {
            GroupMember member = group.GetMember(Identity);
            if (member == null)
                return;

            CharacterGroup characterGroup = GetGroup(group.Id);
            if (characterGroup == null)
                return;

            await _messagePublisher.PublishAsync(new GroupMemberLeftMessage
            {
                Group         = await group.ToInternalGroup(),
                RemovedMember = await member.ToInternalGroupMember(),
                Reason        = removeReason
            });

            if (SecondaryGroup == characterGroup)
            {
                Model.Groups.Remove(SecondaryGroup.Model);
                SecondaryGroup = null;
            }
            else
            {
                Model.Groups.Remove(PrimaryGroup.Model);

                if (SecondaryGroup != null)
                    PrimaryGroup = SecondaryGroup;
                else
                    PrimaryGroup = null;
            }

            await SendGroupAssociationUpdated(null);
        }

        private async Task SendGroupAssociationUpdated(Group.Group group)
        {
            var message = new PlayerGroupAssociationUpdatedMessage
            {
                Identity = Identity.ToInternalIdentity()
            };

            if (group != null)
                message.Group = await group.ToInternalGroup();

            await _messagePublisher.PublishAsync(message);
        }

        /// <summary>
        /// Get the value of a stat.
        /// </summary>
        /// <param name="stat">Stat to get the value.</param>
        public float? GetStat(Stat stat)
        {
            if (!_stats.TryGetValue(stat, out CharacterStat characterStat))
                return null;

            return characterStat.Value;
        }

        /// <summary>
        /// Set the value of a stat.
        /// </summary>
        /// <param name="stat">Stat to set the value.</param>
        /// <param name="value">Value to assign to the stat.</param>
        public void SetStat(Stat stat, float value)
        {
            if (!_stats.TryGetValue(stat, out CharacterStat characterStat))
            {
                characterStat = new CharacterStat();
                characterStat.Initialise();
                characterStat.Stat = stat;

                _stats.Add(stat, characterStat);
                Model.Stats.Add(characterStat.Model);
            }

            characterStat.Value = value;

            StatsDirty = true;
        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        /// <param name="property">Property to get the value.</param>
        /// <returns></returns>
        public float? GetProperty(Property property)
        {
            if (!_properties.TryGetValue(property, out CharacterProperty characterProperty))
                return null;

            return characterProperty.Value;
        }

        /// <summary>
        /// Set the value of a property.
        /// </summary>
        /// <param name="property">Property to set the value.</param>
        /// <param name="value">Value to assign to the property.</param>
        public void SetProperty(Property property, float value)
        {
            if (!_properties.TryGetValue(property, out CharacterProperty characterProperty))
            {
                characterProperty = new CharacterProperty();
                characterProperty.Initialise();
                characterProperty.Property = property;

                _properties.Add(property, characterProperty);
                Model.Properties.Add(characterProperty.Model);
            }

            characterProperty.Value = value;

            StatsDirty = true;
        }
    }
}
