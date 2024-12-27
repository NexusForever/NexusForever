using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;

namespace NexusForever.Script.Compile
{
    public class CSharpCompiler : ICompiler
    {
        private string name;

        private readonly List<MetadataReference> metadataReferences = new();
        private readonly List<SyntaxTree> syntaxTrees = new();

        #region Dependency Injection

        private readonly ILogger log;

        public CSharpCompiler(
            ILogger<ICompiler> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="ICompiler"/> with supplied name.
        /// </summary>
        public void Initialise(string name)
        {
            this.name = name;

            log.LogTrace("Initialised {name} compile.", name);
        }

        /// <summary>
        /// Add library reference to <see cref="ICompiler"/>.
        /// </summary>
        public void AddReference(string path)
        {
            try
            {
                PortableExecutableReference reference = MetadataReference.CreateFromFile(path);
                metadataReferences.Add(reference);
            }
            catch (Exception e)
            {
                log.LogWarning(e, "Failed to add reference {path} to compile!", path);
            }

            log.LogTrace("Added reference {path} to compile.", Path.GetFileName(path));
        }

        /// <summary>
        /// Add source file to <see cref="ICompiler"/>.
        /// </summary>
        public void AddSourceFile(string path)
        {
            try
            {
                AddSource(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                log.LogWarning(e, "Failed to add source {path} to compile!", path);
            }

            log.LogTrace("Added source {path} to compile.", Path.GetFileName(path));
        }

        /// <summary>
        /// Add source text to <see cref="ICompiler"/>.
        /// </summary>
        public void AddSource(string text)
        {
            try
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(text);
                syntaxTrees.Add(syntaxTree);
            }
            catch (Exception e)
            {
                log.LogWarning(e, "Failed to add text {text} to compile!", text);
            }
        }

        /// <summary>
        /// Compile any supplied source files with references.
        /// The output <see cref="Assembly"/> will be written to the supplied <see cref="Stream"/>.
        /// </summary>
        public void Compile(Stream stream)
        {
            var sw = Stopwatch.StartNew();
            log.LogInformation("Starting {name} compile.", name);

            var compilation = CSharpCompilation.Create(name, 
                syntaxTrees,
                Basic.Reference.Assemblies.Net80.References.All
                    .Concat(metadataReferences),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                #if DEBUG
                    .WithOptimizationLevel(OptimizationLevel.Debug));
                #else
                    .WithOptimizationLevel(OptimizationLevel.Release));
                #endif

            EmitResult result = compilation.Emit(stream);

            if (!result.Success)
            {
                var sb = new StringBuilder();
                foreach (Diagnostic diagnostic in result.Diagnostics)
                    sb.AppendLine(diagnostic.ToString());

                throw new CompileException(sb.ToString());
            }

            metadataReferences.Clear();
            syntaxTrees.Clear();

            log.LogInformation("Finished {name} compile in {ElapsedMilliseconds}ms.", name, sw.ElapsedMilliseconds);
        }
    }
}
