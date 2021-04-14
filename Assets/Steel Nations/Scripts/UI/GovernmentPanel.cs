using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AwesomeCharts;
using System;

namespace WorldMapStrategyKit
{
    public class GovernmentPanel : MonoBehaviour
    {
        private static GovernmentPanel instance;
        public static GovernmentPanel Instance
        {
            get { return instance; }
        }

        public GameObject politiksCategoryContent;
        public GameObject intelligenceAgencyCategoryContent;
        public GameObject tradeCategoryContent;
        public GameObject militaryCategoryContent;
        public GameObject supportCategoryContent;
        public GameObject regionCategoryContent;

        public GameObject buildingItem;


        public GameObject relationItem;
        public GameObject weaponItem;
        public GameObject organizationItem;
        public GameObject organizationFlag;

        public GameObject allyContent;
        public GameObject enemyContent;
        public GameObject atWarContent;

        public GameObject overviewContent;
        public GameObject mineralContent;
        public GameObject selectedMilitaryContent;
        public GameObject myMilitaryContent;

        public GameObject buildingContent;

        public GameObject governmentPanel;
        public GameObject governmentItem;
        public GameObject mineralItem;

        public GameObject flag_country;

        public GameObject president;
        public RawImage presidentPicture;
        public TextMeshProUGUI presidentName;
        public TextMeshProUGUI since;
        public TextMeshProUGUI birthDate;

        public GameObject diplomacyButton;

        public TextMeshProUGUI countryName;

        public GameObject founderContent;
        public GameObject fullMemberContent;
        public GameObject observerContent;
        public GameObject dialoguePartner;
        public GameObject appliedFullMemberContent;
        public GameObject appliedObserverContent;
        public GameObject appliedDialoguePartner;

        public BarChart armyChart;

        private void Start()
        {
            instance = this;
        }

