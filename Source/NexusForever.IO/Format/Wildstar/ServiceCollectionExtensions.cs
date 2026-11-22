using Microsoft.Extensions.DependencyInjection;
using NexusForever.IO.Format.Wildstar.DGN;
using NexusForever.IO.Format.Wildstar.DGN.Dressing;
using NexusForever.IO.Format.WildStar.Area;
using NexusForever.IO.Format.WildStar.I3;
using NexusForever.IO.Format.WildStar.I3.Sector;
using NexusForever.IO.Format.WildStar.M3;

namespace NexusForever.IO.Format.WildStar
{
    public static class ServiceCollectionExtensions
    {
        public static void AddIOFormatWildStar(this IServiceCollection sc)
        {
            sc.AddFile<AreaFile>();
            sc.AddTransient<IFileReader<AreaFile>, FileBinaryReader<AreaFile>>();

            sc.AddFile<M3File>();
            sc.AddTransient<IFileReader<M3File>, FileBinaryReader<M3File>>();

            sc.AddFile<I3File>();
            sc.AddTransient<IFileReader<I3File>, FileBinaryReader<I3File>>();

            sc.AddFile<I3SectorFile>();
            sc.AddTransient<IFileReader<I3SectorFile>, FileBinaryReader<I3SectorFile>>();

            sc.AddFile<DNGFile>();
            sc.AddTransient<IFileReader<DNGFile>, FileBinaryReader<DNGFile>>();

            sc.AddFile<DNGDressingFile>();
            sc.AddTransient<IFileReader<DNGDressingFile>, FileBinaryReader<DNGDressingFile>>();
        }
    }
}
