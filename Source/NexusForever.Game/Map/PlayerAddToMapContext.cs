using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Map;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Map
{
    public class PlayerAddToMapContext : IPlayerAddToMapContext
    {
        public DateTime Time { get; init; }

        public IPlayer Player { get; init; }
        public IMapPosition SourcePosition { get; init; }
        public IMapPosition DestinationPosition { get; init; }

        public OnAddDelegate OnAdd { get; init; }
        public OnGenericErrorDelegate OnError { get; init; }
        public OnExceptionDelegate OnException { get; init; }

        public GenericError? Error { get; set; }

        public PlayerAddToMapStatus Status { get; set; } = PlayerAddToMapStatus.Ready;
    }
}
