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
            foreach ((EntityCommand id, IEntityCommand command) in entityCommand.Commands)
            {
                switch (command)
                {
                    case SetPositionCommand setPosition:
                        // this is causing issues after moving to soon after mounting:
                        // session.Player.CancelSpellsOnMove();

                        session.Player.Map.EnqueueRelocate(session.Player, setPosition.Position.Vector);
                        if (session.Player.MountId > 0)
                            session.Player.Map.EnqueueRelocate(session.Player.Map.GetEntity<Mount>(session.Player.MountId), setPosition.Position.Vector);

                        if (session.Player.PetId > 0)
                            session.Player.Map.EnqueueRelocate(session.Player.Map.GetEntity<VanityPet>(session.Player.PetId), setPosition.Position.Vector);
                        break;

                    case SetRotationCommand setRotation:
                        session.Player.Rotation = setRotation.Position.Vector;
                        break;
                }
            }

            session.Player.EnqueueToVisible(new ServerEntityCommand
            {
                Guid     = session.Player.MountId > 0 ? session.Player.MountId : session.Player.Guid,
                Time     = entityCommand.Time,
                Unknown2 = true,
                Commands = entityCommand.Commands
            });
        }
    }
}
