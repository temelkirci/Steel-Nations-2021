using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class MilitaryForces
    {
        List<GameObjectAnimator> allDivisionInMilitaryForces = new List<GameObjectAnimator>();
        List<Weapon> allWeaponsInMilitaryForces = new List<Weapon>();

        // add general
        MILITARY_FORCES_TYPE militaryForcesType;

        public MILITARY_FORCES_TYPE GetMilitaryForcesType()
        {
            return militaryForcesType;
        }

        public MilitaryForces()
        {
            allDivisionInMilitaryForces.Clear();
        }

        public int GetMilitaryPower()
        {
            int power = 0;
            foreach(Weapon weapon in allWeaponsInMilitaryForces)
            {
                power += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttack;
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

        public List<Weapon> GetWeaponInMilitaryForcesInventory(int weaponID)
        {
            List<Weapon> tempWeaponList = new List<Weapon>();

            foreach (Weapon weapon in allWeaponsInMilitaryForces)
                if (weapon.weaponTemplateID == weaponID)
                    tempWeaponList.Add(weapon);

            return tempWeaponList;
        }

        public void CreateDivision(GameObjectAnimator tempDivision, DivisionTemplate divisionTemplate)
        {
            Division militaryDivision = tempDivision.CreateDivisionGameObject();
            militaryDivision.divisionTemplate = divisionTemplate;
            militaryDivision.divisionName = tempDivision.name;

            List<Weapon> mainUnitList = new List<Weapon>();
            foreach(int weaponID in divisionTemplate.mainUnitIDList)
                mainUnitList.AddRange(GetWeaponInMilitaryForcesInventory(weaponID));

            List<Weapon> secondUnitList = new List<Weapon>();
            foreach (int weaponID in divisionTemplate.secondUnitList)
                secondUnitList.AddRange(GetWeaponInMilitaryForcesInventory(weaponID));

            List<Weapon> thirdUnitList = new List<Weapon>();
            foreach (int weaponID in divisionTemplate.thirdUnitList)
                thirdUnitList.AddRange(GetWeaponInMilitaryForcesInventory(weaponID));

            if (mainUnitList.Count >= divisionTemplate.mainUnitMaximum)
            {
                for (int i = 0; i < divisionTemplate.mainUnitMaximum; i++)
                {
                    militaryDivision.AddWeaponToDivision(mainUnitList[i]);
                }

                int secondUnitNumber = divisionTemplate.secondUnitMaximum;
                int thirdUnitNumber = divisionTemplate.thirdUnitMaximum;

                if (secondUnitList.Count < divisionTemplate.secondUnitMaximum)
                {
                    secondUnitNumber = secondUnitList.Count;
                }
                if (thirdUnitList.Count < divisionTemplate.thirdUnitMaximum)
                {
                    thirdUnitNumber = thirdUnitList.Count;
                }
                
                for (int i = 0; i < secondUnitNumber; i++)
                {
                    militaryDivision.AddWeaponToDivision(mainUnitList[i]);
                }

                for (int i = 0; i < thirdUnitNumber; i++)
                {
                    militaryDivision.AddWeaponToDivision(mainUnitList[i]);
                }

                AddDivisionToMilitaryForces(tempDivision);
            }  
            else
            {
                //Debug.Log(divisionTemplate.mainUnit + " -> " + divisionTemplate.mainUnitMaximum);
            }
            
        }

        public int PossibleDivisionNumber(DivisionTemplate divisionTemplate)
        {
            int requiredWeaponNumber = divisionTemplate.mainUnitMaximum;
            int currentWeaponNumber = 0;
            
            foreach(int weaponID in divisionTemplate.mainUnitIDList)
                currentWeaponNumber = currentWeaponNumber + GetWeaponNumberInMilitaryForcesInventory(weaponID);
  
            return currentWeaponNumber / requiredWeaponNumber;
        }

        public void AddDivisionToMilitaryForces(GameObjectAnimator tempDivision)
        {
            allDivisionInMilitaryForces.Add(tempDivision);
        }

        public List<GameObjectAnimator> GetAllDivisionInMilitaryForces()
        {
            return allDivisionInMilitaryForces;
        }
    }
}