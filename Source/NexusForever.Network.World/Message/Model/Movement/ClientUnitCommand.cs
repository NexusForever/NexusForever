using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model.Movement
{
    [Message(GameMessageOpcode.ClientUnitCommand)]
    public class ClientUnitCommand : IReadable
    {
        public uint Time { get; set; }
        public List<INetworkEntityCommand> Commands { get; } = [];

        public void Read(GamePacketReader reader)
        {
            Time = reader.ReadUInt();

            uint commandCount = reader.ReadUInt();
            for (uint i = 0u; i < commandCount; i++)
            {
                EntityCommand command     = reader.ReadEnum<EntityCommand>(5);
                IEntityCommandModel model = EntityCommandManager.Instance.NewEntityCommand(command);
                if (model == null)
                    return;

                model.Read(reader);
                Commands.Add(new NetworkEntityCommand
                {
                    Command = command,
                    Model   = model
                });
            }
        }
    }
}
