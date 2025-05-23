using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildBankTabRename)]
    public class ClientGuildBankTabRename : ClientGuildOperation
    {
        // This is the same packet structure as ClientGuildOperation (0x4B1) however this message
        // is only sent for renaming guild tabs.

        // The Data field is the bank tab index being renamed.

        // The TextValue is the new name for the bank tab.
    }
}
