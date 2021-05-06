using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AwesomeCharts;
using System;
using System.Collections.Generic;
using DuloGames.UI;

namespace WorldMapStrategyKit
{
    public class GovernmentPanel : MonoBehaviour
    {
        private static GovernmentPanel instance;
        public static GovernmentPanel Instance
        {
            get { return instance; }
        }

        public GameObject mineralTab;
        public GameObject diplomacyTab;
        public GameObject armyTab;

        public TextMeshProUGUI manpowerValue;
        public TextMeshProUGUI tensionValue;
        public TextMeshProUGUI budgetValue;
        public TextMeshProUGUI religionValue;
        public TextMeshProUGUI systemOfGovernmentValue;
        public TextMeshProUGUI unemploymentRateValue;
        public TextMeshProUGUI birthRateValue;
        public TextMeshProUGUI militaryRankValue;

        public TextMeshProUGUI oilValue;
        public TextMeshProUGUI steelValue;
        public TextMeshProUGUI ironValue;
        public TextMeshProUGUI uraniumValue;
        public TextMeshProUGUI aluminiumValue;

        public TextMeshProUGUI oilRafineryValue;
        public TextMeshProUGUI dockyardValue;
        public TextMeshProUGUI militaryFactoryValue;
        public TextMeshProUGUI factoryValue;
        public TextMeshProUGUI tradePortValue;
        public TextMeshProUGUI hospitalValue;
        public TextMeshProUGUI nuclearFacilityValue;
        public TextMeshProUGUI mineralFactoryValue;
        public TextMeshProUGUI universityValue;
        public TextMeshProUGUI GarrisonValue;

        public TextMeshProUGUI myLandAttackPowerValue;
        public TextMeshProUGUI myAirAttackPowerValue;
        public TextMeshProUGUI myNavalAttackPowerValue;

        public TextMeshProUGUI selectedCountryLandAttackPowerValue;
        public TextMeshProUGUI selectedCountryAirAttackPowerValue;
        public TextMeshProUGUI selectedCountryNavalAttackPowerValue;


        public GameObject politiksCategoryContent;
        public GameObject intelligenceAgencyCategoryContent;
        public GameObject tradeCategoryContent;
        public GameObject militaryCategoryContent;
        public GameObject supportCategoryContent;
        public GameObject regionCategoryContent;

        public GameObject relationItem;
        public GameObject organizationItem;
        public GameObject organizationFlag;

        public GameObject allyContent;
        public GameObject enemyContent;
        public GameObject atWarContent;

        public GameObject governmentPanel;
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

        public GameObject AskForControlOfProvinceButton;
        public GameObject AskForGunSupportButton;
        public GameObject AskForMoneySupportButton;
        public GameObject AssassinationOfPresidentButton;
        public GameObject NuclearWarButton;
        public GameObject CancelMilitaryAccessButton;
        public GameObject DeclareWarButton;
        public GameObject GiveControlOfStateButton;
        public GameObject GiveGarrisonSupportButton;
        public GameObject GiveGunSupportButton;
        public GameObject GiveMilitaryAccessButton;
        public GameObject GiveMoneySupportButton;
        public GameObject MakeMilitaryCoupButton;
        public GameObject PlaceArmsEmbargoButton;
        public GameObject PlaceTradeEmbargoButton;
        public GameObject RequestGarrisonSupportButton;
        public GameObject RequestLicenseProductionButton;
        public GameObject SignPeaceTreatyButton;
        public GameObject StealTechnologyButton;
        public GameObject SignTradeTreatyButton;
        public GameObject AskForMilitaryAccessButton;

