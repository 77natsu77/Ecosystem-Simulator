// this entitity will use A* pathfinding to construct a path every 3 seconds to find food and avoid predators through the predator interface, the sight radius will increease by 2.5 times but speed will decrease by 90% when performing this action


using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Principal;
/* Trying to work on the logic flow for the A* algorithm
I think it should be that you initialize all the nodes with their world positions and then when you perform the scan, you populate the movement penalties based on the current positions of predators and food, then you perform the A* algorithm to find the best path to the best destination, which is determined by a utility function that takes into account the presence of food (reward) and predators (risk) as well as the distance to the destination (efficiency), then you follow that path for a certain amount of time before performing another scan to update your path based on any changes in the environment, such as predators moving around or new food sources appearing, this creates a more dynamic and interesting simulation, since in reality animals need to constantly update their information about their surroundings and make decisions based on that information, this also allows for better predator avoidance and food finding behavior, since the entity will be able to see predators and food sources that are within its sight radius and make decisions based on that information, which creates a more engaging and realistic simulation
I need to create functions to evalute their G and H costs, F is already done by the struct
Then I need to utilize the priority queu more to reduce execution time from O(n) to O(log n) when finding the node with the lowest F cost, this will be crucial for performance as the number of nodes increases, especially since the entity will be performing this scan every few seconds and we want to keep the simulation running smoothly even with many entities and a large world, this also adds an interesting layer of strategy to the simulation, since the entity will need to balance the energy it invests in finding optimal paths and avoiding predators with the energy it needs to sustain itself and reproduce, which creates a trade-off that the entity will need to navigate as it evolves, this also allows us to see how the entity evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
SO the main thing is creating the g and h functions, then simply implementing A* pathfinding
*/


public class Smarty : AnimalEntityTemplate
{
    private float _scanTimer = 0f;
    private bool _hasPerformedAmbitionPulse = false; // First pulse: Identify best destination
    private bool _hasPerformedSafetyPulse = false; // Second pulse: Verify path safety before committing to it
    public bool IsScanning { get; private set; } = false;
    public float timeSinceLastPathUpdate { get; private set; } = 0f;
    public Queue<Vector2> CurrentPath { get; private set; } = new Queue<Vector2>(); // using a queue as a simple way to store the path waypoints, the entity will move towards the next waypoint and then dequeue it once reached, this is a simple implementation and can be improved in the future by adding more complex path following behavior, such as steering towards the next waypoint instead of just moving directly towards it, which would create smoother movement and allow for better predator avoidance while following the path
    public Vector2 BestDestination { get; private set; } // the best destination identified by the A* scan, this is used for debugging and visualization purposes, it will show where the entity is trying to go based on the current stimuli in the environment, this can be useful for understanding the entity's behavior and decision making process, as well as for debugging and improving the A* pathfinding implementation, since it allows us to see if the entity is correctly identifying good destinations based on the presence of food and predators in its sight radius, which creates a more engaging and informative simulation
    public float PathUpdateInterval { get; private set; } // how often the entity will perform a new A* scan and update its path, in seconds
    public float PathfindingSightRadiusMultiplier { get; private set; } = Settings.SmartyPathfindingSightRadiusMultiplier; // when performing A* pathfinding, the entity's sight radius is multiplied by this value to allow it to see more of the environment and make better decisions about where to move, this is important for predator avoidance and finding food in a more efficient way, since the entity will be able to see predators from farther away and avoid them while still being able to find food, which creates a more interesting and dynamic simulation
    public float ScanDuration { get; private set; } = Settings.SmartyScanDuration; // how long the entity will spend performing the A* scan before it can move again, in seconds, this is important to prevent the entity from being able to instantly update its path and move towards food or away from predators without any delay, which would make the simulation less realistic and less interesting, since in reality animals need to spend some time processing information and making decisions before they can act on them, this also adds a layer of strategy to the simulation, since the entity will need to decide when is the best time to perform a scan and update its path based on the current situation in the environment, such as the presence of predators or food sources, which creates a more dynamic and engaging simulation
    public float PathfindingSpeedReductionMultiplier { get; private set; } = Settings.SmartyPathfindingSpeedReductionMultiplier; // when performing the A* scan, the entity's speed is multiplied by this value to simulate the fact that it is spending time processing information and making decisions, which creates a more realistic and interesting simulation, since in reality animals are not able to move at full speed while they are processing information and making decisions, this also adds a layer of strategy to the simulation, since the entity will need to decide when is the best time to perform a scan and update its path based on the current situation in the environment, such as the presence of predators or food sources, which creates a more dynamic and engaging simulation
    public AStarGrid grid { get; private set; }
    public bool MovingAlongPath { get; private set; } = false;
    public void MoveAlongPath(double deltaTime)
    {
        MovingAlongPath = true;
       // Console.WriteLine($"Smarty {Id} - Moving along path with {CurrentPath.Count} waypoints remaining.");
        Vector2 targetDirection = CurrentPath.Peek() - this.Position;
        if (CurrentPath.Count == 0) return; // No path to follow
        if (targetDirection.Length < Settings.EatDistance / 2) // If close enough to the current target waypoint, move to the next one
        {
           // Console.WriteLine($"Smarty {Id} - Reached waypoint at ({CurrentPath.Peek().X:F2}, {CurrentPath.Peek().Y:F2}), moving to next waypoint.");
            CurrentPath.Dequeue();

        }
        else
        {
            SteerTowards(this.Position + targetDirection); // Steer towards the next waypoint in the path
        }
    }

