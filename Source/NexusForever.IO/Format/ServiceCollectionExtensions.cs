using Microsoft.Extensions.DependencyInjection;
using NexusForever.IO.Format.NexusForever;
using NexusForever.IO.Format.Wavefront;
using NexusForever.IO.Format.WildStar;
using NexusForever.Shared;

namespace NexusForever.IO.Format
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIOFormat(this IServiceCollection sc)
        {
            sc.AddFile<WavefrontFile>();
            sc.AddTransient<IFileWriter<WavefrontFile>, FileTextWriter<WavefrontFile>>();

            sc.AddIOFormatNexusForever();
            sc.AddIOFormatWildStar();

            return sc;
        }

        public static IServiceCollection AddFile<T>(this IServiceCollection sc) where T : class
        {
            sc.AddTransient<T>();
            sc.AddSingleton<IFactory<T>, Factory<T>>();

            sc.AddTransientFactory<IFile<T>, File<T>>();
            return sc;
        }
    }
}
