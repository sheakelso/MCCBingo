using MCCStatTracker;

namespace MCCBingoServer;

public interface IBingoTask
{
    public string Title { get; }
    public void InitializeStatistics(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer);
    public void InitializeTask(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer);
    public bool IsTaskComplete(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer);
}

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

public class BingoBoard
{
    public readonly IBingoTask[] Tasks = new IBingoTask[25];
    
    private readonly MCCPlayerTracker _playerTracker;
    private readonly TrackedPlayer _trackedPlayer;
    
    private List<int> _tasksCompleted = new ();
    
    public BingoBoard(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer)
    {
        _playerTracker = playerTracker;
        _trackedPlayer = trackedPlayer;
        for (int i = 0; i < Tasks.Length; i++)
        {
            Tasks[i] = CreateTask();
        }
    }
    
    private IBingoTask CreateTask()
    {
        IBingoTask task = new ParkourMedalsTask();
        task.InitializeStatistics(_playerTracker, _trackedPlayer);
        return task;
    }

    public void InitializeTasks()
    {
        for (int i = 0; i < Tasks.Length; i++)
        {
            Tasks[i].InitializeTask(_playerTracker, _trackedPlayer);
        }
    }

    public int[] CheckTasks()
    {
        List<int> newCompletedTasks = new();
        for (int i = 0; i < Tasks.Length; i++)
        {
            if (Tasks[i].IsTaskComplete(_playerTracker, _trackedPlayer) && !_tasksCompleted.Contains(i))
            {
                _tasksCompleted.Add(i);
                newCompletedTasks.Add(i);
                Console.WriteLine($"Task {i} completed");
            }
        }
        return newCompletedTasks.ToArray();
    }
}