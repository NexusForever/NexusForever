using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public static class ItemHandler
    {
        [CommandHandler("itemadd")]
        public static void HandleItemAdd(WorldSession session, string[] parameters)
        {
            session.Player.Inventory.ItemCreate(uint.Parse(parameters[0]), uint.Parse(parameters[1]));
        }
    }
}
