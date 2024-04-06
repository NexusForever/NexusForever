using System;
using NexusForever.Game;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Character;
using NexusForever.Game.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class MiscHandler
    {
        [MessageHandler(GameMessageOpcode.ClientPing)]
        public static void HandlePing(IWorldSession session, ClientPing ping)
        {
            session.Heartbeat.OnHeartbeat();
        }

        /// <summary>
        /// Handled responses to Player Info Requests.
        /// TODO: Put this in the right place, this is used by Mail & Contacts, at minimum. Probably used by Guilds, Circles, etc. too.
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientPlayerInfoRequest)]
        public static void HandlePlayerInfoRequest(IWorldSession session, ClientPlayerInfoRequest request)
        {
            ICharacter character = CharacterManager.Instance.GetCharacter(request.Identity.CharacterId);
            if (character == null)
                throw new InvalidPacketValueException();

            float? onlineStatus = character.GetOnlineStatus();
            session.EnqueueMessageEncrypted(new ServerPlayerInfoFullResponse
            {
                BaseData = new ServerPlayerInfoFullResponse.Base
                {
                    ResultCode = 0,
                    Identity = new TargetPlayerIdentity
                    {
                        RealmId = RealmContext.Instance.RealmId,
                        CharacterId = character.CharacterId
                    },
                    Name = character.Name,
                    Faction = character.Faction1
                },
                IsClassPathSet = true,
                Path  = character.Path,
                Class = character.Class,
                Level = character.Level,
                IsLastLoggedOnInDaysSet = onlineStatus.HasValue,
                LastLoggedInDays = onlineStatus.GetValueOrDefault(0f)
            });
        }

        [MessageHandler(GameMessageOpcode.ClientToggleWeapons)]
        public static void HandleWeaponToggle(IWorldSession session, ClientToggleWeapons toggleWeapons)
        {
            session.Player.Sheathed = toggleWeapons.ToggleState;
        }

        [MessageHandler(GameMessageOpcode.ClientRandomRollRequest)]
        public static void HandleRandomRoll(IWorldSession session, ClientRandomRollRequest randomRoll)
        {
            if (randomRoll.MinRandom > randomRoll.MaxRandom)
                throw new InvalidPacketValueException();

            if (randomRoll.MaxRandom > 1000000u)
                throw new InvalidPacketValueException();

            session.EnqueueMessageEncrypted(new ServerRandomRollResponse
            {
                TargetPlayerIdentity = new TargetPlayerIdentity
                {
                    RealmId = RealmContext.Instance.RealmId,
                    CharacterId = session.Player.CharacterId
                },
                MinRandom = randomRoll.MinRandom,
                MaxRandom = randomRoll.MaxRandom,
                RandomRollResult = new Random().Next((int)randomRoll.MinRandom, (int)randomRoll.MaxRandom)
            });
        }

        [MessageHandler(GameMessageOpcode.ClientZoneChange)]
        public static void HandleClientZoneChange(IWorldSession session, ClientZoneChange zoneChange)
        {
        }

        /// <summary>
        /// Client sends this when it has received everything it needs to leave the loading screen.
        /// For housing maps, this also includes things such as residences and plots.
        /// See 0x732990 in the client for more information.
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientEnteredWorld)]
        public static void HandleClientEnteredWorld(IWorldSession session, ClientEnteredWorld enteredWorld)
        {
            if (!session.Player.IsLoading)
                throw new InvalidPacketValueException();

            session.Player.OnEnteredWorld();
        }

        [MessageHandler(GameMessageOpcode.ClientCinematicState)]
        public static void HandleCinematicState(IWorldSession session, ClientCinematicState cinematicState)
        {
            session.Player.CinematicManager.HandleClientCinematicState(cinematicState.State);
        }

        /// <summary>
        /// Client sends this when the user has filled out any customer surver. 
        /// The response object contains the type of the survey, additional parmeters and the answers of the user.
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientCustomerSurveySubmit)]
        public static void HandleClientCustomerSurvey(WorldSession session, ClientCustomerSurveySubmit surveyResponse)
        {
        }
    }
}
