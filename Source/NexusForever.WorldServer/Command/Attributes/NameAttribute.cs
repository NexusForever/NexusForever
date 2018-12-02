using System;

namespace NexusForever.WorldServer.Command.Attributes
{

    public class NameAttribute : Attribute
    {
        public string Name { get; }

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
