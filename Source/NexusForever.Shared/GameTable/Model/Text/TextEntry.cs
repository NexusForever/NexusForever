namespace NexusForever.Shared.GameTable.Model.Text
{
    public class TextEntry
    {
        public TextEntry(Language language, uint id, string text)
        {
            Id = id;
            Language = Language;
            Text = text;
        }
        public uint Id { get; private set; }
        public Language Language { get; private set; }
        public string Text { get; private set; }
    }
}