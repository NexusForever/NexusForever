namespace NexusForever.Shared.Game.Map
{
    public class MapDefines
    {
        /// <summary>
        /// Amount of grids in a world for a single direction.
        /// </summary>
        public const int WorldGridCount = 128;

        /// <summary>
        /// Centre grid in the world, this represents position 0.
        /// </summary>
        public const int WorldGridOrigin = WorldGridCount / 2;

        /// <summary>
        /// Size of grid, represents 512 world units on all sides.
        /// </summary>
        public const int GridSize = 512;

        /// <summary>
        /// Size of cell, represents 32 world units on all sides.
        /// </summary>
        public const int GridCellSize = 32;

        /// <summary>
        /// Amount of cells in a grid for a single direction.
        /// </summary>
        public const int GridCellCount = GridSize / GridCellSize;
    }
}
