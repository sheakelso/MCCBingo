using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using MCCBingoCommon;
using MCCStatTracker;
using Newtonsoft.Json;

namespace MCCBingoServer;

static class MCCBingoServer
{
    private static readonly TcpListener TcpListener = new (IPAddress.Parse("127.0.0.1"), 6422);
    public static readonly List<BingoPlayer> Players = new();
    public static readonly MCCPlayerTracker MCCPlayerTracker = new ();
    
    public static async Task Main()
    {
        Console.Title = "MCCBingoServer";
        Console.WriteLine("Starting MCCBingoServer...");
        TcpListener.Start();
        await AcceptClients();
    }

    public static async Task AcceptClients()
    {
        Console.WriteLine("Accepting clients...");
        while (true)
        {
            TcpClient tcpClient = await TcpListener.AcceptTcpClientAsync();
            AcceptPlayer(tcpClient);
        }
    }

    public static async void AcceptPlayer(TcpClient tcpClient)
    {
        NetworkStream stream = tcpClient.GetStream();
        while (!stream.DataAvailable);
        byte[] buffer = new byte[tcpClient.Available];
        _ = await stream.ReadAsync(buffer, 0, buffer.Length);
        string data = Encoding.UTF8.GetString(buffer);
        CommandPacket packet = JsonConvert.DeserializeObject<CommandPacket>(data);
        if (packet.Command != Command.ConnectClient)
        {
            tcpClient.Close();
            return;
        }
        Players.Add(new BingoPlayer(packet.Message, tcpClient, MCCPlayerTracker));
    }
}