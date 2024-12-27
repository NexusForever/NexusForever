using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class GhostEntity : WorldEntity, IGhostEntity
    {
        public override EntityType Type => EntityType.Ghost;

        public uint OwnerGuid { get; private set; }
        public string Name { get; private set; }
        public Race Race { get; private set; }
        public Class Class { get; private set; }
        public Sex Sex { get; private set; }
        public List<ulong> GuildIds { get; private set; } = new();
        public string GuildName { get; private set; }
        public GuildType GuildType { get; private set; }
        public ushort Title { get; private set; }

        #region Dependency Injection

        public GhostEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Initialise(IPlayer owner)
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
        /// Invoked when <see cref="IGhostEntity"/> is added to <see cref="IBaseMap"/>.
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
