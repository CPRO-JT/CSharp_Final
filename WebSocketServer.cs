using System;
using System.IO;
using Newtonsoft.Json;

public class WebSocketServer
{
    public string StoragePath { get; set; } = Environment.GetEnvironmentVariable("USERPROFILE");
    public string ServerIP { get; set; } = "127.0.0.1";
    public int ServerPort { get; set; } = 42069;

    public WebSocketServer()
    {
        LoadSettings();
    }

    public void LoadSettings()
    {
        if (File.Exists("amt_server_settings.json"))
        {
            var json = File.ReadAllText("amt_server_settings.json");
            var settings = JsonConvert.DeserializeObject<WebSocketServerSettings>(json);
            if (settings != null)
            {
                StoragePath = settings.StoragePath;
                ServerIP = settings.ServerIP;
                ServerPort = settings.ServerPort;
            }
        }
    }

    public void SaveSettings()
    {
        var settings = new WebSocketServerSettings
        {
            StoragePath = StoragePath,
            ServerIP = ServerIP,
            ServerPort = ServerPort
        };
        var json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("amt_server_settings.json", json);
    }

    public void SyncWithWebApp()
    {
        // Implementation for syncing with web app
        Console.WriteLine("Syncing data with web application...");
    }
}

public class WebSocketServerSettings
{
    public string StoragePath { get; set; }
    public string ServerIP { get; set; }
    public int ServerPort { get; set; }
}