using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace MCCStatTracker;

public class PlayerByUsernameResponse
{
    public Player? PlayerByUsername { get; set; }
}

public class PlayerResponse
{
    public Player? Player { get; set; }
}

public class Player
{
    public string Uuid { get; set; } = "";
    public string? Username { get; set; }
    public Rank[]? Ranks { get; set; }
    public MCCPlusStatus? MCCPlusStatus { get; set; }
    public CrownLevel? CrownLevel { get; set; }
    public Status? Status { get; set; }
    public Dictionary<string, int>? Statistics { get; set; }
}

public class Statistics
{
    public int? RotationValue { get; set; }
}

public class Social
{
    public Player[]? Friends { get; set; }
    public Party? Party { get; set; }
}

public class Party
{
    public bool? Active { get; set; }
    public Player? Leader { get; set; }
    public Player[]? Members { get; set; }
}

public class MCCPlusStatus
{
    public int? Evolution { get; set; }
    public DateTime? StreakStart { get; set; }
    public int? TotalDays { get; set; }
}

public class CrownLevel
{
    public LevelData? LevelData { get; set; }
    public LevelData? FishingLevelData { get; set; }
    public TrophyData? Trophies { get; set; }
}

public class TrophyData
{
    public int? Obtained { get; set; }
    public int? Obtainable { get; set; }
    public int? Bonus { get; set; }
}

public class LevelData
{
    public int? Level { get; set; }
    public int? Evolution { get; set; }
    public int? NextEvolutionLevel { get; set; }
    public ProgressionData? NextLevelProgress { get; set; }
}

public class ProgressionData
{
    public int? Obtained { get; set; }
    public int? Obtainable { get; set; }
}

public enum Rank
{
    CHAMP,
    GRAND_CHAMP,
    GRAND_CHAMP_ROYALE,
    CREATOR,
    CONTESTANT,
    MODERATOR,
    NOXCREW
}
    
public class Status
{
    public bool? Online { get; set; }
    public Server? Server { get; set; }
    public DateTime? LastJoin { get; set; }
    public DateTime? FirstJoin { get; set; }
}

public class Server
{
    public ServerCategory? Category { get; set; }
    public string? SubType { get; set; }
    public Game? AssociatedGame { get; set; }
}

public enum Game
{
    HOLE_IN_THE_WALL,
    TGTTOS,
    BATTLE_BOX,
    SKY_BATTLE,
    PARKOUR_WARRIOR,
    DYNABALL,
    ROCKET_SPLEEF
}

public enum ServerCategory
{
    Lobby,
    Game,
    Limbo,
    Queue
}

public enum Rotation
{
    DAILY,
    WEEKLY,
    MONTHLY,
    YEARLY,
    LIFETIME
}

public class TrackedPlayer
{
    public string Username { get; set; }
    public List<(string, Rotation)> TrackedStatistics { get; set; } = new ();

    public TrackedPlayer(string username)
    {
        Username = username;
    }
}

public class MCCPlayerTracker
{
    public event Action<Dictionary<string, Player>> PlayerDataUpdated;
    
    public Dictionary<string, Player> PlayerData = new();
    
    private readonly List<TrackedPlayer> _trackedPlayers = new();
    private readonly GraphQLHttpClient _client;
    private readonly int _updateInterval = 1100;

    public MCCPlayerTracker()
    {
        _client = new GraphQLHttpClient("https://api.mccisland.net/graphql", new NewtonsoftJsonSerializer());
        _client.HttpClient.DefaultRequestHeaders.Add("X-API-Key", "aLc4XrZvyB6VuDDY3zcYkfrZ8zixZNlL");
        _client.HttpClient.DefaultRequestHeaders.Add("User-Agent", "MCCBingo Discord-boopetydoopety");

        Thread thread = new Thread(UpdateThread);
        thread.Start();
    }

    public void StartTracking(TrackedPlayer trackedPlayer)
    {
        Console.WriteLine($"Starting tracking of {trackedPlayer.Username}");
        _trackedPlayers.Add(trackedPlayer);
    }

    public void StopTracking(TrackedPlayer trackedPlayer)
    {
        _trackedPlayers.Remove(trackedPlayer);
    }

    private void UpdateThread()
    {
        while (true)
        {
            Thread.Sleep(_updateInterval);
            lock (_trackedPlayers)
            {
                UpdatePlayerData();
            }
        }
    }

    private async Task UpdatePlayerData()
    {
        if(_trackedPlayers.Count == 0) return;
        Dictionary<string, Player>? playerData = await GetPlayerData();
        if (playerData == null) return;
        lock (PlayerData)
        {
            PlayerData = playerData;
        }
        PlayerDataUpdated(PlayerData);
    }

    private async Task<Dictionary<string, Player>?> GetPlayerData()
    {
        GraphQLRequest request = new()
        {
            Query = GetQuery()
        };
        GraphQLResponse<Dictionary<string, Player>> response = await _client.SendQueryAsync<Dictionary<string, Player>>(request);
        if (response.Errors != null) return null;
        return response.Data;
    }

    private string GetQuery()
    {
        if(_trackedPlayers.Count == 0) return string.Empty;
        string query = "query players {\n";
        foreach (TrackedPlayer player in _trackedPlayers)
        {
            query += GetTrackedPlayerQuery(player) + "\n";
        }
        return query + "}";
    }

    private string GetTrackedPlayerQuery(TrackedPlayer trackedPlayer)
    {
        return 
            $@"
            {trackedPlayer.Username}: playerByUsername(username: ""{trackedPlayer.Username}"") {{
                uuid
                username
                ranks
                mccPlusStatus {{
                    evolution
                    streakStart
                    totalDays
                }}
                crownLevel {{
                    levelData {{
                        level
                        evolution
                        nextEvolutionLevel
                        nextLevelProgress {{
                        obtained
                        obtainable
                        }}
                    }}
                    fishingLevelData {{
                        level
                        evolution
                        nextEvolutionLevel
                        nextLevelProgress {{
                            obtained
                            obtainable
                        }}
                    }}
                    trophies {{
                        obtained
                        obtainable
                        bonus
                    }}
                }}
                status {{
                    online
                    lastJoin
                    firstJoin
                    server {{
                        category
                        subType
                        associatedGame
                    }}
                }}
                {GetTrackedPlayerStatisticsQuery(trackedPlayer.TrackedStatistics)}     
            }}
            ";
    }

    private string GetTrackedPlayerStatisticsQuery(List<(string, Rotation)> statistics)
    {
        if(statistics.Count == 0) return string.Empty;
        string query = "statistics{\n";
        foreach ((string, Rotation) statistic in statistics)
        {
            query += @$"
                     {GetStatisticString(statistic)}: rotationValue(
                       rotation: {statistic.Item2}
                       statisticKey: ""{statistic.Item1}""
                     )
                     ";
            query += "\n";
        }
        return query + "}";
    }

    public static string GetStatisticString((string, Rotation) statistic)
    {
        return statistic.Item1 + statistic.Item2;
    }
}