using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Map;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Abstract.Map
{
    public interface IPlayerAddToMapContext
    {
        DateTime Time { get; }

        IPlayer Player { get; init; }
        IMapPosition SourcePosition { get; init; }
        IMapPosition DestinationPosition { get; init; }

        OnAddDelegate OnAdd { get; init; }
        OnGenericErrorDelegate OnError { get; init; }
        OnExceptionDelegate OnException { get; init; }

        GenericError? Error { get; set; }

        PlayerAddToMapStatus Status { get; set; }
    }
}
