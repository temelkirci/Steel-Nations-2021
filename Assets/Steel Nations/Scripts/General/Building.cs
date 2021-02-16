public class Building
{
    public string buildingName;
    public int maxBuildingInCity;
    public int currentBuildingInCity;
    public int leftConstructionDay;
    public int constructionCost;
    public int constructionTime;
    public int incomePerWeek;
    public int expensePerWeek;
    public string buildingDescription;
    public int buildingLevel;
    public BUILDING_TYPE buildingType;
    
    public Building(string name, int level, BUILDING_TYPE type, int current, int max, int income, int expense, int cost, int time, string description)
    {
        buildingName = name;
        maxBuildingInCity = max;
        currentBuildingInCity = current;
        leftConstructionDay = 0;
        incomePerWeek = income;
        expensePerWeek = expense;
        constructionCost = cost;
        constructionTime = time;
        buildingDescription = description;
        buildingType = type;
        buildingLevel = level;
    }

    public Building Clone()
    {
        Building building = new Building(buildingName, buildingLevel, buildingType, currentBuildingInCity, maxBuildingInCity, incomePerWeek, expensePerWeek, constructionCost, constructionTime, buildingDescription);
        building.leftConstructionDay = 0;

        return building;
    }
}
