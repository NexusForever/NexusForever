using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
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
            WorldEntity mover = session.Player;
            if (session.Player.ControlGuid != session.Player.Guid)
                mover = session.Player.GetVisible<WorldEntity>(session.Player.ControlGuid);

            foreach ((EntityCommand id, IEntityCommand command) in entityCommand.Commands)
            {
                switch (command)
                {
                    case SetPositionCommand setPosition:
                    {
                        // this is causing issues after moving to soon after mounting:
                        // session.Player.CancelSpellsOnMove();

                        mover.Map.EnqueueRelocate(mover, setPosition.Position.Vector);
                        break;
                    }
                    case SetRotationCommand setRotation:
                        mover.Rotation = setRotation.Position.Vector;
                        break;
                }
            }

            mover.EnqueueToVisible(new ServerEntityCommand
            {
                Guid     = mover.Guid,
                Time     = entityCommand.Time,
                Unknown2 = true,
                Commands = entityCommand.Commands
            });
        }
    }
}
