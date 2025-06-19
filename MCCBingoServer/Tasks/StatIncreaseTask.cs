using MCCStatTracker;

namespace MCCBingoServer.Tasks;

public class StatIncreaseTask : IBingoTask
{
    public string Title { get; set; }

    public int InitialStat;
    public bool Initialized;

    public int IncreaseNeeded = 1;
    
    public (string, Rotation) Stat;
    public string StatKey;
    
    public void InitializeStatistics(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        (string, Rotation) statistic = Stat;
        if(!trackedPlayer.TrackedStatistics.Contains(statistic)) trackedPlayer.TrackedStatistics.Add(statistic);
    }

    public void InitializeTask(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        InitialStat = playerTracker.PlayerData[trackedPlayer.Username].Statistics[StatKey];
        Initialized = true;
    }

    public bool IsTaskComplete(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        if(!Initialized) return false;
        return playerTracker.PlayerData[trackedPlayer.Username].Statistics[StatKey] >= InitialStat + IncreaseNeeded;
    }
}