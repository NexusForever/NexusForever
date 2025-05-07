using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingRoleCheckStarted)]
    public class ServerMatchingRoleCheckStarted : IWritable
    {
        public bool RolesRequired { get; set; } // Passed to Lua event MatchingRoleCheckStarted but not used in Carbine's Lua code

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RolesRequired);
        }
    }
}
