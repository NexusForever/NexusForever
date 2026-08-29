using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
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
