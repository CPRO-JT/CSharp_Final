using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

public class ApplianceManager
{
    private string ApplianceFile = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\amt_appliances.json";
    private List<Appliance> appliances;

    public ApplianceManager()
    {
        if (File.Exists(ApplianceFile))
        {
            var json = File.ReadAllText(ApplianceFile);
            appliances = JsonConvert.DeserializeObject<List<Appliance>>(json) ?? new List<Appliance>();
        }
        else
        {
            appliances = new List<Appliance>();
        }
    }

    public List<Appliance> GetAllAppliances() => appliances;

    public void AddAppliance(Appliance appliance)
    {
        appliances.Add(appliance);
        SaveToFile();
    }

    public void UpdateAppliance(Appliance appliance)
    {
        var index = appliances.FindIndex(a => a.Id == appliance.Id);
        if (index != -1)
        {
            appliances[index] = appliance;
            SaveToFile();
        }
    }

    public void DeleteAppliance(string id)
    {
        appliances.RemoveAll(a => a.Id == id);
        SaveToFile();
    }

    private void SaveToFile()
    {
        var json = JsonConvert.SerializeObject(appliances, Formatting.Indented);
        File.WriteAllText(ApplianceFile, json);
    }
}

public class Appliance
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Brand { get; set; }
    public string ModelNumber { get; set; }
    public string ModelName { get; set; }
    public string SerialNumber { get; set; }
    public string Details { get; set; }
    public ObservableCollection<MaintenanceTask> Tasks { get; set; } = new ObservableCollection<MaintenanceTask>();
}