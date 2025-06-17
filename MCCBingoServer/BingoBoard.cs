using MCCBingoServer.Tasks;
using MCCStatTracker;

namespace MCCBingoServer;

public class BingoBoard
{
    public readonly IBingoTask[] Tasks = new IBingoTask[25];
    
    private readonly MCCPlayerTracker _playerTracker;
    private readonly TrackedPlayer _trackedPlayer;
    
    private readonly List<int> _tasksCompleted = new ();
    
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
        Random random = new Random();
        IBingoTask task = new PlayGamesTask();
        task.InitializeStatistics(_playerTracker, _trackedPlayer);
        switch (random.Next(0, 2))
        {
            case 0:
                task = new ParkourMedalsTask();
                task.InitializeStatistics(_playerTracker, _trackedPlayer);
                return task;
            case 1:
                task = new PlayGamesTask();
                task.InitializeStatistics(_playerTracker, _trackedPlayer);
                return task;
                
        }
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