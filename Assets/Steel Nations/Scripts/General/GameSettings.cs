
public static class GameSettings
{
    public enum GAME_SPEED
    {
        GAME_SPEED_x1,
        GAME_SPEED_x2,
        GAME_SPEED_x4,
        GAME_SPEED_x8
    }

    public static int gameSpeed;

    public static int landUnitProductionSpeed;
    public static int airUnitProductionSpeed;
    public static int navalUnitProductionSpeed;

    public static int starterOilReserves;
    public static int starterUraniumReserves;
    public static int starterIronReserves;
    public static int starterSteelReserves;
    public static int starterAluminiumReserves;

    public static void SetGameSettings(string key, int value)
    {
        if (key == "Game Speed")
            gameSpeed = value;
        if (key == "Land Unit Production Speed")
            landUnitProductionSpeed = value;
        if (key == "Air Unit Production Speed")
            airUnitProductionSpeed = value;
        if (key == "Naval Unit Production Speed")
            navalUnitProductionSpeed = value;
        if (key == "Starter Oil Reserve")
            starterOilReserves = value;
        if (key == "Starter Uranium Reserve")
            starterUraniumReserves = value;
        if (key == "Starter Iron Reserve")
            starterIronReserves = value;
        if (key == "Starter Steel Reserve")
            starterSteelReserves = value;
        if (key == "Starter Aluminium Reserve")
            starterAluminiumReserves = value;       
    }
}
