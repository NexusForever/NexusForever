using NexusForever.Game.Static;
using NexusForever.Game.Static.TextFilter;

namespace NexusForever.Game.Abstract.Text.Filter
{
    public interface ITextFilterManager
    {
        void Initialise();

        /// <summary>
        /// Returns if supplied string meets <see cref="TextFilterClass"/> filtering.
        /// </summary>
        bool IsTextValid(string text, TextFilterClass filterClass = TextFilterClass.Strict, Language language = Language.English);

        /// <summary>
        /// Returns if supplied string meets <see cref="UserText"/> filtering.
        /// </summary>
        bool IsTextValid(string text, UserText userText);
    }
}