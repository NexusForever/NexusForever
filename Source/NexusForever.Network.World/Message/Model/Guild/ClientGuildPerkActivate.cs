using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildPerkActivate)]
    public class ClientGuildPerkActivate : ClientGuildOperation
    {
        // This is the same packet structure as ClientGuildOperation (0x4B1) however this message
        // is only sent for activating guild perks.

        // The Data field is the GuildPerkId of the perk being activated.
    }
}
