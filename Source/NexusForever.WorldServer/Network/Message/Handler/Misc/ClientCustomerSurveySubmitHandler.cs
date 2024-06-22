using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientCustomerSurveySubmitHandler : IMessageHandler<IWorldSession, ClientCustomerSurveySubmit>
    {
        /// <summary>
        /// Client sends this when the user has filled out any customer surver. 
        /// The response object contains the type of the survey, additional parmeters and the answers of the user.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientCustomerSurveySubmit surveyResponse)
        {
        }
    }
}
