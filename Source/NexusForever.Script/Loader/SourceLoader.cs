using System.Reflection;
using NexusForever.Script.Compile;
using NexusForever.Shared;

namespace NexusForever.Script.Loader
{
    public class SourceLoader : ISourceLoader
    {
        private readonly List<string> sourceFolderIgnores = new()
        {
            "bin",
            "obj"
        };

        #region Dependency Injection

        private readonly ICompiler compiler;

        public SourceLoader(
            ICompiler compiler)
        {
            this.compiler = compiler;
        }

        #endregion

        /// <summary>
        /// Load supplied path into an assembly <see cref="Stream"/>.
        /// </summary>
        public Stream Load(string path)
        {
            compiler.Initialise(Path.GetFileName(path));

            AddReferences();
            AddSourceFiles(path);

            var stream = new MemoryStream();
            compiler.Compile(stream);
            stream.Position = 0;

            return stream;
        }

        private void AddReferences()
        {
            foreach (Assembly assemblies in NexusForeverAssemblyHelper.GetAssemblies())
                compiler.AddReference(assemblies.Location);
        }

        private void AddSourceFiles(string path)
        {
            // add support for global usings
            compiler.AddSource("""
                global using global::System;
                global using global::System.Collections.Generic;
                global using global::System.IO;
                global using global::System.Linq;
                global using global::System.Net.Http;
                global using global::System.Threading;
                global using global::System.Threading.Tasks;
                """);

            foreach (string file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(f => !sourceFolderIgnores.Any(f.Contains)))
                compiler.AddSourceFile(file);
        }
    }
}
