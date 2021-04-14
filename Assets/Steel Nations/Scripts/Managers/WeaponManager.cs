using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    List<WeaponTemplate> weaponTemplateList = new List<WeaponTemplate>();

    public void CreateWeaponTemplate(int weapon_id, 
        string weapon_name,
        int weapon_speed,
        int weapon_attack,
        int weapon_defense, 
        int weapon_attack_range,
        int weapon_cost,
        int weapon_level,
        int weapon_terrain_capability,
        int weapon_duration,
        int weapon_research_year,
        int weapon_research_time,
        int weapon_research_cost,
        string weapon_description,
        string weapon_image_directory,
        int weapon_required_oil,
        int weapon_required_uranium,
        int weapon_required_iron,
        int weapon_required_steel,
        int weapon_required_aluminium)
    {
        WeaponTemplate tempWeapon = new WeaponTemplate();

        tempWeapon.weaponID = weapon_id;
        tempWeapon.weaponSpeed = weapon_speed;
        tempWeapon.weaponAttack = weapon_attack;
        tempWeapon.weaponDefense = weapon_defense;
        tempWeapon.weaponAttackRange = weapon_attack_range;
        tempWeapon.weaponCost = weapon_cost;
        tempWeapon.weaponLevel = weapon_level;
        tempWeapon.weaponTerrainType = weapon_terrain_capability;
        tempWeapon.weaponProductionTime = weapon_duration;
        tempWeapon.weaponName = weapon_name;
        tempWeapon.weaponResearchYear = weapon_research_year;
        tempWeapon.weaponResearchTime = weapon_research_time;
        tempWeapon.weaponResearchCost = weapon_research_cost;
        tempWeapon.weaponDescription = weapon_description;
        tempWeapon.weaponImageDirectory = weapon_image_directory;

        tempWeapon.requiredOil = weapon_required_oil;
        tempWeapon.requiredUranium = weapon_required_uranium;

        tempWeapon.requiredIron = weapon_required_iron;
        tempWeapon.requiredSteel = weapon_required_steel;
        tempWeapon.requiredAluminium = weapon_required_aluminium;

        tempWeapon.weaponType = GetWeaponType(weapon_name);

        AddWeaponTemplate(tempWeapon);
    }

    public WEAPON_TYPE GetWeaponType(string weaponName)
    {
        if (weaponName == "Rifle")
        {
            return WEAPON_TYPE.RIFLE;
        }
        else if (weaponName == "Tank")
        {
            return WEAPON_TYPE.TANK;
        }
        else if (weaponName == "Towed Artillery")
        {
            return WEAPON_TYPE.TOWED_ARTILLERY;
        }
        else if (weaponName == "Self-Propelled Artillery")
        {
            return WEAPON_TYPE.SELF_PROPELLED_ARTILLERY;
        }
        else if (weaponName == "Armored Vehicle")
        {
            return WEAPON_TYPE.ARMORED_VEHICLE;
        }
        else if (weaponName == "Rocket Projector")
        {
            return WEAPON_TYPE.ROCKET_PROJECTOR;
        }
        else if (weaponName == "Laser Gun")
        {
            return WEAPON_TYPE.LASER_GUN;
        }


        else if (weaponName == "Fighter")
        {
            return WEAPON_TYPE.FIGHTER;
        }
        else if (weaponName == "Helicopter")
        {
            return WEAPON_TYPE.HELICOPTER;
        }
        else if (weaponName == "Drone")
        {
            return WEAPON_TYPE.DRONE;
        }

        else if (weaponName == "Coastal Patrol")
        {
            return WEAPON_TYPE.COASTAL_PATROL;
        }
        else if (weaponName == "Corvette")
        {
            return WEAPON_TYPE.CORVETTE;
        }
        else if (weaponName == "Frigate")
        {
            return WEAPON_TYPE.FRIGATE;
        }
        else if (weaponName == "Destroyer")
        {
            return WEAPON_TYPE.DESTROYER;
        }
        else if (weaponName == "Submarine")
        {
            return WEAPON_TYPE.SUBMARINE;
        }
        else if (weaponName == "Aircraft Carrier")
        {
            return WEAPON_TYPE.AIRCRAFT_CARRIER;
        }

        else if (weaponName == "Missile")
        {
            return WEAPON_TYPE.ICBM;
        }
        else if (weaponName == "Neutron Bomb")
        {
            return WEAPON_TYPE.NEUTRON_BOMB;
        }
        else if (weaponName == "Anti Matter Bomb")
        {
            return WEAPON_TYPE.ANTI_MATTER_BOMB;
        }

        return WEAPON_TYPE.NONE;
    }

    public void AddWeaponTemplate(WeaponTemplate weapon)
    {
        weaponTemplateList.Add(weapon);
    }

    public List<WeaponTemplate> GetWeaponTemplateList()
    {
        return weaponTemplateList;
    }

    public WeaponTemplate GetWeaponByWeaponTypeAndTech(WEAPON_TYPE weaponType, int weaponTech)
    {
        foreach (WeaponTemplate weaponTemplate in weaponTemplateList)
            if (weaponTemplate.weaponType == weaponType && weaponTemplate.weaponLevel == weaponTech)
                return weaponTemplate;

        return null;
    }

    public List<int> GetWeaponIDListByWeaponType(WEAPON_TYPE weaponType)
    {
        List<int> weaponIDList = new List<int>();

        foreach (WeaponTemplate weaponTemplate in weaponTemplateList)
            if (weaponTemplate.weaponType == weaponType)
                weaponIDList.Add(weaponTemplate.weaponID);

        return weaponIDList;
    }

    public Texture2D GetWeaponTemplateIconByID(int weaponID)
    {
        foreach(WeaponTemplate weaponTemplate in weaponTemplateList)
        {
            if(weaponTemplate.weaponID == weaponID)
                return Resources.Load("weapons/" + weaponTemplate.weaponName + "/" + weaponTemplate.weaponImageDirectory) as Texture2D;
        }
        return null;
    }

    public int GetLowestGenerationByWeaponType(WEAPON_TYPE weaponType)
    {
        int lowest = 10;

        foreach (WeaponTemplate weaponTemplate in weaponTemplateList)
        {
            if (weaponTemplate.weaponType == weaponType)
                if (weaponTemplate.weaponLevel < lowest)
                    lowest = weaponTemplate.weaponLevel;
        }
        return lowest;
    }

    public int GetLowestGenerationWeaponIDByWeaponType(WEAPON_TYPE weaponType)
    {
        int lowest = 10;
        int weaponID = 0;

        foreach (WeaponTemplate weaponTemplate in weaponTemplateList)
        {
            if (weaponTemplate.weaponType == weaponType)
            {
                if (weaponTemplate.weaponLevel < lowest)
                {
                    lowest = weaponTemplate.weaponLevel;
                    weaponID = weaponTemplate.weaponID;
                }
            }
        }
        return weaponID;
    }

    public int GetHighestGenerationByWeaponType(WEAPON_TYPE weaponType)
    {
        int highest = 1;

        foreach (WeaponTemplate weaponTemplate in weaponTemplateList)
        {
            if (weaponTemplate.weaponType == weaponType)
                if (weaponTemplate.weaponLevel > highest)
                    highest = weaponTemplate.weaponLevel;
        }
        return highest;
    }

    public List<WeaponTemplate> GetWeaponTemplateListByName(string weaponName)
    {
        List<WeaponTemplate> tempWeaponList = new List<WeaponTemplate>();

        foreach (WeaponTemplate weapon in weaponTemplateList)
        {
            if (weapon.weaponName == weaponName)
                tempWeaponList.Add(weapon);
        }

        return tempWeaponList;
    }
    public WeaponTemplate GetWeaponTemplateByID(int weaponID)
    {
        foreach (WeaponTemplate weapon in weaponTemplateList)
            if (weapon.weaponID == weaponID)
                return weapon;
        return null;
    }
}
