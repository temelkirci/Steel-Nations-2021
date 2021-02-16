using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit 
{
	public class Army
	{
        MilitaryForces landForces; // land forces in army
        MilitaryForces airForces; // air forces in army
        MilitaryForces navalForces; // naval forces in army

        int soldierNumber;
        int defenseBudget;

        // add genelkurmay başkanı

        public Army()
        {
            landForces = null;
            airForces = null;
            navalForces = null;

            defenseBudget = 0;

            CreateArmy();
        }

        #region Defense Budget
        public int GetDefenseBudget()
        {
            return defenseBudget;
        }
        public void SetDefenseBudget(int defenseBudget)
        {
            this.defenseBudget = defenseBudget;
        }
        #endregion

        #region Division
        public List<GameObjectAnimator> GetAllDivisionInArmy()
        {
            List<GameObjectAnimator> allDivisionsInArmy = new List<GameObjectAnimator>();

            allDivisionsInArmy.Clear();

            foreach (GameObjectAnimator tempDivision in GetLandForces().GetAllDivisionInMilitaryForces())
                allDivisionsInArmy.Add(tempDivision);
            foreach (GameObjectAnimator tempDivision in GetAirForces().GetAllDivisionInMilitaryForces())
                allDivisionsInArmy.Add(tempDivision);
            foreach (GameObjectAnimator tempDivision in GetNavalForces().GetAllDivisionInMilitaryForces())
                allDivisionsInArmy.Add(tempDivision);

            return allDivisionsInArmy;
        }
        #endregion

        public void SetSoldierNumber(int number)
        {
            soldierNumber = number;
        }
        public int GetSoldierNumber()
        {
            return soldierNumber;
        }

        public void CreateArmy()
        {
            CreateLandForces();
            CreateAirForces();
            CreateNavalForces();
        }

        public void CreateLandForces()
        {
            landForces = new MilitaryForces();
            landForces.SetMilitaryForcesType(MILITARY_FORCES_TYPE.LAND_FORCES);
        }

        public void CreateAirForces()
        {
            airForces = new MilitaryForces();
            airForces.SetMilitaryForcesType(MILITARY_FORCES_TYPE.AIR_FORCES);

        }

        public void CreateNavalForces()
        {
            navalForces = new MilitaryForces();
            navalForces.SetMilitaryForcesType(MILITARY_FORCES_TYPE.NAVAL_FORCES);
        }

        public MilitaryForces GetLandForces()
        {
            return landForces;
        }

        public MilitaryForces GetAirForces()
        {
            return airForces;
        }

        public MilitaryForces GetNavalForces()
        {
            return navalForces;
        }
        
        public int GetArmyExpenseMonthly()
        {
            float totalExpense = 0;

            foreach(Weapon weapon in GetLandForces().GetAllWeaponsInMilitaryForces())
            {
                totalExpense += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponCost;
            }
            foreach (Weapon weapon in GetNavalForces().GetAllWeaponsInMilitaryForces())
            {
                totalExpense += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponCost;
            }
            foreach (Weapon weapon in GetAirForces().GetAllWeaponsInMilitaryForces())
            {
                totalExpense += WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponCost;
            }

            int expense = (int)(totalExpense / 100f);

            return expense;
        }

        public Dictionary<WeaponTemplate, int> GetAllWeaponsInArmyInventory()
        {
            Dictionary<WeaponTemplate, int> allUnitsInArmyInventory = new Dictionary<WeaponTemplate, int>(); // weapons in inventory

            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                if(weapon.weaponTerrainType == 1)
                    GetLandForces().GetWeaponNumberInMilitaryForcesInventory(weapon.weaponID);
                if (weapon.weaponTerrainType == 2)
                    GetNavalForces().GetWeaponNumberInMilitaryForcesInventory(weapon.weaponID);
                if (weapon.weaponTerrainType == 3 || weapon.weaponTerrainType == 4)
                    GetAirForces().GetWeaponNumberInMilitaryForcesInventory(weapon.weaponID);
            }

            return allUnitsInArmyInventory;
        }
    }
}