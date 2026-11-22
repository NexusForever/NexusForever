using Microsoft.Extensions.DependencyInjection;
using NexusForever.IO.Format.NexusForever.Map;

namespace NexusForever.IO.Format.NexusForever
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIOFormatNexusForever(this IServiceCollection sc)
        {
            sc.AddFile<MapFile>();
            sc.AddTransient<IFileReader<MapFile>, FileBinaryReader<MapFile>>();

            sc.AddFile<WritableMapFile>();
            sc.AddTransient<IFileWriter<WritableMapFile>, FileBinaryWriter<WritableMapFile>>();

            return sc;
        }
    }
}
