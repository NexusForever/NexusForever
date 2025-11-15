using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    // Something to do with allowing ClientEvents of type Skybox
    // More research required, specifically what location players were in when receiving these messages
    [Message(GameMessageOpcode.ServerClientEventSkybox)]
    public class ServerClientEventSkybox : IWritable
    {
        public uint EventData { get; set; } // or maybe EventId
        public bool Operation { get; set; } // 1 = add, 0 = remove

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventData);
            writer.Write(Operation);
        }
    }
}
