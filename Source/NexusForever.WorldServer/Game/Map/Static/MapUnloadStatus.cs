namespace NexusForever.WorldServer.Game.Map.Static
{
    public enum MapUnloadStatus
    {
        AwaitingGridActions,

        /// <summary>
        /// Unload has been commenced, waiting to start player unload.
        /// </summary>
        AwaitingUnloadPlayers,

        /// <summary>
        /// Players have been unloaded, waiting to start entity unload. 
        /// </summary>
        AwaitingUnloadEntities,

        /// <summary>
        /// Map players are being unloaded and moved to their return positions.
        /// </summary>
        UnloadingPlayers,

        /// <summary>
        /// Map grids are being unloaded and entities are being cleaned up.
        /// </summary>
        UnloadingEntities,

        /// <summary>
        /// Map instance has completely unloaded.
        /// </summary>
        Complete
    }
}
