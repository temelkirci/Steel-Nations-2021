using UnityEngine;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class BuildingManager : MonoBehaviour
    {
        private static BuildingManager instance;
        public static BuildingManager Instance
        {
            get { return instance; }
        }

        WMSK map;

        public GameObject CapitalBuilding;
        public GameObject HarborBuilding;

        List<Building> buildingList = new List<Building>();

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;           
        }
        
        public void AddBuilding(Building building)
        {
            buildingList.Add(building);
        }

        public List<Building> GetAllBuildings()
        {
            return buildingList;
        }

        public Texture2D GetBuildingImage(BUILDING_TYPE buildingType)
        {
            foreach (Building building in buildingList)
                if (building.buildingType == buildingType)
                    return building.buildingImage;
            return null;
        }

        public void SetBuildingsInCity(City city)
        {
            foreach (Building building in GetAllBuildings())
            {
                if (building.buildingType == BUILDING_TYPE.FACTORY)
                {
                    int desiredBuildingNumber = city.CityIncome / (GetBuildingIncomeMonthly(BUILDING_TYPE.FACTORY));
                    if (IsPossibleConstructionInCity(city, building.buildingType))
                    {
                        if (desiredBuildingNumber > building.maxBuildingInCity)
                            desiredBuildingNumber = building.maxBuildingInCity;
                        city.AddBuilding(building.buildingType, desiredBuildingNumber);
                    }
                }

                if (building.buildingType == BUILDING_TYPE.TRADE_PORT)
                {
                    if (city.Constructible_Dockyard_Area == 1)
                    {
                        int desiredBuildingNumber = city.CityIncome / (GetBuildingIncomeMonthly(BUILDING_TYPE.TRADE_PORT));
                        if (IsPossibleConstructionInCity(city, building.buildingType))
                        {
                            if (desiredBuildingNumber > building.maxBuildingInCity)
                                desiredBuildingNumber = building.maxBuildingInCity;
                            city.AddBuilding(building.buildingType, desiredBuildingNumber);
                        }
                    }
                }

                if (building.buildingType == BUILDING_TYPE.DOCKYARD)
                {
                    if (city.Constructible_Dockyard_Area == 1)
                    {
                        if (IsPossibleConstructionInCity(city, building.buildingType))
                        {
                            city.AddBuilding(building.buildingType, 1);
                            Add3DBuilding(city, MY_UNIT_TYPE.DOCKYARD);
                        }
                    }
                }

                if (building.buildingType == BUILDING_TYPE.GARRISON)
                {
                    if (IsPossibleConstructionInCity(city, building.buildingType))
                    {
                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            city.AddBuilding(building.buildingType, 4);
                        if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                            city.AddBuilding(building.buildingType, 2);
                        if (city.cityClass == CITY_CLASS.CITY)
                            city.AddBuilding(building.buildingType, 1);
                    }
                }


                if (building.buildingType == BUILDING_TYPE.HOSPITAL)
                {
                    int desiredBuildingNumber = city.population / building.requiredEmployee;
                    if (IsPossibleConstructionInCity(city, building.buildingType))
                    {
                        if (desiredBuildingNumber > building.maxBuildingInCity)
                            desiredBuildingNumber = building.maxBuildingInCity;

                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber);
                        if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 2);
                        if (city.cityClass == CITY_CLASS.CITY)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 3);
                    }
                }

                if (building.buildingType == BUILDING_TYPE.MINERAL_FACTORY)
                {
                    int desiredBuildingNumber = city.population / building.requiredEmployee;
                    if (IsPossibleConstructionInCity(city, building.buildingType))
                    {
                        if (desiredBuildingNumber > building.maxBuildingInCity)
                            desiredBuildingNumber = building.maxBuildingInCity;

                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 3);
                        if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 2);
                        if (city.cityClass == CITY_CLASS.CITY)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber);
                    }
                }

                if (building.buildingType == BUILDING_TYPE.MILITARY_FACTORY)
                {
                    int desiredBuildingNumber = city.population / building.requiredEmployee;
                    if (IsPossibleConstructionInCity(city, building.buildingType))
                    {
                        if (desiredBuildingNumber > building.maxBuildingInCity)
                            desiredBuildingNumber = building.maxBuildingInCity;

                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber);
                        if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 2);
                    }
                }

                if (building.buildingType == BUILDING_TYPE.NUCLEAR_FACILITY)
                {
                    if (MapManager.Instance.CityHasCoast(city))
                    {
                        int desiredBuildingNumber = 1;
                        if (IsPossibleConstructionInCity(city, building.buildingType))
                        {
                            if (desiredBuildingNumber > building.maxBuildingInCity)
                                desiredBuildingNumber = building.maxBuildingInCity;

                            if (city.cityClass != CITY_CLASS.COUNTRY_CAPITAL)
                                city.AddBuilding(building.buildingType, desiredBuildingNumber);
                        }
                    }
                }

                if (building.buildingType == BUILDING_TYPE.OIL_RAFINERY)
                {
                    int desiredBuildingNumber = city.population / building.requiredEmployee;
                    if (IsPossibleConstructionInCity(city, building.buildingType))
                    {
                        if (desiredBuildingNumber > building.maxBuildingInCity)
                            desiredBuildingNumber = building.maxBuildingInCity;

                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 3);
                        if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 2);
                        if (city.cityClass == CITY_CLASS.CITY)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber);
                    }
                }

                if (building.buildingType == BUILDING_TYPE.UNIVERSITY)
                {
                    int desiredBuildingNumber = city.population / building.requiredEmployee;
                    if (IsPossibleConstructionInCity(city, building.buildingType))
                    {
                        if (desiredBuildingNumber > building.maxBuildingInCity)
                            desiredBuildingNumber = building.maxBuildingInCity;

                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber);
                        if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 2);
                        if (city.cityClass == CITY_CLASS.CITY)
                            city.AddBuilding(building.buildingType, desiredBuildingNumber / 3);
                    }
                }
            }

            city.CityIncome = 0;
            city.CityIncome = city.CityIncome + city.GetBuildingNumber(BUILDING_TYPE.FACTORY) * GetBuildingIncomeMonthly(BUILDING_TYPE.FACTORY);
            city.CityIncome = city.CityIncome + city.GetBuildingNumber(BUILDING_TYPE.TRADE_PORT) * GetBuildingIncomeMonthly(BUILDING_TYPE.TRADE_PORT);
        }

        public bool IsPossibleConstructionInCity(City city, BUILDING_TYPE buildingType)
        {
            Building building = GetBuildingByBuildingType(buildingType);

            if (building.requiredManpower < city.population)
            {
                return true;

                /*
                if (building.requiredEmployee < city.Unemployed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                */
            }

            return false;
        }
        
        public void UpdateConstructionInCity(City city)
        {
            Dictionary<BUILDING_TYPE, int> buildingsInConstruction = city.GetAllBuildingsInConstruction();

            foreach (var building in buildingsInConstruction)
            {
                if (building.Value > 1)
                {
                    int leftDay = building.Value - 1;

                    buildingsInConstruction[building.Key] = leftDay;
                }
            }
        }
        
        public void Add3DBuilding(City city, MY_UNIT_TYPE unitType)
        {
            GameObject go = null;

            if (MY_UNIT_TYPE.DOCKYARD == unitType)
            {
                go = Instantiate(HarborBuilding);

                go.transform.localScale = Misc.Vector3one * 0.0005f;

                GameObjectAnimator anim = go.WMSK_MoveTo(city.unity2DLocation);
                anim.name = city.name + " Dockyard";
                anim.type = (int)unitType;
                anim.pivotY = 0.5f;

                anim.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter 
                anim.updateWhenOffScreen = false;
                anim.enabled = false;
                city.Dockyard = anim;
            }
            else if (unitType == MY_UNIT_TYPE.COUNTRY_CAPITAL_BUILDING)
            {
                
            }
        }

        public int GetBuildingIncomeMonthly(BUILDING_TYPE buildingType)
        {
            foreach (Building building in buildingList)
                if (building.buildingType == buildingType)
                    return building.incomeMonthly;

            return 0;
        }

        public Building GetBuildingByBuildingType(BUILDING_TYPE buildingType)
        {
            foreach (var building in buildingList)
                if (building.buildingType == buildingType)
                    return building;

            return null;
        }

        public BUILDING_TYPE GetBuildingTypeByBuildingName(string buildingName)
        {
            if (buildingName == "Airport")
                return BUILDING_TYPE.AIRPORT;
            if (buildingName == "Trade Port")
                return BUILDING_TYPE.TRADE_PORT;
            if (buildingName == "Military Factory")
                return BUILDING_TYPE.MILITARY_FACTORY;
            if (buildingName == "University")
                return BUILDING_TYPE.UNIVERSITY;
            if (buildingName == "Nuclear Facility")
                return BUILDING_TYPE.NUCLEAR_FACILITY;
            if (buildingName == "Hospital")
                return BUILDING_TYPE.HOSPITAL;
            if (buildingName == "Oil Rafinery")
                return BUILDING_TYPE.OIL_RAFINERY;
            if (buildingName == "Factory")
                return BUILDING_TYPE.FACTORY;
            if (buildingName == "Mineral Factory")
                return BUILDING_TYPE.MINERAL_FACTORY;
            if (buildingName == "Garrison")
                return BUILDING_TYPE.GARRISON;
            if (buildingName == "Dockyard")
                return BUILDING_TYPE.DOCKYARD;
            if (buildingName == "Military Base")
                return BUILDING_TYPE.MILITARY_BASE;

            return BUILDING_TYPE.NONE;
        }
    }
}
