namespace OriDE.RandoStatsCollector.Enums
{

    public enum DictEnums
    {
        SeedParamsDict,
        PPMDict,
        PickupsDict
    }
    
    public enum StatParserEnums
    {
        // First line of stats.txt, seed related stuff
        LogicMode = 0,
        KeyMode ,
        GoalMode,
        FillAlg,
        Pool,
        Seed,
        
        // PPM of areas etc
        Glades = 100,
        Grove, 
        Grotto,
        Blackroot,
        Swamp,
        Ginso ,
        Valley ,
        Misty,
        Forlorn,
        Sorrow,
        Horu,
        Misc,
        Total,
        
        // Important item locations etc
        WallJump = 200,
        Dash,
        DoubleJump,
        Bash,
        Stomp,
        ChargeFlame,
        Glide,
        ChargeJump,
        Climb,
        Grenade,
        WaterVein,
        GumonSeal,
        Sunstone,
        CleanWater,
        WindRestored,
        WarmthReturned
    }
}