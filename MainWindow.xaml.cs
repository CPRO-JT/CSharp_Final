using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace MaintenanceApp
{
    public partial class MainWindow : Window
    {
        // Initialize WebSocketServer
        public static WebSocketServer webSocketServer = new WebSocketServer();

        // Manages appliance data and actions like add, update, and delete
        private ApplianceManager applianceManager;

        // Defines collection bound to the DataGrid to display appliances
        public ObservableCollection<Appliance> Appliances { get; set; }

        // Constructor for MainWindow
        public MainWindow()
        {
            // Initializes UI components defined in the XAML file
            InitializeComponent();

            // Instantiate the ApplianceManager to manage appliance data
            applianceManager = new ApplianceManager();

            // Populate the ObservableCollection with appliances fetched from the ApplianceManager
            Appliances = new ObservableCollection<Appliance>(applianceManager.GetAllAppliances());

            // Bind the DataGrid to the ObservableCollection to display appliance data
            ApplianceDataGrid.ItemsSource = Appliances;

            // Start the WebSocketServer
            webSocketServer.Start();
        }

        // Method to load appliances from the ApplianceManager into the DataGrid
        private void LoadAppliances()
        {
            // Fetch all appliances from the ApplianceManager and set them as the ItemsSource for the DataGrid
            ApplianceDataGrid.ItemsSource = applianceManager.GetAllAppliances();
        }

        // Event handler for right-clicking (context menu opening) on the DataGrid
        private void ApplianceDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // If no appliance is selected, prevent the context menu from opening
            if (ApplianceDataGrid.SelectedItem == null)
            {
                // Stop further handling of the event
                e.Handled = true;
                return;
            }

            // Create the context menu dynamically
            var contextMenu = new ContextMenu();

            // Add "Edit Appliance" menu item to the context menu
            var editApplianceMenuItem = new MenuItem { Header = "Edit Appliance" };
            editApplianceMenuItem.Click += EditApplianceMenuItem_Click; // Attach event handler.
            contextMenu.Items.Add(editApplianceMenuItem);

            // Add "Delete Appliance" menu item to the context menu
            var deleteApplianceMenuItem = new MenuItem { Header = "Delete Appliance" };
            deleteApplianceMenuItem.Click += DeleteApplianceMenuItem_Click; // Attach event handler.
            contextMenu.Items.Add(deleteApplianceMenuItem);

            // Attach the created context menu to the DataGrid
            ApplianceDataGrid.ContextMenu = contextMenu;
        }

        // Event handler for editing an appliance when the "Edit Appliance" menu item is clicked
        private void EditApplianceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that an appliance is selected in the DataGrid
            if (ApplianceDataGrid.SelectedItem is Appliance selectedAppliance)
            {
                // Open the ApplianceManagementWindow for editing the selected appliance
                var editingWindow = new ApplianceManagementWindow(selectedAppliance, applianceManager);

                // Refresh the appliance list after closing the editing window if changes were made
                if (editingWindow.ShowDialog() == true)
                {
                    RefreshAppliances();
                }
            }
            else
            {
                // Display an error if no appliance is selected
                System.Windows.MessageBox.Show("Please select an appliance to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for deleting an appliance when the "Delete Appliance" menu item is clicked
        private void DeleteApplianceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that an appliance is selected in the DataGrid
            if (ApplianceDataGrid.SelectedItem is Appliance selectedAppliance)
            {
                // Show a confirmation dialog before deleting the appliance.
                var result = System.Windows.MessageBox.Show($"Are you sure you want to delete the appliance '{selectedAppliance.Name}'?",
                                                            "Confirm Deletion",
                                                            MessageBoxButton.YesNo,
                                                            MessageBoxImage.Warning);

                // If the user confirms, delete the appliance and refresh the list
                if (result == MessageBoxResult.Yes)
                {
                    // Delete the appliance using its ID
                    applianceManager.DeleteAppliance(selectedAppliance.Id);
                    // Refresh the DataGrid to reflect changes
                    RefreshAppliances();

                    // Display a success message after deletion
                    System.Windows.MessageBox.Show("Appliance deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                // Display an error if no appliance is selected
                System.Windows.MessageBox.Show("Please select an appliance to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for adding a new appliance when the "Add Appliance" button is clicked
        private void AddAppliance_Click(object sender, RoutedEventArgs e)
        {
            // Open the ApplianceManagementWindow to add a new appliance
            var applianceWindow = new ApplianceManagementWindow(applianceManager);

            // Show the window as a dialog, then refresh the appliance list if changes were made
            applianceWindow.ShowDialog();
            RefreshAppliances();
        }

        // Event handler for opening the settings window when the "Settings" button is clicked
        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            // Open the SettingsWindow for configuring application settings
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        // Method to refresh the appliance list displayed in the DataGrid
        private void RefreshAppliances()
        {
            // Clear the existing ObservableCollection
            Appliances.Clear();

            // Fetch the updated list of appliances and add them to the ObservableCollection
            foreach (var appliance in applianceManager.GetAllAppliances())
            {
                Appliances.Add(appliance); // The ObservableCollection automatically updates the UI
            }
        }
    }
}
