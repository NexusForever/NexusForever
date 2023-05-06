using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Compile
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptCompile(this IServiceCollection sc)
        {
            sc.AddTransient<ICompiler, CSharpCompiler>();
        }
    }
}
