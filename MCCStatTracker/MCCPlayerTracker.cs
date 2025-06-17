using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace MCCStatTracker;

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