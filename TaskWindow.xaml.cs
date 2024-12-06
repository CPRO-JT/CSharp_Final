using System.Windows;

namespace MaintenanceApp
{
    public partial class TaskWindow : Window
    {
        public MaintenanceTask Task { get; private set; }
        private MaintenanceTaskManager _taskManager = new MaintenanceTaskManager();
        private Appliance _appliance { get; set; }
        private ApplianceManager applianceManager = new ApplianceManager();

        public TaskWindow(Appliance appliance)
        {
            InitializeComponent();
            _appliance = appliance;
            Task = new MaintenanceTask();
        }

        private void SaveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DueDateTextBox.Text) ||
                string.IsNullOrWhiteSpace(FrequencyTextBox.Text))
            {
                System.Windows.MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create the new task
            Task = new MaintenanceTask
            {
                Name = TaskNameTextBox.Text,
                Frequency = TimeSpan.FromDays(int.TryParse(FrequencyTextBox.Text, out var freq) ? freq : 0),
                DueDate = DueDateTextBox.SelectedDate.GetValueOrDefault(DateTime.Now.AddDays(7)),
                Description = DescriptionTextBox.Text,
            };

            // Add the task to the appliance's task list
            _appliance.Tasks.Add(Task);

            // Update the appliance in the ApplianceManager
            applianceManager.UpdateAppliance(_appliance);

            System.Windows.MessageBox.Show("Task added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

    }
}
