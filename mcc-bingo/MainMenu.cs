using System;
using System.Net.Sockets;
using Godot;
using MCCBingoCommon;
using Newtonsoft.Json;

namespace MCCBingo;

public partial class MainMenu : Node
{
    [Export] public Button StartButton;
    [Export] public TextEdit UsernameField;
    [Export] public RichTextLabel ErrorLabel;
    
    MCCBingoClient _client;

    public override void _Ready()
    {
        StartButton.Pressed += OnStartButtonPressed;
    }

    public void OnStartButtonPressed()
    {
        if (_client != null) return;
        try
        {
            _client = new MCCBingoClient();
        }
        catch (SocketException e)
        {
            ErrorLabel.Text = "Unable to connect to server: " + e.Message;
            _client = null;
            return;
        }
        _client.CommandReceived += OnCommandReceived;
        _client.SendCommand(Command.ConnectClient, UsernameField.Text);
    }

    private void OnCommandReceived(CommandPacket packet)
    {
        if (packet.Command == Command.ConnectResult)
        {
            bool b = Enum.TryParse(packet.Message, out ConnectResult c);
            if (!b)
            {
                ErrorLabel.Text = "Unable to connect to server: Unknown error";
                _client.Client.Close();
                _client = null;
                return;
            }

            switch (c)
            {
                case ConnectResult.Success:
                    ErrorLabel.CallDeferred(RichTextLabel.MethodName.SetText, "Connected to server");
                    GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://Scenes/BingoScene.tscn");
                    return;
                case ConnectResult.PlayerDoesNotExist:
                    ErrorLabel.CallDeferred(RichTextLabel.MethodName.SetText, "This player does not exist on MCCI");
                    _client.Close().Wait();
                    _client = null;
                    return;
            }
        }
    }
}