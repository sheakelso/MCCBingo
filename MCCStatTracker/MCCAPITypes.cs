namespace MCCStatTracker;

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