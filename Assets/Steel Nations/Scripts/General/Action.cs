
public class Action
{
    WorldMapStrategyKit.Country country;
    ACTION_TYPE actionType;
    ACTION_CATEGORY actionCategory;
    string actionName;

    Division division;
    int militaryAccess;

    MINERAL_TYPE mineralType;
    int amountOfMineral;

    WeaponTemplate weaponTemplate;
    int amountOfWeapon;

    WorldMapStrategyKit.Province province;

    int payMoney;
    int getMoney;

    bool atWar;
    bool enemy;
    bool ally;

    int effectInCountryRelationOnSuccess;
    int effectInCountryRelationOnFailure;

    int successRate;
    int leftEvaluationTime;

    public Action()
    {
        country = null;

        actionType = ACTION_TYPE.NONE;
        mineralType = MINERAL_TYPE.NONE;

        amountOfMineral = 0;
        amountOfWeapon = 0;

        payMoney = 0;
        getMoney = 0;

        weaponTemplate = null;
        division = null;
        militaryAccess = 0;

        successRate = 0;

        effectInCountryRelationOnSuccess = 0;
        effectInCountryRelationOnFailure = 0;

        leftEvaluationTime = 0;

        atWar = false;
        enemy = false;
        ally = false;
    }

    public int MilitaryAccess
    {
        get { return militaryAccess; }
        set { militaryAccess = value; }
    }

    public int EarnMoney
    {
        get { return getMoney; }
        set { getMoney = value; }
    }

    public bool AtWar
    {
        get { return atWar; }
        set { atWar = value; }
    }

    public bool Enemy
    {
        get { return enemy; }
        set { enemy = value; }
    }

    public bool Ally
    {
        get { return ally; }
        set { ally = value; }
    }

    public string ActionName
    {
        get { return actionName; }
        set { actionName = value; }
    }

    public ACTION_CATEGORY ActionCategory
    {
        get { return actionCategory; }
        set { actionCategory = value; }
    }

    public int EffectInCountryRelationOnSuccess
    {
        get { return effectInCountryRelationOnSuccess; }
        set { effectInCountryRelationOnSuccess = value; }
    }

    public int EffectInCountryRelationOnFailure
    {
        get { return effectInCountryRelationOnFailure; }
        set { effectInCountryRelationOnFailure = value; }
    }

    public Division Division
    {
        get { return division; }
        set { division = value; }
    }

    public int SuccessRate
    {
        get { return successRate; }
        set { successRate = value; }
    }

    public int MineralAmount
    {
        get { return amountOfMineral; }
        set { amountOfMineral = value; }
    }

    public MINERAL_TYPE MineralType
    {
        get { return mineralType; }
        set { mineralType = value; }
    }

    public WeaponTemplate Weapon
    {
        get { return weaponTemplate; }
        set { weaponTemplate = value; }
    }

    public WorldMapStrategyKit.Country Country
    {
        get { return country; }
        set { country = value; }
    }

    public WorldMapStrategyKit.Province Province
    {
        get { return province; }
        set { province = value; }
    }

    public void SetMineral(MINERAL_TYPE mineralType, int amountOfMineral)
    {
        this.mineralType = mineralType;
        this.amountOfMineral = amountOfMineral;
    }

    public int PayMoney
    {
        get { return payMoney; }
        set { payMoney = value; }
    }


    public int WeaponAmount
    {
        get { return amountOfWeapon; }
        set { amountOfWeapon = value; }
    }

    public ACTION_TYPE ActionType
    {
        get { return actionType; }
        set { actionType = value; }
    }

    public int EventFinishTime
    {
        get { return leftEvaluationTime; }
        set { leftEvaluationTime = value; }
    }

    public void SetAction(ACTION_TYPE actionType,
        string actionName,
        WorldMapStrategyKit.Country country, 
        int successRate, 
        int effectOnCountryTensionOnSuccess, 
        int effectOnCountryTensionOnFailure, 
        int atWar, 
        int enemy, 
        int neutral,
        int ally,
        int leftEvaluationTime,
        ACTION_CATEGORY actionCategory)
    {
        this.actionName = actionName;
        this.actionCategory = actionCategory;
        this.country = country;
        this.actionType = actionType;
        this.successRate = successRate;

        this.effectInCountryRelationOnSuccess = effectOnCountryTensionOnSuccess;
        this.effectInCountryRelationOnFailure = effectOnCountryTensionOnFailure;

        this.atWar = false;
        this.enemy = false;
        this.ally = false;


        if (atWar == 1)
            this.atWar = true;

        if (enemy == 1)
            this.enemy = true;

        if (ally == 1)
            this.ally = true;

        this.leftEvaluationTime = leftEvaluationTime;
    }
}
