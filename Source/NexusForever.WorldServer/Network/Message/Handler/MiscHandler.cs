using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class MiscHandler
    {
        [MessageHandler(GameMessageOpcode.ClientPing)]
        public static void HandlePing(WorldSession session, ClientPing ping)
        {
            session.Heartbeat.OnHeartbeat();
        }

        /// <summary>
        /// Handled responses to Player Info Requests.
        /// TODO: Put this in the right place, this is used by Mail & Contacts, at minimum. Probably used by Guilds, Circles, etc. too.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="request"></param>
        [MessageHandler(GameMessageOpcode.ClientPlayerInfoRequest)]
        public static void HandlePlayerInfoRequest(WorldSession session, ClientPlayerInfoRequest request)
        {
            Character character = CharacterDatabase.GetCharacterById(request.CharacterId);
            if (character != null)
                session.EnqueueMessageEncrypted(new ServerPlayerInfoFullResponse
                {
                    Unk0 = 0,
                    Realm = WorldServer.RealmId,
                    CharacterId = character.Id,
                    Name = character.Name,
                    Faction = (Faction)character.FactionId,
                    Path = (Path)character.ActivePath,
                    Class = (Class)character.Class,
                    Level = character.Level,
                    LastOnlineInDays = -1f
                });
        }
    }
}
