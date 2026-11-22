using Microsoft.Extensions.DependencyInjection;
using NexusForever.IO.Format.WildStar.Area.Static;
using NexusForever.Tool.Extractor.Extract.Prop;

namespace NexusForever.Tool.Extractor.Extract
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExtract(this IServiceCollection sc)
        {
            sc.AddSingleton<GameTableExtractionManager>();
            sc.AddSingleton<AreaExtractionManager>();
            sc.AddSingleton<FileExtractor>();

            sc.AddSingleton<PropExtractionManager>();
            sc.AddTransient<WorldPropExtractor>();
            sc.AddTransient<AreaPropExtractor>();

            sc.AddSingleton<PropExtractorFactory>();
            sc.AddKeyedTransient<IPropExtractor, M3PropExtractor>(PropType.M3);
            sc.AddKeyedTransient<IPropExtractor, I3PropExtractor>(PropType.I3);
            sc.AddKeyedTransient<IPropExtractor, DGNPropExtractor>(PropType.DGN);

            return sc;
        }
    }
}