    public Smarty(Vector2 startPos, SmartyGenome dna, float Energy = Settings.SmartyStartingEnergy) : base(startPos, dna, Energy)
    {
        // make new genome or create a general one
        _hungerThreshold = Settings.SmartyHungerEnergyThreshold;
        _speedRatioWhenNotHungry = Settings.SmartySpeedRatioWhenNotHungry;
        _birthEnergyShareRatio = Settings.SmartyBirthEnergyShareRatio;
        _eatDistance = Settings.EatDistance;
        PathUpdateInterval = dna.PathUpdateInterval;
        this.grid = new AStarGrid(this.Position, this.SightRadius * Settings.SmartyPathfindingSightRadiusMultiplier); // need to stop making a new one every call
    }


    public override void SpawnChild() // need to make a system where reproduction only occurs when the smarty is in a safe location or something similar
    {
        float baby_energy = this.Energy * Settings.SmartyBirthEnergyShareRatio;
        this.Energy -= baby_energy;

        // Create new baby at parent position
        Smarty baby = new Smarty(this.Position, new SmartyGenome(this.Speed, this.SightRadius, this.MetabolismEfficiency, this.ReproductionThreshold, this.PathUpdateInterval, true), baby_energy);


        //  Trigger spawn event
        RequestSpawn(baby);
    }


    private void ChooseBestTarget(IEnumerable<IEntity> nearbyEntities)
    {
        BestDestination = this.Position;
        float bestScore = float.MinValue;
        List<IEatable> FoodList = new List<IEatable>();
        List<Predator> PredatorList = new List<Predator>();
        foreach (var entity in nearbyEntities) // seperate entities into food and predators
        {
            if (entity.IsPendingRemoval) continue;
            if (entity is IEatable food)
            {
                FoodList.Add(food); 
            }
            else if (entity is Predator predator)
            {
                PredatorList.Add(predator);
            }
        }

        foreach (IEatable food in FoodList)
        {
            float dist = Vector2.Distance(this.Position, food.Position);
            float danger = 0f;
            // increase danger based on predator proximity
            foreach (Predator p in PredatorList) { danger += 1f / (Vector2.Distance(food.Position, p.Position) + 0.001f); }

            float score = 100f - 1 / (dist * 1000000f + 0.001f) - danger * 2000f; // tune weights
            if (score > bestScore && dist < this.SightRadius * Settings.SmartyPathfindingSightRadiusMultiplier) // only consider food within the pathfinding sight radius
            {
                bestScore = score;
                BestDestination = food.Position;
            }
           // Console.WriteLine($"Evaluating food at ({food.Position.X:F2}, {food.Position.Y:F2}) - Distance: {dist:F2}, Danger: {danger:F2}, Score: {score:F2}");
        }
         //Console.WriteLine($"Food count: {FoodList.Count}");
        // output current and best position
        //Console.WriteLine($"Smarty {Id} - Current Position: ({Position.X:F2}, {Position.Y:F2}), Best Destination: ({BestDestination.X:F2}, {BestDestination.Y:F2}), Best Score: {bestScore:F2}");
    }


