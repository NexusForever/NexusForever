using System;

namespace NexusForever.Shared.GameTable
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GameTableFieldArrayAttribute : Attribute
    {
        public uint Length { get; }

        public GameTableFieldArrayAttribute(uint length)
        {
            Length = length;
        }
    }
}
