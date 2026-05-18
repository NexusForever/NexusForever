using NexusForever.Game.Static;
using NexusForever.GameTable.Text.Static;

namespace NexusForever.GameTable.Text.Filter
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