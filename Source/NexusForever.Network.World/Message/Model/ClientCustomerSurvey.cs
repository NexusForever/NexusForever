using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCustomerSurveySubmit)]
    public class ClientCustomerSurveySubmit : IReadable
    {
        public SurveyType Type { get; set; }
        public ISurvey Survey { get; set; }
        public string Comment { get; set; }

        public void Read(GamePacketReader reader)
        {
            Type = (SurveyType)reader.ReadInt(14);

            switch (Type)
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
