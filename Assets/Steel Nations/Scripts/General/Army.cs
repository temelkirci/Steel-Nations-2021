using System.Collections.Generic;

namespace WorldMapStrategyKit 
{
	public class Army
	{
        MilitaryForces landForces; // land forces in army
        MilitaryForces airForces; // air forces in army
        MilitaryForces navalForces; // naval forces in army

        int soldierNumber;
        int defenseBudget;

        Person chiefOfGeneralStaff;
        Country country;

        public Army()
        {
            landForces = null;
            airForces = null;
            navalForces = null;

            defenseBudget = 0;
            soldierNumber = 0;
        }

        public Country Country
        {
            get { return country; }
            set { country = value; }
        }

        #region Defense Budget
        public int Defense_Budget
        {
            get { return defenseBudget; }
            set { defenseBudget = value; }
        }
        #endregion

        #region Division
        public List<GameObjectAnimator> GetAllDivisionInArmy()
        {
            List<GameObjectAnimator> allDivisionsInArmy = new List<GameObjectAnimator>();

            foreach (GameObjectAnimator tempDivision in GetLandForces().GetAllDivisionInMilitaryForces().ToArray())
                allDivisionsInArmy.Add(tempDivision);
            foreach (GameObjectAnimator tempDivision in GetAirForces().GetAllDivisionInMilitaryForces().ToArray())
                allDivisionsInArmy.Add(tempDivision);
            foreach (GameObjectAnimator tempDivision in GetNavalForces().GetAllDivisionInMilitaryForces().ToArray())
                allDivisionsInArmy.Add(tempDivision);

            return allDivisionsInArmy;
        }

        public void RemoveDivisionFromArmy(GameObjectAnimator division)
        {
            if (GetLandForces().GetAllDivisionInMilitaryForces().Contains(division))
                GetLandForces().RemoveDivisionInMilitaryForces(division);

            if (GetAirForces().GetAllDivisionInMilitaryForces().Contains(division))
                GetAirForces().RemoveDivisionInMilitaryForces(division);

            if (GetNavalForces().GetAllDivisionInMilitaryForces().Contains(division))
                GetNavalForces().RemoveDivisionInMilitaryForces(division);
        }
        #endregion

        public GameObjectAnimator GetDivisionGOA(Division division)
        {
            foreach (GameObjectAnimator GOA in GetAllDivisionInArmy())
                if (GOA.GetDivision() == division)
                    return GOA;
            return null;
        }

        public int GetArmyPower()
        {
            int landPower = landForces.GetMilitaryPower();
            int airPower = airForces.GetMilitaryPower();
            int navalPower = navalForces.GetMilitaryPower();

            return landPower + airPower + navalPower;
        }
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
            chiefOfGeneralStaff = PeopleManager.Instance.CreatePerson(PERSON_TYPE.SUPREME_COMMANDER, string.Empty);

            CreateLandForces();
            CreateAirForces();
            CreateNavalForces();
        }

        public void CreateLandForces()
        {
            landForces = new MilitaryForces();
            landForces.SetMilitaryForcesType(MILITARY_FORCES_TYPE.LAND_FORCES);

            landForces.SupremeCommander = PeopleManager.Instance.CreatePerson(PERSON_TYPE.SUPREME_COMMANDER, string.Empty);
        }

        public void CreateAirForces()
        {
            airForces = new MilitaryForces();
            airForces.SetMilitaryForcesType(MILITARY_FORCES_TYPE.AIR_FORCES);

            airForces.SupremeCommander = PeopleManager.Instance.CreatePerson(PERSON_TYPE.SUPREME_COMMANDER, string.Empty);
        }

        public void CreateNavalForces()
        {
            navalForces = new MilitaryForces();
            navalForces.SetMilitaryForcesType(MILITARY_FORCES_TYPE.NAVAL_FORCES);

            navalForces.SupremeCommander = PeopleManager.Instance.CreatePerson(PERSON_TYPE.SUPREME_COMMANDER, string.Empty);
        }

        public void AddWeaponToMilitaryForces(WeaponTemplate template, int amount)
        {
            Weapon weapon = new Weapon();
            weapon.weaponTemplateID = template.weaponID;
            weapon.weaponLeftHealth = template.weaponDefense;

            if (template.weaponTerrainType == 1)
                for(int i=0; i<amount; i++)
                    GetLandForces().AddWeaponToMilitaryForces(weapon);

            if (template.weaponTerrainType == 2)
                for (int i = 0; i < amount; i++)
                    GetNavalForces().AddWeaponToMilitaryForces(weapon);

            if (template.weaponTerrainType == 3 || template.weaponTerrainType == 4)
                for (int i = 0; i < amount; i++)
                    GetAirForces().AddWeaponToMilitaryForces(weapon);
        }
        public void RemoveWeaponFromMilitaryForcesByWeaponTemplate(WeaponTemplate weaponTemplate, int amount)
        {
            if (weaponTemplate.weaponTerrainType == 1)
                for (int i = 0; i < amount; i++)
                    GetLandForces().RemoveWeaponFromMilitaryForcesByWeaponID(weaponTemplate.weaponID);

            if (weaponTemplate.weaponTerrainType == 2)
                for (int i = 0; i < amount; i++)
                    GetNavalForces().RemoveWeaponFromMilitaryForcesByWeaponID(weaponTemplate.weaponID);

            if (weaponTemplate.weaponTerrainType == 3 || weaponTemplate.weaponTerrainType == 4)
                for (int i = 0; i < amount; i++)
                    GetAirForces().RemoveWeaponFromMilitaryForcesByWeaponID(weaponTemplate.weaponID);
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
                int number = 0;
                if (weapon.weaponTerrainType == 1)
                {
                    number = GetLandForces().GetWeaponNumberInMilitaryForcesInventory(weapon.weaponID);
                }
                if (weapon.weaponTerrainType == 2)
                {
                    number = GetNavalForces().GetWeaponNumberInMilitaryForcesInventory(weapon.weaponID);
                }
                if (weapon.weaponTerrainType == 3 || weapon.weaponTerrainType == 4)
                {
                    number = GetAirForces().GetWeaponNumberInMilitaryForcesInventory(weapon.weaponID);
                }

                allUnitsInArmyInventory.Add(weapon, number);
            }

            return allUnitsInArmyInventory;
        }

    }
}