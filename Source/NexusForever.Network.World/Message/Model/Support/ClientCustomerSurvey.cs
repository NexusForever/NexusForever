using NexusForever.Game.Static.Support;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Support
{
    [Message(GameMessageOpcode.ClientCustomerSurveySubmit)]
    public class ClientCustomerSurveySubmit : IReadable
    {
        public SurveyType CustomerSurveyId { get; private set; }
        public ISurvey Survey { get; private set; }
        public string Comment { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CustomerSurveyId = (SurveyType)reader.ReadInt(14);

            switch (CustomerSurveyId)
            {
                case SurveyType.QuestGeneric:
                    Survey = new Survey.QuestDifficultySurvey();
                    break;
                case SurveyType.TSpellQuest:
                    Survey = new Survey.QuestTSpellSurvey();
                    break;
                case SurveyType.HoldoutQuest:
                    Survey = new Survey.QuestHoldoutSurvey();
                    break;
                case SurveyType.LevelUp:
                    Survey = new Survey.LevelingSurvey();
                    break;
                case SurveyType.GenericChallenge:
                    Survey = new Survey.ChallengesSurvey();
                    break;
            }

            Survey.Read(reader);

            Comment = reader.ReadWideString();
        }
    }
}