        public void HidePanel()
        {
            governmentPanel.SetActive(false);

            GameEventHandler.Instance.GetPlayer().SetSelectedCountry(null);
            ActionManager.Instance.HideAllActionPanels();

            ClearAllContents();
        }
        public void ShowSelectCountryPanel(Country country)
        {
            if (country == null)
                return;

            governmentPanel.SetActive(true);

            ActionManager.Instance.HideAllActionPanels();

            countryName.text = country.name;
            flag_country.GetComponent<RawImage>().texture = country.GetCountryFlag();

            ClearAllContents();

            if (country.President != null)
            {
                president.SetActive(true);

                presidentName.text = country.President.PersonName;
                since.text = country.President.RoleStartDate.ToString() + " ~ Present";

                string[] date = country.President.BirthDate.Split('.');

                int age = 0;

                if (GameEventHandler.Instance.IsGameStarted() == false)
                    age = 2020 - Int32.Parse(date[2]);
                else
                    age = GameEventHandler.Instance.GetCurrentYear() - Int32.Parse(date[2]);

                birthDate.text = age.ToString();

                presidentPicture.texture = country.President.PersonPicture;
            }
            else
            {
                president.SetActive(false);
            }

            CreateOverviewButton("Manpower", string.Format("{0:#,0}", CountryManager.Instance.GetAvailableManpower(country)));
            CreateOverviewButton("Tension", country.Tension.ToString());

            CreateOverviewButton("Budget", "$ " + string.Format("{0:#,0}", float.Parse(country.Budget.ToString())) + " M");

            CreateOverviewButton("Religion", CountryManager.Instance.GetReligionNameByReligionType(country.Religion));
            CreateOverviewButton("System Of Government", country.System_Of_Government);
            CreateOverviewButton("Unemployment Rate", country.Unemployment_Rate.ToString());
            CreateOverviewButton("Birth Rate", country.Fertility_Rate_PerWeek.ToString());
            CreateOverviewButton("Military Rank", country.Military_Rank.ToString());

            CreateMineralButton("Oil", country.GetMineral(MINERAL_TYPE.OIL).ToString()).GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.OIL));
            CreateMineralButton("Uranium", country.GetMineral(MINERAL_TYPE.URANIUM).ToString()).GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.URANIUM));
            CreateMineralButton("Iron", country.GetMineral(MINERAL_TYPE.IRON).ToString()).GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.IRON));
            CreateMineralButton("Steel", country.GetMineral(MINERAL_TYPE.STEEL).ToString()).GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.STEEL));
            CreateMineralButton("Aluminium", country.GetMineral(MINERAL_TYPE.ALUMINIUM).ToString()).GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.ALUMINIUM));
            
            foreach (Country allyCountry in CountryManager.Instance.GetAllAllies(country))
            {
                CreateRelation(allyCountry, allyContent.transform);
            }
            foreach (Country allyCountry in CountryManager.Instance.GetAllEnemies(country))
            {
                CreateRelation(allyCountry, enemyContent.transform);
            }
            foreach (Country allyCountry in CountryManager.Instance.GetAtWarCountryList(country))
            {
                CreateRelation(allyCountry, atWarContent.transform);
            }

            if(country.GetArmy() != null)
            {
                CreateMilitaryButton("Land Attack Power", string.Format("{0:#,0}", country.GetArmy().GetLandForces().GetMilitaryPower()), selectedMilitaryContent.transform);
                CreateMilitaryButton("Air Attack Power", string.Format("{0:#,0}", country.GetArmy().GetAirForces().GetMilitaryPower()), selectedMilitaryContent.transform);
                CreateMilitaryButton("Naval Attack Power", string.Format("{0:#,0}", country.GetArmy().GetNavalForces().GetMilitaryPower()), selectedMilitaryContent.transform);
            }
            else
            {
                CreateMilitaryButton("Land Attack Power", "0", selectedMilitaryContent.transform);
                CreateMilitaryButton("Air Attack Power", "0", selectedMilitaryContent.transform);
                CreateMilitaryButton("Naval Attack Power", "0", selectedMilitaryContent.transform);
            }

            CreateBuildingButton("Oil Rafinery", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.OIL_RAFINERY).ToString());
            CreateBuildingButton("University", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.UNIVERSITY).ToString());
            CreateBuildingButton("Nuclear Facility", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.NUCLEAR_FACILITY).ToString());
            CreateBuildingButton("Trade Port", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.TRADE_PORT).ToString());
            CreateBuildingButton("Dockyard", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.DOCKYARD).ToString());
            CreateBuildingButton("Hospital", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.HOSPITAL).ToString());
            CreateBuildingButton("Factory", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.FACTORY).ToString());
            CreateBuildingButton("Military Factory", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.MILITARY_FACTORY).ToString());
            CreateBuildingButton("Mineral Factory", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.MINERAL_FACTORY).ToString());
            CreateBuildingButton("Garrison", CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.GARRISON).ToString());

            foreach (Organization org in OrganizationManager.Instance.GetAllOrganizations())
            {
                GameObject flag = null;

                if (org.GetObserverList().Contains(country))
                {
                    flag = Instantiate(organizationFlag, observerContent.transform.GetChild(4));
                }

                if (org.GetFullMemberList().Contains(country))
                {
                    flag = Instantiate(organizationFlag, fullMemberContent.transform.GetChild(4));
                }

                if (org.GetFounderList().Contains(country))
                {
                    flag = Instantiate(organizationFlag, founderContent.transform.GetChild(4));
                }

                if (org.GetDialoguePartnerList().Contains(country))
                {
                    flag = Instantiate(organizationFlag, dialoguePartner.transform.GetChild(4));
                }

                if (org.GetApplyForFullMemberList().Contains(country))
                {
                    flag = Instantiate(organizationFlag, appliedFullMemberContent.transform.GetChild(4));
                }

                if (org.GetApplyForObserverList().Contains(country))
                {
                    flag = Instantiate(organizationFlag, appliedObserverContent.transform.GetChild(4));
                }

                if (org.GetApplyForDialoguePartnerList().Contains(country))
                {
                    flag = Instantiate(organizationFlag, appliedDialoguePartner.transform.GetChild(4));
                }

                if (flag != null)
                {
                    flag.transform.GetChild(0).GetComponent<RawImage>().texture = org.organizationLogo;
                    flag.GetComponent<SimpleTooltip>().infoLeft = org.organizationName;
                }
            }
        }

        public void SetArmyBarChart()
        {
            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();
            Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();

            if(myCountry.GetArmy() != null)
            {
                armyChart.GetChartData().DataSets[0].GetEntryAt(0).Value = myCountry.GetArmy().GetLandForces().GetMilitaryPower(); // land
                armyChart.GetChartData().DataSets[0].GetEntryAt(1).Value = myCountry.GetArmy().GetAirForces().GetMilitaryPower(); // air
                armyChart.GetChartData().DataSets[0].GetEntryAt(2).Value = myCountry.GetArmy().GetNavalForces().GetMilitaryPower(); // naval
            }

            if(selectedCountry.GetArmy() != null)
            {
                armyChart.GetChartData().DataSets[1].GetEntryAt(0).Value = selectedCountry.GetArmy().GetLandForces().GetMilitaryPower(); // land
                armyChart.GetChartData().DataSets[1].GetEntryAt(1).Value = selectedCountry.GetArmy().GetAirForces().GetMilitaryPower(); // air
                armyChart.GetChartData().DataSets[1].GetEntryAt(2).Value = selectedCountry.GetArmy().GetNavalForces().GetMilitaryPower(); // naval
            }

            //	Refresh	chart	after	data	change							
            armyChart.SetDirty();
            
        }

        void ClearAllContents()
        {
            ClearBuildingContent();
            ClearDiplomacyContent();
            ClearMineralContent();
            ClearOverviewContent();
            ClearMilitaryContent();
            ClearOrganizationContent();
            ClearRelationContents();
        }

        void ClearRelationContents()
        {
            foreach (Transform child in allyContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in enemyContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in atWarContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        void ClearOverviewContent()
        {
            foreach (Transform child in overviewContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        void ClearMineralContent()
        {
            foreach (Transform child in mineralContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        void ClearMilitaryContent()
        {
            foreach (Transform child in myMilitaryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in selectedMilitaryContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        void ClearDiplomacyContent()
        {
            foreach (Transform child in politiksCategoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in intelligenceAgencyCategoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in tradeCategoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in militaryCategoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in supportCategoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in regionCategoryContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        void ClearBuildingContent()
        {
            foreach (Transform child in buildingContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        void ClearOrganizationContent()
        {
            foreach (Transform child in founderContent.transform.GetChild(4))
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in fullMemberContent.transform.GetChild(4))
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in observerContent.transform.GetChild(4))
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in dialoguePartner.transform.GetChild(4))
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in appliedFullMemberContent.transform.GetChild(4))
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in appliedObserverContent.transform.GetChild(4))
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in appliedDialoguePartner.transform.GetChild(4))
            {
                Destroy(child.gameObject);
            }
        }
        
        public void ShowGovernmentPanel()
        {
            Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();
            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            ShowSelectCountryPanel(selectedCountry);

            SelectCountry.Instance.startButton.SetActive(false);

            if (myCountry.GetArmy() != null)
            {
                CreateMilitaryButton("Land Attack Power", string.Format("{0:#,0}", myCountry.GetArmy().GetLandForces().GetMilitaryPower()), myMilitaryContent.transform);
                CreateMilitaryButton("Air Attack Power", string.Format("{0:#,0}", myCountry.GetArmy().GetAirForces().GetMilitaryPower()), myMilitaryContent.transform);
                CreateMilitaryButton("Naval Attack Power", string.Format("{0:#,0}", myCountry.GetArmy().GetNavalForces().GetMilitaryPower()), myMilitaryContent.transform);
            }
            else
            {
                CreateMilitaryButton("Land Attack Power", "0", myMilitaryContent.transform);
                CreateMilitaryButton("Air Attack Power", "0", myMilitaryContent.transform);
                CreateMilitaryButton("Naval Attack Power", "0", myMilitaryContent.transform);
            }

            SetArmyBarChart();

            foreach(Action action in ActionManager.Instance.GetActionList())
            {
                GameObject GO = CreateDiplomacyButton(action);

                if(GO != null)
                {
                    if (action.ActionType == ACTION_TYPE.Ask_For_Control_Of_Region)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AskForControlOfProvincePanel());

                    if (action.ActionType == ACTION_TYPE.Ask_For_Gun_Support)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AskForGunSupportPanel());

                    if (action.ActionType == ACTION_TYPE.Ask_For_Money_Support)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AskForMoneySupportPanel());

                    if (action.ActionType == ACTION_TYPE.Assassination_Of_President)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AssassinationOfPresidentPanel());

                    if (action.ActionType == ACTION_TYPE.Begin_Nuclear_War)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.NuclearWarPanel());

                    if (action.ActionType == ACTION_TYPE.Cancel_Military_Access)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.CancelMilitaryAccessPanel());

                    if (action.ActionType == ACTION_TYPE.Declare_War)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.DeclareWarPanel());

                    if (action.ActionType == ACTION_TYPE.Give_Control_Of_Region)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveControlOfStatePanel());

                    if (action.ActionType == ACTION_TYPE.Give_Garrison_Support)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveGarrisonSupportPanel());

                    if (action.ActionType == ACTION_TYPE.Give_Gun_Support)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveGunSupportPanel());

                    if (action.ActionType == ACTION_TYPE.Give_Military_Access)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveMilitaryAccessPanel());

                    if (action.ActionType == ACTION_TYPE.Give_Money_Support)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveMoneySupportPanel());

                    if (action.ActionType == ACTION_TYPE.Make_A_Military_Coup)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.MakeMilitaryCoupPanel());

                    if (action.ActionType == ACTION_TYPE.Place_Arms_Embargo)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.PlaceArmsEmbargoPanel());

                    if (action.ActionType == ACTION_TYPE.Place_Trade_Embargo)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.PlaceTradeEmbargoPanel());

                    if (action.ActionType == ACTION_TYPE.Request_Garrison_Support)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.RequestGarrisonSupportPanel());

                    if (action.ActionType == ACTION_TYPE.Request_License_Production)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.RequestLicenseProductionPanel());

                    if (action.ActionType == ACTION_TYPE.Sign_A_Peace_Treaty)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.SignPeaceTreatyPanel());

                    if (action.ActionType == ACTION_TYPE.Steal_Technology)
                        GO.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.StealTechnologyPanel());
                }
                
            }           
        }    

        GameObject CreateOverviewButton(string text, string value)
        {
            GameObject GO = Instantiate(governmentItem, overviewContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;
            GO.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = value;

            return GO;
        }

        GameObject CreateDiplomacyButton(Action action)
        {
            GameObject GO = null;

            bool isPossible = ActionManager.Instance.IsPossibleAction(action, GameEventHandler.Instance.GetPlayer().GetMyCountry(), GameEventHandler.Instance.GetPlayer().GetSelectedCountry());

            if(action.ActionCategory == ACTION_CATEGORY.INTELLIGENCE_AGENCY && isPossible)
                GO = Instantiate(diplomacyButton, intelligenceAgencyCategoryContent.transform);
            else if (action.ActionCategory == ACTION_CATEGORY.MILITARY && isPossible)
                GO = Instantiate(diplomacyButton, militaryCategoryContent.transform);
            //else if (action.GetActionCategory() == ACTION_CATEGORY.POLITIKS && isPossible)
                //GO = Instantiate(diplomacyButton, politiksCategoryContent.transform);
            else if (action.ActionCategory == ACTION_CATEGORY.REGION && isPossible)
                GO = Instantiate(diplomacyButton, regionCategoryContent.transform);
            else if (action.ActionCategory == ACTION_CATEGORY.SUPPORT && isPossible)
                GO = Instantiate(diplomacyButton, supportCategoryContent.transform);
            else if (action.ActionCategory == ACTION_CATEGORY.TRADE && isPossible)
                GO = Instantiate(diplomacyButton, tradeCategoryContent.transform);

            if(GO != null)
                GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = action.ActionName;

            return GO;
        }

        GameObject CreateMineralButton(string text, string value)
        {
            GameObject GO = Instantiate(mineralItem, mineralContent.transform);
            GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value;
            GO.transform.GetChild(1).GetComponent<SimpleTooltip>().infoLeft = text;
            GO.transform.GetChild(1).GetComponent<RawImage>().texture = ResourceManager.Instance.LoadTexture(RESOURCE_TYPE.MINERAL, text);

            return GO;
        }
        GameObject CreateBuildingButton(string text, string value)
        {
            GameObject GO = Instantiate(buildingItem, buildingContent.transform);
            GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            GO.transform.GetChild(1).GetComponent<RawImage>().texture = BuildingManager.Instance.GetBuildingImage(BuildingManager.Instance.GetBuildingTypeByBuildingName(text));
            GO.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = value;

            return GO;
        }
        GameObject CreateMilitaryButton(string text, string value, Transform content)
        {
            GameObject GO = Instantiate(governmentItem, content);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;
            GO.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = value;

            return GO;
        }
        
        GameObject CreateRelation(Country country, Transform content)
        {
            GameObject temp = Instantiate(relationItem, content);
            temp.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
            temp.GetComponent<SimpleTooltip>().infoLeft = country.name;

            return temp;
        }
    }
}
