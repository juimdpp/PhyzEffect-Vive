using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

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

    public static int LoadFile(string path, SortedDictionary<double, Vector3> trajectory)
    {
        
        if (!File.Exists(path))
        {
            Debug.LogError($"File not found at {path}.");
            return -1;
        }

        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                bool firstLine = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }
                    string[] parts = line.Split(",");
                    if (parts.Length == 4 &&
                        double.TryParse(parts[0], out double timestamp) &&
                        float.TryParse(parts[1], out float x) &&
                        float.TryParse(parts[2], out float y) &&
                        float.TryParse(parts[3], out float z))
                    {
                        trajectory.Add(timestamp, new Vector3(x, y, z));
                    }
                    else
                    {
                        Debug.Log($"Invalid file line {line}");
                    }
                }
                Debug.Log($"Finished reading file. Read a total of {trajectory.Count} entries.");
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"Error while reading file {e}");
            return -1;
        }
        return 1;
    }

}

