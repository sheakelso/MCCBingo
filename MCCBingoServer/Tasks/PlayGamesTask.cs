using MCCStatTracker;

namespace MCCBingoServer.Tasks;

public class PlayGamesTask : IBingoTask
{
    public string Title { get; }

    private int _initialGames;
    private bool _initialized;

    private readonly int _gamesNeeded;
    
    private readonly (string, Rotation) _gamesStat;
    private readonly string _statKey;

    public PlayGamesTask()
    {
        Random random = new Random();
        Game game = (Game)random.Next(0, Enum.GetValues(typeof(Game)).Length);
        _gamesStat = (GlobalUtils.GameStatNames[game] + "_games_played", Rotation.LIFETIME);
        _statKey = MCCPlayerTracker.GetStatisticString(_gamesStat);
        _gamesNeeded = random.Next(1, 3);
        Title = $"Play {_gamesNeeded} games of {GlobalUtils.GameDisplayNames[game]}";
    }
    
    public void InitializeStatistics(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        (string, Rotation) statistic = _gamesStat;
        if(!trackedPlayer.TrackedStatistics.Contains(statistic)) trackedPlayer.TrackedStatistics.Add(statistic);
    }

    public void InitializeTask(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        _initialGames = playerTracker.PlayerData[trackedPlayer.Username].Statistics[_statKey];
        _initialized = true;
    }

    public bool IsTaskComplete(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        if(!_initialized) return false;
        return playerTracker.PlayerData[trackedPlayer.Username].Statistics[_statKey] >= _initialGames + _gamesNeeded;
    }
}