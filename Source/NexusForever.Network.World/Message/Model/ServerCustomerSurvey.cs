using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerNewCustomerSurveyRequest)]
    public class ServerNewCustomerSurveyRequest : IWritable
    {
        public SurveyType Type { get; set; }
        public int SecParam { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 14);
            writer.Write(SecParam, 32);
        }
    }
}
