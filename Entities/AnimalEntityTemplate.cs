using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.delegates;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Core.Structs;
namespace Ecosystem_Simulator.Entities
{
    public class AnimalEntityTemplate :  IUpdatable, IMovable
    {
        protected float _hungerThreshold; // This will be set by child classes based on their specific hunger thresholds
        protected float _speedRatioWhenNotHungry; // This will be set by child classes based on their specific speed ratios when not hungry
        protected float _birthEnergyShareRatio; // This will be set by child classes based on their specific birth energy share ratios
        protected float _eatDistance; // This will be set by child classes based on their specific eat distances
        protected readonly IEnergyPolicy _metabolism;
        protected readonly IGenome _dna;

        protected float _wanderAngle;
        public float SightRadius { get; protected set; }
        public float ReproductionThreshold { get; protected set; }
        public float MetabolismEfficiency { get; protected set; }

        public Vector2 Position { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public float Speed { get; protected set; }
        public float Energy { get; protected set; }
        public bool IsPendingRemoval { get; protected set; }
        public int Id { get; protected set; } // Unique identifier for the animal, set in constructor
        public void SetEntityId(int value) => Id = value; // Method to set the entity ID, used by World when spawning

        public event SpawnRequestDelegate OnSpawnRequested;

        public AnimalEntityTemplate(Vector2 startPos, IGenome dna, float Energy = 0)
        {
            this.Position = startPos;
            this._dna = dna;
            this.Energy = Energy;
            this.Id = this.GetHashCode(); // Assign a unique ID to the animal

            this.Speed = dna.Speed;
            this.SightRadius = dna.SightRadius;
            this.MetabolismEfficiency = dna.MetabolismEfficiency;
            this.ReproductionThreshold = dna.ReproductionThreshold;

            this._metabolism = new StandardMetabolism(MetabolismEfficiency); 

            // Set initial velocity using the new unique speed
            float angle = (float)(Settings.Rng.NextDouble() * Math.PI * 2);
            this.Velocity = new Vector2((float)Math.Cos(angle) * this.Speed, (float)Math.Sin(angle) * this.Speed);
        }

         public void DetectCollisions(IEnumerable<IEntity> nearbyEntities)
        {
            foreach (IEntity entity in nearbyEntities)
            {
                if (entity != this && entity is ICollidable collidable && !collidable.IsPendingRemoval)
                {
                    float dX = entity.Position.X - this.Position.X;
                    float dY = entity.Position.Y - this.Position.Y;
                    float distSq = (dX * dX) + (dY * dY);
                    float collisionDistSq = Settings.CollisionDistance * Settings.CollisionDistance;
                   
                    if (distSq < 0.5f) continue; // Skip if positions are exactly the same, likely a spawn issue
                    if (distSq < collisionDistSq)
                    {
                        // Simple collision response: invert velocity
                        InvertVelocityX();
                        InvertVelocityY();
                        break; // Only handle one collision per update for simplicity
                    }
                }
            }
        }

        public void Update(double deltaTime, IEnumerable<IEntity> nearbyEntities)
        {
            //DetectCollisions(nearbyEntities);
            if (!this.IsPendingRemoval)
            {
                ProcessStimuli(deltaTime, nearbyEntities);
            }
        }

        public virtual void DecideAction(double deltaTime, Vector2 target)
        {
            // This method can be overridden by child classes to implement specific decision-making logic based on the stimuli detected in the ProcessStimuli method. For example, a Predator class might override this method to implement logic for chasing prey, while a Critter class might override it to implement logic for avoiding predators or seeking mates. By separating the decision-making logic into its own method, we can keep the code organized and make it easier to maintain and extend in the future as we add more complex behaviors and interactions between entities.
            if (target != Vector2.Zero)
            {
                SteerTowards(target);
            }
            else
            {
                Wander(deltaTime); // Keep moving if nothing is found
            }
        }

        public virtual void SenseSurroundings(IEnumerable<IEntity> nearbyEntities, ref IEatable closestFood)
        {
            // This method can be overridden by child classes to implement specific sensing logic for different types of entities. For example, a Predator class might override this method to implement logic for detecting nearby prey, while a Critter class might override it to implement logic for detecting nearby predators or mates. By separating the sensing logic into its own method, we can keep the code organized and make it easier to maintain and extend in the future as we add more complex behaviors and interactions between entities.
            float minDistanceSq = float.MaxValue;
            float eatDistSq = _eatDistance * _eatDistance;
            float sightRadiusSq = SightRadius * SightRadius;

            foreach (IEntity entity in nearbyEntities) // Detect stimuli and provide suitable response
            {
                // TODO SPLIT UP INTO DIFFERENT FUNCTIONS FOR EATING NEARBY AND SELECTING CLOSEST FOOD
                if (entity is IEatable food && !food.IsPendingRemoval)
                {
                    float dX = entity.Position.X - this.Position.X;
                    float dY = entity.Position.Y - this.Position.Y;
                    float distSq = (dX * dX) + (dY * dY);
                    // Very complex logic incoming, brace yourself!
                    // ACTION 1: EATING
                    if (distSq < eatDistSq)
                    {
                        this.Energy += food.Consume();
                        continue; // Move to next entity, this one is gone
                    }

                    // ACTION 2: SENSING FOOD (Only if hungry)
                    if (this.Energy < _hungerThreshold)
                    {
                        if (distSq < sightRadiusSq && distSq < minDistanceSq)
                        {
                            minDistanceSq = distSq;
                            closestFood = food;
                        }
                    }
                }
            }
        }

        public virtual void ProcessStimuli(double deltaTime, IEnumerable<IEntity> nearbyEntities)
        {
            // Decided to split up into sensing, deciding and acting to further decouple the logic and make it easier to maintain and extend in the future as we add more complex behaviors and interactions between entities. 
            IEatable closestFood = null;
            if (this.Energy >= ReproductionThreshold) 
            { 
                SpawnChild();
                return; // return early to reduce computation, cheap exits are the best kind of exits >:)
            }

            SenseSurroundings(nearbyEntities, ref closestFood);

            // DECISION PHASE
            DecideAction(deltaTime, closestFood?.Position ?? Vector2.Zero);

            //ACTION PHASE
            // Apply movement and consume energy for existing before checking for reproduction to ensure animals don't reproduce on the same frame they eat
            ApplyMovement(deltaTime);
            ConsumeEnergy(_metabolism.CalculateLoss(this.Velocity, this.SightRadius, deltaTime));
        }

        public void ConsumeEnergy(float amount)
        {
            this.Energy -= amount;
            if (this.Energy <= 0)
            {
                Death();
            }
        }
        
        public virtual void SpawnChild() // ???
        {
            
        }

        
        public void InvertVelocityX()
        {
            Vector2 newVelocity = new Vector2();
            newVelocity.X = -this.Velocity.X;
            newVelocity.Y = this.Velocity.Y;
            this.Velocity = newVelocity;
            // Sync the wander angle to the new direction
            _wanderAngle = (float)Math.Atan2(this.Velocity.Y, this.Velocity.X);
        }
        public void InvertVelocityY()
        {
            Vector2 newVelocity = new Vector2();
            newVelocity.X = this.Velocity.X;
            newVelocity.Y = -this.Velocity.Y;
            this.Velocity = newVelocity;
            // Sync the wander angle to the new direction
            _wanderAngle = (float)Math.Atan2(this.Velocity.Y, this.Velocity.X);
        }
        public void ApplyMovement(double deltaTime)
        {
            Vector2 newPos = new Vector2();
            newPos.X = (float)(this.Position.X + (this.Velocity.X * deltaTime));
            newPos.Y = (float)(this.Position.Y + (this.Velocity.Y * deltaTime));
            this.Position = newPos;
        }

        protected void SteerTowards(Vector2 target)
        {
            float diffX = target.X - this.Position.X;
            float diffY = target.Y - this.Position.Y;
            float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);

            if (distance > 0.1f) // Avoid division by zero
            {
                // Normalize and scale by speed
                float moveX = (diffX / distance) * this.Speed;
                float moveY = (diffY / distance) * this.Speed;

                this.Velocity = new Vector2(moveX, moveY);
            }
        } 

        protected void Wander(double deltaTime, float speedModifier = 1f)
        {
            // Slightly change the angle every frame for a smooth "curving" motion
            _wanderAngle += (float)(Settings.Rng.NextDouble() * 0.5 - 0.25); // Small jitter

            // Move a bit slower when wondering to reduce energy consumption
            float moveX = (float)Math.Cos(_wanderAngle) * (this.Speed * speedModifier);
            float moveY = (float)Math.Sin(_wanderAngle) * (this.Speed * speedModifier);

            this.Velocity = new Vector2(moveX, moveY);
        }

        // This method allows child classes to trigger the event
    protected virtual void RequestSpawn(IEntity childEntity)
    {
       OnSpawnRequested?.Invoke(childEntity);
    }
        public void ForcePosition(Vector2 newPos) => this.Position = newPos;
        public void Death() => this.IsPendingRemoval = true;
    }
}