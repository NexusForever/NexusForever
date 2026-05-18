using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Template.Event
{
    public class ScriptEventFactory : IScriptEventFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ScriptEventFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public T CreateEvent<T>() where T : IScriptEvent
        {
            return serviceProvider.GetRequiredService<T>();
        }
    }
}
