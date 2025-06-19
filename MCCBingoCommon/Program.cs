
namespace MCCBingoCommon;

public enum Command
{
    ConnectClient,
    ConnectResult,
    DisconnectClient,
    RequestBoard,
    BingoBoard,
    TasksCompleted,
    RequestModeOptions,
    ModeOptions
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

public class BingoInfo
{
    public int Seed  { get; set; }
    public Mode Mode { get; set; }

    public BingoInfo(int seed, Mode mode)
    {
        Seed = seed;
        Mode = mode;
    }
}

public enum ConnectResult
{
    Success,
    PlayerDoesNotExist
}

public enum Mode
{
    Normal,
    DynaballOnly
}