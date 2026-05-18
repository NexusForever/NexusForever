using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetMode)]
    public class SetModeCommand : IEntityCommandModel
    {
        public ModeType Mode { get; set; }

        public void Read(GamePacketReader reader)
        {
            Mode = reader.ReadEnum<ModeType>(32);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Mode, 32);
        }
    }
}
