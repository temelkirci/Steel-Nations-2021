using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace WorldMapStrategyKit
{
    public class CityInfoPanel : MonoBehaviour
    {
        private static CityInfoPanel instance;
        public static CityInfoPanel Instance
        {
            get { return instance; }
        }

        List<Building> buildingList = new List<Building>();

        WMSK map;
        /// <summary>
        /// City Panel
        /// </summary>
        public RectTransform cityPanel;
        public GameObject buildingItem;
        public GameObject divisionItem;

        public GameObject cityMenu;

        public GameObject buildingContent;
        public GameObject garrisonContent;
        public GameObject mineralContent;

        public GameObject divisionContent;

        public TextMeshProUGUI cityName;
        public TextMeshProUGUI cityType;
        public TextMeshProUGUI province;

        public TextMeshProUGUI manpower;

        public TextMeshProUGUI oilInCity;
        public TextMeshProUGUI ironInCity;
        public TextMeshProUGUI steelInCity;
        public TextMeshProUGUI uraniumInCity;
        public TextMeshProUGUI aluminiumInCity;        

        //public GameObject missileStars;
        public GameObject cityImage;

        public void Start()
        {
            map = WMSK.instance;
            
            instance = this;

            HideAllTabs();
        }

        public void HideAllTabs()
        {
            foreach (Transform child in cityPanel.transform.GetChild(2).transform.GetChild(0).transform)
            {
                child.gameObject.SetActive(false);
            }

            foreach (Transform child in cityPanel.transform.GetChild(3).transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        public void HideCityPanel()
        {
            cityMenu.gameObject.SetActive(false);
            cityPanel.gameObject.SetActive(false);
        }
        public void ShowCityMenu()
        {
            cityMenu.gameObject.SetActive(true);
            cityPanel.gameObject.SetActive(false);
        }
        public void ShowCityInfo()
        {
            City city = GameEventHandler.Instance.GetPlayer().GetSelectedCity();
            if (city == null)
                return;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            cityPanel.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);

            string countryName = map.GetCityCountryName(city);
            Country country = map.GetCountry(countryName);

            cityImage.GetComponent<RawImage>().texture = country.GetCountryFlag();

            // Update text labels
            cityName.text = city.name;

            manpower.text = string.Format("{0:#,0}", float.Parse(city.GetPopulation().ToString()));
            province.text = city.province;

            if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                cityType.text = "Country Capital";
            if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                cityType.text = "Region Capital";
            if (city.cityClass == CITY_CLASS.CITY)
                cityType.text = "Small City";
        }

        public void AddBuilding(Building building)
        {
            buildingList.Add(building);
        }

        public void ShowEconomy()
        {
            City city = GameEventHandler.Instance.GetPlayer().GetSelectedCity();

            if (city == null)
                return;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
            cityPanel.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(true);

            foreach (Transform child in mineralContent.transform)
            {
                Destroy(child.gameObject);
            }

            oilInCity.text = city.GetOilReserves().ToString();
            ironInCity.text = city.GetIronReserves().ToString();
            uraniumInCity.text = city.GetUraniumReserves().ToString();
            steelInCity.text = city.GetSteelReserves().ToString();
            aluminiumInCity.text = city.GetAluminiumReserves().ToString();

            foreach (Building building in city.GetAllBuildings())
            {
                GameObject temp = null;

                if (building.buildingType == BUILDING_TYPE.MINERAL_FACTORY ||
                    building.buildingType == BUILDING_TYPE.OIL_RAFINERY ||
                    building.buildingType == BUILDING_TYPE.NUCLEAR_FACILITY)
                {
                    temp = Instantiate(buildingItem, mineralContent.transform);
                }


                if (temp != null)
                {
                    temp.gameObject.name = building.buildingName;
                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.buildingName;
                    temp.gameObject.transform.GetChild(1).GetComponent<RawImage>().texture = Resources.Load("buildings/" + building.buildingName) as Texture2D;
                    temp.gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (GameEventHandler.Instance.GetPlayer().GetSelectedCountry().GetBudget() >= building.constructionCost)
                            city.StartConstruction(building);
                    });

                    temp.gameObject.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.currentBuildingInCity.ToString();
                    temp.gameObject.transform.GetChild(4).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = building.maxBuildingInCity.ToString();

                    temp.gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = building.incomeMonthly.ToString();
                    temp.gameObject.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = building.expenseMonthly.ToString();

                    temp.gameObject.transform.GetChild(13).GetComponent<TextMeshProUGUI>().text = building.constructionCost.ToString();
                    temp.gameObject.transform.GetChild(14).GetComponent<TextMeshProUGUI>().text = building.constructionTime.ToString() + " days";

                    temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0.0f;
                }
            }
        }

        public void ShowBuildings()
        {
            if (GameEventHandler.Instance.GetPlayer().GetSelectedCity() == null)
                return;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
            cityPanel.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(true);

            foreach (Transform child in buildingContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Building building in GameEventHandler.Instance.GetPlayer().GetSelectedCity().GetAllBuildings())
            {
                GameObject temp = null;

                if (building.buildingType == BUILDING_TYPE.HOSPITAL ||
                    building.buildingType == BUILDING_TYPE.FACTORY ||
                    building.buildingType == BUILDING_TYPE.TRADE_PORT ||
                    building.buildingType == BUILDING_TYPE.UNIVERSITY ||
                    building.buildingType == BUILDING_TYPE.MILITARY_FACTORY)
                {
                    temp = Instantiate(buildingItem, buildingContent.transform);
                }

                if (temp != null)
                {
                    temp.gameObject.name = building.buildingName;
                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.buildingName;
                    temp.gameObject.transform.GetChild(1).GetComponent<RawImage>().texture = Resources.Load("buildings/" + building.buildingName) as Texture2D;
                    temp.gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (GameEventHandler.Instance.GetPlayer().GetSelectedCountry().GetBudget() >= building.constructionCost)
                            GameEventHandler.Instance.GetPlayer().GetSelectedCity().StartConstruction(building);
                    });

                    temp.gameObject.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.currentBuildingInCity.ToString();
                    temp.gameObject.transform.GetChild(4).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = building.maxBuildingInCity.ToString();

                    temp.gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = building.incomeMonthly.ToString();
                    temp.gameObject.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = building.expenseMonthly.ToString();

                    temp.gameObject.transform.GetChild(13).GetComponent<TextMeshProUGUI>().text = building.constructionCost.ToString();
                    temp.gameObject.transform.GetChild(14).GetComponent<TextMeshProUGUI>().text = building.constructionTime.ToString() + " days";

                    temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0.0f;
                }
            }
        }

        public void ShowGarrison()
        {
            City city = GameEventHandler.Instance.GetPlayer().GetSelectedCity();

            if (city == null)
                return;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
            cityPanel.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(true);

            foreach (Transform child in garrisonContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in divisionContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Building building in city.GetAllBuildings())
            {
                GameObject temp = null;

                if (building.buildingType == BUILDING_TYPE.GARRISON ||
                    building.buildingType == BUILDING_TYPE.MILITARY_BASE)
                {
                    temp = Instantiate(buildingItem, garrisonContent.transform);
                }

                if (temp != null)
                {
                    temp.gameObject.name = building.buildingName;
                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.buildingName;
                    temp.gameObject.transform.GetChild(1).GetComponent<RawImage>().texture = Resources.Load("buildings/" + building.buildingName) as Texture2D;
                    temp.gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (GameEventHandler.Instance.GetPlayer().GetSelectedCountry().GetBudget() >= building.constructionCost)
                            city.StartConstruction(building);
                    });

                    temp.gameObject.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.currentBuildingInCity.ToString();
                    temp.gameObject.transform.GetChild(4).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = building.maxBuildingInCity.ToString();

                    temp.gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = building.incomeMonthly.ToString();
                    temp.gameObject.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = building.expenseMonthly.ToString();

                    temp.gameObject.transform.GetChild(13).GetComponent<TextMeshProUGUI>().text = building.constructionCost.ToString();
                    temp.gameObject.transform.GetChild(14).GetComponent<TextMeshProUGUI>().text = building.constructionTime.ToString() + " days";

                    temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0.0f;
                }
            }

            if(city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.GARRISON) > 0)
            {
                foreach (GameObjectAnimator divison in city.GetAllDivisionsInCity())
                {
                    GameObject temp = Instantiate(divisionItem, divisionContent.transform);

                    temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = divison.GetDivision().divisionName;

                    temp.gameObject.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        city.VisibleDivision(divison);
                        ShowGarrison();
                    });
                }
            }
        }

        public void UpdateAllBuildings()
        {
            if (cityPanel.gameObject.activeSelf)
            {
                foreach (Building building in GameEventHandler.Instance.GetPlayer().GetSelectedCity().GetAllBuildings())
                {
                    GameObject temp = null;

                    foreach (Transform child in buildingContent.transform)
                    {
                        if(child.gameObject.name == building.buildingName)
                            temp = child.gameObject;

                    }
                    foreach (Transform child in garrisonContent.transform)
                    {
                        if (child.gameObject.name == building.buildingName)
                            temp = child.gameObject;
                    }
                    foreach (Transform child in mineralContent.transform)
                    {
                        if (child.gameObject.name == building.buildingName)
                            temp = child.gameObject;
                    }

                    if(temp != null)
                    {
                        if(building.leftConstructionDay == 0)
                            temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0;
                        else
                            temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 100.0f - (building.leftConstructionDay * 100.0f) / building.constructionTime;
                    }

                }
            }
        }
        
        public Building CreateBuildingByType(BUILDING_TYPE buildingType)
        {
            foreach (Building building in buildingList)
            {
                if (building.buildingType == buildingType)
                { 
                    return building.Clone();
                }
            }
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