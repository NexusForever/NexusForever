using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class VehicleHandler
    {
        [MessageHandler(GameMessageOpcode.ClientVehicleDisembark)]
        public static void HandleVehicleDisembark(WorldSession session, ClientVehicleDisembark disembark)
        {
            if (session.Player.VehicleGuid == 0u)
                throw new InvalidPacketValueException();

            session.Player.Dismount();
        }
    }
}
