using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public static class ItemHandler
    {
        [CommandHandler("itemadd")]
        [CommandHandler("additem")]
        public static void HandleItemAdd(WorldSession session, string[] parameters)
        {
            if(parameters.Length <= 0)
            {
                return;
            }
            uint amount = 1;
            if(parameters.Length > 1)
            {
                amount = uint.Parse(parameters[1]);
            }
            session.Player.Inventory.ItemCreate(uint.Parse(parameters[0]), amount);
        }
    }
}
