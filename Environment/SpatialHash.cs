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
            if (!_buckets.TryGetValue(key, out HashSet< IUpdatable > bucket))
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
            if (oldKey != newKey)
            {
                _buckets[oldKey].Remove(entity);
                _buckets[newKey].Add(entity);
            }
        }

        // This is what the Critter calls to "See" nearby food
        public List<IEntity> GetNearby(Vector2 position)
        {
            List<IEntity> GetNearbyList = new List<IEntity>();
            int[] dx = { -1, 0, 1 };
            int[] dy = { -1, 0, 1 };

            foreach (int i in dx)
            {
                foreach (int j in dy)
                {
                    Vector2 pos = new Vector2(position.X + i * _cellSize,position.Y + j * _cellSize);
                    int key = GetHashKey(pos);
                    if (_buckets.TryGetValue(key, out HashSet<IUpdatable> value))
                    {
                        GetNearbyList.AddRange(value);
                    }
                    
                }
            }

            return GetNearbyList;
        }

        public int GetHashKey(Vector2 position)
        {
            int x = (int)Math.Floor(position.X / _cellSize);
            int y = (int)Math.Floor(position.Y / _cellSize);
            return (x * 73856093) ^ (y * 19349663);
        }
    }
}