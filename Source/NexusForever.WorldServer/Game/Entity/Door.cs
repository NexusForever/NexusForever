using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;
using System.Numerics;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.Door)]
    public class Door : UnitEntity
    {
        public uint CreatureId { get; private set; }

        public bool IsOpen => GetStatInteger(Stat.StandState) == 4;

        public Door()
            : base(EntityType.Door)
        {
        }

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            CreatureId        = model.Creature;

            SetStat(Stat.StandState, 3); // Closed on spawn
            Properties.Add(Property.BaseHealth, new PropertyValue(Property.BaseHealth, 101f, 101f)); // Sniffs showed all doors had 101hp for me.
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new DoorEntityModel
            {
                CreatureId        = CreatureId
            };
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            var entityModel = base.BuildCreatePacket();
            return entityModel;
        }

        /// <summary>
        /// Used to open this <see cref="Door"/>
        /// </summary>
        public void OpenDoor(Player player, bool openForAll = true)
        {
            var doorEmote = new ServerEmote
            {
                Guid = Guid,
                StandState = 4
            };

            SetStat(Stat.StandState, 4);
            if (openForAll)
                EnqueueToVisible(doorEmote);
            else
                player.Session.EnqueueMessageEncrypted(doorEmote);
        }

        /// <summary>
        /// Used to close this <see cref="Door"/>
        /// </summary>
        public void CloseDoor(Player player, bool openForAll = true)
        {
            var doorEmote = new ServerEmote
            {
                Guid = Guid,
                StandState = 3
            };

            SetStat(Stat.StandState, 3);
            if (openForAll)
                EnqueueToVisible(doorEmote);
            else
                player.Session.EnqueueMessageEncrypted(doorEmote);
        }
    }
}
