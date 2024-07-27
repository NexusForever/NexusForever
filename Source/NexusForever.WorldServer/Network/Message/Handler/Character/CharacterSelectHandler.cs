using System;
using System.Linq;
using System.Numerics;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Map;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Character
{
    public class CharacterSelectHandler : IMessageHandler<IWorldSession, ClientCharacterSelect>
    {
        #region Dependency Injection

        private readonly ICleanupManager cleanupManager;
        private readonly IEntityFactory entityFactory;
        private readonly IGameTableManager gameTableManager;
        private readonly IMapManager mapManager;

        public CharacterSelectHandler(
            ICleanupManager cleanupManager,
            IEntityFactory entityFactory,
            IGameTableManager gameTableManager,
            IMapManager mapManager)
        {
            this.cleanupManager         = cleanupManager;
            this.entityFactory          = entityFactory;
            this.gameTableManager       = gameTableManager;
            this.mapManager             = mapManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientCharacterSelect characterSelect)
        {
            if (session.IsQueued == true)
                throw new InvalidPacketValueException();

            CharacterModel character = session.Characters.SingleOrDefault(c => c.Id == characterSelect.CharacterId);
            if (character == null)
            {
                session.EnqueueMessageEncrypted(new ServerCharacterSelectFail
                {
                    Result = CharacterSelectResult.Failed
                });
                return;
            }

            if (cleanupManager.IsAccountLocked(session.Account))
            {
                session.EnqueueMessageEncrypted(new ServerCharacterSelectFail
                {
                    Result = CharacterSelectResult.FailedCharacterInWorld
                });
                return;
            }

            session.Player = entityFactory.CreateEntity<IPlayer>();
            session.Player.Initialise(session, session.Account, character);

            WorldEntry entry = gameTableManager.World.GetEntry(character.WorldId);
            if (entry == null)
                throw new ArgumentOutOfRangeException();

            session.Player.Rotation = new Vector3(character.RotationX, character.RotationY, character.RotationZ);
            mapManager.AddToMap(session.Player, new MapPosition
            {
                Info = new MapInfo
                {
                    Entry = entry
                },
                Position = new Vector3(character.LocationX, character.LocationY, character.LocationZ)
            });
        }
    }
}
