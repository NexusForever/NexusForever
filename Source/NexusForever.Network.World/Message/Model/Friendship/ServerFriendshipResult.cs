using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipResult)]
    public class ServerFriendshipResult : IWritable
    {
        public string Message { get; set; } // In UI causes Event_FireGenericEvent("GenericEvent_SystemChannelMessage", strMessage) 
        public FriendshipResult Result { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Message);
            writer.Write(Result, 6);
        }
    }
}
