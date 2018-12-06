namespace NexusForever.Shared.GameTable.Model.Text
{
    public class TextEntry
    {
        public uint Id { get; }
        public Language Language { get; }
        public string Text { get; }

        public TextEntry(Language language, uint id, string text)
        {
            Id = id;
            Language = Language;
            Text = text;
        }
    }
}
