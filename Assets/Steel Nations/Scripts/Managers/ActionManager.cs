using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Michsky.UI.ModernUIPack;
using WorldMapStrategyKit;
using System.Collections.Generic;

public class ActionManager : MonoBehaviour
{
    private static ActionManager instance;
    public static ActionManager Instance
    {
        get { return instance; }
    }

    WMSK map;
    
    List<Action> actionList = new List<Action>();

    public GameObject actionContent;

    public GameObject actionPanel;
    public TextMeshProUGUI actionTitle;
    public TextMeshProUGUI actionDescription;
    public Button acceptButton;
    public Button declineButton;

    // Declare War
    public GameObject declareWarPanel;
    public GameObject declareWarItem;
    public RectTransform declareWarLeftContent;
    public RectTransform declareWarRightContent;
    public TextMeshProUGUI leftCountryText;
    public TextMeshProUGUI rightCountryText;
    public GameObject declareWarButton;

    // Buy Mineral
    public GameObject buyMineralPanel;
    public TextMeshProUGUI mineralName;
    public RawImage mineralImage;
    public GameObject buyMineralButton;
    public TextMeshProUGUI mineralTotalCost;
    public TMP_InputField mineralInputField;
    int totalMineralCost;
    int mineralNumber = 0;
    MINERAL_TYPE mineralType;

    // Steal Weapon
    public GameObject stealTech;
    public CustomDropdown weaponDropdown;
    public TextMeshProUGUI selectedWeaponToSteal;
    public TextMeshProUGUI stealWeaponSuccessRate;
    public RawImage weaponImage;
    WeaponTemplate stealWeapon;
    List<int> stealWeaponList = new List<int>();
    public GameObject stealWeaponTechButton;


    // Assassination Of President
    public GameObject assassinationOfPresidentPanel;
    public GameObject assassinationOfPresidentButton;
    public TextMeshProUGUI assassinationOfPresidentSuccessRate;

    // Military Coup
    public GameObject militaryCoupPanel;
    public GameObject militaryCoupButton;
    public TextMeshProUGUI militaryCoupSuccessRate;

    // Ask For Control Of Region
    public GameObject askForControlOfRegionPanel;
    public GameObject askForControlOfRegionButton;
    public TextMeshProUGUI askForControlOfRegionSuccessRate;
    public CustomDropdown askForControlOfRegionDropdown;
    public TextMeshProUGUI askForControlOfRegionSelectedProvinceName;
    Province askForControlOfRegionProvince;

    // Give Control Of Region
    public GameObject giveControlOfRegionPanel;
    public GameObject giveControlOfRegionButton;
    public TextMeshProUGUI giveControlOfRegionSuccessRate;
    public CustomDropdown giveControlOfRegionDropdown;
    public TextMeshProUGUI giveControlOfRegionSelectedProvinceName;
    Province giveControlOfRegionProvince;

    // Place Trade Embargo
    public GameObject placeTradeEmbargoPanel;
    public GameObject placeTradeEmbargoButton;

    // Place Arms Embargo
    public GameObject placeArmsEmbargoPanel;
    public GameObject placeArmsEmbargoButton;

    // Give Money Support
    public GameObject moneySupportPanel;
    public GameObject giveMoneySupportButton;
    public TextMeshProUGUI moneyAmountText;
    public TMP_InputField moneyAmountInputField;
    int giveMoneyAmount;

    // Ask For Money Support
    public GameObject askForMoneySupportPanel;
    public GameObject askForMoneySupportButton;

    // Give Garrison Support
    public GameObject giveGarrisonSupportPanel;
    public GameObject giveGarrisonSupportButton;
    public CustomDropdown giveGarrisonDropdown;
    public TextMeshProUGUI giveSelectedGarrison;
    Division giveDivision;

    // Request Garrison Support
    public GameObject requestGarrisonSupportPanel;
    public GameObject requestGarrisonSupportButton;
    public CustomDropdown requestGarrisonDropdown;
    public TextMeshProUGUI requestSelectedGarrison;


    // Request License Production
    public GameObject requestLicenseProductionPanel;
    public GameObject requestLicenseProductionButton;
    public CustomDropdown licenseDropdown;
    public TextMeshProUGUI requestLicenseProductionSuccessRate;
    public TextMeshProUGUI selectedWeaponLicense;
    public RawImage requestLicenseWeaponImage;
    List<int> requestLicenseProductionList = new List<int>();
    WeaponTemplate requestLicenseWeapon;


    // Begin Nuclear War
    public GameObject beginNuclearWarPanel;
    public GameObject beginNuclearWarButton;

    // Sign A Peace Treaty
    public GameObject signPeaceTreatyPanel;
    public GameObject signPeaceTreatyButton;

    // Cancel Military Access
    public GameObject cancelMilitaryAccessPanel;
    public GameObject cancelMilitaryAccessButton;

    // Give Military Access
    public GameObject giveMilitaryAccessPanel;
    public GameObject giveMilitaryAccessButton;
    public TextMeshProUGUI giveMilitaryAccessAmountText;
    public TMP_InputField giveMilitaryAccessAmountInputField;
    int giveMilitaryAccessAmount;


    // Give Gun Support
    public GameObject giveGunSupportPanel;
    public GameObject giveGunSupportButton;
    public TextMeshProUGUI giveGunAmountText;
    public TMP_InputField giveGunAmountInputField;
    WeaponTemplate giveGunWeaponTemplate;
    int giveGunAmount;


