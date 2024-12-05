using System;
using System.IO;
using Newtonsoft.Json;

public static class JsonFileHandler<T>
{
    public static List<T> ReadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return new List<T>();
        var jsonData = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<T>>(jsonData) ?? new List<T>();
    }

    public static void WriteToFile(string filePath, List<T> data)
    {
        var jsonData = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, jsonData);
    }
}