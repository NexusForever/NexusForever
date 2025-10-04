using NexusForever.Game.Static;
using NexusForever.GameTable.Text.Static;

namespace NexusForever.GameTable.Text.Filter
{
    public interface ITextFilterLanguage
    {
        Language Language { get; }

        /// <summary>
        /// Returns if supplied string meets <see cref="TextFilterClass"/> filtering.
        /// </summary>
        bool IsTextValid(string text, TextFilterClass filterClass);
    }
}