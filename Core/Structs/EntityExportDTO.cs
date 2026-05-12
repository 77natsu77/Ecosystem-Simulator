namespace Ecosystem_Simulator.Core.Structs
{
    // This struct is used to transfer entity data from the backend to the frontend in a format that is easy to serialize and deserialize. It includes all the properties of an entity that are relevant for rendering and interaction in the frontend, such as position, size, color, sight range, velocity, speed, cannibalism status, energy level, and a unique identifier (Id) for selection and inspection purposes.
    public struct EntityExportDTO
    {
        public int Id { get; set; } // Added Id for selection and inspection purposes, so the frontend can identify entities uniquely even if they have the same position and type. This is important for clicking on them and showing their details in the inspector panel. The Id is generated in the constructors of Critter, Predator, and FoodPellet using GetHashCode(), which should provide a unique identifier for each entity instance.
        public string Type { get; set; } = "";
        public float X { get; set; }
        public float Y { get; set; }
        public float Size { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public float Sight { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public float Speed { get; set; }
        public bool Cannibal { get; set; }
        public bool IsScanning { get; set; } // Added: To indicate if a Smarty is currently performing a scan, which can be used to change its appearance in the frontend (e.g., color or brightness) to visually distinguish when it's actively scanning for targets versus when it's just idling or following a path.
        public float Energy { get; set; }   // Added: To show in the inspector
        public float MetabolismEfficiency { get; set; } // Added: To show in the inspector
        public float ReproductionThreshold { get; set; } // Added: To show in the inspector
        public EntityExportDTO(string type, float x, float y, float size = 5,float energy = 0, int r = 255, int g = 255, int b = 255, float sight = 0, float velX = 0, float velY = 0, float speed = 0, bool cannibal = false, int id = 0)
        {
            Type = type;
            X = x;
            Y = y;
            Size = size;
            R = r;
            G = g;
            B = b;
            Sight = sight;
            VelX = velX;
            VelY = velY;
            Speed = speed;
            Cannibal = cannibal;
            Energy = energy; // Initialize Energy to the provided value
            Id = id; // Initialize Id to the provided value
        }
    }
}