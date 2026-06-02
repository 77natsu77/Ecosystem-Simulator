using Ecosystem_Simulator.Entities;

namespace Ecosystem_Simulator.Core.Structs
{
    
    public struct EntityStats
    {
        int CritterCount;
        int PredatorCount;
        int SmartyCount;
        int FoodCount;
        float sumCritterEnergy = 0, sumCritterSpeed = 0, sumCritterSight = 0, sumCritterMetab = 0, sumCritterRepro = 0;
        float sumPredatorEnergy = 0, sumPredatorSpeed = 0, sumPredatorSight = 0, sumPredatorMetab = 0, sumPredatorRepro = 0;
        float sumSmartyEnergy = 0, sumSmartySpeed = 0, sumSmartySight = 0, sumSmartyMetab = 0, sumSmartyRepro = 0, sumSmartyPathUpdateInterval = 0;
        private float AverageCritterEnergy => CritterCount > 0 ? sumCritterEnergy / CritterCount : 0;
        private float AverageCritterSpeed => CritterCount > 0 ? sumCritterSpeed / CritterCount : 0;
        private float AverageCritterSightRadius => CritterCount > 0 ? sumCritterSight / CritterCount : 0;
        private float AverageCritterMetabolismEfficiency => CritterCount > 0 ? sumCritterMetab / CritterCount : 0;
        private float AverageCritterReproductionThreshold => CritterCount > 0 ? sumCritterRepro / CritterCount : 0;
        private float AveragePredatorEnergy => PredatorCount > 0 ? sumPredatorEnergy / PredatorCount : 0;
        private float AveragePredatorSpeed => PredatorCount > 0 ? sumPredatorSpeed / PredatorCount : 0;
        private float AveragePredatorSightRadius => PredatorCount > 0 ? sumPredatorSight / PredatorCount : 0;
        private float AveragePredatorMetabolismEfficiency => PredatorCount > 0 ? sumPredatorMetab / PredatorCount : 0;
        private float AveragePredatorReproductionThreshold => PredatorCount > 0 ? sumPredatorRepro / PredatorCount : 0;
        private float AverageSmartyEnergy => SmartyCount > 0 ? sumSmartyEnergy / SmartyCount : 0;
        private float AverageSmartySpeed => SmartyCount > 0 ? sumSmartySpeed / SmartyCount : 0;
        private float AverageSmartySightRadius => SmartyCount > 0 ? sumSmartySight / SmartyCount : 0;
        private float AverageSmartyMetabolismEfficiency => SmartyCount > 0 ? sumSmartyMetab / SmartyCount : 0;
        private float AverageSmartyReproductionThreshold => SmartyCount > 0 ? sumSmartyRepro / SmartyCount : 0;
        private float AverageSmartyPathUpdateInterval => SmartyCount > 0 ? sumSmartyPathUpdateInterval / SmartyCount : 0;    

        public void Reset()
        {
            CritterCount = 0;
            PredatorCount = 0;
            SmartyCount = 0;
            FoodCount = 0;
            SumCritterEnergy = 0;
            SumCritterMetabolismEfficiency = 0;
            SumCritterReproductionThreshold = 0;
            SumCritterSpeed = 0;
            SumCritterSightRadius = 0;
            SumPredatorEnergy = 0;
            SumPredatorMetabolismEfficiency = 0;
            SumPredatorReproductionThreshold = 0;
            SumPredatorSpeed = 0;
            SumPredatorSightRadius = 0;
            SumSmartyEnergy = 0;
            SumSmartyMetabolismEfficiency = 0;
            SumSmartyReproductionThreshold = 0;
            SumSmartySpeed = 0;
            SumSmartySightRadius = 0;
            SumSmartyPathUpdateInterval = 0;
        }
        public EntityStats()
        {
            Reset();
        }

        public void IncrementCritterStats(float energy = 0, float speed = 0, float sightRadius = 0, float metabolismEfficiency = 0, float reproductionThreshold = 0)
        {
            CritterCount++;
            SumCritterEnergy += energy;
            SumCritterMetabolismEfficiency += metabolismEfficiency;
            SumCritterReproductionThreshold += reproductionThreshold;
            SumCritterSpeed += speed;
            SumCritterSightRadius += sightRadius;
        }

        public void IncrementPredatorStats(float energy = 0, float speed = 0, float sightRadius = 0, float metabolismEfficiency = 0, float reproductionThreshold = 0)
        {
            PredatorCount++;
            SumPredatorEnergy += energy;
            SumPredatorMetabolismEfficiency += metabolismEfficiency;
            SumPredatorReproductionThreshold += reproductionThreshold;
            SumPredatorSpeed += speed;
            SumPredatorSightRadius += sightRadius;
        }

        public void IncrementSmartyStats(float energy = 0, float speed = 0, float sightRadius = 0, float metabolismEfficiency = 0, float reproductionThreshold = 0, float PathUpdateInterval = 0)
        {
            SmartyCount++;
            SumSmartyEnergy += energy;
            SumSmartyMetabolismEfficiency += metabolismEfficiency;
            SumSmartyReproductionThreshold += reproductionThreshold;
            SumSmartySpeed += speed;
            SumSmartySightRadius += sightRadius;
            SumSmartyPathUpdateInterval += PathUpdateInterval;
        }

        public void IncrementFoodStats()
        {
            FoodCount++;
        }

        public override string ToString()
        {
            return $"Critters: {CritterCount}, Predators: {PredatorCount}, Smarties: {SmartyCount}, Food: {FoodCount}, Avg Critter Energy: {AverageCritterEnergy}, Avg Critter Metabolism Efficiency: {AverageCritterMetabolismEfficiency}, Avg Critter Reproduction Threshold: {AverageCritterReproductionThreshold}, Avg Critter Speed: {AverageCritterSpeed}, Avg Critter Sight Radius: {AverageCritterSightRadius}, Avg Predator Energy: {AveragePredatorEnergy}, Avg Predator Metabolism Efficiency: {AveragePredatorMetabolismEfficiency}, Avg Predator Reproduction Threshold: {AveragePredatorReproductionThreshold}, Avg Predator Speed: {AveragePredatorSpeed}, Avg Predator Sight Radius: {AveragePredatorSightRadius}, Avg Smarty Energy: {AverageSmartyEnergy}, Avg Smarty Metabolism Efficiency: {AverageSmartyMetabolismEfficiency}, Avg Smarty Reproduction Threshold: {AverageSmartyReproductionThreshold}, Avg Smarty Speed: {AverageSmartySpeed}, Avg Smarty Sight Radius: {AverageSmartySightRadius}";
        }
    }

}