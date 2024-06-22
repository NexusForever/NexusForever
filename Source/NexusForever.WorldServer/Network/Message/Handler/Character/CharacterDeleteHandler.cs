using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared.Game.Events;

namespace NexusForever.WorldServer.Network.Message.Handler.Character
{
    public class CharacterDeleteHandler : IMessageHandler<IWorldSession, ClientCharacterDelete>
    {
        #region Dependency Injection

        private readonly IGlobalGuildManager globalGuildManager;
        private readonly IDatabaseManager databaseManager;
        private readonly IGlobalResidenceManager globalResidenceManager;
        private readonly ICharacterManager characterManager;

        public CharacterDeleteHandler(
            IGlobalGuildManager globalGuildManager,
            IDatabaseManager databaseManager,
            IGlobalResidenceManager globalResidenceManager,
            ICharacterManager characterManager)
        {
            this.globalGuildManager     = globalGuildManager;
            this.databaseManager        = databaseManager;
            this.globalResidenceManager = globalResidenceManager;
            this.characterManager       = characterManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientCharacterDelete characterDelete)
        {
            if (session.IsQueued == true)
                throw new InvalidPacketValueException();

            CharacterModel characterToDelete = session.Characters.FirstOrDefault(c => c.Id == characterDelete.CharacterId);

            (CharacterModifyResult, uint) GetResult()
            {
                if (characterToDelete == null)
                    return (CharacterModifyResult.DeleteFailed, 0);

                // TODO: Not sure if this is definitely the case, but put it in for good measure
                if (characterToDelete.Mail.Count > 0)
                {
                    foreach (CharacterMailModel characterMail in characterToDelete.Mail)
                    {
                        if (characterMail.Attachment.Count > 0)
                            return (CharacterModifyResult.DeleteFailed, 0);
                    }
                }

                uint leaderCount = (uint)globalGuildManager.GetCharacterGuilds(characterToDelete.Id)
                    .Count(g => g.LeaderId == characterDelete.CharacterId);
                if (leaderCount > 0)
                    return (CharacterModifyResult.DeleteFailed, leaderCount);

                return (CharacterModifyResult.DeleteOk, 0);
            }

            (CharacterModifyResult result, uint data) deleteCheck = GetResult();
            if (deleteCheck.result != CharacterModifyResult.DeleteOk)
            {
                session.EnqueueMessageEncrypted(new ServerCharacterDeleteResult
                {
                    Result = deleteCheck.result,
                    Data   = deleteCheck.data
                });
                return;
            }

            session.CanProcessIncomingPackets = false;

            void Save(CharacterContext context)
            {
                var model = new CharacterModel
                {
                    Id = characterToDelete.Id
                };

                EntityEntry<CharacterModel> entity = context.Attach(model);

                model.DeleteTime = DateTime.UtcNow;
                entity.Property(e => e.DeleteTime).IsModified = true;
                
                model.OriginalName = characterToDelete.Name;
                entity.Property(e => e.OriginalName).IsModified = true;

                model.Name = null;
                entity.Property(e => e.Name).IsModified = true;
            }

            session.Events.EnqueueEvent(new TaskEvent(databaseManager.GetDatabase<CharacterDatabase>().Save(Save),
                () =>
            {
                session.CanProcessIncomingPackets = true;

                globalResidenceManager.RemoveResidence(characterToDelete.Name);
                characterManager.DeleteCharacter(characterToDelete.Id, characterToDelete.Name);

                session.EnqueueMessageEncrypted(new ServerCharacterDeleteResult
                {
                    Result = deleteCheck.result
                });
            }));
        }
    }
}
