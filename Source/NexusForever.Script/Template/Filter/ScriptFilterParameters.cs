using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Script.Template.Filter.Dynamic;

namespace NexusForever.Script.Template.Filter
{
    public class ScriptFilterParameters : IScriptFilterParameters
    {
        public Type ScriptType { get; private set; }
        public HashSet<uint> Id { get; set; }
        public HashSet<uint> CreatureId { get; set; }
        public HashSet<ulong> ActivePropId { get; set; }
        public string ScriptName { get; set; }

        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public ScriptFilterParameters(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public void Initialise(Type type)
        {
            ScriptType = type;

            ScriptFilterOwnerIdAttribute objectIdAttribute = ScriptType.GetCustomAttribute<ScriptFilterOwnerIdAttribute>();
            if (objectIdAttribute != null)
                Id = new HashSet<uint>(objectIdAttribute.Id);

            ScriptFilterCreatureIdAttribute creatureIdAttribute = ScriptType.GetCustomAttribute<ScriptFilterCreatureIdAttribute>();
            if (creatureIdAttribute != null)
                CreatureId = new HashSet<uint>(creatureIdAttribute.CreatureId);

            ScriptFilterActivePropIdAttribute activePropIdAttribute = ScriptType.GetCustomAttribute<ScriptFilterActivePropIdAttribute>();
            if (activePropIdAttribute != null)
                ActivePropId = new HashSet<ulong>(activePropIdAttribute.ActivePropId);

            Attribute dynamicAttribute = ScriptType.GetCustomAttribute(typeof(ScriptFilterDynamicAttribute<>));
            if (dynamicAttribute != null)
            {
                Type dynamicFilterType = dynamicAttribute.GetType().GetGenericArguments().First();
                IScriptFilterDynamic dynamicFilter = (IScriptFilterDynamic)serviceProvider.GetRequiredService(dynamicFilterType);
                dynamicFilter.Filter(this);
            }

            ScriptFilterScriptNameAttribute scriptNameAttribute = ScriptType.GetCustomAttribute<ScriptFilterScriptNameAttribute>();
            if (scriptNameAttribute != null)
                ScriptName = scriptNameAttribute.ScriptName;
        }
    }
}
