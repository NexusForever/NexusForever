using Microsoft.Extensions.DependencyInjection;
using NexusForever.IO.Format.WildStar.Area.Static;

namespace NexusForever.Tool.Extractor.Extract.Prop
{
    public class PropExtractorFactory
    {
        private readonly IServiceProvider serviceProvider;

        public PropExtractorFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IPropExtractor Resolve(PropType propType)
        {
            return serviceProvider.GetKeyedService<IPropExtractor>(propType);
        }
    }
}
