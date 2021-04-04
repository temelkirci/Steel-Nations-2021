using System.Collections.Generic;
using UnityEngine;

public class Division
{
    public DivisionTemplate divisionTemplate;

    public string divisionName;
    public int currentSoldier;
    public bool onBattle = false;

    List<Weapon> weaponsInDivision = new List<Weapon>();

    int mainWeaponID;
    int secondWeaponID;
    int thirdWeaponID;

    int mainWeaponNumber;
    int secondWeaponNumber;
    int thirdWeaponNumber;

    public int MainWeaponNumber
    {
        get { return mainWeaponNumber; }
        set { mainWeaponNumber = value; }
    }

    public int SecondWeaponNumber
    {
        get { return secondWeaponNumber; }
        set { secondWeaponNumber = value; }
    }

    public int ThirdWeaponNumber
    {
        get { return thirdWeaponNumber; }
        set { thirdWeaponNumber = value; }
    }

    public int MainWeapon
    {
        get { return mainWeaponID; }
        set { mainWeaponID = value; }
    }

    public int SecondWeapon
    {
        get { return secondWeaponID; }
        set { secondWeaponID = value; }
    }

    public int ThirdWeapon
    {
        get { return thirdWeaponID; }
        set { thirdWeaponID = value; }
    }

    public Texture2D GetDivisionIcon()
    {
        if(divisionTemplate.mainUnitIDList.Count > 0)
            return WeaponManager.Instance.GetWeaponTemplateIconByID(divisionTemplate.mainUnitIDList[0]);
        if (divisionTemplate.secondUnitList.Count > 0)
            return WeaponManager.Instance.GetWeaponTemplateIconByID(divisionTemplate.secondUnitList[0]);
        if (divisionTemplate.thirdUnitList.Count > 0)
            return WeaponManager.Instance.GetWeaponTemplateIconByID(divisionTemplate.thirdUnitList[0]);

        return null;
    }

    public void AddWeaponToDivision(Weapon weapon)
    {
        weaponsInDivision.Add(weapon);
    }

    public int GetDivisionLeftDefense()
    {
        int power = 0;
        foreach (Weapon weapon in GetWeaponsInDivision())
        {
            power += weapon.weaponLeftHealth;
        }

        return power;
    }
    public int GetDivisionDefense()
    {
        int power = 0;
        foreach (Weapon weapon in GetWeaponsInDivision())
        {
            power += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponDefense;
        }

        return power;
    }

    public int GetDivisionMinimumAttackRange()
    {
        int power = 100000;
        foreach (Weapon weapon in GetWeaponsInDivision())
        {
            if (WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttackRange < power)
                power = WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttackRange;
        }

        return power;
    }
    public int GetDivisionMaximumAttackRange()
    {
        int power = 0;
        foreach (Weapon weapon in GetWeaponsInDivision())
        {
            if (WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttackRange > power)
                power = WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttackRange;
        }

        return power;
    }

    public int GetDivisionPower()
    {
        int power = 0;
        foreach(Weapon weapon in GetWeaponsInDivision())
        {
            power += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttack;
        }

        return power;
    }
    public List<Weapon> GetWeaponListByWeaponIDInDivision(int weaponID)
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        foreach (Weapon weapon in GetWeaponsInDivision())
        {
            if (weapon.weaponTemplateID == weaponID)
            {
                tempWeaponList.Add(weapon);
            }
        }

        return tempWeaponList;
    }
   
    
    public void DeleteWeaponInDivision(Weapon weapon)
    {
        weaponsInDivision.Remove(weapon);
    }

    public List<Weapon> GetWeaponsInDivision()
    {
        return weaponsInDivision;
    }

    public Weapon GetRandomWeaponsInDivision()
    {
        return weaponsInDivision[Random.Range(0, weaponsInDivision.Count)];
    }

    public float GetDivisionSpeed()
    {
        float speed = 0;
        int weaponNumber = 0;
        foreach(Weapon weapon in weaponsInDivision)
        {
            weaponNumber++;
            speed += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponSpeed;
        }

        if (weaponNumber == 0)
            return 0;

        speed = (speed / weaponNumber);
        return speed;
    }
}
