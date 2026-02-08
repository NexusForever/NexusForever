using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Who.Parameter;

namespace NexusForever.Network.World.Message.Model.Who
{
    // If no parameters are sent, send a who response for the current zone with sensible parameters
    // Can get sent multiple sets of parameters that AND and OR together
    [Message(GameMessageOpcode.ClientWhoRequest)]
    public class ClientWhoRequest : IReadable
    {
        public List<WhoParameter> Parameters { get; private set; } = [];
        public List<int> ParameterGroupCounts { get; private set; } = []; // Number of ANDed parameters in each parameter group
                                                                  // Each new group count is another set of ANDed parameters
                                                                  // Collectively each parameter group gets ORed with the other groups

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (uint i = 0; i < count; i++)
            {
                WhoParameter parameter = new WhoParameter();
                parameter.Read(reader);
                Parameters.Add(parameter);
            }

            count = reader.ReadUInt();
            for (uint i = 0; i < count; i++)
            {
                ParameterGroupCounts.Add(reader.ReadInt());
            }
        }
    }
}
