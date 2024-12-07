using System;
using System.IO;
using Newtonsoft.Json;

// A generic static class for handling JSON file operations (read and write) for any type T.
public static class JsonFileHandler<T>
{
    // Reads data from a JSON file and deserializes it into a list of objects of type T.
    public static List<T> ReadFromFile(string filePath)
    {
        // Check if the file exists at the specified path.
        if (!File.Exists(filePath))
            return new List<T>(); // If the file doesn't exist, return an empty list of type T.

        // Read the file contents into a JSON string.
        var jsonData = File.ReadAllText(filePath);

        // Deserialize the JSON string into a List<T>. If null, return an empty list.
        return JsonConvert.DeserializeObject<List<T>>(jsonData) ?? new List<T>();
    }

    // Writes a list of objects of type T into a JSON file at the specified path.
    public static void WriteToFile(string filePath, List<T> data)
    {
        // Serialize the list of objects into a formatted JSON string.
        var jsonData = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

        // Write the JSON string to the file at the specified path.
        File.WriteAllText(filePath, jsonData);
    }
}
