
public class WeaponTemplate
{
    public int weaponID;
    public int weaponSpeed;
    public int weaponAttack;
    public int weaponDefense;
    public int weaponAttackRange;
    public int weaponCost;
    public int weaponLevel;
    public int weaponTerrainType;
    public int weaponProductionTime;
    public string weaponName;
    public int weaponResearchYear;
    public int weaponResearchCost;
    public int weaponResearchTime;
    public string weaponDescription;
    public string weaponImageDirectory;

    public int requiredOil;
    public int requiredUranium;

    public int requiredSteel;
    public int requiredIron;
    public int requiredAluminium;

    public int landHitChance;
    public int airHitChance;
    public int navalHitChance;

    public WEAPON_TYPE weaponType;

    public WeaponTemplate()
    {
        weaponID = -1;
        weaponSpeed = -1;
        weaponAttack = -1;
        weaponDefense = -1;
        weaponAttackRange = -1;
        weaponCost = -1;
        weaponLevel = -1;
        weaponTerrainType = -1;
        weaponProductionTime = -1;
        weaponName = "";
        weaponResearchYear = 0;
        weaponResearchCost = 0;
        weaponResearchTime = 0;

        requiredOil = 0;
        requiredUranium = 0;

        requiredSteel = 0;
        requiredIron = 0;
        requiredAluminium = 0;

        landHitChance = 0;
        airHitChance = 0;
        navalHitChance = 0;

        weaponDescription = string.Empty;
        weaponImageDirectory = string.Empty;
    }

}
