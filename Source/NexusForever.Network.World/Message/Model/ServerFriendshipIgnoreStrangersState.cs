using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendshipIgnoreStrangersState)]
    public class ServerFriendshipIgnoreStrangersState : IWritable
    {
        public uint Flags { get; set; } // Send value of 2 to set IgnoreStrangers state on client

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Flags);
        }
    }
}
