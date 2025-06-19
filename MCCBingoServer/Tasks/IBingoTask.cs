using MCCStatTracker;

namespace MCCBingoServer.Tasks;

public interface IBingoTask
{
    public string Title { get; set; }
    public void InitializeStatistics(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer);
    public void InitializeTask(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer);
    public bool IsTaskComplete(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer);
}