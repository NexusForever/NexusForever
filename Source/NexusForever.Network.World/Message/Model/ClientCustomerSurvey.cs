using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCustomerSurveySubmit)]
    public class ClientCustomerSurveySubmit : IReadable
    {
        public SurveyType Type { get; set; }
        // second parameter send with the survey request
        public ISurvey Survey { get; set; }
        public string Comment { get; set; }

        public void Read(GamePacketReader reader)
        {
            Type = (SurveyType) reader.ReadInt(14);

            switch (Type)
            {
                case SurveyType.QuestDifficulty:
                    Survey = new Survey.QuestDifficultySurvey();
                    break;
                case SurveyType.QuestTSpell:
                    Survey = new Survey.QuestTSpellSurvey();
                    break;
                case SurveyType.QuestHoldout:
                    Survey = new Survey.QuestHoldoutSurvey();
                    break;
                case SurveyType.Level:
                    Survey = new Survey.LevelingSurvey();
                    break;
                case SurveyType.Challenge:
                    Survey = new Survey.ChallengesSurvey();
                    break;
                // TODO figure out Bossfight/Dungeon
            }
            Survey.Read(reader);

            Comment = reader.ReadWideString();
        }
    }
}
