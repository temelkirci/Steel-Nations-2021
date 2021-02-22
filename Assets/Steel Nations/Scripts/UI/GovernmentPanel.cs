using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WorldMapStrategyKit
{
    public class GovernmentPanel : MonoBehaviour
    {
        private static GovernmentPanel instance;
        public static GovernmentPanel Instance
        {
            get { return instance; }
        }

        WMSK map;

        public GameObject weaponItem;
        public GameObject organizationItem;
        public GameObject organizationFlag;

        public GameObject overviewContent;
        public GameObject mineralContent;
        public GameObject militaryContent;
        public GameObject diplomacyContent;
        public GameObject buildingContent;

        public GameObject governmentPanel;
        public GameObject governmentItem;

        public GameObject flag_country;

        public GameObject diplomacyButton;

        public TextMeshProUGUI countryName;

        public TextMeshProUGUI leftCountryText;
        public TextMeshProUGUI rightCountryText;

        //declare war panel
        public GameObject declareWarPanel;
        public GameObject declareWarItem;
        public RectTransform declareWarLeftContent;
        public RectTransform declareWarRightContent;

        public GameObject declareWarButton;

        Country selectedCountry;
        bool IsMyCountry = false;

        public GameObject founderContent;
        public GameObject fullMemberContent;
        public GameObject observerContent;
        public GameObject dialoguePartner;
        public GameObject appliedFullMemberContent;
        public GameObject appliedObserverContent;
        public GameObject appliedDialoguePartner;

        public TextMeshProUGUI OilText;
        public TextMeshProUGUI IronText;
        public TextMeshProUGUI SteelText;
        public TextMeshProUGUI UraniumText;
        public TextMeshProUGUI AluminiumText;
        private void Start()
        {
            instance = this;
            map = WMSK.instance;            
        }

        public void HidePanel()
        {
            governmentPanel.SetActive(false);
            declareWarPanel.SetActive(false);

            GameEventHandler.Instance.GetPlayer().SetSelectedCountry(null);

            ClearAllContents();
        }
        public void ShowSelectCountryPanel(Country country)
        {
            governmentPanel.SetActive(true);
            declareWarPanel.SetActive(false);

            countryName.text = country.name;
            flag_country.GetComponent<RawImage>().texture = country.GetCountryFlag();

            ClearAllContents();

            CreateOverviewButton("Manpower", string.Format("{0:#,0}", country.GetAvailableManpower()));
            CreateOverviewButton("Tension", country.GetTension().ToString());
            CreateOverviewButton("Defense Budget", "$ " + string.Format("{0:#,0}", float.Parse(country.GetArmy().GetDefenseBudget().ToString())) + " M");
            CreateOverviewButton("Religion", country.GetReligion());
            CreateOverviewButton("System Of Government", country.GetSystemOfGovernment());
            CreateOverviewButton("Unemployment Rate", country.GetUnemploymentRate().ToString());
            CreateOverviewButton("Birth Rate", country.GetFertilityRatePerWeek().ToString());
            CreateOverviewButton("Rank", country.GetMilitaryRank().ToString());
            /*
            CreateMineralButton("Oil", country.GetTotalOilReserves().ToString());
            CreateMineralButton("Uranium", country.GetTotalUraniumReserves().ToString());
            CreateMineralButton("Iron", country.GetTotalIronReserves().ToString());
            CreateMineralButton("Steel", country.GetTotalSteelReserves().ToString());
            CreateMineralButton("Aluminium", country.GetTotalAluminiumReserves().ToString());
            */

            SetMinerals();

            CreateMilitaryButton("Land Attack Power", string.Format("{0:#,0}", country.GetArmy().GetLandForces().GetMilitaryPower()));
            CreateMilitaryButton("Air Attack Power", string.Format("{0:#,0}", country.GetArmy().GetAirForces().GetMilitaryPower()));
            CreateMilitaryButton("Naval Attack Power", string.Format("{0:#,0}", country.GetArmy().GetNavalForces().GetMilitaryPower()));

            CreateBuildingButton("Oil Rafinery", country.GetTotalBuildings(BUILDING_TYPE.OIL_RAFINERY).ToString());
            CreateBuildingButton("University", country.GetTotalBuildings(BUILDING_TYPE.UNIVERSITY).ToString());
            CreateBuildingButton("Nuclear Facility", country.GetTotalBuildings(BUILDING_TYPE.NUCLEAR_FACILITY).ToString());
            CreateBuildingButton("Trade Port", country.GetTotalBuildings(BUILDING_TYPE.TRADE_PORT).ToString());
            CreateBuildingButton("Airport", country.GetTotalBuildings(BUILDING_TYPE.AIRPORT).ToString());
            CreateBuildingButton("Hospital", country.GetTotalBuildings(BUILDING_TYPE.HOSPITAL).ToString());
            CreateBuildingButton("Factory", country.GetTotalBuildings(BUILDING_TYPE.FACTORY).ToString());
            CreateBuildingButton("Military Factory", country.GetTotalBuildings(BUILDING_TYPE.MILITARY_FACTORY).ToString());
            CreateBuildingButton("Mineral Factory", country.GetTotalBuildings(BUILDING_TYPE.MINERAL_FACTORY).ToString());

            CreateOrganizations();
        }

        void SetMinerals()
        {
            OilText.text = selectedCountry.GetOil().ToString();
            IronText.text = selectedCountry.GetIron().ToString();
            SteelText.text = selectedCountry.GetSteel().ToString();
            AluminiumText.text = selectedCountry.GetAluminium().ToString();
            UraniumText.text = selectedCountry.GetUranium().ToString();
        }
        void ClearAllContents()
        {
            ClearBuildingContent();
            ClearDiplomacyContent();
            ClearMineralContent();
            ClearOverviewContent();
            ClearMilitaryContent();
            ClearOrganizationContent();
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
            foreach (Transform child in militaryContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        void ClearDiplomacyContent()
        {
            foreach (Transform child in diplomacyContent.transform)
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

        public void CreateOrganizations()
        {
            foreach (Organization org in OrganizationManager.Instance.GetAllOrganizations())
            {
                GameObject flag = null;

                if (org.GetObserverList().Contains(selectedCountry))
                {
                    flag = Instantiate(organizationFlag, observerContent.transform.GetChild(4));
                }

                if (org.GetFullMemberList().Contains(selectedCountry))
                {
                    flag = Instantiate(organizationFlag, fullMemberContent.transform.GetChild(4));
                }

                if (org.GetFounderList().Contains(selectedCountry))
                {
                    flag = Instantiate(organizationFlag, founderContent.transform.GetChild(4));
                }

                if (org.GetDialoguePartnerList().Contains(selectedCountry))
                {
                    flag = Instantiate(organizationFlag, dialoguePartner.transform.GetChild(4));
                }

                if (org.GetApplyForFullMemberList().Contains(selectedCountry))
                {
                    flag = Instantiate(organizationFlag, appliedFullMemberContent.transform.GetChild(4));
                }

                if (org.GetApplyForObserverList().Contains(selectedCountry))
                {
                    flag = Instantiate(organizationFlag, appliedObserverContent.transform.GetChild(4));
                }

                if (org.GetApplyForDialoguePartnerList().Contains(selectedCountry))
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
        public void ShowGovernmentPanel()
        {
            selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();

            if (selectedCountry == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                IsMyCountry = true;

            ShowSelectCountryPanel(selectedCountry);

            SelectCountry.Instance.startButton.SetActive(false); 
            

            if(IsMyCountry == true)
            {
                CreateDiplomacyButton("Change System Of Government").GetComponent<Button>().onClick.AddListener(() => PlaceArmsEmbargo());
            }
            else
            {
                if (GameEventHandler.Instance.IsGameStarted() == true)
                {
                    if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetIntelligenceAgency() != null)
                    {
                        CreateDiplomacyButton("Assassination of President").GetComponent<Button>().onClick.AddListener(() => AssassinationOfPresident());
                        CreateDiplomacyButton("Steal Technology").GetComponent<Button>().onClick.AddListener(() => StealTechnology());
                        CreateDiplomacyButton("Make A Military Coup").GetComponent<Button>().onClick.AddListener(() => MakeMilitaryCoup());
                    }


                    if (GameEventHandler.Instance.GetPlayer().GetMyCountry().AtWar(selectedCountry))
                    {
                        CreateDiplomacyButton("Sign a peace treaty").GetComponent<Button>().onClick.AddListener(() => SignPeaceTreaty());

                        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetNuclearWarHead() > 0)
                        {
                            CreateDiplomacyButton("Begin Nuclear War").GetComponent<Button>().onClick.AddListener(() => BeginNuclearWar());
                        }
                    }
                    else
                    {
                        CreateDiplomacyButton("Declare War").GetComponent<Button>().onClick.AddListener(() => ShowDeclareWarPanel());

                        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetBudget() < selectedCountry.GetBudget())
                        {
                            CreateDiplomacyButton("Ask For Money Support").GetComponent<Button>().onClick.AddListener(() => AskForMoneySupport());
                        }
                        else
                        {
                            CreateDiplomacyButton("Give Money Support").GetComponent<Button>().onClick.AddListener(() => GiveMoneySupport());
                        }

                        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetLeftMilitaryAccess(selectedCountry) > 0)
                        {
                            CreateDiplomacyButton("Cancel military access").GetComponent<Button>().onClick.AddListener(() => CancelMilitaryAccess());
                        }
                        else
                        {
                            CreateDiplomacyButton("Give military access").GetComponent<Button>().onClick.AddListener(() => GiveMilitaryAccess());
                        }

                        if (selectedCountry.GetLeftMilitaryAccess(GameEventHandler.Instance.GetPlayer().GetMyCountry()) > 0)
                        {
                            CreateDiplomacyButton("Cancel military access").GetComponent<Button>().onClick.AddListener(() => CancelMilitaryAccess());
                        }

                        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetCountryListAtWar().Count > 0)
                        {
                            CreateDiplomacyButton("Request Garrison support").GetComponent<Button>().onClick.AddListener(() => RequestGarrisonSupport());
                        }

                        if (selectedCountry.GetCountryListAtWar().Count > 0)
                        {
                            CreateDiplomacyButton("Give Garrison support").GetComponent<Button>().onClick.AddListener(() => GiveGarrisonSupport());
                        }

                        CreateDiplomacyButton("Give Gun Support").GetComponent<Button>().onClick.AddListener(() => GiveGunSupport());
                        CreateDiplomacyButton("Ask For Gun Support").GetComponent<Button>().onClick.AddListener(() => AskForGunSupport());
                        CreateDiplomacyButton("Request License Production").GetComponent<Button>().onClick.AddListener(() => RequestLicenseProduction());
                    }

                    CreateDiplomacyButton("Place Arms Embargo").GetComponent<Button>().onClick.AddListener(() => PlaceArmsEmbargo());
                    CreateDiplomacyButton("Place Trade Embargo").GetComponent<Button>().onClick.AddListener(() => PlaceTradeEmbargo());
                    CreateDiplomacyButton("Ask for control of state").GetComponent<Button>().onClick.AddListener(() => AskForControlOfState());
                    CreateDiplomacyButton("Give control of state").GetComponent<Button>().onClick.AddListener(() => GiveControlOfState());
                }
            }
            
        }

        public void AssassinationOfPresident()
        {
            if (selectedCountry.GetIntelligenceAgency() == null)
            {
                NotificationManager.Instance.CreateNotification("The president of " + selectedCountry.name + " was killed");
            }
            else
            {
                int succes = Random.Range(0, GameEventHandler.Instance.GetPlayer().GetMyCountry().GetIntelligenceAgency().GetAssassinationOfPresident());
                int enemy = Random.Range(0, selectedCountry.GetIntelligenceAgency().GetAssassinationOfPresident());

                if(succes > enemy)
                {
                    NotificationManager.Instance.CreateNotification("The president of " + selectedCountry.name + " was killed");
                }
                else
                {
                    NotificationManager.Instance.CreateNotification("The president of " + selectedCountry.name + " was saved");
                }
            }

        }
        public void StealTechnology()
        {

        }
        public void MakeMilitaryCoup()
        {

        }

        public void AskForControlOfState()
        {

        }

        public void GiveControlOfState()
        {

        }
        public void PlaceTradeEmbargo()
        {
            GameEventHandler.Instance.GetPlayer().GetMyCountry().PlaceTradeEmbargo(GameEventHandler.Instance.GetPlayer().GetSelectedCountry());
        }
        public void PlaceArmsEmbargo()
        {
            GameEventHandler.Instance.GetPlayer().GetMyCountry().PlaceArmsEmbargo(GameEventHandler.Instance.GetPlayer().GetSelectedCountry());
        }

        public void RequestLicenseProduction()
        {

        }

        public void RequestGarrisonSupport()
        {

        }
        public void GiveGarrisonSupport()
        {

        }
        public void GiveGunSupport()
        {

        }
        public void AskForGunSupport()
        {

        }
        public void GiveMoneySupport()
        {

        }

        public void AskForMoneySupport()
        {

        }

        public void CancelMilitaryAccess()
        {

        }
        public void AskForMilitaryAccess()
        {

        }
        public void GiveMilitaryAccess()
        {
            GameEventHandler.Instance.GetPlayer().GetMyCountry().AddMilitaryAccess(GameEventHandler.Instance.GetPlayer().GetSelectedCountry());
        }
        public void BeginNuclearWar()
        {
            GameEventHandler.Instance.GetPlayer().GetMyCountry().BeginNuclearWar(GameEventHandler.Instance.GetPlayer().GetSelectedCountry());
        }

        public void SignPeaceTreaty()
        {

        }

        public void ShowDeclareWarPanel()
        {
            if (GameEventHandler.Instance.GetPlayer().GetSelectedCountry() == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                return;

            declareWarPanel.SetActive(true);

            leftCountryText.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().name + "'s Allies";
            rightCountryText.text = selectedCountry.name + "'s Allies";

            declareWarButton.GetComponent<Button>().onClick.AddListener(() => GameEventHandler.Instance.GetPlayer().GetMyCountry().DeclareWar(selectedCountry));

            foreach (Transform child in declareWarLeftContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in declareWarRightContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Country country in CountryManager.Instance.GetAllCountries())
            {
                if (country != GameEventHandler.Instance.GetPlayer().GetMyCountry() && GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib[country.name] >= 50)
                {
                    GameObject temp = Instantiate(declareWarItem, declareWarLeftContent.transform);

                    temp.gameObject.transform.GetChild(1).gameObject.SetActive(true);

                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = country.name;
                    temp.gameObject.transform.GetChild(2).GetComponent<RawImage>().texture = country.GetCountryFlag();
                }
            }

            foreach (Country country in CountryManager.Instance.GetAllCountries())
            {
                if (country != GameEventHandler.Instance.GetPlayer().GetMyCountry() && GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib[country.name] >= 50)
                {
                    GameObject temp = Instantiate(declareWarItem, declareWarRightContent.transform);

                    temp.gameObject.transform.GetChild(1).gameObject.SetActive(true);

                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = country.name;
                    temp.gameObject.transform.GetChild(2).GetComponent<RawImage>().texture = country.GetCountryFlag();
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

        GameObject CreateDiplomacyButton(string text)
        {
            GameObject GO = Instantiate(diplomacyButton, diplomacyContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;

            return GO;
        }

        GameObject CreateMineralButton(string text, string value)
        {
            GameObject GO = Instantiate(governmentItem, mineralContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;
            GO.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = value;

            return GO;
        }
        GameObject CreateBuildingButton(string text, string value)
        {
            GameObject GO = Instantiate(governmentItem, buildingContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;
            GO.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = value;

            return GO;
        }
        GameObject CreateMilitaryButton(string text, string value)
        {
            GameObject GO = Instantiate(governmentItem, militaryContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;
            GO.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = value;

            return GO;
        }
        
    }
}
