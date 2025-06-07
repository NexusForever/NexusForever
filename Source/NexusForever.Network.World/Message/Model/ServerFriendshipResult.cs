using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendshipResult)]
    public class ServerFriendshipResult : IWritable
    {
        public string Message { get; set; } // In UI causes Event_FireGenericEvent("GenericEvent_SystemChannelMessage", strMessage) 
        public FriendshipResult Results { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Message);
            writer.Write(Results, 6);
        }
    }
}
