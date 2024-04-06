using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.State;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetState)]
    public class SetStateCommand : IEntityCommandModel
    {
        public StateFlags State { get; set; }

        public void Read(GamePacketReader reader)
        {
            State = reader.ReadEnum<StateFlags>(32);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(State, 32);
        }
    }
}
