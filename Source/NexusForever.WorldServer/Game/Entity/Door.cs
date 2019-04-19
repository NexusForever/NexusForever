using NexusForever.Database.World.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.Door)]
    public class Door : WorldEntity
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
        /// Used to open this <see cref="Door"/>.
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
        /// Used to close this <see cref="Door"/>.
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
