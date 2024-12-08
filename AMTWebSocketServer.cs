using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;


public class AMTWebSocketServer : WebSocketBehavior
{
    // Set the storage path to the user home directory
    public string StoragePath { get; set; } = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\amt_server_settings.json";
    // Set the ip we listen on to a local loopback address and a funny joke
    public string DefaultServerIP { get; set; } = "127.0.0.1";
    public string ServerIP { get; set; } = "127.0.0.1";
    // Set the port we listen on to a funny joke
    public int DefaultServerPort { get; set; } = 42069;
    public int ServerPort { get; set; } = 42069;

    private WebSocketServer webSocketServer { get; set; }

    public AMTWebSocketServer()
    {
        LoadSettings(); // Load AMTWebSocketServer settings
    }

    public void Start()
    {
        Thread t = new Thread(CreateServer); // create a new thread for the server
        t.IsBackground = true;
        t.Start(); // start the thread
    }

    public void CreateServer()
    {
        try
        {
            webSocketServer = new WebSocketServer($"ws://{ServerIP}:{ServerPort}");
            webSocketServer.AddWebSocketService<AMTWebSocketServer>("/amt");
            webSocketServer.Start();
            System.Windows.MessageBox.Show($"WebSocket Server is listening at ws://{ServerIP}:{ServerPort}");
        }
        catch (ArgumentException e)
        {
            System.Windows.MessageBox.Show($"Invalid ServerIP or ServerPort!\nRestoring default server settings!\n\n{e}");
            ServerIP = DefaultServerIP;
            ServerPort = DefaultServerPort;
            SaveSettings();
            CreateServer();
        }
    }

    public void StopServer()
    {
        webSocketServer.Stop();
    }

    public void RestartServer()
    {
        StopServer();
        Start();
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        System.Windows.MessageBox.Show($"{e.Data}");
        Send("Hello from AMT Desktop!");
    }

    protected override void OnOpen()
    {
        System.Windows.MessageBox.Show("Client Connected!");
        Send("Hello from AMT Desktop!");
    }

    /*private void CreateServer()
    {
        // Set this to the background so it closes the server when the application closes.
        Thread.CurrentThread.IsBackground = true; 

        // Create the server that will listen on ServerIP and ServerPort
        TcpListener server = new TcpListener(IPAddress.Parse(ServerIP), ServerPort);

        server.Start(); // Start the listener
        MessageBox.Show($"WebSocket Server is listening at ws://{ServerIP}:{ServerPort}\n\nWarning: This WSS does not have TLS capabilities outside of the minimum required by the RFC 6455 proposal standard.");

        TcpClient client = server.AcceptTcpClient(); // Start accepting Client connections

        Console.WriteLine("A client connected."); // Say it in the console, don't bug the user with a messagebox every time they connect.

        NetworkStream stream = client.GetStream(); // Get the Client Network Stream

        // enter to an infinite cycle to be able to handle every change in stream
        while (true)
        {
            while (!stream.DataAvailable) ;
            while (client.Available < 3) ; // match against "get"

            byte[] bytes = new byte[client.Available];  // create an array the size of all data the client has made available so far
            stream.Read(bytes, 0, bytes.Length); // read the whole stream
            string s = Encoding.UTF8.GetString(bytes); // get the string of bytes

            if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase)) // If the message is a GET request, we craft a handshake response
            {
                Console.WriteLine("=====Handshaking from client=====\n{0}", s);

                // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                // 3. Compute SHA-1 and Base64 hash of the new value
                // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                string swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                string swkAndSalt = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                byte[] swkAndSaltSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swkAndSalt));
                string swkAndSaltSha1Base64 = Convert.ToBase64String(swkAndSaltSha1);

                // HTTP/1.1 defines the sequence CR LF as the end-of-line marker, so we gotta go it
                byte[] response = Encoding.UTF8.GetBytes(
                    "HTTP/1.1 101 Switching Protocols\r\n" +
                    "Connection: Upgrade\r\n" +
                    "Upgrade: websocket\r\n" +
                    "Sec-WebSocket-Accept: " + swkAndSaltSha1Base64 + "\r\n\r\n");

                stream.Write(response, 0, response.Length); // Send the response to the client.
            }
            else
            {
                bool fin = (bytes[0] & 0b10000000) != 0, // Tells us when the full message has been sent from the client
                    mask = (bytes[1] & 0b10000000) != 0; // tells us if the payload is masked. this must be true, "All messages from the client to the server have this bit set" not manditory for server to client
                int opcode = bytes[0] & 0b00001111; // expecting 1 - text message, we only support text opcodes currently.
                ulong offset = 2,
                      msglen = bytes[1] & (ulong)0b01111111; // length of the message so we know how to decode it

                if (msglen == 126) // tells us the following two bits (127 and 128) are a 16-bit unsigned integer that holds the payload length
                {
                    // bytes are reversed because websocket will print them in Big-Endian, whereas
                    // BitConverter will want them arranged in little-endian on windows
                    msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                    offset = 4;
                }
                else if (msglen == 127) // tells us the following 8 bits (interpreted as a 64-bit unsigned integer) are the payload length
                {
                    // We need to manually buffer larger messages since the NIC's autobuffering
                    // may be too latency-friendly for this code to run properly
                    msglen = BitConverter.ToUInt64(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] }, 0); //Most significant bit must be zero.
                    offset = 10;
                }

                if (msglen == 0) // If its 0 then do nothing as nothing was sent. Odd concept, sending nothing from client to server.
                {
                    Console.WriteLine("msglen == 0");
                }
                else if (mask) // the message is 0-125 bytes in length, this is probably payload data, check if its masked as we recieved it from the client and it must be masked.
                {
                    // This is a mess that I will comment better when i fix all the bugs. So far it seems to work but I don't trust it quite yet.
                    byte[] decoded = new byte[msglen];
                    byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                    offset += 4;

                    for (ulong i = 0; i < msglen; ++i)
                        decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                    string text = Encoding.UTF8.GetString(decoded); // decode the bytes into text
                    Console.WriteLine("{0}", text); // print the client message to the console
                }
                else // this could be a payload but its not masked, or it could be something else that we don't implement the necessary support for
                    Console.WriteLine("mask bit not set");

                Console.WriteLine(); // new line
            }
        }
    }*/

    public void LoadSettings()
    {
        if (File.Exists("amt_server_settings.json")) // check for the file
        {
            var json = File.ReadAllText("amt_server_settings.json"); // read it if it exists
            var settings = JsonConvert.DeserializeObject<WebSocketServerSettings>(json); // convert the json to something we can use in C#
            if (settings != null) // if there are settings, set them to the current environment
            {
                StoragePath = settings.StoragePath;
                ServerIP = settings.ServerIP;
                ServerPort = settings.ServerPort;
            }
        }
    }

    public void SaveSettings()
    {
        var settings = new WebSocketServerSettings // we make a new settings object with this environments settings
        {
            StoragePath = StoragePath,
            ServerIP = ServerIP,
            ServerPort = ServerPort
        };
        var json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented); // turn it back into json
        File.WriteAllText("amt_server_settings.json", json); // save the json settings to file
    }

    public void SyncWithWebApp()
    {
        // Implementation for syncing with web app
        Console.WriteLine("Syncing data with web application...");
    }
}

public class WebSocketServerSettings
{
    // Path we store the server settings at
    public string StoragePath { get; set; }
    
    // server ip we listen on
    public string ServerIP { get; set; }

    // server port we listen on
    public int ServerPort { get; set; }
}