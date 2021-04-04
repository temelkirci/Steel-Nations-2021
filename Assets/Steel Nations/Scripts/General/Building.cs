using UnityEngine;
using UnityEngine.UI;

public class Building
{
    public string buildingName;

    public int maxBuildingInCity;
    public int leftConstructionDay;
    public int requiredEmployee;
    public int requiredManpower;

    public int constructionCost;
    public int constructionTime;
    public int incomeMonthly;

    public string buildingDescription;
    public BUILDING_TYPE buildingType;

    public Texture2D buildingImage;

    public Building(string name, int constTime, int constCost, int income, int employee, int manpower, int maxBuilding, string description)
    {
        buildingName = name;
        leftConstructionDay = 0;
        incomeMonthly = income;
        constructionCost = constCost;
        constructionTime = constTime;
        buildingDescription = description;
        requiredEmployee = employee;
        requiredManpower = manpower;
        maxBuildingInCity = maxBuilding;

        buildingImage = Resources.Load("buildings/" + buildingName) as Texture2D;

        buildingType = WorldMapStrategyKit.BuildingManager.Instance.GetBuildingTypeByBuildingName(name);
    }
}
