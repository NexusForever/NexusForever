namespace NexusForever.WorldServer.Game.Housing
{
    public class PublicResidence
    {
        public ulong ResidenceId { get; }
        public string Owner { get; }
        public string Name { get; }

        public PublicResidence(ulong residenceId, string owner, string name)
        {
            ResidenceId = residenceId;
            Owner       = owner;
            Name        = name;
        }
    }
}