    public override void ProcessStimuli(double deltaTime, IEnumerable<IEntity> nearbyEntities) // makeing it so that they can only eat other animal entities
    {
        float eatDistSq = _eatDistance * _eatDistance;
        //loop through nearby entities and eat if close enough, sense closest food otherwise
        foreach (IEntity entity in nearbyEntities) // Detect stimuli and provide suitable response
        {
            if (entity != this && !entity.IsPendingRemoval) // eat food nearby
            {
                float dX = entity.Position.X - this.Position.X;
                float dY = entity.Position.Y - this.Position.Y;
                float distanceSq = dX * dX + dY * dY;


                if (distanceSq < eatDistSq)
                {
                    if (entity is IEatable food)
                    {
                        // Eat the food and gain energy
                        this.Energy += food.Consume();
                        continue; // Move to next entity, this one is gone
                    }
                    else if (entity is Critter c)
                    {
                        //take energy and kill critter
                        //c.Death();
                        //this.Energy += Settings.PredatorEnergyGainFromCritter;
                    }
                }
            }
        }
        //check path length
        //Console.WriteLine($"Smarty {Id} - Path Length: {CurrentPath.Count}, Best Target: ({BestDestination.X:F2}, {BestDestination.Y:F2}), Energy: {Energy:F2}");
        if (IsScanning)
        {
            PerformMultiStageScan(deltaTime, nearbyEntities);
        }
        else
        {
            // update cooldown timer for pathfinding scans
            timeSinceLastPathUpdate += (float)deltaTime;
            if (timeSinceLastPathUpdate >= PathUpdateInterval && !MovingAlongPath)
            {
                IsScanning = true;
                timeSinceLastPathUpdate = 0f;
                ChooseBestTarget(nearbyEntities);
                if (BestDestination != this.Position) // Only perform scan if there is a better destination to go to, otherwise just keep wandering
                {
                    PerformMultiStageScan(deltaTime, nearbyEntities);
                }
            }


            if (CurrentPath.Count > 0)
            {
                MoveAlongPath(deltaTime);
            }
            else
            {
                MovingAlongPath = false;
                Wander(deltaTime); // If no path to follow, just wander
            }
        }


        ApplyMovement(deltaTime);
        this.Energy -= _metabolism.CalculateLoss(this.Velocity, this.SightRadius, deltaTime);


        if (this.Energy >= ReproductionThreshold)
        {
            SpawnChild();
        }


        if (this.Energy <= 0)
        {
            this.IsPendingRemoval = true;
            return;
        }
    }


