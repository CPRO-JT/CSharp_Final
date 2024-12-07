using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp
{
    // This class represents the window for managing appliances and their associated tasks
    public partial class ApplianceManagementWindow : Window
    {
        // Manages maintenance tasks for appliances
        private MaintenanceTaskManager taskManager = new MaintenanceTaskManager();

        // Manages appliances (adding, updating, deleting)
        private ApplianceManager applianceManager;

        // The appliance being edited or created
        private Appliance _editingAppliance;

        // Flag to determine if the current operation is editing an appliance
        private bool _isEditing;

        // Constructor for creating a new appliance
        public ApplianceManagementWindow(ApplianceManager manager)
        {
            InitializeComponent();

            // Assign the passed appliance manager instance
            applianceManager = manager;

            // Create a new appliance instance
            _editingAppliance = new Appliance();

            // Bind the DataGrid to the appliance's tasks
            TasksDataGrid.ItemsSource = _editingAppliance.Tasks;
        }

        // Constructor for editing an existing appliance
        public ApplianceManagementWindow(Appliance applianceToEdit, ApplianceManager manager)
        {
            InitializeComponent();

            // Assign the passed appliance manager instance
            applianceManager = manager;

            // Assign the appliance to be edited
            _editingAppliance = applianceToEdit;

            // Set the flag to indicate editing mode
            _isEditing = true;

            // Bind the DataGrid to the appliance's tasks
            TasksDataGrid.ItemsSource = _editingAppliance.Tasks;

            // Pre-fill the input fields with the existing appliance details
            ApplianceNameBox.Text = applianceToEdit.Name;
            BrandBox.Text = applianceToEdit.Brand;
            ModelNameBox.Text = applianceToEdit.ModelName;
            ModelNumberBox.Text = applianceToEdit.ModelNumber;
            SerialNumberBox.Text = applianceToEdit.SerialNumber;
            DetailsBox.Text = applianceToEdit.Details;
        }

        // Event handler for opening the context menu in the DataGrid
        private void TasksDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Dynamically create the context menu
            var contextMenu = new ContextMenu();

            // Add "Add Task" menu item to the context menu
            var addTaskMenuItem = new MenuItem { Header = "Add Task" };
            addTaskMenuItem.Click += AddTaskMenuItem_Click; // Attach event handler
            contextMenu.Items.Add(addTaskMenuItem);

            // Add "Remove Task" menu item to the context menu
            var removeTaskMenuItem = new MenuItem { Header = "Remove Task" };
            removeTaskMenuItem.Click += RemoveTaskMenuItem_Click; // Attach event handler
            contextMenu.Items.Add(removeTaskMenuItem);

            // Attach the context menu to the DataGrid
            TasksDataGrid.ContextMenu = contextMenu;
        }

        // Event handler for adding a new task to the appliance
        private void AddTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Create the TaskWindow for the current appliance
            var taskWindow = new TaskWindow(_editingAppliance);

            // Show the TaskWindow as this is required for the window to be displayed.
            taskWindow.ShowDialog();

            // Refresh the DataGrid to display the updated task list
            RefreshTaskGrid();
        }

        // Event handler for removing a selected task from the appliance
        private void RemoveTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Ensure there are tasks to remove
            if (_editingAppliance.Tasks.Count < 1) { return; }

            // Check if a task is selected in the DataGrid
            if (TasksDataGrid.SelectedItem is MaintenanceTask selectedTask)
            {
                // Remove the selected task
                _editingAppliance.Tasks.Remove(selectedTask);
                RefreshTaskGrid(); // Refresh the DataGrid
            }
            else
            {
                // Show an error if no task is selected
                System.Windows.MessageBox.Show("Please select a task to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for saving an appliance (add or update)
        private void SaveAppliance_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing) // If editing an existing appliance
            {
                // Update the appliance's properties with the values from the input fields
                _editingAppliance.Name = ApplianceNameBox.Text;
                _editingAppliance.Brand = BrandBox.Text;
                _editingAppliance.ModelNumber = ModelNumberBox.Text;
                _editingAppliance.ModelName = ModelNameBox.Text;
                _editingAppliance.SerialNumber = SerialNumberBox.Text;
                _editingAppliance.Details = DetailsBox.Text;

                // Refresh the task grid to ensure the tasks are displayed correctly
                RefreshTaskGrid();

                // Save the changes to the appliance
                applianceManager.UpdateAppliance(_editingAppliance);

                // Notify the user of success
                System.Windows.MessageBox.Show("Appliance updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else // If adding a new appliance
            {
                // Create a new appliance with the values from the input fields
                var newAppliance = new Appliance
                {
                    Name = ApplianceNameBox.Text,
                    Brand = BrandBox.Text,
                    ModelNumber = ModelNumberBox.Text,
                    ModelName = ModelNameBox.Text,
                    SerialNumber = SerialNumberBox.Text,
                    Details = DetailsBox.Text,
                };

                // Add the new appliance using the appliance manager
                applianceManager.AddAppliance(newAppliance);

                // Notify the user of success
                System.Windows.MessageBox.Show("Appliance saved successfully.");
            }

            // Close the window after saving, notifying the caller that we executed with no errors 
            DialogResult = true;
            Close();
        }

        // Refreshes the DataGrid to display the updated tasks
        private void RefreshTaskGrid()
        {
            TasksDataGrid.ItemsSource = null; // Clear the current binding
            TasksDataGrid.ItemsSource = _editingAppliance.Tasks; // Re-bind to the updated tasks list
        }
    }
}
