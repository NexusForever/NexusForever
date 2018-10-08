using System;
using System.Collections.Generic;
using System.Numerics;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Map
{
    public class MapGrid
    {
        /// <summary>
        /// Size of grid, represents 512 world units on all sides.
        /// </summary>
        public const int Size      = 512;
        public const int CellCount = Size / MapCell.Size;

        public Vector3 Vector { get; }

        private readonly MapCell[] cells = new MapCell[CellCount * CellCount];

        /// <summary>
        /// Initialise a grid at the specified world position.
        /// </summary>
        public MapGrid(Vector3 vector)
        {
            Vector = vector;

            for (int z = 0; z < CellCount; z++)
            {
                for (int x = 0; x < CellCount; x++)
                {
                    var cellVector = new Vector3(Vector.X + (x * MapCell.Size), 0f, Vector.Z + (z * MapCell.Size));
                    cells[z * CellCount + x] = new MapCell(cellVector);
                }
            }
        }

        /// <summary>
        /// Return <see cref="MapCell"/> in grid that surrounds position.
        /// </summary>
        private MapCell GetCell(Vector3 vector)
        {
            int cellX = (int)Math.Floor((vector.X - Vector.X) / MapCell.Size);
            if (cellX < 0 || cellX > CellCount)
                throw new ArgumentOutOfRangeException($"Position X: {vector.X} is invalid for cell!");

            int cellY = (int)Math.Floor((vector.Z - Vector.Z) / MapCell.Size);
            if (cellY < 0 || cellY > CellCount)
                throw new ArgumentOutOfRangeException($"Position Z: {vector.Z} is invalid for cell!");

            return cells[cellY * CellCount + cellX];
        }

        public void AddEntity(GridAction action)
        {
            GetCell(action.Position).AddEntity(action.Entity);
        }

        public void RemoveEntity(GridEntity entity)
        {
            GetCell(entity.Position).RemoveEntity(entity);
        }

        public void RelocateEntity(GridAction action)
        {
            MapCell oldCell = GetCell(action.Entity.Position);
            MapCell newCell = GetCell(action.Position);

            // new position is in the same cell, no need to transfer entity to another cell
            if (newCell.Vector == oldCell.Vector)
                return;

            oldCell.RemoveEntity(action.Entity);
            newCell.AddEntity(action.Entity);
        }

        /// <summary>
        /// Return all <see cref="GridEntity"/>'s in grid that satisfy <see cref="ISearchCheck"/>.
        /// </summary>
        public void Search(Vector3 position, ISearchCheck check, List<GridEntity> intersectedEntities)
        {
            GetCell(position).Search(check, intersectedEntities);
        }

        public void Update(double lastTick)
        {
            foreach (MapCell cell in cells)
                cell.Update(lastTick);
        }
    }
}
