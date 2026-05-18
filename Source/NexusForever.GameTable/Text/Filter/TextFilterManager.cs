using System.Collections.Immutable;
using System.Reflection;
using NexusForever.Game.Static;
using NexusForever.GameTable.Text.Static;
using NexusForever.Shared;

namespace NexusForever.GameTable.Text.Filter
{
    /// <summary>
    /// A manager to validate client text, this is based on client class.
    /// </summary>
    public sealed class TextFilterManager : Singleton<TextFilterManager>, ITextFilterManager
    {
        private ImmutableDictionary<Language, TextFilterLanguage> textFilters;
        private ImmutableDictionary<UserText, UserTextAttribute> userTextAttributes;

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public TextFilterManager(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void Initialise()
        {
            InitialiseTextFilter();
            InitialiseUserText();
        }

        private void InitialiseTextFilter()
        {
            textFilters = gameTableManager.WordFilter.Entries
                .GroupBy(e => e.LanguageId)
                .ToImmutableDictionary(g => (Language)g.Key,
                    g => new TextFilterLanguage((Language)g.Key, g.ToList()));
        }

        private void InitialiseUserText()
        {
            var builder = ImmutableDictionary.CreateBuilder<UserText, UserTextAttribute>();
            foreach (FieldInfo field in typeof(UserText).GetFields())
            {
                UserTextAttribute attribute = field.GetCustomAttribute<UserTextAttribute>();
                if (attribute != null)
                    builder.Add((UserText)field.GetValue(null), attribute);
            }

            userTextAttributes = builder.ToImmutable();
        }

        /// <summary>
        /// Returns if supplied string meets <see cref="TextFilterClass"/> filtering.
        /// </summary>
        public bool IsTextValid(string text, TextFilterClass filterClass = TextFilterClass.Strict, Language language = Language.English)
        {
            if (!textFilters.TryGetValue(language, out TextFilterLanguage textFilterLanguage))
                return false;

            return textFilterLanguage.IsTextValid(text, filterClass);
        }

        /// <summary>
        /// Returns if supplied string meets <see cref="UserText"/> filtering.
        /// </summary>
        public bool IsTextValid(string text, UserText userText)
        {
            if (!userTextAttributes.TryGetValue(userText, out UserTextAttribute attribute))
                return false;

            if (attribute.MaxLength < text.Length)
                return false;

            if (attribute.MinLength != 0u && attribute.MinLength > text.Length)
                return false;

            return IsTextValid(text, attribute.Flags);
        }

        private bool IsTextValid(string text, UserTextFlags flags)
        {
            if ((flags & UserTextFlags.NoSpace) != 0 && text.Contains(' '))
                return false;

            // ensure there is a single space
            int index = text.IndexOf(' ');
            if ((flags & UserTextFlags.RequireSpace) != 0 && (index == -1 || index != text.LastIndexOf(' ')))
                return false;

            return true;
        }
    }
}
