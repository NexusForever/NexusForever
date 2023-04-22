using NexusForever.Game.Abstract.Housing;

namespace NexusForever.Game.Housing
{
    public class ResidenceChild : IResidenceChild
    {
        public IResidence Residence { get; init; }
        public bool IsTemporary { get; set; }
        public DateTime? RemovalTime { get; set; }
    }
}