    // Ask For Gun Support
    public GameObject askForGunSupportPanel;
    public GameObject askForGunSupportButton;

    // Ask For Military Access
    public GameObject askForMilitaryAccessPanel;
    public GameObject askForMilitaryAccessButton;

    void Start()
    {
        instance = this;
        map = WMSK.instance;
    }

    public void Init()
    {
        weaponDropdown.dropdownEvent.AddListener(SelectWeaponToSteal);
        askForControlOfRegionDropdown.dropdownEvent.AddListener(AskForControlOfProvinceSelectProvince);
        giveControlOfRegionDropdown.dropdownEvent.AddListener(GiveControlOfRegionSelectProvince);
        licenseDropdown.dropdownEvent.AddListener(RequestLicenseProduction);

        mineralInputField.onValueChanged.AddListener(delegate { CalculateMineralCost(); });
        moneyAmountInputField.onValueChanged.AddListener(delegate { GiveMoneySupport(); });

        giveGarrisonSupportButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Give_Garrison_Support,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                giveDivision,
                0,
                0,
                0));

        requestGarrisonSupportButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Request_Garrison_Support,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        beginNuclearWarButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Begin_Nuclear_War,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        signPeaceTreatyButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Sign_A_Peace_Treaty,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        cancelMilitaryAccessButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Cancel_Military_Access,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        giveMilitaryAccessButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Give_Military_Access,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                giveMilitaryAccessAmount,
                0,
                0));

        askForMilitaryAccessButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Ask_For_Military_Access,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                giveMilitaryAccessAmount,
                0,
                0));

        askForGunSupportButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Ask_For_Gun_Support,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        giveGunSupportButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Give_Gun_Support,
                MINERAL_TYPE.NONE,
                0,
                giveGunWeaponTemplate,
                giveGunAmount,
                null,
                null,
                0,
                0,
                0));


        placeArmsEmbargoButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Place_Arms_Embargo,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        placeTradeEmbargoButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Place_Trade_Embargo,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        giveControlOfRegionButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Give_Control_Of_Region,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                giveControlOfRegionProvince,
                null,
                0,
                0,
                0));


        stealWeaponTechButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Steal_Technology,
                MINERAL_TYPE.NONE,
                0,
                stealWeapon,
                0,
                null,
                null,
                0,
                0,
                0));



        assassinationOfPresidentButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Assassination_Of_President,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        militaryCoupButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Make_A_Military_Coup,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));


        declareWarButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Declare_War,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                0,
                0));

        buyMineralButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Buy_Mineral,
                mineralType,
                mineralNumber,
                null,
                0,
                null,
                null,
                0,
                totalMineralCost,
                0));

        giveMoneySupportButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Give_Money_Support,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                null,
                null,
                0,
                giveMoneyAmount,
                0));


        requestLicenseProductionButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Request_License_Production,
                MINERAL_TYPE.NONE,
                0,
                requestLicenseWeapon,
                0,
                null,
                null,
                0,
                0,
                0));

        askForControlOfRegionButton.GetComponent<Button>().onClick.AddListener(() => CreateAction(
                GameEventHandler.Instance.GetPlayer().GetMyCountry(),
                GameEventHandler.Instance.GetPlayer().GetSelectedCountry(),
                ACTION_TYPE.Ask_For_Control_Of_Region,
                MINERAL_TYPE.NONE,
                0,
                null,
                0,
                askForControlOfRegionProvince,
                null,
                0,
                0,
                0));

    }

    public void ShowActionPanel(Country country, Action action)
    {
        actionPanel.SetActive(true);

        actionTitle.text = action.ActionName;
        actionDescription.text = GetActionDescription(country, action);

        acceptButton.onClick.AddListener(() => ApplyMyAction(country, action, true));
        declineButton.onClick.AddListener(() => ApplyMyAction(country, action, false));
    }

    void ApplyMyAction(Country country, Action action, bool accept)
    {
        actionPanel.SetActive(false);

        ApplyAction(country, action, accept);
    }

    public void HideAllActionPanels()
    {
        foreach (Transform child in actionContent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void AddAction(Action action)
    {
        actionList.Add(action);
    }

    public List<Action> GetActionList()
    {
        return actionList;
    }

    public Action GetActionByActionType(ACTION_TYPE ActionType)
    {
        foreach (Action action in actionList)
            if (action.ActionType == ActionType)
                return action;

        return null;
    }

    public void CreateAction(Country country1,
        Country country2, 
        ACTION_TYPE actionType, 
        MINERAL_TYPE mineralType, 
        int amountOfMineral, 
        WeaponTemplate weaponTemplate,
        int amountOfWeapon, 
        Province province,
        Division division,
        int militaryAccess,
        int payMoney,
        int getMoney)
    {
        Action action = new Action();

        action = GetActionByActionType(actionType);
        action.Country = country2;

        if (country1 == GameEventHandler.Instance.GetPlayer().GetMyCountry())
            HideAllActionPanels();

        if (actionType == ACTION_TYPE.Ask_For_Gun_Support || 
            actionType == ACTION_TYPE.Give_Gun_Support || 
            actionType == ACTION_TYPE.Purchase_Weapon || 
            actionType == ACTION_TYPE.Steal_Technology ||
            actionType == ACTION_TYPE.Request_License_Production)

            action.Weapon = weaponTemplate;

        if (actionType == ACTION_TYPE.Purchase_Weapon)
        {
            action.PayMoney = payMoney;
            action.WeaponAmount = amountOfWeapon;
            action.Weapon = weaponTemplate;

            action.EventFinishTime = weaponTemplate.weaponProductionTime * amountOfWeapon;
        }

        if(actionType == ACTION_TYPE.Steal_Technology)
        {
            action.SuccessRate = country1.Intelligence_Agency.ReverseEnginering;
        }

        if(actionType == ACTION_TYPE.Ask_For_Control_Of_Region || actionType == ACTION_TYPE.Give_Control_Of_Region)
        {
            action.Province = province;
        }

        if(actionType == ACTION_TYPE.Buy_Mineral)
        {
            if (country2.GetMineral(mineralType) < amountOfMineral)
            {
                action = null;
                return;
            }
            else
            {
                if (country1.Budget < totalMineralCost)
                {
                    action = null;
                    return;
                }
                else
                {
                    action.SetMineral(mineralType, amountOfMineral);
                }
            }
        }

        if(actionType == ACTION_TYPE.Give_Money_Support)
        {
            action.PayMoney = payMoney;
        }

        if (actionType == ACTION_TYPE.Ask_For_Money_Support)
        {
            action.EarnMoney = getMoney;
        }

        country1.AddAction(action);
    }


    public void ApplyAction(Country country, Action action, bool accept)
    {
        int success = 100;

        if (action.Country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
        {
            if (accept)
                action.SuccessRate = 100;
            else
                action.SuccessRate = 0;
        }
        else
        {
            if (action.SuccessRate < 100)
                success = UnityEngine.Random.Range(0, 100);
        }


        if (action.ActionType == ACTION_TYPE.Ask_For_Control_Of_Region)
        {
            if(action.SuccessRate >= success)
            {
                map.CountryTransferProvinceRegion(map.GetCountryIndex(country), action.Province.mainRegion, true);

                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                    HUDManager.Instance.PrivateNotification("You have taken control of " + action.Province.name);
            }
            else
            {
                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                    HUDManager.Instance.PrivateNotification(action.Country.name + " has denied your request to take control of " + action.Province.name);
            }
        }

        if (action.ActionType == ACTION_TYPE.Ask_For_Gun_Support)
        {
            if (action.SuccessRate >= success)
            {

            }
            else
            {
                
            }
        }

        if (action.ActionType == ACTION_TYPE.Ask_For_Money_Support)
        {
            if (action.SuccessRate >= success)
            {
                int addMoneyPerc = UnityEngine.Random.Range(0, 10);

                long addMoney = (action.Country.Budget * addMoneyPerc) / 100;

                country.Budget += addMoney;
                action.Country.Budget -= addMoney;
            }
            else
            {

            }
        }

        if (action.ActionType == ACTION_TYPE.Assassination_Of_President)
        {
            if (action.SuccessRate >= success)
            {
                WorldMapStrategyKit.NotificationManager.Instance.CreatePublicNotification("The president of " + action.Country.name + " was killed");

                action.Country.Tension -= 25;
            }
            else
            {
                WorldMapStrategyKit.NotificationManager.Instance.CreatePublicNotification("The president of " + action.Country.name + " was saved");

                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                {
                    HUDManager.Instance.PrivateNotification("The president of " + action.Country.name + " was saved");
                }
            }
        }

        if (action.ActionType == ACTION_TYPE.Begin_Nuclear_War)
        {
            if (action.SuccessRate >= success)
            {
                WarManager.Instance.BeginMissileWar(country, action.Country);
            }
        }

        if (action.ActionType == ACTION_TYPE.Buy_Mineral)
        {
            country.AddMineral(action.MineralType, action.MineralAmount);
            country.Budget -= action.PayMoney;

            action.Country.AddMineral(action.MineralType, -action.MineralAmount);
            action.Country.Budget += action.PayMoney;

            HUDManager.Instance.PrivateNotification("You have bought " + MineralManager.Instance.GetMineralNameByType(action.MineralType) + "x" + action.MineralAmount);

            GovernmentPanel.Instance.ShowGovernmentPanel();
            HUDManager.Instance.UpdateMyResources();
        }

        if (action.ActionType == ACTION_TYPE.Cancel_Military_Access)
        {
            if (action.SuccessRate >= success)
            {
                CountryManager.Instance.CancelMilitaryAccess(country, action.Country);
            }
        }

        if (action.ActionType == ACTION_TYPE.Declare_War)
        {
            WarManager.Instance.DeclareWar(country, action.Country);

            //OrganizationManager.Instance.DeclareWarToDefense(country, action.Country);
        }

        if (action.ActionType == ACTION_TYPE.Give_Control_Of_Region)
        {
            if (action.SuccessRate >= success)
            {
                map.CountryTransferProvinceRegion(map.GetCountryIndex(action.Country), action.Province.mainRegion, true);

                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                    HUDManager.Instance.PrivateNotification("You gave control of " + action.Province.name);
            }
        }

        if (action.ActionType == ACTION_TYPE.Give_Garrison_Support)
        {
            if (action.SuccessRate >= success)
            {
                DivisionManager.Instance.DivisionTransferToCountry(country, action.Country, action.Division);
            }
        }

        if (action.ActionType == ACTION_TYPE.Give_Gun_Support)
        {
            if (action.SuccessRate >= success)
            {
                action.Country.GetArmy().AddWeaponToMilitaryForces(action.Weapon, action.WeaponAmount);

                country.GetArmy().RemoveWeaponFromMilitaryForcesByWeaponTemplate(action.Weapon, action.WeaponAmount);
            }
        }

        if (action.ActionType == ACTION_TYPE.Give_Military_Access)
        {
            if (action.SuccessRate >= success)
            {
                CountryManager.Instance.AddMilitaryAccess(country, action.Country, action.MilitaryAccess);
            }
        }

        if (action.ActionType == ACTION_TYPE.Ask_For_Military_Access)
        {
            if (action.SuccessRate >= success)
            {
                CountryManager.Instance.AddMilitaryAccess(action.Country, country, action.MilitaryAccess);
            }
        }

        if (action.ActionType == ACTION_TYPE.Give_Money_Support)
        {
            if (action.SuccessRate >= success)
            {
                int moneyAmount = action.PayMoney;

                int newBudget = (int)country.Budget - moneyAmount;
                country.Budget = newBudget;
                action.Country.Budget = action.Country.Budget + moneyAmount;

                /*
                int relationPoint = 5;

                if (moneyAmount >= 50 && moneyAmount < 250)
                    relationPoint = 1;
                if (moneyAmount >= 250 && moneyAmount < 1000)
                    relationPoint = 5;
                if (moneyAmount >= 1000 && moneyAmount < 5000)
                    relationPoint = 10;
                if (moneyAmount >= 5000 && moneyAmount < 10000)
                    relationPoint = 15;
                if (moneyAmount >= 10000 && moneyAmount < 100000)
                    relationPoint = 20;
                if (moneyAmount >= 100000)
                    relationPoint = 25;
                */
            }
        }

        if (action.ActionType == ACTION_TYPE.Make_A_Military_Coup)
        {
            if (action.SuccessRate >= success)
            {
                WorldMapStrategyKit.NotificationManager.Instance.CreatePublicNotification("Made a military coup in " + action.Country.name);

                float currentTension = action.Country.Tension - 30;
                action.Country.Tension = currentTension;
            }
            else
            {
                action.Country.SetRelations(country, -35);
                country.SetRelations(action.Country, -35);

                WorldMapStrategyKit.NotificationManager.Instance.CreatePublicNotification(action.Country.name + " has prevented a military coup");

                if (action.Country == GameEventHandler.Instance.GetPlayer().GetMyCountry() || country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                {
                    HUDManager.Instance.PrivateNotification(action.Country.name + " has prevented a military coup");
                }
            }
        }

        if (action.ActionType == ACTION_TYPE.Place_Arms_Embargo)
        {
            if (action.SuccessRate >= success)
            {
                country.PlaceArmsEmbargo(action.Country);
            }
        }

        if (action.ActionType == ACTION_TYPE.Place_Trade_Embargo)
        {
            if (action.SuccessRate >= success)
            {
                country.PlaceTradeEmbargo(action.Country);
            }
        }

        if (action.ActionType == ACTION_TYPE.Purchase_Weapon)
        {
            if (action.SuccessRate >= success)
            {
                country.GetArmy().AddWeaponToMilitaryForces(action.Weapon, action.WeaponAmount);

                int current = 0;
                country.GetArmy().GetAllWeaponsInArmyInventory().TryGetValue(action.Weapon, out current);

                Debug.Log(action.Weapon.weaponName + " -----> " + current);

                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                    HUDManager.Instance.PrivateNotification("You have bought " + action.Weapon.weaponName + "x" + action.WeaponAmount + " from " + action.Country.name);
            }
        }

        if (action.ActionType == ACTION_TYPE.Request_Garrison_Support)
        {
            if (action.SuccessRate >= success)
            {
                int index = UnityEngine.Random.Range(0, action.Country.GetArmy().GetLandForces().GetAllDivisionInMilitaryForces().Count);
                GameObjectAnimator division = action.Country.GetArmy().GetLandForces().GetAllDivisionInMilitaryForces()[index];

                country.GetArmy().GetLandForces().AddDivisionToMilitaryForces(division);
                action.Country.GetArmy().GetLandForces().RemoveDivisionInMilitaryForces(division);
            }
            else
            {
                
            }
        }

        if (action.ActionType == ACTION_TYPE.Request_License_Production)
        {
            if (action.SuccessRate >= success)
            {
                country.AddProducibleWeaponToInventory(action.Weapon.weaponID);
                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                    HUDManager.Instance.PrivateNotification(country.name + " got production license of " + action.Weapon.weaponName);
            }
            else
            {
                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                    HUDManager.Instance.PrivateNotification(action.Country.name + " refused production license request for " + country.name);
            }
        }

        if (action.ActionType == ACTION_TYPE.Sign_A_Peace_Treaty)
        {
            if (action.SuccessRate >= success)
            {
                CountryManager.Instance.SignPeaceOfTreaty(country, action.Country);
            }
            else
            {

            }
        }

        if (action.ActionType == ACTION_TYPE.Steal_Technology)
        {
            if (action.SuccessRate >= success)
            {
                country.AddProducibleWeaponToInventory(action.Weapon.weaponID);

                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                {
                    HUDManager.Instance.PrivateNotification("You stole " + action.Weapon.weaponName + " succesfully from " + action.Country.name);
                }
                if (action.Country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                {
                    HUDManager.Instance.PrivateNotification(country.name + " has stolen " + action.Weapon.weaponName + " from you");
                }
            }
            else
            {
                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                {
                    HUDManager.Instance.PrivateNotification("Fail...You could not steal " + action.Weapon.weaponName);
                }
            }
        }

        if (action.SuccessRate >= success)
        {
            country.SetRelations(action.Country, action.EffectInCountryRelationOnSuccess);
        }
        else
        {
            country.SetRelations(action.Country, action.EffectInCountryRelationOnFailure);
        }

        CountryManager.Instance.WarDecision(country, action.Country);
        country.GetActionList().Remove(action);
    }


    public ACTION_TYPE GetActionTypeByActionName(string ActionName)
    {
        if (ActionName == "Change System Of Government")
            return ACTION_TYPE.Change_System_Of_Government;
        if (ActionName == "Assassination Of President")
            return ACTION_TYPE.Assassination_Of_President;
        if (ActionName == "Steal Technology")
            return ACTION_TYPE.Steal_Technology;
        if (ActionName == "Make A Military Coup")
            return ACTION_TYPE.Make_A_Military_Coup;
        if (ActionName == "Sign A Peace Treaty")
            return ACTION_TYPE.Sign_A_Peace_Treaty;
        if (ActionName == "Begin Nuclear War")
            return ACTION_TYPE.Begin_Nuclear_War;
        if (ActionName == "Declare War")
            return ACTION_TYPE.Declare_War;
        if (ActionName == "Purchase Weapon")
            return ACTION_TYPE.Purchase_Weapon;
        if (ActionName == "Ask For Money Support")
            return ACTION_TYPE.Ask_For_Money_Support;
        if (ActionName == "Give Money Support")
            return ACTION_TYPE.Give_Money_Support;
        if (ActionName == "Cancel Military Access")
            return ACTION_TYPE.Cancel_Military_Access;
        if (ActionName == "Give Military Access")
            return ACTION_TYPE.Give_Military_Access;
        if (ActionName == "Ask For Military Access")
            return ACTION_TYPE.Ask_For_Military_Access;
        if (ActionName == "Request Garrison Support")
            return ACTION_TYPE.Request_Garrison_Support;
        if (ActionName == "Give Garrison Support")
            return ACTION_TYPE.Give_Garrison_Support;
        if (ActionName == "Give Gun Support")
            return ACTION_TYPE.Give_Gun_Support;
        if (ActionName == "Ask For Gun Support")
            return ACTION_TYPE.Ask_For_Gun_Support;
        if (ActionName == "Request License Production")
            return ACTION_TYPE.Request_License_Production;
        if (ActionName == "Place Arms Embargo")
            return ACTION_TYPE.Place_Arms_Embargo;
        if (ActionName == "Place Trade Embargo")
            return ACTION_TYPE.Place_Trade_Embargo;
        if (ActionName == "Ask For Control Of Region")
            return ACTION_TYPE.Ask_For_Control_Of_Region;
        if (ActionName == "Give Control Of Region")
            return ACTION_TYPE.Give_Control_Of_Region;
        if (ActionName == "Buy Mineral")
            return ACTION_TYPE.Buy_Mineral;
        if (ActionName == "Sign Trade Treaty")
            return ACTION_TYPE.Sign_Trade_Treaty;

        return ACTION_TYPE.NONE;
    }

    public ACTION_CATEGORY GetActionCategoryTypeByCategoryName(string ActionCategory)
    {
        if (ActionCategory == "TRADE")
            return ACTION_CATEGORY.TRADE;
        if (ActionCategory == "POLITIKS")
            return ACTION_CATEGORY.POLITIKS;
        if (ActionCategory == "SUPPORT")
            return ACTION_CATEGORY.SUPPORT;
        if (ActionCategory == "INTELLIGENCE_AGENCY")
            return ACTION_CATEGORY.INTELLIGENCE_AGENCY;
        if (ActionCategory == "MILITARY")
            return ACTION_CATEGORY.MILITARY;
        if (ActionCategory == "REGION")
            return ACTION_CATEGORY.REGION;

        return ACTION_CATEGORY.NONE;
    }

    public bool IsPossibleAction(Action action, Country country, Country targetCountry)
    {
        if (CountryManager.Instance.GetAtWarCountryList(country).Contains(targetCountry) != action.AtWar)
            return false;
        if (CountryManager.Instance.GetAllAllies(country).Contains(targetCountry) != action.Ally)
            return false;
        if (CountryManager.Instance.GetAllEnemies(country).Contains(targetCountry) != action.Enemy)
            return false;

        return true;
    }

    public void UpdateActionsInProgress(Country country)
    {
        List<Action> actionList = country.GetActionList();

        foreach (Action action in actionList.ToArray())
        {
            if (action.EventFinishTime == 0)
            {
                ApplyAction(country, action, false);
            }

            if (action.EventFinishTime > 0)
            {
                action.EventFinishTime--;

                if (action.EventFinishTime == 0)
                {
                    ApplyAction(country, action, false);
                }
            }
        }
    }

    public void ShowMineralBuy(MINERAL_TYPE mineralType)
    {
        if (GameEventHandler.Instance.IsGameStarted() == false)
            return;

        Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();
        Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();

        if (selectedCountry == myCountry)
            return;

        if (CountryManager.Instance.GetAllEnemies(myCountry).Contains(selectedCountry))
        {
            HUDManager.Instance.PrivateNotification("You cannot buy mineral from your enemy");
            return;
        }
        if (CountryManager.Instance.GetAtWarCountryList(myCountry).Contains(selectedCountry))
        {
            HUDManager.Instance.PrivateNotification("You cannot buy mineral from country which at war");
            return;
        }

        HideAllActionPanels();
        buyMineralPanel.SetActive(true);

        this.mineralType = mineralType;
        string mineral = MineralManager.Instance.GetMineralNameByType(mineralType);
        mineralName.text = mineral;
        mineralImage.texture = ResourceManager.Instance.LoadTexture(RESOURCE_TYPE.MINERAL, mineral);

        if (mineralInputField.text != string.Empty)
            mineralNumber = Int32.Parse(mineralInputField.text);

        if (myCountry.CountryIsContainsInActionList(selectedCountry, ACTION_TYPE.Buy_Mineral))
            buyMineralButton.SetActive(false);
        else
            buyMineralButton.SetActive(true);
    }

    public void CalculateMineralCost()
    {
        if (mineralInputField.text == string.Empty)
            return;

        mineralNumber = Int32.Parse(mineralInputField.text);

        if (mineralNumber < 0)
            return;

        int unitPrice = MineralManager.Instance.GetMineralCost(mineralName.text);

        totalMineralCost = mineralNumber * unitPrice;
        mineralTotalCost.text = "$ " + string.Format("{0:#,0}", float.Parse(totalMineralCost.ToString())) + " M";
    }

    public void AssassinationOfPresidentPanel()
    {
        HideAllActionPanels();
        assassinationOfPresidentPanel.SetActive(true);

        assassinationOfPresidentSuccessRate.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().Intelligence_Agency.Assassination.ToString() + "%";

        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().CountryIsContainsInActionList(GameEventHandler.Instance.GetPlayer().GetSelectedCountry(), ACTION_TYPE.Assassination_Of_President))
            assassinationOfPresidentButton.SetActive(false);
        else
            assassinationOfPresidentButton.SetActive(true);
    }

    public void StealTechnologyPanel()
    {
        HideAllActionPanels();
        stealTech.SetActive(true);

        Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();
        Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

        weaponDropdown.dropdownItems.Clear();

        weaponDropdown.enableIcon = false;
        stealWeaponList.Clear();

        foreach (int ID in selectedCountry.GetProducibleWeapons())
        {
            if (myCountry.GetProducibleWeapons().Contains(ID) == false)
            {
                WeaponTemplate template = WeaponManager.Instance.GetWeaponTemplateByID(ID);
                stealWeaponList.Add(ID);

                weaponDropdown.SetItemTitle(template.weaponName + "   [ " + template.weaponLevel + " ]");

                //Texture2D weaponTexture = WeaponManager.Instance.GetWeaponTemplateIconByID(ID);
                //Sprite icon = Sprite.Create(weaponTexture, new Rect(0, 0, weaponTexture.width, weaponTexture.height), Vector2.zero);
                //weaponDropdown.SetItemIcon(icon);
                weaponDropdown.CreateNewItem();
            }
        }

        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().CountryIsContainsInActionList(GameEventHandler.Instance.GetPlayer().GetSelectedCountry(), ACTION_TYPE.Steal_Technology))
            stealWeaponTechButton.SetActive(false);
        else
            stealWeaponTechButton.SetActive(true);
    }

    void SelectWeaponToSteal(int index)
    {
        int ID = stealWeaponList[index];

        stealWeapon = WeaponManager.Instance.GetWeaponTemplateByID(ID);
         
        selectedWeaponToSteal.text = weaponDropdown.dropdownItems[index].itemName;
        weaponImage.texture = WeaponManager.Instance.GetWeaponTemplateIconByID(ID);
        stealWeaponSuccessRate.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().Intelligence_Agency.ReverseEnginering.ToString() + "%";
    }

    public void MakeMilitaryCoupPanel()
    {
        HideAllActionPanels();
        militaryCoupPanel.SetActive(true);

        militaryCoupSuccessRate.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().Intelligence_Agency.MilitaryCoup.ToString() + "%";

        if(GameEventHandler.Instance.GetPlayer().GetMyCountry().CountryIsContainsInActionList(GameEventHandler.Instance.GetPlayer().GetSelectedCountry(), ACTION_TYPE.Make_A_Military_Coup))
            militaryCoupButton.SetActive(false);
        else
            militaryCoupButton.SetActive(true);
    }

    public void AskForControlOfProvincePanel()
    {
        HideAllActionPanels();
        askForControlOfRegionPanel.SetActive(true);

        Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();
        Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

        askForControlOfRegionDropdown.dropdownItems.Clear();
        askForControlOfRegionDropdown.selectedItemIndex = 0;

        foreach (Province tempProvince in map.GetProvinces(selectedCountry))
        {
            askForControlOfRegionDropdown.SetItemIcon(null);
            askForControlOfRegionDropdown.SetItemTitle(tempProvince.name);
            askForControlOfRegionDropdown.CreateNewItem();
        }

        askForControlOfRegionDropdown.selectedItemIndex = 0;
        askForControlOfRegionDropdown.SetupDropdown();

        askForControlOfRegionSuccessRate.text = GetActionByActionType(ACTION_TYPE.Ask_For_Control_Of_Region).SuccessRate + "%";

        if (myCountry.CountryIsContainsInActionList(selectedCountry, ACTION_TYPE.Ask_For_Control_Of_Region))
            askForControlOfRegionButton.SetActive(false);
        else
            askForControlOfRegionButton.SetActive(true);
    }

    void AskForControlOfProvinceSelectProvince(int index)
    {
        string provinceName = askForControlOfRegionDropdown.dropdownItems[index].itemName;

        if (provinceName != string.Empty)
        {
            askForControlOfRegionProvince = map.GetProvince(provinceName, GameEventHandler.Instance.GetPlayer().GetSelectedCountry().name);

            askForControlOfRegionSelectedProvinceName.text = askForControlOfRegionProvince.name;
        }
    }


    public void GiveControlOfStatePanel()
    {
        HideAllActionPanels();
        giveControlOfRegionPanel.SetActive(true);

        Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();
        Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

        giveControlOfRegionDropdown.dropdownItems.Clear();
        giveControlOfRegionDropdown.selectedItemIndex = 0;

        foreach (Province tempProvince in map.GetProvinces(selectedCountry))
        {
            giveControlOfRegionDropdown.SetItemIcon(null);
            giveControlOfRegionDropdown.SetItemTitle(tempProvince.name);
            giveControlOfRegionDropdown.CreateNewItem();
        }

        giveControlOfRegionDropdown.selectedItemIndex = 0;
        giveControlOfRegionDropdown.SetupDropdown();

        giveControlOfRegionSuccessRate.text = GetActionByActionType(ACTION_TYPE.Give_Control_Of_Region).SuccessRate + "%";

        if (myCountry.CountryIsContainsInActionList(selectedCountry, ACTION_TYPE.Give_Control_Of_Region))
            giveControlOfRegionButton.SetActive(false);
        else
            giveControlOfRegionButton.SetActive(true);
    }

    void GiveControlOfRegionSelectProvince(int index)
    {
        string provinceName = giveControlOfRegionDropdown.dropdownItems[index].itemName;

        if (provinceName != string.Empty)
        {
            giveControlOfRegionProvince = map.GetProvince(provinceName, GameEventHandler.Instance.GetPlayer().GetSelectedCountry().name);

            giveControlOfRegionSelectedProvinceName.text = giveControlOfRegionProvince.name;
        }
    }

    public void PlaceTradeEmbargoPanel()
    {
        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().CountryIsContainsInActionList(GameEventHandler.Instance.GetPlayer().GetSelectedCountry(), ACTION_TYPE.Place_Trade_Embargo))
            placeTradeEmbargoButton.SetActive(false);
        else
            placeTradeEmbargoButton.SetActive(true);
    }
    public void PlaceArmsEmbargoPanel()
    {
        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().CountryIsContainsInActionList(GameEventHandler.Instance.GetPlayer().GetSelectedCountry(), ACTION_TYPE.Place_Arms_Embargo))
            placeArmsEmbargoButton.SetActive(false);
        else
            placeArmsEmbargoButton.SetActive(true);
    }

    public void RequestLicenseProductionPanel()
    {
        HideAllActionPanels();
        requestLicenseProductionPanel.SetActive(true);

        Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();
        Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

        licenseDropdown.dropdownItems.Clear();

        licenseDropdown.enableIcon = false;
        requestLicenseProductionList.Clear();

        foreach (int ID in selectedCountry.GetProducibleWeapons())
        {
            if (myCountry.GetProducibleWeapons().Contains(ID) == false)
            {
                WeaponTemplate template = WeaponManager.Instance.GetWeaponTemplateByID(ID);
                requestLicenseProductionList.Add(ID);

                licenseDropdown.SetItemTitle(template.weaponName + "   [ " + template.weaponLevel + " ]");

                //Texture2D weaponTexture = WeaponManager.Instance.GetWeaponTemplateIconByID(ID);
                //Sprite icon = Sprite.Create(weaponTexture, new Rect(0, 0, weaponTexture.width, weaponTexture.height), Vector2.zero);
                //weaponDropdown.SetItemIcon(icon);
                licenseDropdown.CreateNewItem();
            }
        }

        if (myCountry.CountryIsContainsInActionList(selectedCountry, ACTION_TYPE.Request_License_Production))
            requestLicenseProductionButton.SetActive(false);
        else
            requestLicenseProductionButton.SetActive(true);      
    }
    void RequestLicenseProduction(int index)
    {
        int ID = requestLicenseProductionList[index];

        requestLicenseWeapon = WeaponManager.Instance.GetWeaponTemplateByID(ID);

        selectedWeaponLicense.text = licenseDropdown.dropdownItems[index].itemName;
        requestLicenseWeaponImage.texture = WeaponManager.Instance.GetWeaponTemplateIconByID(ID);
        requestLicenseProductionSuccessRate.text = GetActionByActionType(ACTION_TYPE.Request_License_Production).SuccessRate + "%";
    }

    public void RequestGarrisonSupportPanel()
    {
        requestGarrisonSupportPanel.SetActive(true);
    }
    public void GiveGarrisonSupportPanel()
    {
        giveGarrisonSupportPanel.SetActive(true);
    }
    public void GiveGunSupportPanel()
    {
        giveGunSupportPanel.SetActive(true);
    }
    public void AskForGunSupportPanel()
    {
        askForGunSupportPanel.SetActive(true);
    }

    public void GiveMoneySupportPanel()
    {
        HideAllActionPanels();
        moneySupportPanel.SetActive(true);
    }
    public void GiveMoneySupport()
    {
        if (moneyAmountInputField.text != string.Empty)
            moneyAmountText.text = moneyAmountInputField.text;

        giveMoneyAmount = Int32.Parse(moneyAmountText.text);
    }
    public void AskForMoneySupportPanel()
    {
        askForMoneySupportPanel.SetActive(true);
    }

    public void CancelMilitaryAccessPanel()
    {
        cancelMilitaryAccessPanel.SetActive(true);
    }
    public void AskForMilitaryAccessPanel()
    {
        askForMilitaryAccessPanel.SetActive(true);
    }
    public void GiveMilitaryAccessPanel()
    {
        giveMilitaryAccessPanel.SetActive(true);
    }
    public void NuclearWarPanel()
    {
        beginNuclearWarPanel.SetActive(true);
    }

    public void SignPeaceTreatyPanel()
    {
        signPeaceTreatyPanel.SetActive(true);
    }

    public void SignTradeTreatyPanel()
    {
        signPeaceTreatyPanel.SetActive(true);
    }

    public void DeclareWarPanel()
    {
        Country selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();
        Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

        HideAllActionPanels();
        declareWarPanel.SetActive(true);

        leftCountryText.text = myCountry.name + "'s Allies";
        rightCountryText.text = selectedCountry.name + "'s Allies";

        foreach (Transform child in declareWarLeftContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in declareWarRightContent.transform)
        {
            Destroy(child.gameObject);
        }

        
        int attackLandPower = myCountry.GetArmy().GetLandForces().GetMilitaryPower();
        int attackAirPower = myCountry.GetArmy().GetAirForces().GetMilitaryPower();
        int attackNavalPower = myCountry.GetArmy().GetNavalForces().GetMilitaryPower();

        int enemyLandPower = selectedCountry.GetArmy().GetLandForces().GetMilitaryPower();
        int enemyAirPower = selectedCountry.GetArmy().GetAirForces().GetMilitaryPower();
        int enemyNavalPower = selectedCountry.GetArmy().GetNavalForces().GetMilitaryPower();

        float landVictoryChance =  (100 * attackLandPower) / (attackLandPower + enemyLandPower);
        float airVictoryChance = (100 * attackAirPower) / (attackAirPower + enemyAirPower);
        float navalVictoryChance = (100 * attackNavalPower) / (attackLandPower + enemyNavalPower);
    
        foreach (Country country in CountryManager.Instance.GetAllAllies(myCountry))
        {
            GameObject temp = Instantiate(declareWarItem, declareWarLeftContent.transform);

            temp.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
            temp.GetComponent<SimpleTooltip>().infoLeft = country.name;
        }

        foreach (Country country in CountryManager.Instance.GetAllAllies(selectedCountry))
        {
            GameObject temp = Instantiate(declareWarItem, declareWarRightContent.transform);

            temp.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
            temp.GetComponent<SimpleTooltip>().infoLeft = country.name;
        }
    }

    public bool ShouldOpenYesOrNoPanel(ACTION_TYPE actionType)
    {
        if (actionType == ACTION_TYPE.Ask_For_Control_Of_Region ||
            actionType == ACTION_TYPE.Ask_For_Gun_Support ||
            actionType == ACTION_TYPE.Ask_For_Military_Access ||
            actionType == ACTION_TYPE.Ask_For_Money_Support ||
            actionType == ACTION_TYPE.Buy_Mineral ||
            actionType == ACTION_TYPE.Give_Control_Of_Region ||
            actionType == ACTION_TYPE.Give_Garrison_Support ||
            actionType == ACTION_TYPE.Give_Gun_Support ||
            actionType == ACTION_TYPE.Give_Military_Access ||
            actionType == ACTION_TYPE.Give_Money_Support)

            return true;


        return false;
    }

    public string GetActionDescription(Country country, Action action)
    {
        string description = string.Empty;

        if(action.ActionType == ACTION_TYPE.Ask_For_Control_Of_Region)
        {
            description = country.name + " asks for control of " + action.Province.name;
        }

        if (action.ActionType == ACTION_TYPE.Ask_For_Gun_Support)
        {
            description = country.name + " asks for gun support : " + action.Weapon + "x" + action.WeaponAmount;
        }

        if (action.ActionType == ACTION_TYPE.Ask_For_Military_Access)
        {
            description = country.name + " asks for military access : " + action.MilitaryAccess + " days";
        }

        if (action.ActionType == ACTION_TYPE.Ask_For_Money_Support)
        {
            description = country.name + " asks for money support";
        }

        if (action.ActionType == ACTION_TYPE.Buy_Mineral)
        {
            description = country.name + " wants to buy " + action.MineralType.ToString() + "x" + action.MineralAmount;
        }

        if (action.ActionType == ACTION_TYPE.Give_Control_Of_Region)
        {
            description = country.name + " wants to give the control of " + action.Province.name;
        }

        if (action.ActionType == ACTION_TYPE.Give_Garrison_Support)
        {
            description = country.name + " wants to give garrison support";
        }

        if (action.ActionType == ACTION_TYPE.Give_Gun_Support)
        {
            description = country.name + " wants to give gun support";
        }

        if (action.ActionType == ACTION_TYPE.Give_Military_Access)
        {
            description = country.name + " give military access : " + action.MilitaryAccess + "days";
        }

        if (action.ActionType == ACTION_TYPE.Give_Money_Support)
        {
            description = country.name + " wants to give money support : " + action.PayMoney + "M";
        }

        return description;
    }
}