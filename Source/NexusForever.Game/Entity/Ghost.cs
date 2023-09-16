using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class Ghost : WorldEntity, IGhost
    {
        public uint OwnerGuid { get; }

        public string Name { get; }
        public Race Race { get; }
        public Class Class { get; }
        public Sex Sex { get; }
        public List<ulong> GuildIds { get; } = new();
        public string GuildName { get; }
        public GuildType GuildType { get; }
        public ushort Title { get; }

        /// <summary>
        /// Create a new <see cref="IGhost"/> for <see cref="IPlayer"/>.
        /// </summary>
        public Ghost(IPlayer owner)
            : base(EntityType.Ghost)
        {
            OwnerGuid = owner.Guid;
            Name      = owner.Name;
            Race      = owner.Race;
            Class     = owner.Class;
            Sex       = owner.Sex;
            GuildIds  = owner.GuildManager
                .Select(g => g.Id)
                .ToList();
            GuildName = owner.GuildManager.GuildAffiliation?.Name;
            GuildType = owner.GuildManager.GuildAffiliation?.Type ?? GuildType.None;
            Title     = owner.TitleManager.ActiveTitleId;

            foreach (IItemVisual visual in owner.GetVisuals())
                AddVisual(visual);

            Position = owner.Position;
            Rotation = owner.Rotation;
            Faction1 = owner.Faction1;
            Faction2 = owner.Faction2;

            CreateFlags |= EntityCreateFlag.NoSpawnAnimation;

            SetBaseProperty(Property.BaseHealth, 101.0f);

            SetStat(Stat.Health, 101u);
            SetStat(Stat.Level, owner.Level);
            SetStat(Stat.Sheathed, 1);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new GhostEntityModel
            {
                Name      = Name,
                Race      = Race,
                Class     = Class,
                Sex       = Sex,
                GuildIds  = GuildIds,
                GuildName = GuildName,
                GuildType = GuildType,
                Title     = Title
            };
        }

        /// <summary>
        /// Invoked when <see cref="IGhost"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            IPlayer owner = GetVisible<IPlayer>(OwnerGuid);
            if (owner == null)
            {
                RemoveFromMap();
                return;
            }

            owner.SetControl(this);
            owner.ResurrectionManager.ShowResurrection();
        }
    }
}
