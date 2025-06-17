using System.Net.Sockets;
using System.Text;
using MCCBingoCommon;
using MCCStatTracker;
using Newtonsoft.Json;

namespace MCCBingoServer;

public class BingoPlayer
{
    public string Username { get; }
    public TcpClient Client { get; }
    
    private BingoBoard? _board;
    
    private readonly MCCPlayerTracker _tracker;
    
    private readonly TrackedPlayer _trackedPlayer;

    public BingoPlayer(string username, TcpClient client, MCCPlayerTracker tracker)
    {
        Username = username;
        Client = client;
        _trackedPlayer = new TrackedPlayer(username);

        _tracker = tracker;
        _tracker.StartTracking(_trackedPlayer);
        _tracker.PlayerDataUpdated += OnPlayerDataUpdated;

        Thread thread = new Thread(PlayerThread);
        thread.Start();
    }

    private bool _playerConnected;
    private bool _boardInitialized;
    private async void OnPlayerDataUpdated(Dictionary<string, Player> playerData)
    {
        if (!_playerConnected)
        {
            if (playerData.ContainsKey(Username))
            {
                await SendCommand(Command.ConnectResult, ConnectResult.Success.ToString());
                Console.WriteLine($"{Username} connected to the server successfully.");
            }
            else
            {
                await SendCommand(Command.ConnectResult, ConnectResult.PlayerDoesNotExist.ToString());
                Console.WriteLine($"{Username} tried to connect, but the username does not exist.");
            }
            _playerConnected = true;
        }

        if (!_boardInitialized && _board != null)
        {
            _board.InitializeTasks();
            _boardInitialized = true;
            Console.WriteLine($"Board initialized for {Username}.");
            string json = JsonConvert.SerializeObject(_board);
            await SendCommand(Command.BingoBoard, json);
            Console.WriteLine(json);
        }

        if (_boardInitialized && _playerConnected)
        {
            int[] completions = _board.CheckTasks();
            if (completions.Length > 0)
            {
                await SendCommand(Command.TasksCompleted, JsonConvert.SerializeObject(completions));
            }
        }
    }

    async void PlayerThread()
    {
        NetworkStream stream = Client.GetStream();
        while (Client.Connected)
        {
            while (!stream.DataAvailable);
            byte[] buffer = new byte[Client.Available];
            _ = await stream.ReadAsync(buffer, 0, buffer.Length);
            string data = Encoding.UTF8.GetString(buffer);
            CommandPacket packet = JsonConvert.DeserializeObject<CommandPacket>(data);
            OnCommandReceived(packet);
        }
    }

    public async Task SendCommand(Command command, string message)
    {
        CommandPacket commandPacket = new CommandPacket(command, message);
        string dataString = JsonConvert.SerializeObject(commandPacket);
        byte[] byteData = Encoding.UTF8.GetBytes(dataString);
        await Client.GetStream().WriteAsync(byteData);
    }

    public void OnCommandReceived(CommandPacket packet)
    {
        switch (packet.Command)
        {
            case Command.DisconnectClient:
                Disconnect();
                break;
            case Command.RequestBoard:
                Console.WriteLine($"{Username} requested a board.");
                CreateBoard();
                break;
        }
    }

    void CreateBoard()
    {
        _board = new BingoBoard(_tracker, _trackedPlayer);
    }

    public void Disconnect()
    {
        lock (MCCBingoServer.Players)
        {
            Client.Close();
            _tracker.StopTracking(_trackedPlayer);
            Console.WriteLine($"{Username} disconnected.");
            MCCBingoServer.Players.Remove(this);
        }
    }
}