using MCCStatTracker;

namespace MCCBingoServer.Tasks;

public class ParkourWarriorEliminatePlayersTask : StatIncreaseTask
{
    public ParkourWarriorEliminatePlayersTask(Random random)
    {
        Stat = ("pw_survival_players_eliminated", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(5, 21);
        Title = $"Eliminate {IncreaseNeeded} players in {GlobalUtils.GameDisplayNames[Game.PARKOUR_WARRIOR]}";
    }
}

public class ParkourWarriorWinLeapsTask : StatIncreaseTask
{
    public ParkourWarriorWinLeapsTask(Random random)
    {
        Stat = ("pw_survival_leap_champions", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 4);
        Title = $"Win {IncreaseNeeded} leaps in {GlobalUtils.GameDisplayNames[Game.PARKOUR_WARRIOR]}";
    }
}

public class ParkourWarriorReachLeapTask : StatIncreaseTask
{
    public ParkourWarriorReachLeapTask(Random random)
    {
        int leap = random.Next(5, 8);
        Stat = ($"pw_survival_leap_{leap}_completion", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 4);
        Title = $"Reach leap {leap} {IncreaseNeeded} times in {GlobalUtils.GameDisplayNames[Game.PARKOUR_WARRIOR]}";
    }
}

public class ParkourWarriorCompleteObstaclesTask : StatIncreaseTask
{
    public ParkourWarriorCompleteObstaclesTask(Random random)
    {
        Stat = ("pw_survival_obstacles_completed", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(10, 51);
        Title = $"Complete {IncreaseNeeded} obstacles in {GlobalUtils.GameDisplayNames[Game.PARKOUR_WARRIOR]}";
    }
}

public class ParkourWarriorCompletionsTask : StatIncreaseTask
{
    public ParkourWarriorCompletionsTask(Random random)
    {
        string type = new []{"standard", "advanced", "expert"}[random.Next(3)];
        Stat = ($"pw_solo_total_{type}_cmpls", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(1, 4);
        Title = $"Get {IncreaseNeeded} {type} completions in Parkour Warrior Dojo";
    }
}

public class ParkourWarriorBankMedalsTask : StatIncreaseTask
{
    public ParkourWarriorBankMedalsTask(Random random)
    {
        Stat = ("pw_solo_total_medals_banked", Rotation.LIFETIME);
        StatKey = MCCPlayerTracker.GetStatisticString(Stat);
        IncreaseNeeded = random.Next(10, 22);
        Title = $"Bank {IncreaseNeeded} medals in Parkour Warrior Dojo";
    }
}