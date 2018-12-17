using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Mount : WorldEntity
    {
        public uint OwnerGuid { get; }

        public Mount(Player owner)
            : base(EntityType.Mount)
        {
            OwnerGuid   = owner.Guid;
            Rotation    = owner.Rotation;
            DisplayInfo = 32786;

            SetPropertyValue(Property.BaseHealth, 800.0f);

            Stats.Add(Stat.Health, new StatValue(Stat.Health, 800));
            Stats.Add(Stat.Level, new StatValue(Stat.Level, 3));
            Stats.Add((Stat)15, new StatValue((Stat)15, 0));
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new MountEntityModel
            {
                CreatureId = 68284,
                Unknown1   = 568,
                Unknown2   = OwnerGuid
            };
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            var lol = base.BuildCreatePacket();
            lol.Unknown60 = 1;
            lol.Unknown68 = 904575;
            return lol;
        }
    }
}
