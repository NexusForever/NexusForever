using NexusForever.Game.Static;
using NexusForever.Game.Static.TextFilter;

namespace NexusForever.Game.Abstract.Text.Filter
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