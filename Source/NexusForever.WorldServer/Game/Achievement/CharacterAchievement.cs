using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;

namespace NexusForever.WorldServer.Game.Achievement
{
    public class CharacterAchievement : Achievement
    {
        private readonly ulong characterId;

        /// <summary>
        /// Create a new <see cref="CharacterAchievement"/> from an existing database model.
        /// </summary>
        public CharacterAchievement(CharacterAchievementModel model, AchievementInfo info)
            : base(info, model.Data0, model.Data1, model.DateCompleted, false)
        {
            characterId = model.Id;
        }

        /// <summary>
        /// Create a new <see cref="CharacterAchievement"/> from supplied <see cref="AchievementInfo"/>.
        /// </summary>
        public CharacterAchievement(ulong characterId, AchievementInfo info)
            : base(info, 0u, 0u, null, true)
        {
            this.characterId = characterId;
        }

        public override void Save(CharacterContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            if ((saveMask & SaveMask.Create) != 0)
            {
                context.Add(new CharacterAchievementModel
                {
                    Id            = characterId,
                    AchievementId = Id,
                    Data0         = Data0,
                    Data1         = Data1,
                    DateCompleted = DateCompleted
                });
            }
            else
            {
                var model = new CharacterAchievementModel
                {
                    Id            = characterId,
                    AchievementId = Id
                };

                EntityEntry<CharacterAchievementModel> entity = context.Attach(model);
                if ((saveMask & SaveMask.Data0) != 0)
                {
                    model.Data0 = Data0;
                    entity.Property(p => p.Data0).IsModified = true;
                }
                if ((saveMask & SaveMask.Data1) != 0)
                {
                    model.Data1 = Data1;
                    entity.Property(p => p.Data1).IsModified = true;
                }
                if ((saveMask & SaveMask.TimeCompleted) != 0)
                {
                    model.DateCompleted = DateCompleted;
                    entity.Property(p => p.DateCompleted).IsModified = true;
                }
            }

            saveMask = SaveMask.None;
        }
    }
}
