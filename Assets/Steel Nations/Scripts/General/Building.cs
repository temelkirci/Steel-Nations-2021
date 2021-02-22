public class Building
{
    public string buildingName;
    public int maxBuildingInCity;
    public int currentBuildingInCity;
    public int leftConstructionDay;
    public int constructionCost;
    public int constructionTime;
    public int incomeMonthly;
    public int expenseMonthly;
    public string buildingDescription;
    public int buildingLevel;
    public BUILDING_TYPE buildingType;
    
    public Building(string name, int level, BUILDING_TYPE type, int current, int income, int expense, int cost, int time, string description)
    {
        buildingName = name;
        currentBuildingInCity = current;
        leftConstructionDay = 0;
        incomeMonthly = income;
        expenseMonthly = expense;
        constructionCost = cost;
        constructionTime = time;
        buildingDescription = description;
        buildingType = type;
        buildingLevel = level;
    }

    public Building Clone()
    {
        Building building = new Building(buildingName, buildingLevel, buildingType, currentBuildingInCity, incomeMonthly, expenseMonthly, constructionCost, constructionTime, buildingDescription);
        building.leftConstructionDay = 0;

        return building;
    }
}
