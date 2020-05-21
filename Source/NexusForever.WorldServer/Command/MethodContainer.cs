using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NexusForever.WorldServer.Command
{
    public class MethodContainer
    {
        public object Parent { get; }
        public MethodInfo Method { get; }

        /// <summary>
        /// Create a new <see cref="MethodContainer"/> from the supplied parent object and <see cref="MethodInfo"/>.
        /// </summary>
        public MethodContainer(object parent, MethodInfo method)
        {
            Parent = parent;
            Method = method;
        }

        public void Invoke(IEnumerable<object> parameters)
        {
            Method.Invoke(Parent, parameters.ToArray());
        }
    }
}
