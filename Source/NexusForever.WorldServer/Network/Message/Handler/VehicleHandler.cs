using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class VehicleHandler
    {
        [MessageHandler(GameMessageOpcode.ClientVehicleDisembark)]
        public static void HandleVehicleDisembark(IWorldSession session, ClientVehicleDisembark disembark)
        {
            // If player is mounted and tries to summon a different mount, the client sends this packet twice.
            // Ignore Disembark request if no vehicle.
            if (session.Player.PlatformGuid == null)
                return;

            session.Player.Dismount();
        }
    }
}
