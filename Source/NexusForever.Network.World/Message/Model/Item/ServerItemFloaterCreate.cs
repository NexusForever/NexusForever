using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemFloaterCreate)]
    public class ServerItemFloaterCreate : ServerItemAdd
    {
        // Same message data as ServerItemAdd but only generates a GenericFloater above the player.
    }
}
