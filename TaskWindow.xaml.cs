using System.Windows;

namespace MaintenanceApp
{

    public partial class TaskWindow : Window
    {
        // Property to hold Task
        public MaintenanceTask Task { get; private set; }

        // Instance of MaintenanceTaskManager to handle task operations
        private MaintenanceTaskManager _taskManager = new MaintenanceTaskManager();

        // Reference to the appliance the task is being added to
        private Appliance _appliance { get; set; }

        // ApplianceManager instance to update appliance data
        private ApplianceManager applianceManager = new ApplianceManager();

        // Constructor for creating a new TaskWindow with associated appliance
        public TaskWindow(Appliance appliance)
        {
            InitializeComponent();
            _appliance = appliance; // Assign the passed appliance
            Task = new MaintenanceTask(); // Initialize a new task object
        }

        // Event handler for the "Save Task"
        private void SaveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if any of the required fields are empty
            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DueDateTextBox.Text) ||
                string.IsNullOrWhiteSpace(FrequencyTextBox.Text))
            {
                System.Windows.MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Exit the method if any field is empty
            }

            // Create the new task from the input values
            Task = new MaintenanceTask
            {
                // Set task name 
                Name = TaskNameTextBox.Text,
                // Parse frequency in days, default to 0 if parsing fails
                Frequency = TimeSpan.FromDays(int.TryParse(FrequencyTextBox.Text, out var freq) ? freq : 0),
                // Set the due date, default to 7 days from now if no date is selected
                DueDate = DueDateTextBox.SelectedDate.GetValueOrDefault(DateTime.Now.AddDays(7)),
                // Set task description
                Description = DescriptionTextBox.Text,
            };

            // Add the newly created task to the appliance's task list
            _appliance.Tasks.Add(Task);

            // Update the appliance data in the ApplianceManager
            applianceManager.UpdateAppliance(_appliance);

            // Message indicating the task was added
            System.Windows.MessageBox.Show("Task added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Set the DialogResult to true and close window
            DialogResult = true;
            Close();
        }
    }
}
