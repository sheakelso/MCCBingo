
namespace MCCBingoCommon;

public enum Command
{
    ConnectClient,
    ConnectResult,
    DisconnectClient,
    RequestBoard,
    BingoBoard,
    TasksCompleted
}

public struct CommandPacket
{
    public Command Command { get; set; }
    public string Message { get; set; }

    public CommandPacket(Command command, string message)
    {
        Command = command;
        Message = message;
    }
}

public enum ConnectResult
{
    Success,
    PlayerDoesNotExist
}