namespace NexusForever.Game.Static.Housing
{
    // Moves from Initialising -> ClearingPlot -> Starting -> InProgress -> Finishing -> Complete
    public enum BuildState
    {
        Initialising = 0,
        Complete     = 1,
        ClearingPlot = 2,
        Starting     = 3,
        InProgress   = 4,
        Finishing    = 5
    }
}
