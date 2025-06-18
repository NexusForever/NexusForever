using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingMatchInitiateLookingForReplacements)]
    public class ClientMatchingMatchInitiateLookingForReplacements : IReadable
    {
        public Role Roles { get; private set; } // Roles being looked for

        public void Read(GamePacketReader reader)
        {
            Roles = reader.ReadEnum<Role>(32u);
        }
    }
}
