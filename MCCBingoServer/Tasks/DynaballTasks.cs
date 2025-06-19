using MCCStatTracker;

namespace MCCBingoServer.Tasks;

public class DynaballEliminatePlayersTask : StatIncreaseTask
{
    public DynaballEliminatePlayersTask(Random random)
    {
        Stat = ("dynaball_players_eliminated", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 11);
        Title = $"Eliminate {IncreaseNeeded} players in {GlobalUtils.GameDisplayNames[Game.DYNABALL]}";
    }
}

public class DynaballWinGamesTask : StatIncreaseTask
{
    public DynaballWinGamesTask(Random random)
    {
        Stat = ("dynaball_wins", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 4);
        Title = $"Win {IncreaseNeeded} games of {GlobalUtils.GameDisplayNames[Game.DYNABALL]}";
    }
}

public class DynaballPlayersStuckTask : StatIncreaseTask
{
    public DynaballPlayersStuckTask(Random random)
    {
        Stat = ("dynaball_players_stuck", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 9);
        Title = $"Stick {IncreaseNeeded} players in {GlobalUtils.GameDisplayNames[Game.DYNABALL]}";
    }
}

public class DynaballDestroyBlocksTask : StatIncreaseTask
{
    public DynaballDestroyBlocksTask(Random random)
    {
        Stat = ("dynaball_blocks_destroyed", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(2, 6) * 1000;
        Title = $"Destroy {IncreaseNeeded} blocks in {GlobalUtils.GameDisplayNames[Game.DYNABALL]}";
    }
}

public class DynaballPlaceBlocksTask : StatIncreaseTask
{
    public DynaballPlaceBlocksTask(Random random)
    {
        Stat = ("dynaball_blocks_placed", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 6) * 100;
        Title = $"Place {IncreaseNeeded} blocks in {GlobalUtils.GameDisplayNames[Game.DYNABALL]}";
    }
}

public class DynaballDestroySpawnersTask : StatIncreaseTask
{
    public DynaballDestroySpawnersTask(Random random)
    {
        Stat = ("dynaball_spawners_destroyed", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 6);
        Title = $"Destroy {IncreaseNeeded} spawners in {GlobalUtils.GameDisplayNames[Game.DYNABALL]}";
    }
}