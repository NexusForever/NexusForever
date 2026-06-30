using System;
using NexusForever.Game;
using NexusForever.Game.Static.Who;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Who;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Who;
using NexusForever.Network.World.Message.Model.Who.Parameter;
using NexusForever.Shared;
using IWhoParameterInternal = NexusForever.Network.Internal.Message.Who.Parameter.IWhoParameter;

namespace NexusForever.WorldServer.Network.Message.Handler.Who
{
    public class ClientWhoRequestHandler : IMessageHandler<IWorldSession, ClientWhoRequest>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientWhoRequestHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession requestingSession, ClientWhoRequest request)
        {
            var message = new WhoRequestMessage
            {
                Identity             = requestingSession.Player.Identity.ToInternalIdentity(),
                ParameterGroupCounts = request.ParameterGroupCounts
            };

            foreach (WhoParameter item in request.Parameters)
            {
                IWhoParameterInternal parameter = item.Type switch
                {
                    WhoParameterType.Level   => (item.Data as WhoParameterLevel).ToInternal(),
                    WhoParameterType.Race    => (item.Data as WhoParameterRace).ToInternal(),
                    WhoParameterType.Path    => (item.Data as WhoParameterPath).ToInternal(),
                    WhoParameterType.Class   => (item.Data as WhoParameterClass).ToInternal(),
                    WhoParameterType.Zone    => (item.Data as WhoParameterZone).ToInternal(),
                    WhoParameterType.Guild   => (item.Data as WhoParameterGuild).ToInternal(),
                    WhoParameterType.Player  => (item.Data as WhoParameterPlayer).ToInternal(),
                    WhoParameterType.Combo   => (item.Data as WhoParameterCombo).ToInternal(),
                    WhoParameterType.Faction => (item.Data as WhoParameterFaction).ToInternal(),
                    _                        => throw new NotImplementedException()
                };
                message.Parameters.Add(parameter);
            }

            messagePublisher.PublishAsync(message)
                .FireAndForgetAsync();
        }
    }
}