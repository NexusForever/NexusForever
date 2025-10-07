using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ClientGuildPerkActivate)]
    public class ClientGuildPerkActivate : ClientGuildOperation
    {
        // This is the same packet structure as ClientGuildOperation (0x4B1) however this message
        // is only sent for activating guild perks.

        // The Data field is the GuildPerkId of the perk being activated.
    }
}
