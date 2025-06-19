using MCCBingoCommon;
using MCCBingoServer.Tasks;
using MCCStatTracker;

namespace MCCBingoServer;

public interface IBingoTaskProvider
{
    public IBingoTask[] GetTasks(Random random);
}

public class NormalBingoTaskProvider : IBingoTaskProvider
{
    public IBingoTask[] GetTasks(Random random)
    {
        IBingoTask[] tasks = new IBingoTask[25];
        for (int i = 0; i < tasks.Length; i++)
        {
            IBingoTask task = new PlayGamesTask(random);
            switch (random.Next(0, 2))
            {
                case 0:
                    task = new ParkourMedalsTask(random);
                    tasks[i] = task;
                    break;
                case 1:
                    task = new PlayGamesTask(random);
                    tasks[i] = task;
                    break;
            }
            tasks[i] = task;
        }
        return tasks;
    }
}

public class DynaballBingoTaskProvider : IBingoTaskProvider
{
    public Type[] DynaballTaskTypes = 
    [
        typeof(DynaballWinGamesTask),
        typeof(DynaballDestroyBlocksTask),
        typeof(DynaballDestroySpawnersTask),
        typeof(DynaballEliminatePlayersTask),
        typeof(DynaballPlaceBlocksTask),
        typeof(DynaballPlayersStuckTask)
    ];
    
    public IBingoTask[] GetTasks(Random random)
    {
        IBingoTask[] tasks = new IBingoTask[25];
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = (IBingoTask)DynaballTaskTypes[random.Next(0, DynaballTaskTypes.Length)].GetConstructor([typeof(Random)])?.Invoke([random]);
        }
        return tasks;
    }
}

public class BingoBoard
{
    public static Dictionary<Mode, IBingoTaskProvider> TaskProviders = new ()
    {
        { Mode.Normal, new NormalBingoTaskProvider() },
        { Mode.DynaballOnly, new DynaballBingoTaskProvider()}
    };
    public readonly IBingoTask[] Tasks;
    
    private readonly MCCPlayerTracker _playerTracker;
    private readonly TrackedPlayer _trackedPlayer;
    
    private readonly List<int> _tasksCompleted = new ();
    
    private readonly Random _random;
    
    public BingoBoard(MCCPlayerTracker playerTracker, TrackedPlayer trackedPlayer, BingoInfo bingoInfo)
    {
        _random = new Random(bingoInfo.Seed);
        _playerTracker = playerTracker;
        _trackedPlayer = trackedPlayer;
        Tasks = TaskProviders[bingoInfo.Mode].GetTasks(_random);
        foreach (var task in Tasks)
        {
            task.InitializeStatistics(playerTracker, trackedPlayer);
        }
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