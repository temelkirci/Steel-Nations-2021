using System.Collections.Generic;
using UnityEngine;

public class MilitaryForces
{
    List<WorldMapStrategyKit.GameObjectAnimator> allDivisionInMilitaryForces = new List<WorldMapStrategyKit.GameObjectAnimator>();
    List<Weapon> allWeaponsInMilitaryForces = new List<Weapon>();

    // add general
    MILITARY_FORCES_TYPE militaryForcesType;
    Person supremeCommander;

    public MilitaryForces()
    {
        allDivisionInMilitaryForces.Clear();
    }

    public Person SupremeCommander
    {
        get { return supremeCommander; }
        set { supremeCommander = value; }
    }

    public int GetMilitaryPower()
    {
        int power = 0;
        foreach (Weapon weapon in allWeaponsInMilitaryForces)
        {
            if (weapon != null)
                power = power + WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttack;
        }

        foreach (WorldMapStrategyKit.GameObjectAnimator GOA in allDivisionInMilitaryForces)
        {
            List<Weapon> weaponList = GOA.GetDivision().GetWeaponsInDivision();
            if (weaponList.Count > 0)
            {
                foreach (Weapon weapon in weaponList)
                    power = power + WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttack;
            }
        }

        return power;
    }

    public void SetMilitaryForcesType(MILITARY_FORCES_TYPE tempMilitaryForcesType)
    {
        militaryForcesType = tempMilitaryForcesType;
    }

    public void AddWeaponToMilitaryForces(Weapon weapon)
    {
        allWeaponsInMilitaryForces.Add(weapon);
    }
    public void RemoveWeaponFromMilitaryForces(Weapon weapon)
    {
        allWeaponsInMilitaryForces.Remove(weapon);
    }
    public void RemoveAllWeaponsFromMilitaryForces()
    {
        allWeaponsInMilitaryForces.Clear();
    }
    public void RemoveWeaponFromMilitaryForcesByWeaponID(int weaponID)
    {
        foreach(Weapon weapon in allWeaponsInMilitaryForces.ToArray())
            if(weapon.weaponTemplateID == weaponID)
                allWeaponsInMilitaryForces.Remove(weapon);
    }
    public void RemoveDivisionInMilitaryForces(WorldMapStrategyKit.GameObjectAnimator division)
    {
        allDivisionInMilitaryForces.Remove(division);
    }

    public List<Weapon> GetAllWeaponsInMilitaryForces()
    {
        return allWeaponsInMilitaryForces;
    }

    public int GetWeaponNumberInMilitaryForcesInventory(int weaponID)
    {
        int number = 0;
        foreach (Weapon weapon in allWeaponsInMilitaryForces)
            if (weapon.weaponTemplateID == weaponID)
                number++;

        return number;
    }

    public List<Weapon> GetWeaponListInMilitaryForcesInventory(int weaponID)
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        foreach (Weapon weapon in allWeaponsInMilitaryForces)
            if (weapon.weaponTemplateID == weaponID)
                tempWeaponList.Add(weapon);

        return tempWeaponList;
    }

    public int PossibleDivisionNumber(DivisionTemplate divisionTemplate)
    {
        int requiredWeaponNumber = divisionTemplate.mainUnitMaximum;
        int currentWeaponNumber = 0;

        foreach (int weaponID in divisionTemplate.mainUnitIDList)
            currentWeaponNumber = currentWeaponNumber + GetWeaponNumberInMilitaryForcesInventory(weaponID);

        return currentWeaponNumber / requiredWeaponNumber;
    }

    public void AddDivisionToMilitaryForces(WorldMapStrategyKit.GameObjectAnimator tempDivision)
    {
        Debug.Log("Created New Division");
        allDivisionInMilitaryForces.Add(tempDivision);
    }

    public List<WorldMapStrategyKit.GameObjectAnimator> GetAllDivisionInMilitaryForces()
    {
        foreach (WorldMapStrategyKit.GameObjectAnimator GOA in allDivisionInMilitaryForces.ToArray())
            if (GOA == null)
                allDivisionInMilitaryForces.Remove(GOA);

        return allDivisionInMilitaryForces;
    }
}