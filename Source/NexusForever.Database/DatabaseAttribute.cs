using System;

namespace NexusForever.Database;

[AttributeUsage(AttributeTargets.Class)]
public class DatabaseAttribute : Attribute
{
    public DatabaseType Type { get; }

    public DatabaseAttribute(DatabaseType type)
    {
        Type = type;
    }
}
