using System;

namespace NexusForever.Shared.GameTable
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GameDataAttribute : Attribute
    {
        public string FileName { get; }

        public GameDataAttribute()
            : this(null)
        {
        }

        public GameDataAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}
