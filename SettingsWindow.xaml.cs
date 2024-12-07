using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace MaintenanceApp
{
    // SettingsWindow class handles the settings for WebSocket server configuration and exporting appliance data to a CSV file
    public partial class SettingsWindow : Window
    {
        // WebSocket server instance for handling server settings
        private WebSocketServer webSocketServer = MainWindow.webSocketServer;

        // Appliance manager to manage appliance data
        private ApplianceManager applianceManager = new ApplianceManager();

        // Constructor initializes the window and sets initial values for settings
        public SettingsWindow()
        {
            InitializeComponent(); // Initializes the UI components defined in XAML

            // Set initial values for server settings from the WebSocketServer instance.
            StoragePathBox.Text = webSocketServer.StoragePath;          //Displays the current storage path used by the WebSocket server
            ServerIPBox.Text = webSocketServer.ServerIP;                //shows the current IP address of the WebSocket server
            ServerPortBox.Text = webSocketServer.ServerPort.ToString(); //displays the server's port number
        }

        // Validates if the given IP address is in a valid IPv4 format
        public bool ValidateIPv4(string ipString)
        {
            // Check if the string is empty or null
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false; // Invalid if it's null or empty
            }

            string[] splitValues = ipString.Split('.'); // Split the string by '.' to check each part of the IP
            if (splitValues.Length != 4)
            {
                return false; // Invalid if it doesn't have 4 parts
            }

            // Temporary variable for validating each part of the IP to see if it can be parsed into a valid byte (0-255)
            byte tempForParsing;
            // Uses LINQ method (All) and lambda function to check all array elements meet byte requirement
            return splitValues.All(r => byte.TryParse(r, out tempForParsing)); // r = array element
        }

        // Event handler for saving the settings when the "Save" button is clicked.
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Save the storage path
            webSocketServer.StoragePath = StoragePathBox.Text;

            // Validate and save the Server IP
            if (ValidateIPv4(ServerIPBox.Text)) // Validate the server IP format.
            {
                webSocketServer.ServerIP = ServerIPBox.Text;
            }
            else
            {
                // Show error message if the IP is not valid.
                System.Windows.MessageBox.Show("IP is not in a valid IPv4/IPv6 format.");
                return;
            }

            // Validate and save the Server Port
            if (int.TryParse(ServerPortBox.Text, out int port)) // Try parsing the port number.
            {
                if (port > 65535 || port < 1) // Check if the port is within the valid range.
                {
                    // Show error message if the port is out of range.
                    System.Windows.MessageBox.Show("Port is not within the valid range.");
                    return;
                }
                webSocketServer.ServerPort = port; // Save the valid port.
            }

            // Save the settings to a file or database
            webSocketServer.SaveSettings();
            // Show success message
            System.Windows.MessageBox.Show("Settings saved successfully.");
            Close(); // Close the settings window after saving the settings.
        }

        // Event handler for exporting appliance data to a CSV file.
        private void ExportCSV_Click(object sender, RoutedEventArgs e)
        {
            // Open a SaveFileDialog to allow the user to select the location for the CSV file.
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv", // Set file filter to only show CSV files.
                Title = "Export Appliance Data to CSV", // Dialog title.
                FileName = "ApplianceData.csv" // Default file name.
            };

            DialogResult dialogResult = saveFileDialog.ShowDialog(); // Show the dialog and get the result.

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    // Get the selected file path
                    var filePath = saveFileDialog.FileName;
                    // Export appliance data to CSV at the selected file path
                    ExportApplianceDataToCsv(filePath);
                    // Show success message after exporting
                    System.Windows.MessageBox.Show("Appliance data successfully exported to CSV!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    // Show error message if an issue occurred during the export
                    System.Windows.MessageBox.Show($"An error occurred while exporting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Show failure message if the user canceled the file export
                System.Windows.MessageBox.Show("Failed to Export", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper method for exporting appliance data to CSV format
        private void ExportApplianceDataToCsv(string filePath)
        {
            // Fetch all appliances from the appliance manager
            var appliances = applianceManager.GetAllAppliances();

            // Initialize a StringBuilder to construct the CSV file
            var csvBuilder = new StringBuilder(); //StringBuilder (dynamic resizing, string manipulation w/o creating new string instance)

            // Add the header row for the CSV file
            csvBuilder.AppendLine("Appliance Name,Brand,Model Name,Model Number,Serial Number,Details,Tasks");

            foreach (var appliance in appliances)
            {
                // Format appliance details into a CSV-friendly string
                var applianceDetails = $"{appliance.Name},{appliance.Brand},{appliance.ModelName},{appliance.ModelNumber},{appliance.Details},{appliance.SerialNumber}";

                // Format the appliance tasks as a string with details
                var tasks = appliance.Tasks.Select(task =>
                    $"{task.Name} (Due: {task.DueDate:yyyy-MM-dd}, Frequency: {task.Frequency}, Description: {task.Description})");

                // Combine appliance details with task information
                var row = $"{applianceDetails},\"{string.Join("; ", tasks)}\"";
                csvBuilder.AppendLine(row); // Add the row to the CSV builder
            }

            // Write the final CSV content to the specified file
            File.WriteAllText(filePath, csvBuilder.ToString());
        }
    }
}
