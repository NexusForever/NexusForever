using System;
using System.Runtime.CompilerServices;

namespace NexusForever.Shared
{
    public static class EnumExtensions
    {
        public static TValue As<TEnum, TValue>(this TEnum e) where TEnum : Enum
        {
            if (Unsafe.SizeOf<TEnum>() != Unsafe.SizeOf<TValue>())
                throw new ArgumentException();

            return Unsafe.As<TEnum, TValue>(ref e);
        }
    }
}
