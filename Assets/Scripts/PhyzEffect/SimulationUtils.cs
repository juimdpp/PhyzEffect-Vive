using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class SimulationUtils
{
    public static void SaveToFile(string filePath, string header, SortedDictionary<double, Vector3> data)
    {
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine(header);
            foreach (var entry in data)
            {
                writer.WriteLine($"{entry.Key},{entry.Value.x},{entry.Value.y},{entry.Value.z}");
            }
        }
        Debug.Log($"Data saved to {filePath}");
    }
}

