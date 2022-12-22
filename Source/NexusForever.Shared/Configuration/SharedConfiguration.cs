using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NLog;

namespace NexusForever.Shared.Configuration
{
    public class SharedConfiguration : Singleton<SharedConfiguration>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IConfiguration Configuration { get; private set; }

        private ImmutableDictionary<Type, string> binds;

        public void Initialise<T>(string file)
        {
            if (Configuration != null)
                return;

            InitialiseConfiguration(file);
            InitialiseBindSections<T>();
        }

        private void InitialiseConfiguration(string file)
        {
            log.Info("Initialising configurations...");

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile(file, false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs()
                    .Skip(1)
                    .ToArray());

            Configuration = builder.Build();
        }

        /// <summary>
        /// Create binds for a configuration file model <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// Takes a configuration model and recursively finds any child configuration models with the <see cref="ConfigurationBindAttribute"/> attribute.<br/>
        /// Any type with this attribute has a bind created where the type is paired with breadcrumbs based on the property name,
        /// this is used when calling <see cref="Get{T}"/> to map the configuration model to a <see cref="IConfigurationSection"/>.
        /// </remarks>
        private void InitialiseBindSections<T>()
        {
            log.Info("Initialising configuration binds...");

            var builder = ImmutableDictionary.CreateBuilder<Type, string>();

            // create binds on root configuration
            foreach (PropertyInfo info in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.GetCustomAttribute<ConfigurationBindAttribute>() != null))
                InitialiseBindSection(info, info.Name, builder);

            binds = builder.ToImmutable();

            log.Trace($"Initalised {binds.Count} configuration bind(s)...");
        }

        private void InitialiseBindSection(PropertyInfo info, string breadcrumbs, ImmutableDictionary<Type, string>.Builder builder)
        {
            builder.Add(info.PropertyType, breadcrumbs);

            // create binds on child configurations
            foreach (PropertyInfo child in info.PropertyType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.GetCustomAttribute<ConfigurationBindAttribute>() != null))
                InitialiseBindSection(child, $"{breadcrumbs}:{child.Name}", builder);
        }

        /// <summary>
        /// Return configuration model <typeparamref name="T"/>.
        /// </summary>
        public T Get<T>()
        {
            if (!binds.TryGetValue(typeof(T), out string key))
                return default;

            return Configuration.GetSection(key).Get<T>();
        }
    }
}
