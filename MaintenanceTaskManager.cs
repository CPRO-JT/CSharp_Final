using System;
using System.Collections.Generic;
using System.Windows;

// Manages maintenance tasks 
public class MaintenanceTaskManager
{
    // Dictionary to store tasks for each appliance
    // The key is the appliance ID, value is a list of tasks associated with that appliance
    private Dictionary<string, List<MaintenanceTask>> applianceTasks;

    // Constructor initializes the dictionary for managing tasks
    public MaintenanceTaskManager()
    {
        applianceTasks = new Dictionary<string, List<MaintenanceTask>>();
    }

    // Retrieves the list of tasks for a specific appliance by its ID
    public List<MaintenanceTask> GetTasksForAppliance(string applianceId)
    {
        // If the appliance exists in the dictionary, return its tasks
        if (applianceTasks.ContainsKey(applianceId))
        {
            return applianceTasks[applianceId];
        }

        // If the appliance ID is not found, return an empty list
        return new List<MaintenanceTask>();
    }

    // Adds a new maintenance task for a specific appliance, identified by its ID
    public void AddTask(string applianceId, MaintenanceTask task)
    {
        // If the appliance ID doesn't exist in the dictionary, initialize a new list for it
        if (!applianceTasks.ContainsKey(applianceId))
        {
            applianceTasks[applianceId] = new List<MaintenanceTask>();
        }

        // Add the task to the appliance's task list
        applianceTasks[applianceId].Add(task);
    }

    // Removes a task from a specific appliance's task list, identified by the task ID
    public void RemoveTask(string applianceId, string taskId)
    {
        // Check if the appliance exists in the dictionary
        if (applianceTasks.ContainsKey(applianceId))
        {
            // Remove the task with the specified task ID from the appliance's task list
            applianceTasks[applianceId].RemoveAll(t => t.Id == taskId);
        }
    }
}

// Represents a maintenance task associated with an appliance
public class MaintenanceTask
{
    // Unique identifier for the task using Guid
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Name of the task
    public string Name { get; set; }

    // Description of the task
    public string Description { get; set; }

    // Due date for the task, specifying when it needs to be completed
    public DateTime DueDate { get; set; }

    // Frequency of the task, represented as a TimeSpan 
    public TimeSpan Frequency { get; set; }
}
