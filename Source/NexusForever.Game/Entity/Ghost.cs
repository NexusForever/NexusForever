using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class Ghost : WorldEntity, IGhost
    {
        public IPlayer Owner { get; }

        public Ghost(IPlayer owner)
            : base(EntityType.Ghost)
        {
            Owner = owner;

            foreach (IItemVisual visual in owner.GetVisuals())
                AddVisual(visual);

            Position = owner.Position;
            Rotation = owner.Rotation;
            Faction1 = owner.Faction1;
            Faction2 = owner.Faction2;

            CreateFlags |= EntityCreateFlag.SpawnAnimation;

            SetBaseProperty(Property.BaseHealth, 101.0f);

            SetStat(Stat.Health, 101u);
            SetStat(Stat.Level, owner.Level);
            SetStat(Stat.Sheathed, 1);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new GhostEntityModel
            {
                Name = Owner.Name,
                Race = Owner.Race,
                Class = Owner.Class,
                Sex = Owner.Sex,
                GuildIds = Owner.GuildManager
                    .Select(g => g.Id)
                    .ToList(),
                GuildName = Owner.GuildManager.GuildAffiliation?.Name,
                GuildType = Owner.GuildManager.GuildAffiliation?.Type ?? GuildType.None,
                Title = Owner.TitleManager.ActiveTitleId
            };
        }

        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            CreateFlags &= ~EntityCreateFlag.SpawnAnimation;
            CreateFlags |= EntityCreateFlag.NoSpawnAnimation;

            Owner.GhostGuid = guid;
            Owner.SetControl(this);
            Owner.Session.EnqueueMessageEncrypted(new ServerResurrectionShow
            {
                GhostId = Guid,
                RezCost = GetCostForRez(),
                TimeUntilRezMs = 0,
                ShowRezFlags = MapManager.Instance.GetRezTypeForMap(Owner),
                Dead = true,
                Unknown0 = false,
                TimeUntilForceRezMs = 0,
                TimeUntilWakeHereMs = 0
            });
        }

        public uint GetCostForRez()
        {
            // TODO: Calculate credit cost correctly. 0 for now.
            return 0u;
        }
    }
}
