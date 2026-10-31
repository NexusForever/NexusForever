namespace NexusForever.Game.Static.Quest
{
    /// <summary>
    /// Communicator gates an evaluation may skip.
    /// </summary>
    [Flags]
    public enum CommunicatorGates : byte
    {
        None     = 0x00,
        World    = 0x01,
        Zone     = 0x02,
        Location = World | Zone,
        Offer    = 0x04
    }
}
