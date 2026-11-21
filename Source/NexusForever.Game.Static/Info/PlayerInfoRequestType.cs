namespace NexusForever.Game.Static.Info
{
    public enum PlayerInfoRequestType
    {
        Default   = 0, // server responds with Basic
        Social    = 1, // server responds with Basic
        Friend    = 2,
        Rival     = 3,
        Neighbour = 4,
        Loot      = 5, // not seen in sniffs but would assume respond with Basic
        BankLog   = 8, // server responds with Basic
        Maker     = 9, // server responds with Basic
        Pvp       = 10 // server responds with Basic
    }
}
