using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Ecosystem_Simulator.Environment
{
    public class SpatialHash
    {
        private readonly float _cellSize = Settings.CellSize;
        // The core storage
        private readonly Dictionary<int, HashSet<IUpdatable>> _buckets = new Dictionary<int, HashSet<IUpdatable>>();

        // Adds an entity to the grid for the first time
        public void Register(IUpdatable entity)
        {

            int key = GetHashKey(entity.Position);
            if (!_buckets.TryGetValue(key, out HashSet<IUpdatable> bucket))
            {
                _buckets.Add(key, new HashSet<IUpdatable> { entity });
            }
            else
            {
                bucket.Add(entity);
            }

        }

        public void Unregister(IUpdatable entity)
        {
            int key = GetHashKey(entity.Position);

            if (_buckets.TryGetValue(key, out var bucket))
            {
                bucket.Remove(entity);

                if (bucket.Count == 0)
                {
                    _buckets.Remove(key);
                }
            }
        }

        // Moving between cells
        public void UpdateEntityPosition(IUpdatable entity, Vector2 oldPos)
        {
            int oldKey = GetHashKey(oldPos);
            int newKey = GetHashKey(entity.Position);

            // OPTIMIZATION: Only do work if the critter actually crossed a boundary 
            if (oldKey != newKey && _buckets.TryGetValue(oldKey, out var oldBucket) && _buckets.TryGetValue(newKey, out var newBucket))
            {
                oldBucket.Remove(entity);
                newBucket.Add(entity);
            }
        }


        public HashSet<IEntity> GetEntitiesInRadius(Vector2 center, float radius)
        {
            HashSet<IEntity> results = new HashSet<IEntity>();

            // How many cells does this radius span?
            int cellRange = (int)Math.Ceiling(radius / _cellSize);

            // Convert world position to grid coordinates
            int centerIdxX = (int)(center.X / _cellSize);
            int centerIdxY = (int)(center.Y / _cellSize);

            // Loop through the box of cells covered by the radius
            for (int x = centerIdxX - cellRange; x <= centerIdxX + cellRange; x++)
            {
                for (int y = centerIdxY - cellRange; y <= centerIdxY + cellRange; y++)
                {
                    int key = GetHashKeyFromCoords(x, y);

                    if (_buckets.TryGetValue(key, out var bucket))
                    {
                        results.UnionWith(bucket);
                    }
                }
            }
            return results;
        }

        public int GetHashKey(Vector2 position)
        {
            int x = (int)Math.Floor(position.X / _cellSize);
            int y = (int)Math.Floor(position.Y / _cellSize);
            return (x * Settings.HashConstantX) ^ (y * Settings.HashConstantY);
        }

        public int GetHashKeyFromCoords(int x, int y) => (x * Settings.HashConstantX) ^ (y * Settings.HashConstantY);
    }
}