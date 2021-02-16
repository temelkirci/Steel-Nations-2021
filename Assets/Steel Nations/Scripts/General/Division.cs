using System.Collections.Generic;

public class Division
{
    public DivisionTemplate divisionTemplate;

    public string divisionName;

    public int currentSoldier;

    public int mainUnitCurrent;
    public int secondUnitCurrent;
    public int thirdUnitCurrent;

    List<Weapon> weaponsInDivision = new List<Weapon>();

    public void AddWeaponToDivision(Weapon weapon)
    {
        weaponsInDivision.Add(weapon);
    }

    public List<Weapon> GetWeaponNumberByWeaponIDInDivision(int weaponID)
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
    
    public float GetDivisionSpeed()
    {
        float speed = 0;

        foreach(Weapon weapon in weaponsInDivision)
        {
            speed += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponSpeed;
        }

        return speed;
    }

    public void AttackToEnemy(Division enemy)
    {

    }
}
