using NexusForever.Shared.Game;

namespace NexusForever.Script.Template.Event
{
    public class PendingScriptEvent : IPendingScriptEvent
    {
        public required UpdateTimer Timer { get; init; }
        public required IScriptEvent Event { get; init; }
        public uint? Id { get; init; }
    }
}