        private void Start()
        {
            instance = this;

            AskForControlOfProvinceButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AskForControlOfProvincePanel());
            AskForGunSupportButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AskForGunSupportPanel());
            AssassinationOfPresidentButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AskForMoneySupportPanel());
            AssassinationOfPresidentButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AssassinationOfPresidentPanel());
            NuclearWarButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.NuclearWarPanel());
            CancelMilitaryAccessButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.CancelMilitaryAccessPanel());
            DeclareWarButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.DeclareWarPanel());
            GiveControlOfStateButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveControlOfStatePanel());
            GiveGarrisonSupportButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveGarrisonSupportPanel());
            GiveGunSupportButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveGunSupportPanel());
            GiveMilitaryAccessButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveMilitaryAccessPanel());
            GiveMoneySupportButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.GiveMoneySupportPanel());
            MakeMilitaryCoupButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.MakeMilitaryCoupPanel());
            PlaceArmsEmbargoButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.PlaceArmsEmbargoPanel());
            PlaceTradeEmbargoButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.PlaceTradeEmbargoPanel());
            RequestGarrisonSupportButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.RequestGarrisonSupportPanel());
            RequestLicenseProductionButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.RequestLicenseProductionPanel());
            SignPeaceTreatyButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.SignPeaceTreatyPanel());
            StealTechnologyButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.StealTechnologyPanel());
            SignTradeTreatyButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.SignTradeTreatyPanel());
            AskForMilitaryAccessButton.GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.AskForMilitaryAccessPanel());
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

            manpowerValue.text = string.Format("{0:#,0}", CountryManager.Instance.GetAvailableManpower(country));
            tensionValue.text = country.Tension.ToString();
            budgetValue.text = string.Format("{0:#,0}", float.Parse(country.Budget.ToString())) + " M";
            religionValue.text = CountryManager.Instance.GetReligionNameByReligionType(country.Religion);
            systemOfGovernmentValue.text = country.System_Of_Government;
            unemploymentRateValue.text = country.Unemployment_Rate.ToString();
            birthRateValue.text = country.Fertility_Rate_PerWeek.ToString();
            militaryRankValue.text = string.Format("{0:#,0}", float.Parse(country.Defense_Budget.ToString())) + " M";

            oilValue.text = country.GetMineral(MINERAL_TYPE.OIL).ToString(); //GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.OIL));
            uraniumValue.text = country.GetMineral(MINERAL_TYPE.URANIUM).ToString(); //GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.URANIUM));
            ironValue.text = country.GetMineral(MINERAL_TYPE.IRON).ToString(); //GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.IRON));
            steelValue.text = country.GetMineral(MINERAL_TYPE.STEEL).ToString(); // GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.STEEL));
            aluminiumValue.text = country.GetMineral(MINERAL_TYPE.ALUMINIUM).ToString(); //GetComponent<Button>().onClick.AddListener(() => ActionManager.Instance.ShowMineralBuy(MINERAL_TYPE.ALUMINIUM));
            
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

            oilRafineryValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.OIL_RAFINERY).ToString();
            universityValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.UNIVERSITY).ToString();
            nuclearFacilityValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.NUCLEAR_FACILITY).ToString();
            tradePortValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.TRADE_PORT).ToString();
            dockyardValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.DOCKYARD).ToString();
            hospitalValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.HOSPITAL).ToString();
            factoryValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.FACTORY).ToString();
            militaryFactoryValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.MILITARY_FACTORY).ToString();
            mineralFactoryValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.MINERAL_FACTORY).ToString();
            GarrisonValue.text = CountryManager.Instance.GetTotalBuildings(country, BUILDING_TYPE.GARRISON).ToString();

            selectedCountryLandAttackPowerValue.text = string.Format("{0:#,0}", country.GetArmy().GetLandForces().GetMilitaryPower());
            selectedCountryAirAttackPowerValue.text = string.Format("{0:#,0}", country.GetArmy().GetAirForces().GetMilitaryPower());
            selectedCountryNavalAttackPowerValue.text = string.Format("{0:#,0}", country.GetArmy().GetNavalForces().GetMilitaryPower());

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

            if (myCountry == null)
                return;

            if (selectedCountry == null)
                return;

            if (myCountry.GetArmy() != null)
            {
                myLandAttackPowerValue.text = string.Format("{0:#,0}", myCountry.GetArmy().GetLandForces().GetMilitaryPower());
                myAirAttackPowerValue.text = string.Format("{0:#,0}", myCountry.GetArmy().GetAirForces().GetMilitaryPower());
                myNavalAttackPowerValue.text = string.Format("{0:#,0}", myCountry.GetArmy().GetNavalForces().GetMilitaryPower());
            }
            else
            {
                myLandAttackPowerValue.text = "0";
                myAirAttackPowerValue.text = "0";
                myNavalAttackPowerValue.text = "0";
            }

            if (myCountry.GetArmy() != null)
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

            mineralTab.GetComponent<UITab>().isOn = false;
            diplomacyTab.GetComponent<UITab>().isOn = false;
            armyTab.GetComponent<UITab>().isOn = false;

            if (myCountry == selectedCountry)
            {
                mineralTab.SetActive(false);
                diplomacyTab.SetActive(false);
                armyTab.SetActive(false);
            }
            else
            {
                mineralTab.SetActive(true);
                diplomacyTab.SetActive(true);
                armyTab.SetActive(true);

                SetArmyBarChart();

                HideDiplomacyButtons();

                List<Country> enemyList = CountryManager.Instance.GetAllEnemies(myCountry);

                if (enemyList.Contains(selectedCountry) == false)
                {
                    AskForGunSupportButton.SetActive(true);
                    GiveGarrisonSupportButton.SetActive(true);
                    AskForMilitaryAccessButton.SetActive(true);
                    RequestGarrisonSupportButton.SetActive(true);
                    RequestLicenseProductionButton.SetActive(true);
                    GiveGunSupportButton.SetActive(true);
                    GiveMoneySupportButton.SetActive(true);
                    GiveControlOfStateButton.SetActive(true);
                    AskForControlOfProvinceButton.SetActive(true);
                }

                if (myCountry.Intelligence_Agency != null)
                {
                    AssassinationOfPresidentButton.SetActive(true);
                    MakeMilitaryCoupButton.SetActive(true);
                    StealTechnologyButton.SetActive(true);
                }

                if (CountryManager.Instance.GetLeftMilitaryAccess(myCountry, selectedCountry) > 0)
                {
                    CancelMilitaryAccessButton.SetActive(true);
                }

                if (myCountry.GetTradeEmbargo().Contains(selectedCountry) == false)
                {
                    PlaceTradeEmbargoButton.SetActive(true);
                }

                if (myCountry.GetArmsEmbargo().Contains(selectedCountry) == false)
                {
                    PlaceArmsEmbargoButton.SetActive(true);
                }

                if (CountryManager.Instance.GetAtWarCountryList(myCountry).Contains(selectedCountry))
                {
                    NuclearWarButton.SetActive(true);
                    SignPeaceTreatyButton.SetActive(true);
                }
                else
                {
                    if (myCountry.GetTradeTreaty().Contains(selectedCountry) != false)
                    {
                        SignTradeTreatyButton.SetActive(true);
                    }

                    DeclareWarButton.SetActive(true);
                    GiveMilitaryAccessButton.SetActive(true);
                }
            }          
        }

        void HideDiplomacyButtons()
        {
            AskForControlOfProvinceButton.SetActive(false);
            AskForGunSupportButton.SetActive(false);
            AssassinationOfPresidentButton.SetActive(false);
            AssassinationOfPresidentButton.SetActive(false);
            NuclearWarButton.SetActive(false);
            CancelMilitaryAccessButton.SetActive(false);
            DeclareWarButton.SetActive(false);
            GiveControlOfStateButton.SetActive(false);
            GiveGarrisonSupportButton.SetActive(false);
            GiveGunSupportButton.SetActive(false);
            GiveMilitaryAccessButton.SetActive(false);
            GiveMoneySupportButton.SetActive(false);
            MakeMilitaryCoupButton.SetActive(false);
            PlaceArmsEmbargoButton.SetActive(false);
            PlaceTradeEmbargoButton.SetActive(false);
            RequestGarrisonSupportButton.SetActive(false);
            RequestLicenseProductionButton.SetActive(false);
            SignPeaceTreatyButton.SetActive(false);
            StealTechnologyButton.SetActive(false);
            SignTradeTreatyButton.SetActive(false);
            AskForMilitaryAccessButton.SetActive(false);
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
