namespace NexusForever.Game.Abstract.Text.Search
{
    public interface ITextReverseIndex
    {
        bool IsEmpty { get; }

        IEnumerable<uint> ExactSearch(string text);
        IEnumerable<uint> FuzzySearch(string text);
    }
}