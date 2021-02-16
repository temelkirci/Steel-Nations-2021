using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WorldMapStrategyKit 
{
	public partial class Country: AdminEntity 
	{
        Army army;
        // -100/-50 = at war
        // -50/-25 = enemy
        // -25/25 = neutral
        // 25/50 = ally
        // 50/100 = friend

        IntelligenceAgency intelligenceAgency;

        List<Country> atWarList = new List<Country>();
        List<Country> nuclearWarList = new List<Country>();

        List<Country> placeTradeEmbargo = new List<Country>();
        List<Country> countriesImposingEconomicEmbargoOnYou = new List<Country>();

        List<Country> placeArmsEmbargo = new List<Country>();
        List<Country> countriesImposingGunEmbargoOnYou = new List<Country>();

        List<int> producibleWeapon = new List<int>();
        List<Policy> acceptedPolicyList = new List<Policy>();

        Texture2D countryFlag;

        List<Research> researchProgress = new List<Research>();
        List<Production> productionProgress = new List<Production>();

        Dictionary<Country, int> militaryAccess = new Dictionary<Country, int>(); // military access that you have
        WMSK map;

        public void SaveCountry()
        {

        }
        #region IntelligenceAgency
        public void CreateIntelligenceAgency(string name, int level, int budget, Texture2D flag)
        {
            intelligenceAgency = new IntelligenceAgency();
            intelligenceAgency.SetIntelligenceAgencyName(name);
            intelligenceAgency.SetIntelligenceAgencyLevel(level);
            intelligenceAgency.SetIntelligenceAgencyBudget(budget);
        }
        public IntelligenceAgency GetIntelligenceAgency()
        {
            return intelligenceAgency;
        }
        #endregion

        #region Dockyard
        public int GetTotalDockyardArea()
        {
            int totalDockyardArea = 0;
            foreach (City city in map.GetCities())
                totalDockyardArea += city.GetConstructibleDockyardAreaNumber();

            return totalDockyardArea;
        }

        public int GetTotalDockyard()
        {
            int totalDockyard = 0;
            foreach (City city in map.GetCities())
                totalDockyard += city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.DOCKYARD);

            return totalDockyard;
        }
        #endregion

        #region Manpower
        public long GetAvailableManpower()
        {
            long availableManpower = 0;
            foreach (City city in GetAllCitiesInCountry())
                availableManpower += city.population;

            return availableManpower;
        }
        #endregion

        #region Army
        public void CreateArmy()
        {
            army = new Army();
        }
        public Army GetArmy()
        {
            return army;
        }
        #endregion

        #region Country Flag
        public Texture2D GetCountryFlag()
        {
            return countryFlag;
        }
        public void SetCountryFlag(Texture2D flag)
        {
            countryFlag = flag;
        }
        #endregion

        #region Nuclear War Head
        public void SetNuclearWarHead(int number)
        {
            attrib["Nuclear WarHead"] = number;
        }

        public int GetNuclearWarHead()
        {
            return attrib["Nuclear WarHead"];
        }
        #endregion

        #region Tension
        public void SetTension(int tension)
        {
            attrib["Tension"] = tension;
        }
        public int GetTension()
        {
            return attrib["Tension"];
        }
        #endregion

        #region Unemployment
        public void SetUnemploymentRate(int unemployment)
        {
            attrib["Unemployment"] = unemployment;
        }
        public int GetUnemploymentRate()
        {
            return attrib["Unemployment"];
        }
        #endregion

        #region Production Speed
        public void SetProductionSpeed(int productionSpeed)
        {
            attrib["Production Speed"] = productionSpeed;
        }
        public int GetProductionSpeed()
        {
            return attrib["Production Speed"];
        }
        #endregion

        #region Fertility Rate
        public void SetFertilityRatePerWeek(float fertilityRate)
        {
            attrib["Fertility Rate"] = fertilityRate;
        }
        public float GetFertilityRatePerWeek()
        {
            return attrib["Fertility Rate"];
        }
        #endregion

        #region Pandemic
        public void SetPandemicDeathRatePerWeek(float pandemicDeathRate)
        {
            attrib["Pandemic Death Rate"] = pandemicDeathRate;
        }
        public float GetPandemicDeathRatePerWeek()
        {
            return attrib["Pandemic Death Rate"];
        }
        #endregion

        #region System Of Government
        public void SetSystemOfGovernment(string systemOfGovernment)
        {
            attrib["System Of Government"] = systemOfGovernment;
        }
        public string GetSystemOfGovernment()
        {
            return attrib["System Of Government"];
        }
        #endregion

        #region Export
        public void AddExport(int export)
        {
            attrib["Current Export"] += export;
        }
        public void SetExportMonthly(int export)
        {
            attrib["Export Monthly"] = export;
        }
        public int GetExportMonthly()
        {
            return attrib["Export Monthly"];
        }

        public void SetCurrentExport(int export)
        {
            attrib["Current Export"] = export;
        }
        public int GetCurrentExport()
        {
            return attrib["Current Export"];
        }

        public void SetPreviousExport(int export)
        {
            attrib["Previous Export"] = export;
        }
        public int GetPreviousExport()
        {
            return attrib["Previous Export"];
        }
        #endregion

        #region Import
        public void SetImportMonthly(int import)
        {
            attrib["Import Monthly"] = import;
        }
        public int GetImportMonthly()
        {
            return attrib["Import Monthly"];
        }

        public void SetCurrentImport(int import)
        {
            attrib["Current Import"] = import;
        }
        public int GetCurrentImport()
        {
            return attrib["Current Import"];
        }

        public void SetPreviousImport(int import)
        {
            attrib["Previous Import"] = import;
        }
        public int GetPreviousImport()
        {
            return attrib["Previous Import"];
        }
        #endregion

        #region Religion
        public void SetReligion(int christian, int muslim, int irreligion, int hindu, int buddist, int folk, int jewish, string religion)
        {
            attrib["Christian"] = christian;
            attrib["Muslim"] = muslim;
            attrib["Irreligion"] = irreligion;
            attrib["Hindu"] = hindu;
            attrib["Buddist"] = buddist;
            attrib["Folk Religion"] = folk;
            attrib["Jewish"] = jewish;

            attrib["Country Religion"] = religion;
        }

        public string GetReligion()
        {
            return attrib["Country Religion"];
        }
        #endregion

        #region Resources
        public void DistributeResources(int oil, int iron, int steel, int aluminium, int uranium)
        {
            foreach (City city in GetAllCitiesInCountry())
                city.SetReserveResources(oil, iron, steel, aluminium, uranium);
        }

        public int GetTotalOilReserves()
        {
            int oil = 0;
            foreach (City city in GetAllCitiesInCountry())
                oil += city.GetOilReserves();
            return oil;
        }
        public int GetTotalIronReserves()
        {
            int iron = 0;
            foreach (City city in GetAllCitiesInCountry())
                iron += city.GetIronReserves();
            return iron;
        }
        public int GetTotalSteelReserves()
        {
            int steel = 0;
            foreach (City city in GetAllCitiesInCountry())
                steel += city.GetSteelReserves();
            return steel;
        }
        public int GetTotalAluminiumReserves()
        {
            int aluminium = 0;
            foreach (City city in GetAllCitiesInCountry())
                aluminium += city.GetAluminiumReserves();
            return aluminium;
        }
        public int GetTotalUraniumReserves()
        {
            int uranium = 0;
            foreach (City city in GetAllCitiesInCountry())
                uranium += city.GetUraniumReserves();
            return uranium;
        }

        public int GetOil()
        {
            return attrib["Oil"];
        }
        public int GetIron()
        {
            return attrib["Iron"];
        }
        public int GetSteel()
        {
            return attrib["Steel"];
        }
        public int GetAluminium()
        {
            return attrib["Aluminium"];
        }
        public int GetUranium()
        {
            return attrib["Uranium"];
        }
        #endregion

        #region Military Access
        public void AddMilitaryAccess(Country tempCountry)
        {
            if (militaryAccess.ContainsKey(tempCountry) == false)
            {
                militaryAccess.Add(tempCountry, 30);
                NotificationManager.Instance.CreateNotification(name + " gave military access to " + tempCountry.name);
            }
        }
        public int GetLeftMilitaryAccess(Country tempCountry)
        {
            if(militaryAccess.ContainsKey(tempCountry))
                return militaryAccess[tempCountry];
            return 0;
        }
        public void DecreaseMilitaryAccess()
        {
            foreach (var pair in militaryAccess)
            {
                militaryAccess[pair.Key] -= 1;

                if(militaryAccess[pair.Key] <= 0)
                {
                    militaryAccess.Remove(pair.Key);
                    NotificationManager.Instance.CreateNotification(name + " finished military access for " + pair.Key.name);
                }
            }
        }
        #endregion

        #region Nuclear War
        public List<Country> GetNuclearWar()
        {
            return nuclearWarList;
        }
        public void AddNuclearWar(Country tempCountry)
        {
            nuclearWarList.Add(tempCountry);
            NotificationManager.Instance.CreateNotification(name + " begun nuclear war to " + tempCountry.name);
        }
        #endregion

        #region War
        public List<Country> GetCountryListAtWar()
        {
            return atWarList;
        }
        public bool AtWar(Country tempCountry)
        {
            return atWarList.Contains(tempCountry);
        }
        #endregion

        #region Arm Embargo
        public void PlaceArmsEmbargo(Country tempCountry)
        {
            placeArmsEmbargo.Add(tempCountry);
            NotificationManager.Instance.CreateNotification(name + " place a arms embargo to " + tempCountry.name);
        }
        #endregion

        #region Trade Embargo
        // Countries that have an economic embargo on you
        public void PlaceTradeEmbargo(Country tempCountry)
        {
            placeTradeEmbargo.Add(tempCountry);
            NotificationManager.Instance.CreateNotification(name + " place a trade embargo to " + tempCountry.name);
        }

        public List<Country> GetCountriesImposingEconomicEmbargoOnYou()
        {
            return placeTradeEmbargo;
        }

        public bool IsImposingEconomicEmbargoOnYou(Country tempCountry)
        {
            return countriesImposingEconomicEmbargoOnYou.Contains(tempCountry);
        }
        #endregion

        #region Debt
        public void SetDebt(int debt)
        {
            attrib["Debt"] = debt;
        }
        public int GetDebt()
        {
            return attrib["Debt"];
        }

        public int GetDebtMonthly()
        {
            return attrib["Debt"]/100;
        }
        #endregion

        #region Trade Ratio
        public int GetTradeRatio()
        {
            int totalGDP = 0;
            foreach(Country country in GameEventHandler.Instance.GetAllCountries())
            {
                totalGDP += country.GetPreviousGDP();
            }
            return attrib["Trade Ratio"] = float.Parse(GetPreviousGDP().ToString()) * 100f / totalGDP;
        }
        #endregion

        #region Military Rank
        public int GetMilitaryRank()
        {
            return attrib["Military Rank"];
        }
        public void SetMilitaryRank(int rank)
        {
            attrib["Military Rank"] = rank;
        }
        #endregion

        #region Taxes
        public int GetIndividualTax()
        {
            return attrib["Individual Tax Monthly"];
        }
        public void SetIndividualTax(int tax)
        {
            attrib["Individual Tax Monthly"] = tax;
        }
        public int GetCorporationTax()
        {
            return attrib["Corporation Tax Monthly"];
        }
        public void SetCorporationTax(int tax)
        {
            attrib["Corporation Tax Monthly"] = tax;
        }
        #endregion

        #region GDP
        public int GetCurrentGDP()
        {
            return attrib["Current GDP"];
        }
        public void SetCurrentGDP(int GDP)
        {
            attrib["Current GDP"] = GDP;
        }

        public int GetPreviousGDP()
        {
            return attrib["Previous GDP"];
        }
        public void SetPreviousGDP(int GDP)
        {
            attrib["Previous GDP"] = GDP;
        }
        /// <summary>
        /// /////////////
        /// </summary>
        /// <returns></returns>
        public int GetCurrentGDPAnnualGrowthRate()
        {
            return attrib["Current GDP Annual Growth Rate"];
        }
        public void SetCurrentGDPAnnualGrowthRate(int GDP)
        {
            attrib["Current GDP Annual Growth Rate"] = GDP;
        }

        public int GetPreviousGDPAnnualGrowthRate()
        {
            return attrib["Previous GDP Annual Growth Rate"];
        }
        public void SetPreviousGDPAnnualGrowthRate(int GDP)
        {
            attrib["Previous GDP Annual Growth Rate"] = GDP;
        }

        #endregion

        #region Budget
        public float GetBudget()
        {
            return attrib["Budget"];
        }
        public void SetBudget(float budget)
        {
            attrib["Budget"] = budget;
        }
        public void AddBudget(float budget)
        {
            attrib["Budget"] += budget;
        }
        #endregion

        #region Policy
        public void AddPolicy(Policy policy)
        {
            acceptedPolicyList.Add(policy);
        }
        public List<Policy> GetAcceptedPolicyList()
        {
            return acceptedPolicyList;
        }
        #endregion

        #region Research
        public List<Research> GetAllResearchsInProgress()
        {
            return researchProgress;
        }
        public void AddResearch(Research research)
        {
            researchProgress.Add(research);
        }
        public void SetResearchSpeed(int researchSpeed)
        {
            attrib["Research Speed"] = researchSpeed;
        }
        public int GetResearchSpeed()
        {
            return attrib["Research Speed"];
        }
        #endregion

        #region Production
        public List<Production> GetAllProductionsInProgress()
        {
            return productionProgress;
        }
        #endregion

        #region Mineral Income
        public void AddMineralIncome(int income)
        {
            attrib["Mineral Income"] += income;
        }
        public void SetMineralIncome(int income)
        {
            attrib["Mineral Income"] = income;
        }
        public int GetMineralIncome()
        {
            return attrib["Mineral Income"];
        }
        #endregion

        #region Gun Income
        public void AddGunIncome(int income)
        {
            attrib["Gun Income"] += income;
        }
        public void SetGunIncome(int income)
        {
            attrib["Gun Income"] = income;
        }
        public int GetGunIncome()
        {
            return attrib["Gun Income"];
        }
        #endregion

        #region Oil Income
        public void AddOilIncome(int income)
        {
            attrib["Oil Income"] += income;
        }
        public void SetOilIncome(int income)
        {
            attrib["Oil Income"] = income;
        }
        public int GetOilIncome()
        {
            return attrib["Oil Income"];
        }
        #endregion

        public bool IsWeaponProducible(int weaponID)
        {
            if (producibleWeapon.Contains(weaponID))
                return true;
            else
                return false;
        }

        public List<int> GetProducibleWeapons()
        {
            return producibleWeapon;
        }

        public void AddProducibleWeaponToInventory(int weaponID)
        {
            if (IsWeaponProducible(weaponID) == false)
                producibleWeapon.Add(weaponID);
        }

        #region Cities
        public List<City> GetAllCitiesInCountry()
        {
            return map.GetCities(this);
        }
        #endregion

        public int GetTotalBuildings(BUILDING_TYPE building)
        {
            int totalBuildings = 0;

            foreach (City city in GetAllCitiesInCountry())
                totalBuildings += city.GetCurrentBuildingNumberInCity(building);
            return totalBuildings;
        }

        public float GetTradeBonus()
        {
            float totalTradeBonus = 0;
            foreach (Organization organization in OrganizationManager.Instance.GetAllOrganizations())
            {
                if (organization.isActive == true && organization.isFullMemberCountry(this))
                {
                    totalTradeBonus = totalTradeBonus + organization.tradeBonusPerWeek;
                }
            }

            return totalTradeBonus;
        }

        public void UpdateEconomy()
        {
            float individualTax = GetIndividualTaxIncomeMonthly();
            float corporationlTax = GetCorporationTaxIncomeMonthly();

            AddBudget(individualTax);
            AddBudget(corporationlTax);

            int export = GetExportMonthly() + (int)(GetExportMonthly() * GetTradeBonus()) / 100;
            AddExport(export);

            //attrib["Current GDP"] = float.Parse(attrib["Current GDP"]) + totalTaxes + totalTrade;
        }

        public float GetIndividualTaxIncomeMonthly()
        {
            float individualTax = GetAvailableManpower() * GetIndividualTax();

            individualTax = (individualTax / 1000000);

            if (individualTax < 1)
                individualTax = 1;

            return individualTax;
        }
        public int GetCorporationTaxIncomeMonthly()
        {
            int corporationlTax = GetTotalBuildings(BUILDING_TYPE.FACTORY) * GetCorporationTax();

            corporationlTax = (corporationlTax / 1000000);

            if (corporationlTax < 1)
                corporationlTax = 1;

            return corporationlTax;
        }

        public void DeclareWar(Country enemy)
        {
            atWarList.Add(enemy);

            foreach (Organization org in OrganizationManager.Instance.GetAllOrganizations())
            {
                if (org.isAttackForMember == true && org.isFullMemberCountry(enemy))
                {
                    foreach (Country tempCountry in org.GetFullMemberList())
                    {
                        tempCountry.atWarList.Add(this);
                    }
                }
            }

            enemy.atWarList.Add(this);
            NotificationManager.Instance.CreateNotification(name + " has declared war to " + enemy.name);
            NotificationManager.Instance.CreateNotification(enemy.name + " has declared war to " + name);
        }

        void CheckWeapon(WeaponTemplate tempWeapon, int unitNumber, int unitTech)
        {
            if (tempWeapon.weaponLevel < unitTech)
            {
                AddProducibleWeaponToInventory(tempWeapon.weaponID);
            }
            else if (tempWeapon.weaponLevel == unitTech)
            {
                AddProducibleWeaponToInventory(tempWeapon.weaponID);

                for (int i = 0; i < unitNumber; i++)
                {
                    Weapon weapon = new Weapon();
                    weapon.weaponTemplateID = tempWeapon.weaponID;
                    weapon.weaponLeftHealth = tempWeapon.weaponDefense;

                    if (tempWeapon.weaponTerrainType == 1)
                        GetArmy().GetLandForces().AddWeaponToMilitaryForces(weapon);
                    if (tempWeapon.weaponTerrainType == 2)
                        GetArmy().GetNavalForces().AddWeaponToMilitaryForces(weapon);
                    if (tempWeapon.weaponTerrainType == 3 || tempWeapon.weaponTerrainType == 4)
                        GetArmy().GetAirForces().AddWeaponToMilitaryForces(weapon);
                }
            }
        }

        public void UpdateResourcesInCountry()
        {
            foreach (City city in GetAllCitiesInCountry())
            {
                if (city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.MINERAL_FACTORY) > 0)
                {
                     city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.MINERAL_FACTORY);
                    attrib["Aluminium"] = attrib["Aluminium"] + city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.MINERAL_FACTORY);
                    attrib["Steel"] = attrib["Steel"] + city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.MINERAL_FACTORY);

                    attrib["Uranium"] = attrib["Uranium"] + city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.NUCLEAR_FACILITY);
                }

                if (city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.OIL_RAFINERY) > 0)
                {
                    attrib["Oil"] = attrib["Oil"] + city.GetOilReserves() * city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.OIL_RAFINERY);
                }
            }
        }

        public void DailyUpdateForCountry()
        {
            DecreaseMilitaryAccess();
        }

        public void UpdateWeaponTechnology()
        {
            producibleWeapon.Clear();

            foreach (WeaponTemplate tempWeapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                string tech = tempWeapon.weaponName + " Tech";

                int unitNumber = attrib[tempWeapon.weaponName];
                int unitTech = attrib[tech];

                CheckWeapon(tempWeapon, unitNumber, unitTech);               
            }
                     
        }

        public void UpdateAllConstructionInCountry()
        {
            foreach(City city in GetAllCitiesInCountry())
            {
                city.UpdateConstructionInCity();
            }
        }

        public void CheckResearch()
        {
            List<WeaponTemplate> tempWeaponList = new List<WeaponTemplate>();

            foreach(WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                if (IsWeaponProducible(weapon.weaponID) == false && ( GameEventHandler.Instance.GetCurrentYear() - weapon.weaponResearchYear) > 30)
                    tempWeaponList.Add(weapon);
            }

            if(tempWeaponList.Count > 0)
            {
                foreach(WeaponTemplate weapon in tempWeaponList)
                {
                    if (weapon.weaponTerrainType == 2 && IsWeaponAvailableForResearch(weapon))
                        ResearchWeapon(weapon);
                    else
                    {
                        if(weapon.weaponTerrainType == 1 && IsWeaponAvailableForResearch(weapon))
                        {
                            ResearchWeapon(weapon);
                        }
                        else if(weapon.weaponTerrainType == 3 && IsWeaponAvailableForResearch(weapon))
                        {
                            ResearchWeapon(weapon);
                        }
                    }
                }                
            }
        }

        public void ResearchWeapon(WeaponTemplate weapon)
        {
            if (IsWeaponProducible(weapon.weaponID) == false)
            {
                if (IsWeaponResearchInProgress(weapon))
                {
                    Research research = new Research();
                    research.techWeapon = weapon;
                    research.researchCountries.Add(this);
                    research.leftDays = weapon.weaponResearchTime;
                    research.totalResearchDay = weapon.weaponResearchTime;

                    attrib["Defense Budget"] = attrib["Defense Budget"] - weapon.weaponResearchCost;
                    researchProgress.Add(research);
                }
            }
            else
            {
                Debug.Log("Defense Budget is not enough");
            }
        }

        public void UpdateNatality()
        {
            foreach (City city in GetAllCitiesInCountry())
            {
                city.population = city.population + ((city.population * attrib["Country Natality"]) / 5000);
                city.population = city.population - ((city.population * attrib["Corona"]) / 10000);
            }
        }

        public void UpdateResearchInProgress()
        {
            foreach (Research production in researchProgress)
            {
                production.leftDays = production.leftDays - 1;

                if (production.leftDays <= 0)
                {
                    foreach (Country country in production.researchCountries)
                    {
                        country.AddProducibleWeaponToInventory(production.techWeapon.weaponID);
                        country.researchProgress.Remove(production);
                    }
                }
            }
        }

        public void CreateBuildings(BUILDING_TYPE type, int buildingNumber)
        {
            List<City> cityList = GetCityListByPopulation();

            int number = buildingNumber;

            if (buildingNumber > cityList.Count)
                number = cityList.Count;

            for (int i = 0; i < number; i++)
            {
                int tempBuildingNumber = 1;

                if (cityList[i].cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                {
                    tempBuildingNumber = 2;
                }
                if (cityList[i].cityClass == CITY_CLASS.REGION_CAPITAL)
                {
                    tempBuildingNumber = 1;
                }
                cityList[i].SetCurrentBuildingNumberInCity(type, tempBuildingNumber);
            }
        }


        /// <summary>
        /// Begin Production
        /// </summary>
        /// <param name="weapon"></param>
        /// 
        public void CheckWeaponProduct()
        {
            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {

            }
        }

        public void ProductWeapon(WeaponTemplate weapon)
        {
            if (GetArmy().GetDefenseBudget() >= weapon.weaponCost &&
                GetTotalIronReserves() >= weapon.requiredIron &&
                GetTotalSteelReserves() >= weapon.requiredSteel &&
                GetTotalAluminiumReserves() >= weapon.requiredAluminium)
            {
                attrib["Defense Budget"] = attrib["Defense Budget"] - weapon.weaponCost;

                attrib["Iron"] = attrib["Iron"] - weapon.requiredIron;
                attrib["Steel"] = attrib["Steel"] - weapon.requiredSteel;
                attrib["Aluminium"] = attrib["Aluminium"] - weapon.requiredAluminium;

                int researchSpeedRotio = (GetTotalBuildings(BUILDING_TYPE.MILITARY_FACTORY) / 10) + GetProductionSpeed();

                int weaponProductionTime = (weapon.weaponProductionTime - ((weapon.weaponProductionTime * researchSpeedRotio) / 100));
                if (weaponProductionTime <= 1)
                    weaponProductionTime = 1;

                Production production = new Production();
                production.techWeapon = weapon;
                production.productionCountries.Add(this);
                production.totalProductionDay = weapon.weaponProductionTime;

                productionProgress.Add(production);
            }
        }
        public void UpdateProductionInProgress()
        {
            foreach (Production research in productionProgress)
            {
                research.leftDays -= 1;

                if (research.leftDays <= 0)
                {
                    foreach (Country country in research.productionCountries)
                    {
                        country.AddProducibleWeaponToInventory(research.techWeapon.weaponID);
                    }

                    productionProgress.Remove(research);
                }
            }
        }

        /// <summary>
        /// End Production
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        /// 

        public bool IsWeaponResearchInProgress(WeaponTemplate weapon)
        {
            foreach (Production production in productionProgress)
            {
                if (production.productionCountries.Contains(this))
                    return true;
            }

            return false;
        }

        public bool IsWeaponAvailableForResearch(WeaponTemplate weapon)
        {
            if (weapon.weaponTerrainType == 2 )
            {
                return UpdateMaximumDockyardAreaInCountry() > 0 && IsWeaponProducible(weapon.weaponID) && GetArmy().GetDefenseBudget() >= weapon.weaponResearchCost;
            }
            else
            {
                return IsWeaponProducible(weapon.weaponID) && GetArmy().GetDefenseBudget() >= weapon.weaponResearchCost;
            }
        }

        public int UpdateMaximumDockyardAreaInCountry()
        {
            int totalDockyardArea = 0;

            foreach(City city in WMSK.instance.GetCities())
            {
                if (city.GetConstructibleDockyardAreaNumber() > 0)
                    totalDockyardArea = totalDockyardArea + 1;
            }

            return totalDockyardArea;
        }

        public int UpdateCurrentDockyardInCountry()
        {
            int totalDockyard = 0;

            foreach (City city in WMSK.instance.GetCities())
            {
                if (city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.DOCKYARD) > 0)
                    totalDockyard = totalDockyard + 1;
            }

            return totalDockyard;
        }

        public void PayDebt()
        {
            if (GetDebt() >= attrib["Debt Per Week"] && GetDebt() >= 0)
            {
                attrib["Debt"] = attrib["Debt"] - attrib["Debt Per Week"];

                if (attrib["Debt"] <= 0)
                    attrib["Debt"] = 0;

                attrib["Debt"] = attrib["Debt"] - attrib["Debt Per Week"];
            }
        }

        public string GetCountryPerCapitaIncome()
        {
            float PerCapitaIncome = (GetPreviousGDP() * 1000000f) / GetAvailableManpower();
            string financialStatus = string.Empty;

            if (PerCapitaIncome >= 100000)
            {
                financialStatus = "Very Rich";
                SetIndividualTax(500);
                SetCorporationTax(1000);
            }
            if (PerCapitaIncome < 100000 && PerCapitaIncome >= 50000)
            {
                financialStatus = "Rich";
                SetIndividualTax(100);
                SetCorporationTax(500);
            }
            if (PerCapitaIncome < 50000 && PerCapitaIncome >= 18000)
            {
                financialStatus = "Avarage";
                SetIndividualTax(25);
                SetCorporationTax(250);
            }
            if (PerCapitaIncome < 18000 && PerCapitaIncome >= 5000)
            {
                financialStatus = "Poor";
                SetIndividualTax(10);
                SetCorporationTax(100);
            }
            if (PerCapitaIncome < 5000 && PerCapitaIncome >= 0)
            {
                financialStatus = "Very Poor";
                SetIndividualTax(1);
                SetCorporationTax(20);
            }
            return financialStatus;
		}

		public string GetCountryTension()
        {
			string tension = string.Empty;

			if(attrib["Tension"] < 20)
				tension = "Very Low";
			if (attrib["Tension"] >=20 && attrib["Tension"] < 40)
				tension = "Low";
			if (attrib["Tension"] >= 40 && attrib["Tension"] < 60)
				tension = "Avarage";
			if (attrib["Tension"] >= 60 && attrib["Tension"] < 80)
				tension = "High";
			if (attrib["Tension"] >= 80)
				tension = "Very High";

			return tension;
		}

        public List<City> GetCityListByPopulation()
        {
            List<City> orderedCity = GetAllCitiesInCountry().OrderBy(city => city.population).ToList();

            orderedCity.Reverse();

            return orderedCity;
        }

        /// <summary>
        /// Continent name.
        /// </summary>
        public string continent;

		/// <summary>
		/// List of provinces that belongs to this country.
		/// </summary>
		public Province[] provinces;

        /// <summary>
        /// The index of the capital city
        /// </summary>
        public int capitalCityIndex = -1;

		int[] _neighbours;

		/// <summary>
		/// Custom array of countries that could be reached from this country. Useful for Country path-finding.
		/// It defaults to natural neighbours of the country but you can modify its contents and add your own potential destinations per country.
		/// </summary>
		public override int[] neighbours {
			get {
				if (_neighbours == null) {
					int cc = 0;
					List<Country> nn = new List<Country>();
					if (regions != null) {
						regions.ForEach(r => {
								if (r != null && r.neighbours != null) {
									r.neighbours.ForEach(n => {
											if (n != null) {
												Country otherCountry = (Country)n.entity;
												if (!nn.Contains(otherCountry))
													nn.Add(otherCountry);
											}
										}
									);

								}
							});
						cc = nn.Count;
					}
					_neighbours = new int[cc];
					for (int k = 0; k < cc; k++) {
						_neighbours[k] = WMSK.instance.GetCountryIndex(nn[k]);
					}
				}
				return _neighbours;
			}
			set {
				_neighbours = value;
			}
		}

		/// <summary>
		/// True for a country acting as a provinces pool created by CreateCountryProvincesPool function.
		/// The effect of this field is that all transfer operations will ignore its borders which results in a faster operation
		/// </summary>
		public bool isPool;

		// Standardized codes
		public string fips10_4 = "";
		public string iso_a2 = "";
		public string iso_a3 = "";
		public string iso_n3 = "";

		/// <summary>
		/// If provinces will be shown for this country
		/// </summary>
		public bool showProvinces = true;

		/// <summary>
		/// If province highlighting is enabled for this country
		/// </summary>
		public bool allowProvincesHighlight = true;

		/// <summary>
		/// Creates a new country
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="continent">Continent.</param>
		/// <param name="uniqueId">For user created countries, use a number between 1-999 to uniquely identify this country.</param>
		public Country(string name, string continent, int uniqueId) {
			this.name = name;
			this.continent = continent;
			this.regions = new List<Region>();
			this.uniqueId = uniqueId;
			this.attrib = new JSONObject();
			this.mainRegionIndex = -1;

            map = WMSK.instance;
        }

		public Country Clone() {
			Country c = new Country(name, continent, uniqueId);
			c.center = center;
            c.regions = new List<Region>(regions.Count);
            for (int k=0;k<regions.Count;k++) {
                c.regions.Add(regions[k].Clone());
            }
			c.customLabel = customLabel;
			c.labelColor = labelColor;
			c.labelColorOverride = labelColorOverride;
			c.labelFontOverride = labelFontOverride;
			c.labelVisible = labelVisible;
			c.labelOffset = labelOffset;
			c.labelRotation = labelRotation;
			c.provinces = provinces;
			c.hidden = this.hidden;
			c.attrib = new JSONObject();
			c.attrib.Absorb(attrib);
            c.regionsRect2D = regionsRect2D;
			return c;
		}
	}

}