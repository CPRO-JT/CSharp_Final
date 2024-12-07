using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

// Manages appliances and associated data
public class ApplianceManager
{
    // File path where appliance data is stored in JSON format, located in the user's home directory.
    private string ApplianceFile = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\amt_appliances.json";

    // List to store all appliances managed by this class.
    private List<Appliance> appliances;

    // Constructor: Initializes the appliance list by reading from the JSON file if it exists, or creates an empty list otherwise.
    public ApplianceManager()
    {
        if (File.Exists(ApplianceFile)) // Check if the data file exists.
        {
            // Read the JSON file and deserialize it into a list of appliances.
            var json = File.ReadAllText(ApplianceFile);
            appliances = JsonConvert.DeserializeObject<List<Appliance>>(json) ?? new List<Appliance>();
        }
        else
        {
            // If no file exists, initialize an empty appliance list.
            appliances = new List<Appliance>();
        }
    }

    // Returns the complete list of appliances managed by this class.
    public List<Appliance> GetAllAppliances() => appliances;

    // Adds a new appliance to the list and saves the updated list to the file.
    public void AddAppliance(Appliance appliance)
    {
        appliances.Add(appliance); // Add the new appliance to the list.
        SaveToFile(); // Save the updated appliance list to the JSON file.
    }

    // Updates an existing appliance in the list and saves the updated list to the file.
    public void UpdateAppliance(Appliance appliance)
    {
        // Find the index of the appliance to be updated by its ID.
        var index = appliances.FindIndex(a => a.Id == appliance.Id);
        if (index != -1) // Ensure the appliance exists in the list.
        {
            appliances[index] = appliance; // Replace the existing appliance with the updated version.
            SaveToFile(); // Save the updated appliance list to the JSON file.
        }
    }

    // Deletes an appliance from the list using its unique ID and saves the updated list to the file.
    public void DeleteAppliance(string id)
    {
        appliances.RemoveAll(a => a.Id == id); // Remove all appliances that match the given ID.
        SaveToFile(); // Save the updated appliance list to the JSON file.
    }

    // Serializes the appliance list into JSON format and writes it to the file.
    private void SaveToFile()
    {
        var json = JsonConvert.SerializeObject(appliances, Formatting.Indented); // Convert the appliance list to JSON with indentation for readability.
        File.WriteAllText(ApplianceFile, json); // Write the JSON string to the file.
    }
}

// Represents an individual appliance, including its details and associated tasks.
public class Appliance
{
    // Unique identifier generated automatically using Guid.
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Appliance name
    public string Name { get; set; }

    // Appliance brand
    public string Brand { get; set; }

    // Model number 
    public string ModelNumber { get; set; }

    // Model name
    public string ModelName { get; set; }

    // Serial number 
    public string SerialNumber { get; set; }

    // Additional details 
    public string Details { get; set; }

    // Collection of tasks associated with the appliance, implemented as an ObservableCollection to support UI updates.
    public ObservableCollection<MaintenanceTask> Tasks { get; set; } = new ObservableCollection<MaintenanceTask>();
}