    private void PerformMultiStageScan(double deltaTime, IEnumerable<IEntity> nearbyEntities)
    {
        // Adjustng base stats and incrementing timer
        _scanTimer += (float)deltaTime;
        SightRadius = _dna.SightRadius * PathfindingSightRadiusMultiplier;
        Speed = _dna.Speed * PathfindingSpeedReductionMultiplier;


        // Initial Pathfinding (at start of scan)
        if (_scanTimer >= Settings.SmartyAmbitionPulseTime && !_hasPerformedAmbitionPulse)
        {
            // Align grid center and current center and empty all nodes before pathfinding
            grid.UpdateGridCenter(this.Position, this.SightRadius);

            grid.ApplyDynamicPenalties(nearbyEntities); // predators → MaxValue, food → negative
            CurrentPath = grid.FindPath(BestDestination);
            _hasPerformedAmbitionPulse = true;
        }


        // PULSE 2: Safety Verification (mid-way through scan)
        if (_scanTimer >= Settings.SmartySafetyPulseTime && !_hasPerformedSafetyPulse)
        {
            VerifyPathSafety(nearbyEntities); // Check if predators moved into the way
            _hasPerformedSafetyPulse = true;
        }


        // FINALIZATION: End of scan
        if (_scanTimer >= Settings.SmartyScanDuration)
        {
            // reset values
            IsScanning = false;
            _scanTimer = 0f;
            _hasPerformedAmbitionPulse = false;
            _hasPerformedSafetyPulse = false;

            // Reset stats to normal
            SightRadius = _dna.SightRadius;
            Speed = _dna.Speed;
        }
    }


    public void VerifyPathSafety(IEnumerable<IEntity> nearbyEntities)
    {
        // This method would check if any predators have moved into the path towards BestDestination
        // If so, it could either recalculate the path or choose to wait and perform another scan soon
        // For simplicity, we'll just check if any predators are now within a certain distance of the BestDestination and if so, we will invalidate that destination and choose to wait for the next scan
       /* foreach (var entity in nearbyEntities)
        {
            if (entity is Predator predator)
            {
                float distToBest = Vector2.Distance(predator.Position, BestDestination);
                if (distToBest < predator.SightRadius) // If a predator is close enough to threaten the path
                {
                    BestDestination = this.Position; // Invalidate the destination and stay put for now
                    break;
                }
            }
        }*/
    }
}


