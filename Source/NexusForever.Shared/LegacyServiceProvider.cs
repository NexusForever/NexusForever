using System;

namespace NexusForever.Shared
{
    /// <summary>
    /// This class is used as a bridge between DI and "legacy" code which has yet to be added to the container directly.
    /// </summary>
    public static class LegacyServiceProvider
    {
        public static IServiceProvider Provider { get; set; }
    }
}
