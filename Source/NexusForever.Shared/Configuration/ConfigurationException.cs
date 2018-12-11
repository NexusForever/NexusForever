using System;

namespace NexusForever.Shared.Configuration
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message)
        {
        }
    }
}
