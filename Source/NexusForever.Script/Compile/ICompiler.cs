using System.Reflection;

namespace NexusForever.Script.Compile
{
    public interface ICompiler
    {
        /// <summary>
        /// Initialise <see cref="ICompiler"/> with supplied name.
        /// </summary>
        void Initialise(string name);

        /// <summary>
        /// Add library reference to <see cref="ICompiler"/>.
        /// </summary>
        void AddReference(string path);

        /// <summary>
        /// Add source file to <see cref="ICompiler"/>.
        /// </summary>
        void AddSourceFile(string path);

        /// <summary>
        /// Add source text to <see cref="ICompiler"/>.
        /// </summary>
        void AddSource(string text);

        /// <summary>
        /// Compile any supplied source files with references.
        /// The output <see cref="Assembly"/> will be written to the supplied <see cref="Stream"/>.
        /// </summary>
        void Compile(Stream stream);
    }
}