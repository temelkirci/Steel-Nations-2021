

public class DivisionTemplate
{
    public DIVISION_TYPE divisionType;

    public int minimumSoldier;
    public int maximumSoldier;

    public WEAPON_TYPE mainWeaponType;
    public int mainUnitMinimum;
    public int mainUnitMaximum;

    public WEAPON_TYPE secondWeaponType;
    public int secondUnitMinimum;
    public int secondUnitMaximum;

    public WEAPON_TYPE thirdWeaponType;
    public int thirdUnitMinimum;
    public int thirdUnitMaximum;


    public void SetDivisionMainWeaponByWeaponName(WEAPON_TYPE weaponType, int minUnit, int maxUnit)
    {
        mainWeaponType = weaponType;
        mainUnitMaximum = maxUnit;
        mainUnitMinimum = minUnit;
    }
    public void SetDivisionSecondWeaponByWeaponName(WEAPON_TYPE weaponType, int minUnit, int maxUnit)
    {
        secondWeaponType = weaponType;
        secondUnitMaximum = maxUnit;
        secondUnitMinimum = minUnit;
    }
    public void SetDivisionThirdWeaponByWeaponName(WEAPON_TYPE weaponType, int minUnit, int maxUnit)
    {
        thirdWeaponType = weaponType;
        thirdUnitMaximum = maxUnit;
        thirdUnitMinimum = minUnit;
    }

    
}
