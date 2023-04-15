namespace NexusForever.Game.Abstract.Housing
{
    public interface IResidenceChild
    {
        IResidence Residence { get; init; }
        bool IsTemporary { get; set; }
        DateTime? RemovalTime { get; set; }
    }
}