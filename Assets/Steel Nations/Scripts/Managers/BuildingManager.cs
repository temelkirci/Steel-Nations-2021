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
            if (city.population < 10000)
                return;

            string countryName = map.GetCityCountryName(city);
            Country country = CountryManager.Instance.GetCountryByName(countryName);

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
                        int desiredBuildingNumber = 1;
                        //int desiredBuildingNumber = city.CityIncome / (GetBuildingIncomeMonthly(BUILDING_TYPE.TRADE_PORT));
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
                    if (country.GetArmy() != null)
                    {
                        if (country.Defense_Budget > 5000 && city.Constructible_Dockyard_Area == 1 && city.cityClass != CITY_CLASS.COUNTRY_CAPITAL)
                        {
                            if (IsPossibleConstructionInCity(city, building.buildingType))
                            {
                                city.AddBuilding(building.buildingType, 1);
                                Add3DBuilding(country, city, MY_UNIT_TYPE.DOCKYARD);
                            }
                        }
                    }
                }

                if (building.buildingType == BUILDING_TYPE.GARRISON)
                {
                    if(CountryManager.Instance.GetCountryByName(countryName).GetArmy() != null)
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
                    if (country.GetArmy() != null)
                    {
                        if(country.Defense_Budget > 5000)
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
                    }
                }

                if (building.buildingType == BUILDING_TYPE.NUCLEAR_FACILITY)
                {
                    if(CountryManager.Instance.GetCountryByName(countryName).Nuclear_Power == true)
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
            
            if(city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                Add3DBuilding(country, city, MY_UNIT_TYPE.COUNTRY_CAPITAL_BUILDING);

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
        /*
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
        */

        /*
         map.AddMarker2DSprite(star, planeLocation, 0.02f, true);
        MarkerClickHandler handler = star.GetComponent<MarkerClickHandler>();
        handler.OnMarkerMouseDown += (buttonIndex => Debug.Log("Click on sprite with button " +
        buttonIndex + "!"));
        */

        public void Add3DBuilding(Country country, City city, MY_UNIT_TYPE unitType)
        {
            GameObject go = null;
            GameObjectAnimator building = null;

            if (MY_UNIT_TYPE.DOCKYARD == unitType)
            {
                go = Instantiate(HarborBuilding);

                go.transform.localScale = Misc.Vector3one * 0.0005f;

                building = go.WMSK_MoveTo(city.unity2DLocation);
                building.name = city.name + " Dockyard";
                
                city.Dockyard = building;

                city.Dockyard.OnPointerEnter += (GameObjectAnimator anim) => GameEventHandler.Instance.GetPlayer().SetMouseOverUnit(anim);
                city.Dockyard.OnPointerExit += (GameObjectAnimator anim) => GameEventHandler.Instance.GetPlayer().SetMouseOverUnit(null);
                city.Dockyard.OnPointerDown += (GameObjectAnimator anim) => MapManager.Instance.BuildingSelection(country, anim);

            }
            else if (unitType == MY_UNIT_TYPE.COUNTRY_CAPITAL_BUILDING)
            {
                go = Instantiate(CapitalBuilding);

                go.transform.localScale = Misc.Vector3one * 0.01f;

                building = go.WMSK_MoveTo(city.unity2DLocation);
                building.name = country.name + " Capital";

                city.Capital_Building = building;

                city.Capital_Building.OnPointerEnter += (GameObjectAnimator anim) => GameEventHandler.Instance.GetPlayer().SetMouseOverUnit(anim);
                city.Capital_Building.OnPointerExit += (GameObjectAnimator anim) => GameEventHandler.Instance.GetPlayer().SetMouseOverUnit(null);
                city.Capital_Building.OnPointerDown += (GameObjectAnimator anim) => MapManager.Instance.BuildingSelection(country, anim);
            }

            if(go != null && building != null)
            {
                go.isStatic = true;

                building.visible = false;

                building.type = (int)unitType;
                building.pivotY = 0.5f;
                building.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter 
                building.updateWhenOffScreen = false;
                building.enabled = false;
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
