using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using System;

namespace WorldMapStrategyKit
{
    public class ArmyPanel : MonoBehaviour
    {
        private static ArmyPanel instance;
        public static ArmyPanel Instance
        {
            get { return instance; }
        }

        WMSK map;

        public GameObject armyPanel;
        public TextMeshProUGUI militaryPersonnel;
        public TextMeshProUGUI defenseBudget;
        public TextMeshProUGUI militaryRank;
        public TextMeshProUGUI dockyard;
        public TextMeshProUGUI militaryFactory;

        public GameObject starSprite;
        public GameObject weaponTechItem;
        public GameObject weaponTechContent;

        public GameObject divisionListItem;
        public GameObject divisionListContent;

        // selected division
        public GameObject selectedDivisionGO;
        public TextMeshProUGUI selectedDivisionName;
        public RawImage selectedDivisionIcon;
        public TextMeshProUGUI selectedDivisionLocation;
        public TextMeshProUGUI selectedDivisionStatus;
        public TextMeshProUGUI selectedDivisionAttackPower;
        public TextMeshProUGUI selectedDivisionDefense;
        public TextMeshProUGUI selectedDivisionAttackRange;
        public GameObject weaponsContent;

        public GameObject searchByWeaponTypeButton;

        public CustomDropdown weaponDropdown;

        public GameObject weaponContent;
        public GameObject weaponItem;

        public RawImage selectedWeaponImage;

        public TextMeshProUGUI weaponName;
        public TextMeshProUGUI weaponLevel;
        public TextMeshProUGUI weaponSpeed;
        public TextMeshProUGUI weaponCost;
        public TextMeshProUGUI weaponAttackPoint;
        public TextMeshProUGUI weaponDefense;
        public TextMeshProUGUI weaponAttackRange;
        public TextMeshProUGUI weaponProductionTime;
        public TextMeshProUGUI weaponTechYear;


        WeaponTemplate selectedWeaponTemplate = null;
        List<int> weaponList = new List<int>();
        List<Country> countryList;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;

            searchByWeaponTypeButton.GetComponent<Button>().onClick.AddListener(() => ShowWeaponsByWeaponType());

            weaponDropdown.dropdownEvent.AddListener(SelectWeaponDropdown);
        }

        public void Init()
        {
            InitWeaponDropdown();
        }

