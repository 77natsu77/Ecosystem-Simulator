using Ecosystem_Simulator.Entities;
using System.Collections.Generic;
using System.Linq;
public class StatsTracker
{
    public List<float> AvgSpeedHistory { get; } = new List<float>();
    public List<float> AvgSightHistory { get; } = new List<float>();

    public void Update(List<Critter> critters)
    {
        //TODO: IMPLEMENT THE SIGHT RADIUS HISTORY TRACKER
        if (critters.Count == 0)
        {
            AvgSpeedHistory.Add(0);
            return;
        }


        float totalSpeed = 0;
        foreach (var c in critters) { totalSpeed += c.Speed; }
        float avgSpeed = totalSpeed / critters.Count;


        AvgSpeedHistory.Add(avgSpeed);


        if (AvgSpeedHistory.Count > 1000) AvgSpeedHistory.RemoveAt(0);
    }

    public float GetAverageSpeed() => (AvgSpeedHistory.Count == 0) ? 0 : AvgSpeedHistory[AvgSpeedHistory.Count - 1];
}