// Highkey should move this somewhere else, but I don't really know where yet :(
public class AStarGrid
{
    public Node[,] Nodes;
    private int _gridSize, _halfGridSize;
    private const float TILE_SIZE = 7f; // TODO add this as a setting const rather than here, bigger size  = faster pathfinding but less precision, smaller size = slower pathfinding but more precise, need to find a good balance for this based on testing, also could potentially make this dynamic based on the entity's speed and sight radius, for example faster entities with larger sight radius could use a larger tile size to reduce the number of nodes and improve performance, while slower entities with smaller sight radius could use a smaller tile size for more precise pathfinding, this would add an interesting layer of strategy to the simulation, since different entities would have different pathfinding capabilities based on their traits, which creates a more dynamic and engaging simulation, this also allows for better predator avoidance and food finding behavior, since the entity will be able to see predators and food sources that are within its sight radius and make decisions based on that information when performing pathfinding calculations, which creates a more engaging and realistic simulation
    private const float BASE_PENALTY = 10000f; // Base penalty for empty nodes, can be adjusted based on nearby entities
    private float _sightRadius;
    private Vector2 _currentPosition;
    private  Queue<Vector2> _path = new Queue<Vector2>(); // Best path, either empty or populated with waypoints to follow
    private PriorityQueue<Node, float> _openSet = new PriorityQueue<Node, float>(); // Using priority queue for O(log n) retrieval of node with lowest F cost
    private HashSet<Node> _closedSet = new HashSet<Node>(); // Using Hashset for O(1) lookups to check if a node has already been evaluated
    private Node[] neighborNodes = new Node[4]; // up, down, left, right
    public void ApplyDynamicPenalties(IEnumerable<IEntity> nearbyEntities)
    {
        Node currentNode;
        //  Populate penalties based on entities
        foreach (var entity in nearbyEntities)
        {
            currentNode = GetNodeFromCoords(entity.Position);
            if (currentNode == null) continue; // Skip entities that are out of bounds for the grid
            int gridX = currentNode.GridX;
            int gridY = currentNode.GridY;

            if (gridX >= 0 && gridX < _gridSize && gridY >= 0 && gridY < _gridSize)
            {
                if (entity is Predator)
                {
                    currentNode.MovementPenalty = float.MaxValue; // High penalty for predators to encourage avoidance

                }
                else if (entity is IEatable)
                {
                    currentNode.MovementPenalty = -50f; // Negative penalty (bonus) for food to encourage seeking
                }
                else
                {
                    currentNode.MovementPenalty = BASE_PENALTY; // Base penalty for empty nodes
                }
            }
            else
            {
                // output grid dimensions and entity position, even if outside of bounds so we can see where the issue is
                // Console.WriteLine($"Grid Size: {_gridSize}x{_gridSize}, Entity Position: ({entity.Position.X:F2}, {entity.Position.Y:F2}), Current Position: ({_currentPosition.X:F2}, {_currentPosition.Y:F2}), Sight Radius: {_sightRadius:F2}");
                //Console.WriteLine($"{gridX}, {gridY}");
                //Console.WriteLine($"Entity at ({entity.Position.X:F2}, {entity.Position.Y:F2}) is out of bounds for penalty application.");
            }
        }
    }
    public AStarGrid(Vector2 currentPosition, float sightRadius)
    {
        _currentPosition = currentPosition;
        _sightRadius = sightRadius;
        _halfGridSize = (int)Math.Ceiling(_sightRadius / TILE_SIZE);
        _gridSize = _halfGridSize * 2 + 1;         // odd so centre is exact
        Nodes = new Node[_gridSize, _gridSize];
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                Vector2 worldPos = _currentPosition + new Vector2((x - _halfGridSize) * TILE_SIZE, (y - _halfGridSize) * TILE_SIZE);
                Nodes[x, y] = new Node(x, y, worldPos);
            }
        }
    }

    public void UpdateGridCenter(Vector2 newCenter, float newSightRadius)
    {
        _currentPosition = newCenter;
        _sightRadius = newSightRadius;

        // _halfGridSize = (int)Math.Ceiling(_sightRadius / TILE_SIZE);
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                // Shift node world positions to be relative to the NEW center
                Nodes[x, y].Position = _currentPosition + new Vector2((x - _halfGridSize) * TILE_SIZE, (y - _halfGridSize) * TILE_SIZE);

                // Reset the node for the new scan
                Nodes[x, y].MovementPenalty = BASE_PENALTY;
                Nodes[x, y].GCost = float.MaxValue;
                Nodes[x, y].ParentX = -1;
                Nodes[x, y].ParentY = -1;
            }
        }
    }

    private Queue<Vector2> RetracePath(Node startNode, Node endNode)
    {
        Stack<Vector2> path = new Stack<Vector2>();
        Node currentNode = endNode;

        while (currentNode.GridX != startNode.GridX || currentNode.GridY != startNode.GridY)
        {
            path.Push(currentNode.Position);
            currentNode = Nodes[currentNode.ParentX, currentNode.ParentY];
        }
        //Console.WriteLine($"Path found with {path.Count} waypoints.");
        return new Queue<Vector2>(path); // Convert stack to queue for easier path following (dequeue from the front)
    }

    public float Heuristic(Node a, Node b)
    {
        // Using Euclidean distance as the heuristic for A* pathfinding, this is important for the A* algorithm to estimate the cost of reaching the target destination from any given node, since it provides a way to prioritize which nodes to explore based on their proximity to the target, this creates a more efficient pathfinding process, since the algorithm will be more likely to explore nodes that are closer to the target destination first, which can lead to finding the optimal path faster, especially in cases where there are many nodes and potential paths to evaluate, this also allows for better predator avoidance and food finding behavior, since the entity will be able to see predators and food sources that are within its sight radius and make decisions based on that information when performing pathfinding calculations, which creates a more engaging and realistic simulation
        return Vector2.Distance(a.Position, b.Position); // could use manhattan distance as well to reduce computation time but we'll use this for now
    }

    public Node GetNodeFromCoords(Vector2 worldPos)
    {
        int halfTiles = _gridSize / 2;

        // Calculate the offset from the bottom-left corner of the grid
        float localX = worldPos.X - (_currentPosition.X - halfTiles * TILE_SIZE);
        float localY = worldPos.Y - (_currentPosition.Y - halfTiles * TILE_SIZE);

        int gridX = (int)Math.Floor(localX / TILE_SIZE);
        int gridY = (int)Math.Floor(localY / TILE_SIZE);

        // Use Clamp to prevent 0.00001 precision errors from returning null
        gridX = Math.Clamp(gridX, 0, _gridSize - 1);
        gridY = Math.Clamp(gridY, 0, _gridSize - 1);

        return Nodes[gridX, gridY]; // need to add safety check to this
    }


    // Implement A* algorithm to find path from center node to best node
    public Queue<Vector2> FindPath(Vector2 target)
    {
       _path.Clear();
        Node endNode = GetNodeFromCoords(target);

        if (endNode == null)
        {
            Console.WriteLine("Target is out of bounds for pathfinding.");
            return _path; // Target is out of bounds, return empty path
        }

        Node startNode = Nodes[_gridSize / 2, _gridSize / 2]; 
        
        // Clearing instead of making new ones to reduce Garbage Collection slowing things down since this method is called quite frequently
        _openSet.Clear();
        _closedSet.Clear();

        startNode.GCost = 0f;
        _openSet.Enqueue(startNode, startNode.FCost);

        while (_openSet.TryDequeue(out Node currentNode, out float f))
        {
            if (_closedSet.Contains(currentNode)) continue; // Skip if we've already evaluated this node

            if (currentNode.GridX == endNode.GridX && currentNode.GridY == endNode.GridY)
            {
                return RetracePath(startNode, endNode); // Found the path to the best destination
            }


            _closedSet.Add(currentNode);
            currentNode.GetNeighbors(Nodes, ref neighborNodes); // Get the neighboring nodes (up, down, left, right) for pathfinding calculations

            for (int i = 0; i < neighborNodes.Length; i++)
            {
                Node neighbor = neighborNodes[i];
                if (neighbor == null || _closedSet.Contains(neighbor)) continue; // Skip out of bounds or already evaluated neighbors

                float tentativeGCost = currentNode.GCost + Vector2.Distance(currentNode.Position, neighbor.Position) + neighbor.MovementPenalty;

                if (tentativeGCost < neighbor.GCost)
                {
                    neighbor.ParentX = currentNode.GridX;
                    neighbor.ParentY = currentNode.GridY;
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = Vector2.Distance(neighbor.Position, endNode.Position);

                    // re enqueue if better cost is found
                    _openSet.Enqueue(neighbor, neighbor.FCost);
                }
            }
        }
       // no path found, return empty path
        return _path;
    }
}