        void InitWeaponDropdown()
        {
            weaponDropdown.dropdownItems.Clear();
            weaponDropdown.enableIcon = false;
            weaponList.Clear();

            List<WeaponTemplate> weaponTemplateList = WeaponManager.Instance.GetWeaponTemplateList();

            foreach (WeaponTemplate weaponTemplate in weaponTemplateList)
            {
                if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetProducibleWeapons().Contains(weaponTemplate.weaponID) == false)
                {
                    weaponList.Add(weaponTemplate.weaponID);
                    weaponDropdown.SetItemTitle(weaponTemplate.weaponName + "   [ " + weaponTemplate.weaponLevel + " ]");
                    weaponDropdown.CreateNewItem();
                }
            }
        }

        void ClearUnits()
        {
            foreach (Transform child in weaponContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        void SelectWeaponDropdown(int index)
        {
            int weaponID = weaponList[index];
            selectedWeaponTemplate = WeaponManager.Instance.GetWeaponTemplateByID(weaponID);

            selectedWeaponImage.texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weaponID);

            weaponName.text = selectedWeaponTemplate.weaponName.ToString();

            weaponLevel.text = selectedWeaponTemplate.weaponLevel.ToString();
            weaponSpeed.text = selectedWeaponTemplate.weaponSpeed.ToString() + " mph";
            weaponCost.text = "$ " + string.Format("{0:#,0}", float.Parse(selectedWeaponTemplate.weaponCost.ToString()));
            weaponAttackPoint.text = selectedWeaponTemplate.weaponAttack.ToString();
            weaponDefense.text = selectedWeaponTemplate.weaponDefense.ToString();
            weaponAttackRange.text = selectedWeaponTemplate.weaponAttackRange.ToString() + " km";
            weaponProductionTime.text = selectedWeaponTemplate.weaponProductionTime.ToString() + " days";
            weaponTechYear.text = selectedWeaponTemplate.weaponResearchYear.ToString();
        }

        void ShowWeaponsByWeaponType()
        {
            if (selectedWeaponTemplate == null)
                return;

            ClearUnits();

            countryList = CountryManager.Instance.GetAllCountriesWhichHaveArmy(false);
            //Texture image = WeaponManager.Instance.GetWeaponTemplateIconByID(selectedWeaponTemplate.weaponID);

            foreach (Country country in countryList)
            {
                GameObject temp = null;

                foreach (int ID in country.GetProducibleWeapons())
                {
                    if (ID == selectedWeaponTemplate.weaponID)
                    {
                        temp = Instantiate(weaponItem, weaponContent.transform);

                        int weaponCost = CountryManager.Instance.GetWeaponPrice(GameEventHandler.Instance.GetPlayer().GetMyCountry(), country, selectedWeaponTemplate, 1);

                        temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = selectedWeaponTemplate.weaponName;
                        temp.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "$ " + string.Format("{0:#,0}", float.Parse(weaponCost.ToString())) + " M";
                        temp.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = selectedWeaponTemplate.weaponProductionTime.ToString() + " days";

                        temp.transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();

                        temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
                        {
                            BuyWeapon(country, weaponCost, Int32.Parse(temp.transform.GetChild(0).transform.GetChild(7).GetComponent<TMP_InputField>().text));
                        });

                        break;
                    }
                }

            }
        }

        void BuyWeapon(Country country, int weaponCost, int weaponNumber)
        {
            if (weaponNumber <= 0)
                return;

            int totalCost = weaponCost * weaponNumber;

            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            if (myCountry.GetArmy().Defense_Budget >= totalCost)
            {
                HUDManager.Instance.PrivateNotification("You have ordered " + selectedWeaponTemplate.weaponName + " x" + weaponNumber);

                int current = 0;
                myCountry.GetArmy().GetAllWeaponsInArmyInventory().TryGetValue(selectedWeaponTemplate, out current);

                Debug.Log(selectedWeaponTemplate.weaponName + " -> " + current);

                int buyerBudget = myCountry.GetArmy().Defense_Budget - totalCost;
                myCountry.GetArmy().Defense_Budget = buyerBudget;

                int sellerBudget = country.GetArmy().Defense_Budget + totalCost;
                country.GetArmy().Defense_Budget = sellerBudget;

                ActionManager.Instance.CreateAction(
                    myCountry,
                    country,
                    ACTION_TYPE.Purchase_Weapon,
                    MINERAL_TYPE.NONE,
                    0,
                    selectedWeaponTemplate,
                    weaponNumber,
                    null,
                    null,
                    0,
                    0,
                    0);
            }
        }

        public void ShowArmyPanel()
        {
            armyPanel.SetActive(true);

            ClearWeaponTechContent();
            ClearDivisionListContent();

            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            militaryPersonnel.text = myCountry.GetArmy().GetSoldierNumber().ToString();

            if( myCountry.GetArmy() != null )
                defenseBudget.text = "$ " + string.Format("{0:#,0}", float.Parse(myCountry.GetArmy().Defense_Budget.ToString())) + " M";

            militaryRank.text = myCountry.Military_Rank.ToString();
            militaryFactory.text = CountryManager.Instance.GetTotalBuildings(myCountry, BUILDING_TYPE.MILITARY_FACTORY).ToString();
            dockyard.text = CountryManager.Instance.GetTotalDockyard(myCountry).ToString();

            foreach(WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
                CreateWeaponButton(weapon.weaponName, myCountry.GetWeaponTech(weapon.weaponType));

            foreach(GameObjectAnimator division in GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy().GetAllDivisionInArmy())
            {
                CreateDivisionListButton(division);
            }
        }

        void CreateWeaponButton(string text, int tech)
        {
            GameObject GO = Instantiate(weaponTechItem, weaponTechContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;

            for(int i=0; i<tech; i++)
                Instantiate(starSprite, GO.transform.GetChild(4).transform);
        }

        void CreateDivisionListButton(GameObjectAnimator division)
        {
            GameObject GO = Instantiate(divisionListItem, divisionListContent.transform);
            GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = division.GetDivision().divisionName;
            GO.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = division.GetDivision().GetDivisionIcon();
            GO.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ShowSelectedDivision(division));
        }

        public void ShowSelectedDivision(GameObjectAnimator division)
        {
            selectedDivisionGO.SetActive(true);
            selectedDivisionName.text = division.GetDivision().divisionName;

            if(map.GetProvince(division.currentMap2DLocation) == null)
                selectedDivisionLocation.text = "On Ocean";
            else
                selectedDivisionLocation.text = map.GetProvince(division.currentMap2DLocation).name;

            selectedDivisionIcon.texture = division.GetDivision().GetDivisionIcon();

            if(division.isVisibleInViewport == true)
                selectedDivisionStatus.text = "In Operation";
            else
                selectedDivisionStatus.text = "In Garrison";

            selectedDivisionAttackPower.text = division.GetDivision().GetDivisionPower().ToString();
            selectedDivisionDefense.text = division.GetDivision().GetDivisionLeftDefense() + "/" + division.GetDivision().GetDivisionDefense();
            selectedDivisionAttackRange.text = division.GetDivision().GetDivisionMinimumAttackRange() + "/" + division.GetDivision().GetDivisionMaximumAttackRange();

            foreach (Transform child in weaponsContent.transform)
                Destroy(child.gameObject);

            GameObject GO = Instantiate(divisionListItem, weaponsContent.transform);
            GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(division.GetDivision().divisionTemplate.mainUnitIDList[0]).weaponName;
            GO.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(division.GetDivision().divisionTemplate.mainUnitIDList[0]);

            GameObject GO1 = Instantiate(divisionListItem, weaponsContent.transform);
            GO1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(division.GetDivision().divisionTemplate.secondUnitList[0]).weaponName;
            GO1.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(division.GetDivision().divisionTemplate.secondUnitList[0]);

            GameObject GO2 = Instantiate(divisionListItem, weaponsContent.transform);
            GO2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(division.GetDivision().divisionTemplate.thirdUnitList[0]).weaponName;
            GO2 .transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(division.GetDivision().divisionTemplate.thirdUnitList[0]);
        }

        public void HidePanel()
        {
            armyPanel.SetActive(false);

            ClearWeaponTechContent();
            ClearDivisionListContent();
        }

        void ClearWeaponTechContent()
        {
            foreach (Transform eachChild in weaponTechContent.transform)
            {
                Destroy(eachChild.gameObject);
            }
        }
        void ClearDivisionListContent()
        {
            foreach (Transform eachChild in divisionListContent.transform)
            {
                Destroy(eachChild.gameObject);
            }
        }
    }
}