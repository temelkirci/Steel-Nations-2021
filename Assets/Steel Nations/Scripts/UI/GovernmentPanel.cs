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
        //public Button declareWarButton;

        public GameObject weaponItem;
        public GameObject militaryContent;
        public GameObject diplomacyContent;

        public GameObject governmentPanel;
        public TextMeshProUGUI countryName;
        public TextMeshProUGUI govermentSystem;
        public TextMeshProUGUI religion;
        public TextMeshProUGUI financialStatus;
        public TextMeshProUGUI unemploymentRate;
        public TextMeshProUGUI natality;
        public TextMeshProUGUI nuclear_power;

        public GameObject flag_country;

        public TextMeshProUGUI manpower;
        public TextMeshProUGUI oil_rafinery;
        public TextMeshProUGUI port;
        public TextMeshProUGUI military_factory;
        public TextMeshProUGUI mining_factory;
        public TextMeshProUGUI airport;
        public TextMeshProUGUI rank;
        public TextMeshProUGUI hospital;
        public TextMeshProUGUI civil_factory;
        public TextMeshProUGUI university;
        public TextMeshProUGUI nuclear_facility;
        public TextMeshProUGUI tension;
        public TextMeshProUGUI defenseBudget;

        public TextMeshProUGUI oil;
        public TextMeshProUGUI iron;
        public TextMeshProUGUI steel;
        public TextMeshProUGUI aluminium;
        public TextMeshProUGUI uranium;

        public GameObject diplomacyButton;

        public TextMeshProUGUI leftCountryText;
        public TextMeshProUGUI rightCountryText;

        //declare war panel
        public GameObject declareWarPanel;
        public GameObject declareWarItem;
        public RectTransform declareWarLeftContent;
        public RectTransform declareWarRightContent;

        public GameObject declareWarButton;

        Country selectedCountry;

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

            foreach (Transform child in militaryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform eachChild in diplomacyContent.transform)
            {
                Destroy(eachChild.gameObject);
            }
        }
        public void ShowSelectCountryPanel(Country country)
        {
            governmentPanel.SetActive(true);
            declareWarPanel.SetActive(false);

            countryName.text = country.name;
            flag_country.GetComponent<RawImage>().texture = country.GetCountryFlag();

            manpower.text = string.Format("{0:#,0}", country.GetAvailableManpower());
            tension.text = country.GetTension().ToString();
            defenseBudget.text = "$ " + string.Format("{0:#,0}", float.Parse(country.GetArmy().GetDefenseBudget().ToString())) + " M";
            oil.text = country.GetTotalOilReserves().ToString();
            uranium.text = country.GetTotalUraniumReserves().ToString();
            iron.text = country.GetTotalIronReserves().ToString();
            steel.text = country.GetTotalSteelReserves().ToString();
            aluminium.text = country.GetTotalAluminiumReserves().ToString();

            religion.text = country.GetReligion();
            govermentSystem.text = country.GetSystemOfGovernment();
            financialStatus.text = country.attrib["Financial Status"];
            unemploymentRate.text = country.GetUnemploymentRate().ToString();
            natality.text = country.GetFertilityRatePerWeek().ToString();

            oil_rafinery.text = country.GetTotalBuildings(BUILDING_TYPE.OIL_RAFINERY).ToString();
            university.text = country.GetTotalBuildings(BUILDING_TYPE.UNIVERSITY).ToString();
            nuclear_facility.text = country.GetTotalBuildings(BUILDING_TYPE.NUCLEAR_FACILITY).ToString();
            port.text = country.GetTotalBuildings(BUILDING_TYPE.TRADE_PORT).ToString();
            airport.text = country.GetTotalBuildings(BUILDING_TYPE.AIRPORT).ToString();
            hospital.text = country.GetTotalBuildings(BUILDING_TYPE.HOSPITAL).ToString();
            civil_factory.text = country.GetTotalBuildings(BUILDING_TYPE.FACTORY).ToString();
            military_factory.text = country.GetTotalBuildings(BUILDING_TYPE.MILITARY_FACTORY).ToString();
            mining_factory.text = country.GetTotalBuildings(BUILDING_TYPE.MINERAL_FACTORY).ToString();

            rank.text = country.GetMilitaryRank().ToString();

            foreach (Transform child in militaryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform eachChild in diplomacyContent.transform)
            {
                Destroy(eachChild.gameObject);
            }
        }
        public void ShowGovernmentPanel()
        {
            selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();

            ShowSelectCountryPanel(selectedCountry);

            SelectCountry.Instance.startButton.SetActive(false);

            /*
            foreach (Transform child in organizationContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Organization org in OrganizationManager.Instance.GetAllOrganizations())
            {
                if (org.isFullMemberCountry(selectedCountry))
                {
                    GameObject flag = Instantiate(organizationFlag, organizationContent.transform);
                    flag.transform.GetChild(0).GetComponent<RawImage>().texture = org.organizationLogo;
                    flag.GetComponent<SimpleTooltip>().infoLeft = org.organizationName;
                }
            }
            */
            foreach (var weapon in selectedCountry.GetArmy().GetAllWeaponsInArmyInventory())
            {
                GameObject temp = Instantiate(weaponItem, militaryContent.transform);

                temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = weapon.Key.weaponName;
                temp.gameObject.transform.GetChild(3).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weapon.Key.weaponID);
                temp.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = weapon.Value.ToString();

                temp.SetActive(true);
            }

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

        GameObject CreateDiplomacyButton(string text)
        {
            GameObject GO = Instantiate(diplomacyButton, diplomacyContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;

            return GO;
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
            GameEventHandler.Instance.GetPlayer().GetMyCountry().AddNuclearWar(GameEventHandler.Instance.GetPlayer().GetSelectedCountry());
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

            foreach (Country country in GameEventHandler.Instance.GetAllCountries())
            {
                if (country != GameEventHandler.Instance.GetPlayer().GetMyCountry() && GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib[country.name] >= 50)
                {
                    GameObject temp = Instantiate(declareWarItem, declareWarLeftContent.transform);

                    temp.gameObject.transform.GetChild(1).gameObject.SetActive(true);

                    temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = country.name;
                    temp.gameObject.transform.GetChild(2).GetComponent<RawImage>().texture = country.GetCountryFlag();
                }
            }

            foreach (Country country in GameEventHandler.Instance.GetAllCountries())
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
    }
}
