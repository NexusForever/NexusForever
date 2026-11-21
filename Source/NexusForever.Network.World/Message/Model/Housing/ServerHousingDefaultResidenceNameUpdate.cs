using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingDefaultResidenceNameUpdate)]
    public class ServerHousingDefaultResidenceNameUpdate : Neighbourhood
    {
        // Client only uses the Name of the Neighbourhood in this message
    }
}
