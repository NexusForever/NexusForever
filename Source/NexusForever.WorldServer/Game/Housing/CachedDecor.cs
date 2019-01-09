namespace NexusForever.WorldServer.Game.Housing
{
    public class CachedDecor
    {
        public uint Id { get; }
        public string Name { get; }

        public CachedDecor(uint id, string name)
        {
            Id   = id;
            Name = name;
        }
    }
}
