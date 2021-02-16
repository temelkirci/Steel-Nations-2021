using UnityEngine;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
	public partial class City: IExtendableAttribute
    {
        int cityLevel;
        List<Building> buildingListInCity;
        GameObjectAnimator dockyard;

        int constructibleDockyardArea = 0;

        public void SetDockyard(GameObjectAnimator GOA)
        {
            dockyard = GOA;
        }
        public GameObjectAnimator GetDockyard()
        {
            return dockyard;
        }

        public int GetConstructibleDockyardAreaNumber()
        {
            return constructibleDockyardArea;
        }
        public void SetConstructibleDockyardAreaNumber(int number)
        {
            constructibleDockyardArea = number;
        }

        public void CalculateCityLevel()
        {
            cityLevel = (int)((population * 100f) / WMSK.instance.GetCountry(countryIndex).GetAvailableManpower());
        }

        public int GetCityLevel()
        {
            return cityLevel;
        }

        public void SetReserveResources(int oil, int iron, int steel, int aluminium, int uranium)
        {
            attrib["Oil"] = oil;
            attrib["Iron"] = iron;
            attrib["Steel"] = steel;
            attrib["Aluminium"] = aluminium;
            attrib["Uranium"] = uranium;
        }

        public List<Building> GetAllBuildings()
        {
            return buildingListInCity;
        }

        public int GetOilReserves()
        {
            return attrib["Oil"];
        }
        public int GetIronReserves()
        {
            return attrib["Iron"];
        }
        public int GetSteelReserves()
        {
            return attrib["Steel"];
        }
        public int GetAluminiumReserves()
        {
            return attrib["Aluminium"];
        }
        public int GetUraniumReserves()
        {
            return attrib["Uranium"];
        }

        public void StartConstruction(Building building)
        {
            if (building.leftConstructionDay == 0)
            {
                building.leftConstructionDay = building.constructionTime;
            }
        }

        public void UpdateConstructionInCity()
        {
            foreach (Building building in buildingListInCity)
            {
                if (building.leftConstructionDay == 1)
                {
                    building.currentBuildingInCity = building.currentBuildingInCity + 1;
                    building.leftConstructionDay = 0;

                    //Debug.Log(building.GetHashCode() + " -> Construction is completed");
                }

                if(building.leftConstructionDay > 1)
                {
                    building.leftConstructionDay = building.leftConstructionDay - 1;
                    //Debug.Log(building.GetHashCode() + " -> Left day for Construction is " + building.leftConstructionDay);
                }
            }
        }

        public void CreateBuilding(BUILDING_TYPE tempBuildingType, int tempCurrentBuilding, int tempMaxBuilding)
        {
            Building building = CityInfoPanel.Instance.CreateBuildingByType(tempBuildingType);

            building.maxBuildingInCity = tempMaxBuilding;
            building.currentBuildingInCity = tempCurrentBuilding;

            buildingListInCity.Add(building);
        }

        public void SetCurrentBuildingNumberInCity(BUILDING_TYPE buildingType, int number)
        {
            foreach (Building building in buildingListInCity)
            {
                if (building.buildingType == buildingType)
                {
                    building.currentBuildingInCity = number;
                    break;
                }
            }
        }

        public void SetMaxBuildingNumberInCity(BUILDING_TYPE buildingType, int number)
        {
            foreach (Building building in buildingListInCity)
            {
                if (building.buildingType == buildingType)
                {
                    building.maxBuildingInCity = number;
                    break;
                }
            }
        }

        public int GetCurrentBuildingNumberInCity(BUILDING_TYPE buildingType)
        {
            foreach (Building building in buildingListInCity)
            {
                if (building.buildingType == buildingType)
                {
                    return building.currentBuildingInCity;
                }
            }

            return 0;
        }

        public int GetMaxBuildingNumberInCity(BUILDING_TYPE buildingType)
        {
            foreach (Building building in buildingListInCity)
            {
                if (building.buildingType == buildingType)
                {
                    return building.maxBuildingInCity;
                }
            }

            return 0;
        }

        public Building GetBuildingByNameInCity(string buildingName)
        {
            foreach (Building building in buildingListInCity)
            {
                if (building.buildingName == buildingName)
                {
                    return building;
                }
            }

            return null;
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

        public void SaveCity()
        {

        }

		/// <summary>
		/// Set by DrawCities method.
		/// </summary>
		public bool isVisible;

		public City(string name, string province, int countryIndex, int population, Vector2 location, CITY_CLASS cityClass, int uniqueId) {
			this.name = name;
			this.province = province;
			this.countryIndex = countryIndex;
			this.population = population;
			this.unity2DLocation = location;
			this.cityClass = cityClass;
			this.uniqueId = uniqueId;
            this.attrib = new JSONObject();

            cityLevel = 0;

            buildingListInCity = new List<Building>();
        }

    public City Clone() {
			City c = new City(name, province, countryIndex, population, unity2DLocation, cityClass, uniqueId);
			c.attrib = new JSONObject();
			c.attrib.Absorb(attrib);
			return c;
		}
    }
}