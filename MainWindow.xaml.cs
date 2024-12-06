using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace MaintenanceApp
{
    public partial class MainWindow : Window
    {
        private ApplianceManager applianceManager;

        public ObservableCollection<Appliance> Appliances { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            applianceManager = new ApplianceManager();
            Appliances = new ObservableCollection<Appliance>(applianceManager.GetAllAppliances());
            ApplianceDataGrid.ItemsSource = Appliances;
        }

        private void LoadAppliances()
        {
            ApplianceDataGrid.ItemsSource = applianceManager.GetAllAppliances();
        }

        private void ApplianceDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (ApplianceDataGrid.SelectedItem == null)
            {
                e.Handled = true;
                return;
            }

            // Create the context menu dynamically
            var contextMenu = new ContextMenu();

            // Add "Edit Appliance" menu item
            var editApplianceMenuItem = new MenuItem { Header = "Edit Appliance" };
            editApplianceMenuItem.Click += EditApplianceMenuItem_Click;
            contextMenu.Items.Add(editApplianceMenuItem);

            // Add "Delete Appliance" menu item
            var deleteApplianceMenuItem = new MenuItem { Header = "Delete Appliance" };
            deleteApplianceMenuItem.Click += DeleteApplianceMenuItem_Click;
            contextMenu.Items.Add(deleteApplianceMenuItem);

            // Attach the context menu to the DataGrid
            ApplianceDataGrid.ContextMenu = contextMenu;
        }

        private void EditApplianceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ApplianceDataGrid.SelectedItem is Appliance selectedAppliance)
            {
                //System.Windows.MessageBox.Show(selectedAppliance.Name);
                var editingWindow = new ApplianceManagementWindow(selectedAppliance, applianceManager); // Assuming AddApplianceWindow can also handle edits.
                if (editingWindow.ShowDialog() == true)
                {
                    RefreshAppliances();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an appliance to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteApplianceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ApplianceDataGrid.SelectedItem is Appliance selectedAppliance)
            {
                var result = System.Windows.MessageBox.Show($"Are you sure you want to delete the appliance '{selectedAppliance.Name}'?",
                                             "Confirm Deletion",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    applianceManager.DeleteAppliance(selectedAppliance.Id);
                    RefreshAppliances();
                    System.Windows.MessageBox.Show("Appliance deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an appliance to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAppliance_Click(object sender, RoutedEventArgs e)
        {
            var applianceWindow = new ApplianceManagementWindow(applianceManager);
            applianceWindow.ShowDialog();
            RefreshAppliances();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void RefreshAppliances()
        {
            Appliances.Clear();
            foreach (var appliance in applianceManager.GetAllAppliances())
            {
                Appliances.Add(appliance);
            }
        }
    }
}
