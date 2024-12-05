using System;
using System.Collections.Generic;
using System.Windows;

public class MaintenanceTaskManager
{
    private Dictionary<string, List<MaintenanceTask>> applianceTasks;

    public MaintenanceTaskManager()
    {
        applianceTasks = new Dictionary<string, List<MaintenanceTask>>();
    }

    public List<MaintenanceTask> GetTasksForAppliance(string applianceId)
    {
        if (applianceTasks.ContainsKey(applianceId))
        {
            return applianceTasks[applianceId];
        }
        return new List<MaintenanceTask>();
    }

    public void AddTask(string applianceId, MaintenanceTask task)
    {
        if (!applianceTasks.ContainsKey(applianceId))
        {
            applianceTasks[applianceId] = new List<MaintenanceTask>();
        }
        applianceTasks[applianceId].Add(task);
    }

    public void RemoveTask(string applianceId, string taskId)
    {
        if (applianceTasks.ContainsKey(applianceId))
        {
            applianceTasks[applianceId].RemoveAll(t => t.Id == taskId);
        }
    }
}

public class MaintenanceTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public TimeSpan Frequency { get; set; }
}