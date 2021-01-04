using System;
using NexusForever.WorldServer.Game.TextFilter.Static;

namespace NexusForever.WorldServer.Game.TextFilter
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UserTextAttribute : Attribute
    {
        public UserTextFlags Flags { get; }
        public uint MaxLength { get; }
        public uint MinLength { get; }

        public UserTextAttribute(UserTextFlags flags, uint maxLength, uint minLength)
        {
            Flags     = flags;
            MaxLength = maxLength;
            MinLength = minLength;
        }
    }
}