public class Node
{
    public int GridX;
    public int GridY;
    public float MovementPenalty; // High for predators, low for food
    public float GCost; // Distance from start node and movement penalties of nodes along the way
    public float HCost; // Heuristic distance to end node
    public float FCost => GCost + HCost;
    public Vector2 Position; // World position of the node, used for distance calculations and movement targets
    // To reconstruct the path later
    public int ParentX; 
    public int ParentY;
    
    public Node(int gridX, int gridY, Vector2 nodePosition)
    {
        GridX = gridX;
        GridY = gridY;
        Position = nodePosition;
        MovementPenalty = 10000f; // Default high penalty for empty nodes, will be adjusted based on nearby entities
        GCost = float.MaxValue;
        HCost = 0f;
        ParentX = -1;
        ParentY = -1;
    }


    public void GetNeighbors(Node[,] grid, ref Node[] neighborNodes)
    {
        int maxX = grid.GetLength(0);
        int maxY = grid.GetLength(1);

        // Up
        if (GridY + 1 < maxY) neighborNodes[0] = grid[GridX, GridY + 1];
        // Down
        if (GridY - 1 >= 0) neighborNodes[1] = grid[GridX, GridY - 1];
        // Right
        if (GridX + 1 < maxX) neighborNodes[2] = grid[GridX + 1, GridY];
        // Left
        if (GridX - 1 >= 0) neighborNodes[3] = grid[GridX - 1, GridY];
    }


}


