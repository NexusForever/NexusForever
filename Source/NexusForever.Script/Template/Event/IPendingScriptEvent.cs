using NexusForever.Shared.Game;

namespace NexusForever.Script.Template.Event
{
    public interface IPendingScriptEvent
    {
        UpdateTimer Timer { get; }
        IScriptEvent Event { get; }
        uint? Id { get; }
    }
}
