using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Network;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientEntityCommand)]
    public class ClientEntityCommand : IReadable
    {
        public uint Time { get; set; }
        public List<(EntityCommand, IEntityCommandModel)> Commands { get; } = new List<(EntityCommand, IEntityCommandModel)>();

        public void Read(GamePacketReader reader)
        {
            Time = reader.ReadUInt();

            uint commandCount = reader.ReadUInt();
            for (uint i = 0u; i < commandCount; i++)
            {
                EntityCommand command = reader.ReadEnum<EntityCommand>(5);
                IEntityCommandModel entityCommand = EntityCommandManager.Instance.NewEntityCommand(command);
                if (entityCommand == null)
                    return;

                entityCommand.Read(reader);
                Commands.Add((command, entityCommand));
            }
        }
    }
}
