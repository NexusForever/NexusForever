using System;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Info;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Info;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Player
{
    public class PlayerInfoResponseHandler : IHandleMessages<PlayerInfoResponseMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public PlayerInfoResponseHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(PlayerInfoResponseMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Source.ToGameIdentity());
            if (player == null)
                return Task.CompletedTask;

            var baseData = new PlayerInfoBase
            {
                // TODO: need to expand this to handle additional result codes...
                ResultCode = (byte)(message.PlayerInfo != null ? 0 : 1),
                Identity   = message.Target.ToNetworkIdentity()
            };

            if (message.PlayerInfo != null)
            {
                baseData.Name    = message.PlayerInfo.IdentityName.Name;
                baseData.Faction = message.PlayerInfo.Faction;
            }

            IWritable response;
            switch (message.Type)
            {
                case PlayerInfoRequestType.Default:
                case PlayerInfoRequestType.Social:
                case PlayerInfoRequestType.Loot:
                case PlayerInfoRequestType.BankLog:
                case PlayerInfoRequestType.Maker:
                case PlayerInfoRequestType.Pvp:
                {
                    response = new ServerPlayerInfoBasicResponse
                    {
                        BaseData = baseData
                    };
                    break;
                }
                default:
                {
                    var fullResponse = new ServerPlayerInfoFullResponse
                    {
                        BaseData = baseData
                    };

                    if (message.PlayerInfo != null)
                    {
                        // when would the path not be set?
                        fullResponse.IsClassPathSet          = true;
                        fullResponse.Path                    = message.PlayerInfo.Path;
                        fullResponse.Class                   = message.PlayerInfo.Class;
                        fullResponse.Level                   = message.PlayerInfo.Level;
                        fullResponse.IsLastLoggedOnInDaysSet = message.PlayerInfo.LastOnline != null;

                        if (message.PlayerInfo.LastOnline != null)
                            fullResponse.LastLoggedInDays = (float)(message.PlayerInfo.LastOnline.Value - DateTime.UtcNow).TotalDays;
                    }

                    response = fullResponse;
                    break;
                }
            }

            player.Session.EnqueueMessageEncrypted(response);

            return Task.CompletedTask;
        }
    }
}
