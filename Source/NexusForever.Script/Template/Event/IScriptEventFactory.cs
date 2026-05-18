namespace NexusForever.Script.Template.Event
{
    public interface IScriptEventFactory
    {
        T CreateEvent<T>() where T : IScriptEvent;
    }
}
