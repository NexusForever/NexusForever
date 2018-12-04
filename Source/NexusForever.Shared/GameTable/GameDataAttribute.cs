using System;

namespace NexusForever.Shared.GameTable
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GameDataAttribute : Attribute
    {
        public GameDataAttribute()
            : this(null) { }
        public GameDataAttribute(string fileName)
        {
            FileName = fileName;
        }
        public string FileName { get; private set; }
    }
}