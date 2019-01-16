using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class EntityHandler
    {
        [MessageHandler(GameMessageOpcode.ClientEntityCommand)]
        public static void HandleClientEntityCommand(WorldSession session, ClientEntityCommand entityCommand)
        {
            foreach ((EntityCommand id, IEntityCommand command) in entityCommand.Commands)
            {
                switch (command)
                {
                    case SetPositionCommand setPosition:
                        session.Player.CancelSpellsOnMove();
                        session.Player.Map.EnqueueRelocate(session.Player, setPosition.Position.Vector);
                        break;
                    case SetRotationCommand setRotation:
                        session.Player.Rotation = setRotation.Position.Vector;
                        break;
                }
            }

            session.Player.EnqueueToVisible(new ServerEntityCommand
            {
                Guid     = session.Player.Guid,
                Time     = entityCommand.Time,
                Unknown2 = true,
                Commands = entityCommand.Commands
            });
        }
    }
}
