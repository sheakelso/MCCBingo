using System.Collections.Generic;
using Godot;
using MCCBingoCommon;
using Newtonsoft.Json;

namespace MCCBingo;

public class BingoData
{
    public BingoTask[] Tasks { get; set; }
}

public class BingoTask
{
    public string Title { get; set; }
}

public partial class BingoSheet : Node
{
    public static BingoInfo CurrentBingoInfo;

    [Export] public PackedScene BingoSpacePrefab;

    private MCCBingoClient _client;
    private BingoData _bingoData;
    
    private RichTextLabel[] _labels;
    
    public override void _Ready()
    {
        if (MCCBingoClient.Instance == null)
        {
            ReturnToMenu();
            return;
        }

        if (MCCBingoClient.Instance.Client.Connected == false)
        {
            ReturnToMenu();
            return;
        }
        
        _client = MCCBingoClient.Instance;
        _client.CommandReceived += OnCommandReceived;

        _client.SendCommand(Command.RequestBoard, JsonConvert.SerializeObject(CurrentBingoInfo));
    }

    void ReturnToMenu()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }

    void OnCommandReceived(CommandPacket packet)
    {
        switch (packet.Command)
        {
            case Command.BingoBoard:
                _bingoData = JsonConvert.DeserializeObject<BingoData>(packet.Message);
                CreateBingoBoard();
                break;
            case Command.TasksCompleted:
                int[] indexes = JsonConvert.DeserializeObject<int[]>(packet.Message);
                GD.Print("Tasks completed");
                foreach (int i in indexes)
                {
                    GD.Print($"Task {i} completed!");
                    _labels[i].CallDeferred(RichTextLabel.MethodName.SetText, "Completed!");
                }
                break;
        }
    }

    void CreateBingoBoard()
    {
        _labels = new RichTextLabel[_bingoData.Tasks.Length];
        int i = 0;
        foreach (BingoTask task in _bingoData.Tasks)
        {
            Node space = BingoSpacePrefab.Instantiate();
            ((RichTextLabel)space.GetChild(0)).Text = task.Title;
            _labels[i] = (RichTextLabel)space.GetChild(0);
            CallDeferred(Node.MethodName.AddChild, space);
            i++;
        }
    }
}