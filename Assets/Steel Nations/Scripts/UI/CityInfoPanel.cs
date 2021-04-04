using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DuloGames.UI;

namespace WorldMapStrategyKit
{
    public class CityInfoPanel : MonoBehaviour
    {
        private static CityInfoPanel instance;
        public static CityInfoPanel Instance
        {
            get { return instance; }
        }

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

        public GameObject cityImage;

        public void Start()
        {            
            instance = this;
            map = WMSK.instance;
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

            cityName.text = city.name;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<UITab>().isOn = true;

            string countryName = map.GetCityCountryName(city);
            Country country = map.GetCountry(countryName);

            cityImage.GetComponent<RawImage>().texture = country.GetCountryFlag();

            manpower.text = string.Format("{0:#,0}", float.Parse(city.population.ToString()));
            province.text = city.province;

            if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                cityType.text = "Country Capital";
            if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                cityType.text = "Region Capital";
            if (city.cityClass == CITY_CLASS.CITY)
                cityType.text = "Small City";
        }

        public void ShowEconomy()
        {
            City city = GameEventHandler.Instance.GetPlayer().GetSelectedCity();

            if (city == null)
                return;

            cityName.text = city.name;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<UITab>().isOn = true;

            foreach (Transform child in mineralContent.transform)
            {
                Destroy(child.gameObject);
            }

            oilInCity.text = city.GetMineralResources(MINERAL_TYPE.OIL).ToString();
            ironInCity.text = city.GetMineralResources(MINERAL_TYPE.IRON).ToString();
            uraniumInCity.text = city.GetMineralResources(MINERAL_TYPE.URANIUM).ToString();
            steelInCity.text = city.GetMineralResources(MINERAL_TYPE.STEEL).ToString();
            aluminiumInCity.text = city.GetMineralResources(MINERAL_TYPE.ALUMINIUM).ToString();

            foreach (var building in city.GetAllBuildings())
            {
                GameObject temp = null;

                if (building.Key == BUILDING_TYPE.MINERAL_FACTORY ||
                    building.Key == BUILDING_TYPE.OIL_RAFINERY ||
                    building.Key == BUILDING_TYPE.NUCLEAR_FACILITY)
                {
                    temp = Instantiate(buildingItem, mineralContent.transform);
                }


                if (temp != null)
                {
                    Building build = BuildingManager.Instance.GetBuildingByBuildingType(building.Key);

                    temp.gameObject.name = build.buildingName;
                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = build.buildingName;
                    temp.gameObject.transform.GetChild(1).GetComponent<RawImage>().texture = build.buildingImage;
                    temp.gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (GameEventHandler.Instance.GetPlayer().GetSelectedCountry().Budget >= build.constructionCost)
                            city.StartConstructionInCity(building.Key);
                    });

                    temp.gameObject.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.Value.ToString();
                    temp.gameObject.transform.GetChild(4).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = build.maxBuildingInCity.ToString();

                    temp.gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = build.incomeMonthly.ToString();
                    //temp.gameObject.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = building.Key.expenseMonthly.ToString();

                    temp.gameObject.transform.GetChild(13).GetComponent<TextMeshProUGUI>().text = build.constructionCost.ToString();
                    temp.gameObject.transform.GetChild(14).GetComponent<TextMeshProUGUI>().text = build.constructionTime.ToString() + " days";

                    temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0.0f;
                }
            }
        }

        public void ShowBuildings()
        {
            City city = GameEventHandler.Instance.GetPlayer().GetSelectedCity();

            if (city == null)
                return;

            cityName.text = city.name;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(2).GetComponent<UITab>().isOn = true;

            foreach (Transform child in buildingContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var building in city.GetAllBuildings())
            {
                GameObject temp = null;

                if (building.Key == BUILDING_TYPE.HOSPITAL ||
                    building.Key == BUILDING_TYPE.FACTORY ||
                    building.Key == BUILDING_TYPE.TRADE_PORT ||
                    building.Key == BUILDING_TYPE.UNIVERSITY ||
                    building.Key == BUILDING_TYPE.MILITARY_FACTORY)
                {
                    temp = Instantiate(buildingItem, buildingContent.transform);
                }

                if (temp != null)
                {
                    Building build = BuildingManager.Instance.GetBuildingByBuildingType(building.Key);

                    temp.gameObject.name = build.buildingName;
                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = build.buildingName;
                    temp.gameObject.transform.GetChild(1).GetComponent<RawImage>().texture = build.buildingImage;
                    temp.gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (GameEventHandler.Instance.GetPlayer().GetSelectedCountry().Budget >= build.constructionCost)
                            GameEventHandler.Instance.GetPlayer().GetSelectedCity().StartConstructionInCity(building.Key);
                    });

                    temp.gameObject.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.Value.ToString();
                    //temp.gameObject.transform.GetChild(4).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = building.maxBuildingInCity.ToString();

                    temp.gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = build.incomeMonthly.ToString();
                    //temp.gameObject.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = building.Key.expenseMonthly.ToString();

                    temp.gameObject.transform.GetChild(13).GetComponent<TextMeshProUGUI>().text = build.constructionCost.ToString();
                    temp.gameObject.transform.GetChild(14).GetComponent<TextMeshProUGUI>().text = build.constructionTime.ToString() + " days";

                    temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0.0f;
                }
            }
        }

        public void ShowGarrison()
        {
            City city = GameEventHandler.Instance.GetPlayer().GetSelectedCity();

            if (city == null)
                return;

            cityName.text = city.name;

            cityPanel.gameObject.SetActive(true);
            cityMenu.gameObject.SetActive(false);

            HideAllTabs();

            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
            cityPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(3).GetComponent<UITab>().isOn = true;

            foreach (Transform child in garrisonContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in divisionContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var building in city.GetAllBuildings())
            {
                GameObject temp = null;

                if (building.Key == BUILDING_TYPE.GARRISON ||
                    building.Key == BUILDING_TYPE.MILITARY_BASE)
                {
                    temp = Instantiate(buildingItem, garrisonContent.transform);
                }

                if (temp != null)
                {
                    Building build = BuildingManager.Instance.GetBuildingByBuildingType(building.Key);

                    temp.gameObject.name = build.buildingName;
                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = build.buildingName;
                    temp.gameObject.transform.GetChild(1).GetComponent<RawImage>().texture = build.buildingImage;
                    temp.gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (GameEventHandler.Instance.GetPlayer().GetSelectedCountry().Budget >= build.constructionCost)
                            city.StartConstructionInCity(building.Key);
                    });

                    temp.gameObject.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.Value.ToString();
                    temp.gameObject.transform.GetChild(4).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = build.maxBuildingInCity.ToString();

                    temp.gameObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = build.incomeMonthly.ToString();
                    //temp.gameObject.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = building.Key.expenseMonthly.ToString();

                    temp.gameObject.transform.GetChild(13).GetComponent<TextMeshProUGUI>().text = build.constructionCost.ToString();
                    temp.gameObject.transform.GetChild(14).GetComponent<TextMeshProUGUI>().text = build.constructionTime.ToString() + " days";

                    temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0.0f;
                }
            }

            if(city.GetBuildingNumber(BUILDING_TYPE.GARRISON) > 0)
            {
                foreach (GameObjectAnimator divison in city.GetAllDivisionsInCity())
                {
                    GameObject temp = Instantiate(divisionItem, divisionContent.transform);

                    temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = divison.GetDivision().divisionName;
                    temp.transform.GetChild(1).GetComponent<RawImage>().texture = divison.GetDivision().GetDivisionIcon();

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
                foreach (var building in GameEventHandler.Instance.GetPlayer().GetSelectedCity().GetAllBuildings())
                {
                    GameObject temp = null;
                    Building build = BuildingManager.Instance.GetBuildingByBuildingType(building.Key);

                    foreach (Transform child in buildingContent.transform)
                    {
                        if(child.gameObject.name == build.buildingName)
                            temp = child.gameObject;
                    }
                    foreach (Transform child in garrisonContent.transform)
                    {
                        if (child.gameObject.name == build.buildingName)
                            temp = child.gameObject;
                    }
                    foreach (Transform child in mineralContent.transform)
                    {
                        if (child.gameObject.name == build.buildingName)
                            temp = child.gameObject;
                    }

                    if(temp != null)
                    {
                        if(build.leftConstructionDay == 0)
                            temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 0;
                        else
                            temp.gameObject.transform.GetChild(3).GetComponent<Slider>().value = 100.0f - (build.leftConstructionDay * 100.0f) / build.constructionTime;
                    }

                }
            }
        }
        
    }
}