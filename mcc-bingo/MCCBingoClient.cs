using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MCCBingoCommon;
using Newtonsoft.Json;

namespace MCCBingo;

public class MCCBingoClient
{
    public event Action<CommandPacket> CommandReceived;
    public TcpClient Client { get; }
    
    public static MCCBingoClient Instance { get; private set; }

    public MCCBingoClient()
    {
        Instance = this;
        Client = new TcpClient();
        Client.Connect(IPAddress.Parse("127.0.0.1"), 6422);
        Thread thread = new Thread(ClientThread);
        thread.Start();
    }

    private async void ClientThread()
    {
        while (Client.Connected)
        {
            while (!Client.GetStream().DataAvailable);
            byte[] buffer = new byte[Client.Available];
            int n = await Client.GetStream().ReadAsync(buffer);
            string json = Encoding.UTF8.GetString(buffer);
            CommandPacket packet = JsonConvert.DeserializeObject<CommandPacket>(json);
            CommandReceived?.Invoke(packet);
        }
    }

    public async Task SendCommand(Command command, string message)
    {
        CommandPacket commandPacket = new CommandPacket(command, message);
        string dataString = JsonConvert.SerializeObject(commandPacket);
        byte[] byteData = Encoding.UTF8.GetBytes(dataString);
        await Client.GetStream().WriteAsync(byteData);
    }

    public async Task Close()
    {
        await SendCommand(Command.DisconnectClient, "");
        Client.Close();
    }
}