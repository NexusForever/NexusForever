using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public interface ISurvey : IReadable
    {
    };

    public class Survey
    {
        public class QuestDifficultySurvey : ISurvey
        {
            // Probably Quest Id
            public short Unknown { get; set; }

            // Was the quest fun? Use 1 for not fun at all, 5 for really funny.
            public short Fun { get; set; }

            // Was the quest too difficult? Use 1 for too difficult, 5 for too easy.
            public short Diffuculty { get; set; }

            // Was the quest reward worth the effort? Use 1 for it was a wast of time, 5 for totally woth it.
            public short Reward { get; set; }

            // No fourth question given
            public short Unused { get; set; }

            public void Read(GamePacketReader reader)
            {
                Unknown    = reader.ReadShort(32);
                Fun        = reader.ReadShort(8);
                Diffuculty = reader.ReadShort(8);
                Reward     = reader.ReadShort(8);
                Unused     = reader.ReadShort(8);
            }
        }

        public class QuestTSpellSurvey : ISurvey
        {
            // Probably Quest Id
            public short Unknown { get; set; }

            // Was the quest fun? Use 1 for not fun at all, 5 for really funny.
            public short Fun { get; set; }

            // How difficult was it to properly use the provided T-Spell for the quest? Use 1 for very difficult, 5 for easy to use.
            public short Diffuculty { get; set; }

            // Would you like to see more content of this type? Use 1 for never again, 5 for yes.
            public short WantMore { get; set; }

            // No fourth question given
            public short Unused { get; set; }

            public void Read(GamePacketReader reader)
            {
                Unknown    = reader.ReadShort(32);
                Fun        = reader.ReadShort(8);
                Diffuculty = reader.ReadShort(8);
                WantMore   = reader.ReadShort(8);
                Unused     = reader.ReadShort(8);
            }
        }

        public class QuestHoldoutSurvey : ISurvey
        {
            // Probably Quest Id
            public short Unknown { get; set; }

            // Was the quest fun? Use 1 for not fun at all, 5 for really funny.
            public short Fun { get; set; }

            // Was the holdout too difficult? Use 1 for too difficult, 5 for to easy.
            public short Diffuculty { get; set; }

            // Would you like to see more content of this type? Use 1 for never again, 5 for yes.
            public short WantMore { get; set; }

            // No fourth question given
            public short Unused { get; set; }

            public void Read(GamePacketReader reader)
            {
                Unknown    = reader.ReadShort(32);
                Fun        = reader.ReadShort(8);
                Diffuculty = reader.ReadShort(8);
                WantMore   = reader.ReadShort(8);
                Unused     = reader.ReadShort(8);
            }
        }

        public class LevelingSurvey : ISurvey
        {
            public short Level { get; set; }

            // Was the level you just passed fun? Use 1 for not fun at all, 5 for really funny.
            public short Fun { get; set; }

            // Did you feel under- or over-powered for your level? Use 1 for under, 5 for over.
            public short Diffuculty { get; set; }

            // Do you feel your time to level was too long or too short? Use 1 for too short, 5 for too long.
            public short TimeSpend { get; set; }

            // No fourth question given
            public short Unused { get; set; }

            public void Read(GamePacketReader reader)
            {
                Level      = reader.ReadShort(32);
                Fun        = reader.ReadShort(8);
                Diffuculty = reader.ReadShort(8);
                TimeSpend  = reader.ReadShort(8);
                Unused     = reader.ReadShort(8);
            }
        }

        public class ChallengesSurvey : ISurvey
        {
            public short ChallengeId { get; set; }

            // Was the challenge fun? Use 1 for not fun at all, 5 for really funny.
            public short Fun { get; set; }

            // Where there enough objective targets available during the challenge? Use 1 for not enough, 5 for more than enough.
            public short TargetAmount { get; set; }

            // Did you feel the rewards offered were worth the effort? Use 1 for not worth it, 5 for totally worth it.
            public short Reward { get; set; }

            // No fourth question given
            public short Unused { get; set; }

            public void Read(GamePacketReader reader)
            {
                ChallengeId  = reader.ReadShort(32);
                Fun          = reader.ReadShort(8);
                TargetAmount = reader.ReadShort(8);
                Reward       = reader.ReadShort(8);
                Unused       = reader.ReadShort(8);
            }
        }

    }
}
