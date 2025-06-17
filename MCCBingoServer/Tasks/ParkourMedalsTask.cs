using MCCStatTracker;

namespace MCCBingoServer.Tasks;

public class ParkourMedalsTask : IBingoTask
{
    public string Title { get; }

    private int _initialMedals;
    private bool _initialized;

    private readonly int _medalsNeeded;
    
    private readonly (string, Rotation) _medalsStat = ("pw_solo_total_medals_banked", Rotation.LIFETIME);
    private readonly string _statKey = MCCPlayerTracker.GetStatisticString(("pw_solo_total_medals_banked", Rotation.LIFETIME));

    public ParkourMedalsTask()
    {
        Random random = new Random();
        _medalsNeeded = random.Next(10, 21);
        Title = $"Earn {_medalsNeeded} medals in Parkour Warrior Dojo";
    }
    
    public void InitializeStatistics(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        (string, Rotation) statistic = _medalsStat;
        if(!trackedPlayer.TrackedStatistics.Contains(statistic)) trackedPlayer.TrackedStatistics.Add(statistic);
    }

    public void InitializeTask(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        _initialMedals = playerTracker.PlayerData[trackedPlayer.Username].Statistics[_statKey];
        _initialized = true;
    }

    public bool IsTaskComplete(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        if(!_initialized) return false;
        return playerTracker.PlayerData[trackedPlayer.Username].Statistics[_statKey] >= _initialMedals + _medalsNeeded;
    }
}