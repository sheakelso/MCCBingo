namespace MCCStatTracker;

public static class GlobalUtils
{
    public static readonly Dictionary<Game, string> GameStatNames = new ()
    {
        {Game.PARKOUR_WARRIOR, "pw_survival"},
        {Game.DYNABALL, "dynaball"},
        {Game.ROCKET_SPLEEF, "rocket_spleef"},
        {Game.TGTTOS, "tgttos"},
        {Game.BATTLE_BOX, "battle_box_quads"},
        {Game.SKY_BATTLE, "sky_battle_quads"},
        {Game.HOLE_IN_THE_WALL, "hole_in_the_wall"}
    };
    
    public static readonly Dictionary<Game, string> GameDisplayNames = new ()
    {
        {Game.PARKOUR_WARRIOR, "Parkour Warrior Survivor"},
        {Game.DYNABALL, "Dynaball"},
        {Game.ROCKET_SPLEEF, "Rocket Spleef Rush"},
        {Game.TGTTOS, "TGTTOS"},
        {Game.BATTLE_BOX, "Battle Box"},
        {Game.SKY_BATTLE, "Sky Battle"},
        {Game.HOLE_IN_THE_WALL, "Hole in the Wall"}
    };
}