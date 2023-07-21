using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    [DatabaseEntity(EntityType.Door)]
    public class Door : WorldEntity, IDoor
    {
        public bool IsOpen => GetStatEnum<StandState>(Stat.StandState) == StandState.State1;

        public Door()
            : base(EntityType.Door)
        {
        }

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);

            SetStat(Stat.StandState, StandState.State0); // Closed on spawn
            SetBaseProperty(Property.BaseHealth, 101f); // Sniffs showed all doors had 101hp for me.
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new DoorEntityModel
            {
                CreatureId = CreatureId
            };
        }

        /// <summary>
        /// Used to open this <see cref="IDoor"/>.
        /// </summary>
        public void OpenDoor()
        {
            SetStat(Stat.StandState, StandState.State1);
            EnqueueToVisible(new ServerEmote
            {
                Guid       = Guid,
                StandState = StandState.State1
            });
        }

        /// <summary>
        /// Used to close this <see cref="IDoor"/>.
        /// </summary>
        public void CloseDoor()
        {
            SetStat(Stat.StandState, StandState.State0);
            EnqueueToVisible(new ServerEmote
            {
                Guid       = Guid,
                StandState = StandState.State0
            });
        }
    }
}
