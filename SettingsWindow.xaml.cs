using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace MaintenanceApp
{
    public partial class SettingsWindow : Window
    {
        private WebSocketServer webSocketServer;
        private ApplianceManager applianceManager = ApplianceManager.getInstance();

        public SettingsWindow()
        {
            InitializeComponent();
            webSocketServer = new WebSocketServer();
            StoragePathBox.Text = webSocketServer.StoragePath;
            ServerIPBox.Text = webSocketServer.ServerIP;
            ServerPortBox.Text = webSocketServer.ServerPort.ToString();
        }

        public bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            webSocketServer.StoragePath = StoragePathBox.Text;
            if (ValidateIPv4(ServerIPBox.Text))
            {
                webSocketServer.ServerIP = ServerIPBox.Text;
            } else
            {
                System.Windows.MessageBox.Show("IP is not in a valid IPv4/IPv6 format.");
                return;
            }
            webSocketServer.ServerIP = ServerIPBox.Text;
            if (int.TryParse(ServerPortBox.Text, out int port))
            {
                if (port > 65535 || port < 1)
                {
                    System.Windows.MessageBox.Show("Port is not within the valid range.");
                    return;
                }
                webSocketServer.ServerPort = port;
            }
            webSocketServer.SaveSettings();
            System.Windows.MessageBox.Show("Settings saved successfully.");
            Close();
        }

        private void ExportCSV_Click(object sender, RoutedEventArgs e)
        {
            // Open a SaveFileDialog to select the CSV file location
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Export Appliance Data to CSV",
                FileName = "ApplianceData.csv"
            };

            DialogResult dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    var filePath = saveFileDialog.FileName;
                    ExportApplianceDataToCsv(filePath);
                    System.Windows.MessageBox.Show("Appliance data successfully exported to CSV!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    System.Windows.MessageBox.Show($"An error occurred while exporting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else
            {
                System.Windows.MessageBox.Show("Failed to Export", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportApplianceDataToCsv(string filePath)
        {
            var appliances = applianceManager.GetAllAppliances(); // Fetch all appliances
            var csvBuilder = new StringBuilder();

            // Add the header row
            csvBuilder.AppendLine("Appliance Name,Brand,Model Name,Model Number,Serial Number,Details,Tasks");

            foreach (var appliance in appliances)
            {
                // Format appliance details
                var applianceDetails = $"{appliance.Name},{appliance.Brand},{appliance.ModelName},{appliance.ModelNumber},{appliance.Details},{appliance.SerialNumber}";

                // Format tasks
                var tasks = appliance.Tasks.Select(task =>
                    $"{task.Name} (Due: {task.DueDate:yyyy-MM-dd}, Frequency: {task.Frequency}, Description: {task.Description})");

                // Combine appliance details with tasks
                var row = $"{applianceDetails},\"{string.Join("; ", tasks)}\"";
                csvBuilder.AppendLine(row);
            }

            // Write to the specified file
            File.WriteAllText(filePath, csvBuilder.ToString());
        }
    }
}
