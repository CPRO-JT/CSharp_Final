using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp
{
    public partial class ApplianceManagementWindow : Window
    {
        private MaintenanceTaskManager taskManager = new MaintenanceTaskManager();
        private ApplianceManager applianceManager;
        private Appliance _editingAppliance;
        private bool _isEditing;

        public ApplianceManagementWindow(ApplianceManager manager)
        {
            InitializeComponent();
            applianceManager = manager;
            _editingAppliance = new Appliance();
            TasksDataGrid.ItemsSource = _editingAppliance.Tasks;
        }

        public ApplianceManagementWindow(Appliance applianceToEdit, ApplianceManager manager)
        {
            InitializeComponent();
            applianceManager = manager;
            _editingAppliance = applianceToEdit;
            TasksDataGrid.ItemsSource = _editingAppliance.Tasks;
            _isEditing = true;

            // Pre-fill the fields with appliance details
            ApplianceNameBox.Text = applianceToEdit.Name;
            BrandBox.Text = applianceToEdit.Brand;
            ModelNameBox.Text = applianceToEdit.ModelName;
            ModelNumberBox.Text = applianceToEdit.ModelNumber;
            SerialNumberBox.Text = applianceToEdit.SerialNumber;
            DetailsBox.Text = applianceToEdit.Details;

            // Load tasks into the DataGrid
            TasksDataGrid.ItemsSource = _editingAppliance.Tasks;
        }

        private void TasksDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Create the context menu dynamically
            var contextMenu = new ContextMenu();

            // Add "Add Task" menu item
            var addTaskMenuItem = new MenuItem { Header = "Add Task" };
            addTaskMenuItem.Click += AddTaskMenuItem_Click;
            contextMenu.Items.Add(addTaskMenuItem);

            // Add "Remove Task" menu item
            var removeTaskMenuItem = new MenuItem { Header = "Remove Task" };
            removeTaskMenuItem.Click += RemoveTaskMenuItem_Click;
            contextMenu.Items.Add(removeTaskMenuItem);

            // Attach the context menu to the DataGrid
            TasksDataGrid.ContextMenu = contextMenu;
        }

        private void AddTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var taskWindow = new TaskWindow(_editingAppliance);
            taskWindow.ShowDialog();
            RefreshTaskGrid(); // Refresh the DataGrid
        }

        private void RemoveTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_editingAppliance.Tasks.Count < 1) { return; }

            if (TasksDataGrid.SelectedItem is MaintenanceTask selectedTask)
            {
                _editingAppliance.Tasks.Remove(selectedTask);
                RefreshTaskGrid();
            }
            else
            {
                System.Windows.MessageBox.Show("Please select a task to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAppliance_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                // Update the existing appliance
                _editingAppliance.Name = ApplianceNameBox.Text;
                _editingAppliance.Brand = BrandBox.Text;
                _editingAppliance.ModelNumber = ModelNumberBox.Text;
                _editingAppliance.ModelName = ModelNameBox.Text;
                _editingAppliance.SerialNumber = SerialNumberBox.Text;
                _editingAppliance.Details = DetailsBox.Text;

                // Update tasks
                RefreshTaskGrid();

                // Save changes
                applianceManager.UpdateAppliance(_editingAppliance);
                System.Windows.MessageBox.Show("Appliance updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var newAppliance = new Appliance
                {
                    Name = ApplianceNameBox.Text,
                    Brand = BrandBox.Text,
                    ModelNumber = ModelNumberBox.Text,
                    ModelName = ModelNameBox.Text,
                    SerialNumber = SerialNumberBox.Text,
                    Details = DetailsBox.Text,
                };

                applianceManager.AddAppliance(newAppliance);
                System.Windows.MessageBox.Show("Appliance saved successfully.");
            }

            DialogResult = true;
            Close();
        }

        private void RefreshTaskGrid()
        {
            TasksDataGrid.ItemsSource = null; // Clear the binding
            TasksDataGrid.ItemsSource = _editingAppliance.Tasks; // Re-bind to the updated Tasks list
        }
    }
}
