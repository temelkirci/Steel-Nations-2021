using UnityEngine;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public enum CITY_CLASS : byte {
		CITY = 1,
		REGION_CAPITAL = 2,
		COUNTRY_CAPITAL = 4
	}

	public partial class City: IExtendableAttribute {

        Dictionary<BUILDING_TYPE, int> buildingListInCity = null;
        Dictionary<BUILDING_TYPE, int> buildingsInConstruction = null;
        List<GameObjectAnimator> divisionsInCity = null;
        GameObjectAnimator dockyard = null;

        Dictionary<MINERAL_TYPE, int> mineralReserves = null;

        int cityIncome;
        float cityLevel;
        int constructibleDockyardArea;
        int unemployed;

        public float CityLevel
        {
            get { return cityLevel; }
            set { cityLevel = value; }
        }

        public int CityIncome
        {
            get { return cityIncome; }
            set { cityIncome = value; }
        }

        public int GetFactoryIncome()
        {
            return GetBuildingNumber(BUILDING_TYPE.FACTORY) * BuildingManager.Instance.GetBuildingIncomeMonthly(BUILDING_TYPE.FACTORY);
        }

        public int GetTradePortIncome()
        {
            return GetBuildingNumber(BUILDING_TYPE.FACTORY) * BuildingManager.Instance.GetBuildingIncomeMonthly(BUILDING_TYPE.FACTORY);
        }

        public int Unemployed
        {
            get { return unemployed; }
            set { unemployed = value; }
        }

        public int GetMineralResources(MINERAL_TYPE mineralType)
        {
            int number = 0;
            mineralReserves.TryGetValue(mineralType, out number);
            return number;
        }

        public void SetMineralResources(MINERAL_TYPE mineralType, int number)
        {
            if (mineralReserves == null)
                mineralReserves = new Dictionary<MINERAL_TYPE, int>();
            mineralReserves.Add(mineralType, number);
        }

        public GameObjectAnimator Dockyard
        {
            get { return dockyard; }
            set { dockyard = value; }
        }

        public int Constructible_Dockyard_Area
        {
            get { return constructibleDockyardArea; }
            set { constructibleDockyardArea = value; }
        }

        public void StartConstructionInCity(BUILDING_TYPE building)
        {
            int buildingTime = BuildingManager.Instance.GetBuildingByBuildingType(building).constructionTime;
            buildingsInConstruction.Add(building, buildingTime);
        }

        public Dictionary<BUILDING_TYPE, int> GetAllBuildingsInConstruction()
        {
            return buildingsInConstruction;
        }

        public int GetBuildingNumber(BUILDING_TYPE buildingType)
        {
            int number = 0;
            if (buildingListInCity == null)
                buildingListInCity = new Dictionary<BUILDING_TYPE, int>();

            buildingListInCity.TryGetValue(buildingType, out number);

            return number;
        }
        public Dictionary<BUILDING_TYPE, int> GetAllBuildings()
        {
            return buildingListInCity;
        }
        public void AddBuilding(BUILDING_TYPE building, int number)
        {
            if (buildingListInCity == null)
                buildingListInCity = new Dictionary<BUILDING_TYPE, int>();
            buildingListInCity.Add(building, number);
        }     

        public List<GameObjectAnimator> GetAllDivisionsInCity()
        {
            return divisionsInCity;
        }

        public void VisibleDivision(GameObjectAnimator division)
        {
            division.visible = true;
            divisionsInCity.Remove(division);
        }

        

        /// <summary>
        /// An unique identifier useful to persist data between sessions. Used by serialization.
        /// </summary>
        public int uniqueId { get; set; }

        /// <summary>
        /// Use this property to add/retrieve custom attributes for this country
        /// </summary>
        public JSONObject attrib { get; set; }

        public string name;
        public string province;
        public int countryIndex;
        public Vector2 unity2DLocation;
        public int population;
        public CITY_CLASS cityClass;

        /// <summary>
        /// Set by DrawCities method.
        /// </summary>
        public bool isVisible;

        public City(string name, string province, int countryIndex, int population, Vector2 location, CITY_CLASS cityClass, int uniqueId)
        {
            this.name = name;
            this.province = province;
            this.countryIndex = countryIndex;
            this.population = population;
            this.unity2DLocation = location;
            this.cityClass = cityClass;
            this.uniqueId = uniqueId;
            this.attrib = new JSONObject();
        }

        public City Clone()
        {
            City c = new City(name, province, countryIndex, population, unity2DLocation, cityClass, uniqueId);
            c.attrib = new JSONObject();
            c.attrib.Absorb(attrib);
            return c;
        }
    }
}