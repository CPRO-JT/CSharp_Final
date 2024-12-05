using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp
{
    public partial class ApplianceManagementWindow : Window
    {
        private ApplianceManager applianceManager = ApplianceManager.getInstance();
        private List<MaintenanceTask> _tasks;
        private Appliance _editingAppliance;
        private bool _isEditing;

        public ApplianceManagementWindow()
        {
            InitializeComponent();
            _tasks = new List<MaintenanceTask>();
            TasksDataGrid.ItemsSource = _tasks;
        }

        public ApplianceManagementWindow(Appliance applianceToEdit) : this()
        {
            InitializeComponent();
            _isEditing = true;
            _editingAppliance = applianceToEdit;

            // Pre-fill the fields with appliance details
            ApplianceNameBox.Text = applianceToEdit.Name;
            BrandBox.Text = applianceToEdit.Brand;
            ModelNameBox.Text = applianceToEdit.ModelName;
            ModelNumberBox.Text = applianceToEdit.ModelNumber;
            SerialNumberBox.Text = applianceToEdit.SerialNumber;
            DetailsBox.Text = applianceToEdit.Details;

            // Load tasks into the DataGrid
            TasksDataGrid.ItemsSource = applianceToEdit.Tasks;
        }

        private void TasksDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Create the context menu dynamically
            var contextMenu = new ContextMenu();

            // Add "Add Task" menu item
            var addTaskMenuItem = new MenuItem { Header = "Add Task" };
            addTaskMenuItem.Click += AddTaskMenuItem_Click;
            contextMenu.Items.Add(addTaskMenuItem);

            // Add "Edit Task" menu item
            var editTaskMenuItem = new MenuItem { Header = "Edit Task" };
            editTaskMenuItem.Click += EditTaskMenuItem_Click;
            contextMenu.Items.Add(editTaskMenuItem);

            // Add "Remove Task" menu item
            var removeTaskMenuItem = new MenuItem { Header = "Remove Task" };
            removeTaskMenuItem.Click += RemoveTaskMenuItem_Click;
            contextMenu.Items.Add(removeTaskMenuItem);

            // Attach the context menu to the DataGrid
            TasksDataGrid.ContextMenu = contextMenu;
        }

        private void AddTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var taskWindow = new TaskWindow();
            if (taskWindow.ShowDialog() == true)
            {
                if (taskWindow.Tag is MaintenanceTask newTask)
                {
                    var tasks = TasksDataGrid.ItemsSource as List<MaintenanceTask>;
                    tasks?.Add(newTask);
                    TasksDataGrid.Items.Refresh(); // Refresh the DataGrid
                }
            }
        }

        private void EditTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is MaintenanceTask selectedTask)
            {
                var taskWindow = new TaskWindow(selectedTask);
                if (taskWindow.ShowDialog() == true)
                {
                    TasksDataGrid.Items.Refresh(); // Refresh the DataGrid
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select a task to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_tasks.Count < 1) { return; }

            if (TasksDataGrid.SelectedItem is MaintenanceTask selectedTask)
            {
                _tasks.Remove(selectedTask);
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
                _editingAppliance.Tasks = TasksDataGrid.ItemsSource as List<MaintenanceTask>;

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
                    Details = DetailsBox.Text
                };

                applianceManager.AddAppliance(newAppliance);
                System.Windows.MessageBox.Show("Appliance saved successfully.");
            }

            DialogResult = true;
            Close();
        }

        private void RefreshTaskGrid()
        {
            TasksDataGrid.Items.Refresh();
        }
    }
}
