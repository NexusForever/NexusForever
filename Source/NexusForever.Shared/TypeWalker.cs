using System;
using System.Reflection;

namespace NexusForever.Shared
{
    public static class TypeWalker
    {
        public static void Walk<T>(Assembly assembly, Action<Type, T> callback) where T : Attribute
        {
            foreach (Type type in assembly.GetTypes())
            {
                T attribute = type.GetCustomAttribute<T>();
                if (attribute == null)
                    continue;

                callback(type, attribute);
            }
        }
    }
}
