using System.Windows;

namespace MaintenanceApp
{
    public partial class TaskWindow : Window
    {
        public MaintenanceTask Task { get; private set; }
        private MaintenanceTask _editingTask;
        private bool _isEditing;

        public TaskWindow()
        {
            InitializeComponent();
        }

        public TaskWindow(MaintenanceTask taskToEdit) : this()
        {
            _isEditing = true;
            _editingTask = taskToEdit;

            // Pre-fill fields with the task's details
            TaskNameTextBox.Text = taskToEdit.Name;
            DescriptionTextBox.Text = taskToEdit.Description;
            DueDateTextBox.SelectedDate = taskToEdit.DueDate;
            FrequencyTextBox.Text = taskToEdit.Frequency.Days.ToString();
        }

        private void SaveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                // Update the existing task
                _editingTask.Name = TaskNameTextBox.Text;
                _editingTask.Description = DescriptionTextBox.Text;
                _editingTask.DueDate = DueDateTextBox.SelectedDate ?? DateTime.Now; // Default to today if null
                _editingTask.Frequency = TimeSpan.FromDays(int.TryParse(FrequencyTextBox.Text, out var freq) ? freq : 0);

                System.Windows.MessageBox.Show("Task updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(FrequencyTextBox.Text) || string.IsNullOrWhiteSpace(DueDateTextBox.Text))
                {
                    System.Windows.MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Task = new MaintenanceTask
                {
                    Name = TaskNameTextBox.Text,
                    Frequency = TimeSpan.FromDays(int.Parse(FrequencyTextBox.Text)),
                    DueDate = DueDateTextBox.SelectedDate.GetValueOrDefault(DateTime.Now.AddDays(7)),
                    Description = DescriptionTextBox.Text,
                };

                System.Windows.MessageBox.Show("Task added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
            Close();
        }
    }
}
