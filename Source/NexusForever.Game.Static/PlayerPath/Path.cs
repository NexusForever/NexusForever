namespace NexusForever.Game.Static.PlayerPath
{
    public enum Path : byte
    {
        None      = 4, // The value for data that does not contain a Path byte resolves to 4. Adding it at the top to communicate it should not be changed.
        Soldier   = 0,
        Settler   = 1,
        Scientist = 2,
        Explorer  = 3
    }
}
