using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class CountryManager : Singleton<CountryManager>
    {
        WMSK map = WMSK.instance;

        public List<Country> GetAllCountriesWhichHaveArmy(bool includedMyCountry)
        {
            List<Country> countryHaveArmyList = new List<Country>();

            foreach (Country country in map.countries)
            {
                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                {
                    if (includedMyCountry == true)
                    {
                        if (country.GetArmy() != null)
                            countryHaveArmyList.Add(country);
                    }
                }
                else
                {
                    if (country.GetArmy() != null)
                        countryHaveArmyList.Add(country);
                }

            }
            return countryHaveArmyList;
        }

        public Country GetCountryByName(string countryName)
        {
            foreach (Country country in map.countries)
            {
                if (country.name == countryName)
                {
                    return country;
                }
            }
            return null;
        }

        public void DailyUpdateForAllCountries()
        {
            CountryAI();
        }

        public void UpdateResources()
        {
            foreach (Country tempCountry in map.countries)
            {
                if (tempCountry != null)
                {
                    UpdateResourcesInCountry(tempCountry);
                }
            }
        }
        public void UpdateEconomy()
        {
            foreach (Country country in map.countries)
            {
                if (country != null)
                {
                    UpdateEconomy(country);
                }
            }
        }

        public void UpdateBirthRate()
        {
            foreach (Country tempCountry in map.countries)
            {
                if (tempCountry != null)
                {
                    UpdateNatality(tempCountry);
                }
            }
        }

        public void CountryAI()
        {
            foreach (Country country in map.countries)
            {
                if (country != null)
                {
                    DecreaseMilitaryAccess(country);
                    UpdateAllConstructionInCountry(country);
                    UpdateResearchInProgress(country);
                    UpdateProductionInProgress(country);
                    ActionManager.Instance.UpdateActionsInProgress(country);
                    WarManager.Instance.WarStrategy(country);
                }
            }
        }

        public void WarDecision(Country country, Country enemy)
        {
            if (GetAtWarCountryList(country).Contains(enemy) == true)
                return;

            if (GetAllEnemies(country).Contains(enemy) == false)
                return;

            int myArmyPower = country.GetArmy().GetArmyPower();

            if (myArmyPower == 0)
                return;

            int danger = 0;

            if (enemy.GetArmsEmbargo().Contains(country))
                danger += 5;
            if (enemy.GetTradeEmbargo().Contains(country))
                danger += 10;

            int countrySupportOrganizationPower = OrganizationManager.Instance.GetOrganizationsPower(country);
            int enemySupportOrganizationPower = OrganizationManager.Instance.GetOrganizationsPower(enemy);

            if(enemySupportOrganizationPower > 0)
                danger *= (countrySupportOrganizationPower / enemySupportOrganizationPower);


            if (enemy.GetArmy() != null)
            {
                int powerRate = 0;
                int enemyMilitaryPower = enemy.GetArmy().GetArmyPower();

                if (enemyMilitaryPower > 0)
                    powerRate = myArmyPower / enemyMilitaryPower;

                if (enemy.Nuclear_Power)
                    danger -= 100;
                if (country.Nuclear_Power)
                    danger += 100;

                danger *= powerRate;
            }

            int random = Random.Range(0, 10000);

            if (random < danger)
            {
                Debug.Log(country.name + " -> " + enemy.name + " -> " + random + " -> " + danger);

                /*
                ActionManager.Instance.CreateAction(
               country,
               enemy,
               ACTION_TYPE.Declare_War,
               MINERAL_TYPE.NONE,
               0,
               null,
               0,
               null,
               null,
               0,
               0,
               0);
                */
            }
        }

        public void InitBudget(Country country)
        {
            country.Current_GDP_Annual_Growth_Rate = ((country.Current_GDP * 100.0f) / country.Previous_GDP) - 100.0f;

            country.Previous_GDP = country.Current_GDP;
            country.Current_GDP = 0;

            //country.Debt = debt;
            //country.Debt_Payment_Rate = 100;
            //country.Debt_Payment = debt / 100;

            if (country.GetArmy() != null)
            {
                country.Budget += country.Defense_Budget;
                country.Defense_Budget = (int)(country.Previous_GDP * country.Defense_Budget_By_GDP) / 100;
            }

            if (country.Intelligence_Agency != null)
            {
                country.Intelligence_Agency.IntelligenceAgencyBudget = (int)(country.Previous_GDP * country.Intelligence_Agency_Budget_By_GDP) / 100;
            }

            if (country.Budget <= 0)
            {

                if(country.Tax_Rate < 5)
                    country.Tax_Rate++;
            }
        }

        public int GetWeaponPrice(Country country1, Country country2, WeaponTemplate template, int amount)
        {
            int weaponCost = (template.weaponCost * amount) - ((template.weaponCost * amount) * country2.GetRelation(country1.name)) / 100;

            return weaponCost;
        }

        public Production ProductWeapon(Country country, WeaponTemplate weapon, int weaponNumber)
        {
            if(CountryHasEnoughResourceToProduceWeapon(country, weapon, weaponNumber))
            {
                Production production = new Production();
                production.techWeapon = weapon;
                production.productionCountries.Add(country);
                production.leftDays = GetWeaponProductionDay(country, weapon) * weaponNumber;
                production.totalDays = GetWeaponProductionDay(country, weapon) * weaponNumber;

                country.GetAllProductionsInProgress().Add(production);

                if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                    HUDManager.Instance.PrivateNotification("Production has started " + weapon.weaponName);

                return production;
            }

            return null;
        }

        public bool CountryHasEnoughResourceToProduceWeapon(Country country, WeaponTemplate weapon, int weaponNumber)
        {
            int requiredDefenseBudget = weapon.weaponCost * weaponNumber;

            int requiredOil = weapon.requiredOil * weaponNumber;
            int requiredIron = weapon.requiredIron * weaponNumber;
            int requiredSteel = weapon.requiredSteel * weaponNumber;
            int requiredAluminium = weapon.requiredAluminium * weaponNumber;
            int requiredUranium = weapon.requiredUranium * weaponNumber;

            if (country.Defense_Budget < requiredDefenseBudget)
                return false;

            if (country.GetMineral(MINERAL_TYPE.IRON) < requiredIron ||
                country.GetMineral(MINERAL_TYPE.STEEL) < requiredSteel ||
                country.GetMineral(MINERAL_TYPE.ALUMINIUM) < requiredAluminium ||
                country.GetMineral(MINERAL_TYPE.OIL) < requiredOil ||
                country.GetMineral(MINERAL_TYPE.URANIUM) < requiredUranium)
            {
                return false;
            }

            country.Defense_Budget -= requiredDefenseBudget;

            country.AddMineral(MINERAL_TYPE.IRON, -weapon.requiredIron);
            country.AddMineral(MINERAL_TYPE.STEEL, -weapon.requiredSteel);
            country.AddMineral(MINERAL_TYPE.ALUMINIUM, -weapon.requiredAluminium);
            country.AddMineral(MINERAL_TYPE.URANIUM, -weapon.requiredUranium);
            country.AddMineral(MINERAL_TYPE.OIL, -weapon.requiredOil);

            return true;
        }

        public int GetWeaponProductionDay(Country country, WeaponTemplate weapon)
        {
            float productionSpeed = 0;

            if (weapon.weaponTerrainType == 1)
                productionSpeed = country.Land_Production_Speed;

            if (weapon.weaponTerrainType == 2)
                productionSpeed = country.Naval_Production_Speed;

            if (weapon.weaponTerrainType == 3 || weapon.weaponTerrainType == 4)
                productionSpeed = country.Air_Production_Speed;

            int weaponProductionTime = (int)(weapon.weaponProductionTime - ((weapon.weaponProductionTime * productionSpeed) / 100.0f));
            if (weaponProductionTime < 1)
                weaponProductionTime = 1;

            return weaponProductionTime;
        }

        public void UpdateProductionInProgress(Country country)
        {
            //productionProgress = productionProgress.Where(i => i != null).ToList();

            foreach (Production production in country.GetAllProductionsInProgress().ToList())
            {
                if (production.IsCompleted() == false)
                {
                    production.UpdateProduction();
                }
                else
                {
                    if(country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                        HUDManager.Instance.PrivateNotification(production.techWeapon.weaponName + " has been produced");

                    country.GetArmy().AddWeaponToMilitaryForces(production.techWeapon, production.number);
                    country.GetAllProductionsInProgress().Remove(production);
                }
            }
        }

        public void HideShowAllBuildings(Country country, bool show)
        {
            foreach (City city in GetAllCitiesInCountry(country))
            {
                if (city.Dockyard != null)
                {
                    city.Dockyard.visible = show;
                    city.Dockyard.enabled = show;
                }

                if (city.Capital_Building != null)
                {
                    city.Capital_Building.visible = show;
                    city.Capital_Building.enabled = show;
                }
            }

            country.allBuildingsVisible = show;
        }

        public void AddMilitaryAccess(Country country, Country tempCountry, int militartAccessAsDay)
        {
            if (country.GetMilitaryAccess().ContainsKey(tempCountry) == false)
            {
                country.GetMilitaryAccess().Add(tempCountry, militartAccessAsDay);
            }
            else
            {
                country.GetMilitaryAccess()[tempCountry] += militartAccessAsDay;
            }
        }

        public void CancelMilitaryAccess(Country country, Country tempCountry)
        {
            if (country.GetMilitaryAccess().ContainsKey(tempCountry) == true)
            {
                country.GetMilitaryAccess()[tempCountry] = 0;
                NotificationManager.Instance.CreatePrivateNotification(country.name + " has canceled the military access of " + tempCountry.name);
            }
        }
        public int GetLeftMilitaryAccess(Country country1, Country country2)
        {
            if (country1 == null)
                return 0;

            if (country2 == null)
                return 0;

            if (country1.GetMilitaryAccess().ContainsKey(country2) == true)
            {
                int leftMilitaryAccess = 0;

                country1.GetMilitaryAccess().TryGetValue(country2, out leftMilitaryAccess);

                return leftMilitaryAccess;
            }
            return 0;
        }

        public void DecreaseMilitaryAccess(Country country)
        {
            foreach (var pair in country.GetMilitaryAccess())
            {
                country.GetMilitaryAccess()[pair.Key] -= 1;

                if (country.GetMilitaryAccess()[pair.Key] <= 0)
                {
                    country.GetMilitaryAccess().Remove(pair.Key);
                    NotificationManager.Instance.CreatePrivateNotification(country.name + " finished military access for " + pair.Key.name);
                }
            }
        }

        /*
        public bool IsHasMilitaryAccess(Country country_1, Country country_2)
        {
            if (country_1.GetMilitaryAccess().ContainsKey(country_2))
                return true;
            else
                return false;
        }
        */
        public List<City> GetCityListByPopulation(Country country, int minPopulation)
        {
            List<City> cityList = new List<City>();

            foreach (City city in GetAllCitiesInCountry(country))
                if (city.population > minPopulation)
                    cityList.Add(city);

            return cityList;
        }

        public List<City> GetSortedCityListByPopulation(Country country)
        {
            List<City> cityList = new List<City>();

            cityList = GetAllCitiesInCountry(country).OrderBy(x => x.population).ToList();
            cityList.Reverse();
            return cityList;
        }

        public int GetEmptyGarrisonNumberInCountry(Country country)
        { 
            int garrison = 0;
            foreach (City city in GetAllCitiesInCountry(country))
                garrison += city.GetEmptyGarrison();
            return garrison;
        }

        public City GetRandomEmptyGarrisonInCountry(Country country)
        {
            if (GetEmptyGarrisonNumberInCountry(country) == 0)
                return null;

            while(true)
            {
                City city = map.GetCityRandom(country);

                if (city.GetEmptyGarrison() > 0)
                    return city;
            }
        }

        public List<City> GetCitiesHaveBuilding(Country country, BUILDING_TYPE buildingType)
        {
            List<City> cityList = new List<City>();

            foreach(City city in GetAllCitiesInCountry(country))
            {
                if (city.GetBuildingNumber(buildingType) > 0)
                    cityList.Add(city);
            }
            return cityList;
        }

        public int GetTotalBuildings(Country country, BUILDING_TYPE building)
        {
            int totalBuildings = 0;

            foreach (City city in GetAllCitiesInCountry(country))
            {
                int number = 0;
                city.GetAllBuildings().TryGetValue(building, out number);
                totalBuildings += number;
            }
            return totalBuildings;
        }

        public void RemoveBuildingsInCity(City city, int perc)
        {
            Dictionary<BUILDING_TYPE, int> allBuildings = city.GetAllBuildings();

            foreach (var building in allBuildings.ToArray())
            {
                int newBuildingNumber = building.Value - (building.Value * perc)/100;
                allBuildings[building.Key] = newBuildingNumber;
            }
        }

        public void AcceptPolicy(Country country, Policy policy)
        {
            int leftDefenseBudget = country.Defense_Budget - policy.requiredDefenseBudget;
            long leftBudget = country.Budget - policy.costPermenant;

            country.Defense_Budget = leftDefenseBudget;
            country.Budget = leftBudget;

            country.AddPolicy(policy);
        }

        public bool IsPolicyAcceptable(Country country, Policy policy)
        {
            if (country.GetArmy() != null)
            {
                if (country.Defense_Budget < policy.requiredDefenseBudget)
                {
                    //Debug.Log("Defense Budget is not enough");
                    return false;
                }
            }

            if (country.Budget < policy.costPermenant)
            {
                //Debug.Log("Budget is not enough");
                return false;
            }

            if (country.GetAcceptedPolicyList().Contains(policy) == true)
            {
                //Debug.Log("You already have it");
                return false;
            }

            return true;
        }

        public float GetGDPTradeBonus(Country country)
        {
            int gdp = GetTotalGDPInWorld();

            if (gdp == 0)
                return 0;
            return (country.Current_GDP * 100.0f) / gdp;
        }

        public float GetTotalTradeBonus(Country country)
        {
            float totalTradeBonus = 0;
            float GDPBonus = GetGDPTradeBonus(country);

            foreach (Organization organization in OrganizationManager.Instance.GetAllOrganizations())
            {
                if (organization.isTrade)
                {
                    if (organization.isFullMemberCountry(country))
                        totalTradeBonus += (organization.GetTradeBonus() - GDPBonus);
                }
            }

            foreach (Country embargo in country.GetTradeEmbargo())
            {
                totalTradeBonus -= GetGDPTradeBonus(embargo);
            }
            foreach (Country embargo in country.GetTradeTreaty())
            {
                totalTradeBonus += GetGDPTradeBonus(embargo);
            }
            foreach (Policy policy in country.GetAcceptedPolicyList())
            {
                totalTradeBonus += policy.GetValue(TRAIT.GDP_BONUS);
            }

            return totalTradeBonus;
        }

        public void UpdateEconomy(Country country)
        {
            int taxIncome = GetIndividualTaxIncomeMonthly(country);
            int cityIncome = 0;

            List<City> cityList = GetAllCitiesInCountry(country);

            foreach (City city in cityList)
            {
                int tradePortIncome = city.GetTradePortIncome();
                int factoryIncome = city.GetFactoryIncome();

                cityIncome = cityIncome + tradePortIncome + factoryIncome;
            }

            float tradeBonus = GetTotalTradeBonus(country);
            int export = (int)((cityIncome * tradeBonus) / 100.0f);

            country.Budget += export;
            country.Budget += taxIncome;

            if(country.Budget > 0)
                PayDebt(country);

            country.Current_GDP = country.Current_GDP + cityIncome + export + taxIncome;
        }

        public int GetTotalGDPInWorld()
        {
            int totalGDP = 0;

            foreach (Country country in map.countries)
            {
                if (country.Current_GDP == 0)
                    totalGDP += country.Previous_GDP;
                else
                    totalGDP += country.Current_GDP;
            }

            return totalGDP;
        }

        public int GetIndividualTaxIncomeMonthly(Country country)
        {
            ulong countryTaxIncome = ((ulong)GetAvailableManpower(country) * (ulong)country.Individual_Tax) / 1000000;

            if (countryTaxIncome < 1)
                countryTaxIncome = 1;

            return (int)countryTaxIncome;
        }

        public void AddMineralIncome(Country country, int income)
        {
            country.Mineral_Income += income;
        }

        public RELIGION GetReligionByReligionName(string religionName)
        {
            if (religionName == "Muslim")
                return RELIGION.MUSLIM;
            if (religionName == "Christian")
                return RELIGION.CHRISTIAN;
            if (religionName == "Irreligion")
                return RELIGION.IRRELIGION;
            if (religionName == "Hindu")
                return RELIGION.HINDU;
            if (religionName == "Buddist")
                return RELIGION.BUDDIST;
            if (religionName == "Folk Religion")
                return RELIGION.FOLK_RELIGION;
            if (religionName == "Jewish")
                return RELIGION.JEWISH;

            return RELIGION.NONE;
        }
        public string GetReligionNameByReligionType(RELIGION religionType)
        {
            if (religionType == RELIGION.MUSLIM)
                return "Muslim";
            if (religionType == RELIGION.CHRISTIAN)
                return "Christian";
            if (religionType == RELIGION.BUDDIST)
                return "Buddist";
            if (religionType == RELIGION.FOLK_RELIGION)
                return "Folk Religion";
            if (religionType == RELIGION.HINDU)
                return "Hindu";
            if (religionType == RELIGION.JEWISH)
                return "Jewish";
            if (religionType == RELIGION.IRRELIGION)
                return "Irreligion";

            return string.Empty;
        }

        public void UpdateResourcesInCountry(Country country)
        {
            if(country != null)
            {
                List<City> cityList = GetAllCitiesInCountry(country);

                foreach (City city in cityList)
                {
                    int mineralFactory = city.GetBuildingNumber(BUILDING_TYPE.MINERAL_FACTORY);
                    int oilRafinery = city.GetBuildingNumber(BUILDING_TYPE.OIL_RAFINERY);

                    if (mineralFactory > 0)
                    {
                        country.AddMineral(MINERAL_TYPE.ALUMINIUM, city.GetMineralResources(MINERAL_TYPE.ALUMINIUM) * mineralFactory);
                        country.AddMineral(MINERAL_TYPE.IRON, city.GetMineralResources(MINERAL_TYPE.IRON) * mineralFactory);
                        country.AddMineral(MINERAL_TYPE.STEEL, city.GetMineralResources(MINERAL_TYPE.STEEL) * mineralFactory);
                        country.AddMineral(MINERAL_TYPE.URANIUM, city.GetMineralResources(MINERAL_TYPE.URANIUM) * mineralFactory);
                    }

                    if (oilRafinery > 0)
                    {
                        country.AddMineral(MINERAL_TYPE.OIL, city.GetMineralResources(MINERAL_TYPE.OIL) * oilRafinery);
                    }
                }
            }           
        }

        public void UpdateAllConstructionInCountry(Country country)
        {
            List<City> cityList = GetAllCitiesInCountry(country);

            foreach (City city in cityList)
            {
                Dictionary<BUILDING_TYPE, int> buildingList = city.GetAllBuildingsInConstruction();

                foreach (var buildings in buildingList.ToArray())
                {
                    int leftDay = buildings.Value - 1;
                    buildingList[buildings.Key] = leftDay;

                    if (leftDay == 0)
                    {
                        city.AddBuilding(buildings.Key, 1);

                        city.GetAllBuildingsInConstruction().Remove(buildings.Key);
                    }
                }
            }
        }

        public void CheckResearch(Country country)
        {
            List<WeaponTemplate> tempWeaponList = new List<WeaponTemplate>();

            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                if (country.IsWeaponProducible(weapon.weaponID) == false && (GameEventHandler.Instance.GetCurrentYear() - weapon.weaponResearchYear) > 30)
                    tempWeaponList.Add(weapon);
            }

            if (tempWeaponList.Count > 0)
            {
                foreach (WeaponTemplate weapon in tempWeaponList)
                {
                    if (weapon.weaponTerrainType == 2 && IsWeaponAvailableForResearch(country, weapon))
                        ResearchWeapon(country, weapon);
                    else
                    {
                        if (weapon.weaponTerrainType == 1 && IsWeaponAvailableForResearch(country, weapon))
                        {
                            ResearchWeapon(country, weapon);
                        }
                        else if (weapon.weaponTerrainType == 3 && IsWeaponAvailableForResearch(country, weapon))
                        {
                            ResearchWeapon(country, weapon);
                        }
                    }
                }
            }
        }

        public Research ResearchWeapon(Country country, WeaponTemplate weapon)
        {
            if (country.IsWeaponProducible(weapon.weaponID) == false && IsWeaponResearchInProgress(country, weapon) == false)
            {
                Research research = new Research();
                research.techWeapon = weapon;
                research.researchCountries.Add(country);
                research.leftDays = weapon.weaponResearchTime;
                research.totalResearchDay = weapon.weaponResearchTime;

                country.Defense_Budget -= weapon.weaponResearchCost;
                country.GetAllResearchsInProgress().Add(research);

                HUDManager.Instance.PrivateNotification("Research has started " + weapon.weaponName);

                return research;
            }
            else
            {
                HUDManager.Instance.PrivateNotification("You cannot research " + weapon.weaponName + " !...");

                return null;
            }
        }

        public void UpdateNatality(Country country)
        {
            foreach (City city in GetAllCitiesInCountry(country))
            {
                city.population += (int)((city.population * country.Fertility_Rate_PerWeek) / 5000);
                city.population -= (int)((city.population * country.Pandemic_Death_Rate_Monthly) / 10000);

                if (city.population < 0)
                    city.population = 0;
            }
        }

        public void UpdateResearchInProgress(Country country)
        {
            foreach (Research research in country.GetAllResearchsInProgress())
            {
                research.UpdateResearch();
            }
        }

        public bool IsWeaponResearchInProgress(Country country, WeaponTemplate weapon)
        {
            foreach (Research research in country.GetAllResearchsInProgress())
            {
                if (research.techWeapon == weapon)
                    return true;
            }

            return false;
        }

        public bool IsWeaponAvailableForResearch(Country country, WeaponTemplate weapon)
        {
            if (weapon.weaponTerrainType == 2)
            {
                return UpdateMaximumDockyardAreaInCountry() > 0 && country.IsWeaponProducible(weapon.weaponID) && country.Defense_Budget >= weapon.weaponResearchCost;
            }
            else
            {
                return country.IsWeaponProducible(weapon.weaponID) && country.Defense_Budget >= weapon.weaponResearchCost;
            }
        }

        public int UpdateMaximumDockyardAreaInCountry()
        {
            int totalDockyardArea = 0;

            foreach (City city in map.GetCities())
            {
                if (city.Constructible_Dockyard_Area > 0)
                    totalDockyardArea++;
            }

            return totalDockyardArea;
        }
        
        public void PayDebt(Country country)
        {
            if ( country.Debt > 0)
            {              
                if ( country.Budget >= country.Debt_Payment )
                {
                    country.Debt -= country.Debt_Payment;
                    country.Budget -= country.Debt_Payment;
                }
            }
        }
        
        public int CalculateCountryPerCapitaIncome(Country country)
        {
            int manpower = GetAvailableManpower(country);
            int perCapitaIncome = 0;

            if (manpower > 0)
            {
                if (country.Previous_GDP > 0)
                {
                    ulong gdp = (ulong)country.Previous_GDP * 1000000;

                    ulong total = gdp / (ulong)manpower;

                    if (total < 0)
                        total = 0;

                    perCapitaIncome = (int)total;
                    country.Individual_Tax = (perCapitaIncome * country.Tax_Rate) / 100;
                }
                else
                {
                    perCapitaIncome = 0;
                    country.Individual_Tax = 0;
                }
            }
            else
            {
                perCapitaIncome = 0;
                country.Individual_Tax = 0;
            }

            return perCapitaIncome;
            //Debug.Log(country.name + " -> " + manpower);
            //Debug.Log(country.name + " -> " + manpower + "    -> Previous_GDP_per_Capita : " + country.Previous_GDP_per_Capita + "   Individual_Tax -> " + country.Individual_Tax);
        }

        public List<City> GetCityListByPopulation(Country country)
        {
            List<City> orderedCity = GetAllCitiesInCountry(country).OrderBy(city => city.population).ToList();

            orderedCity.Reverse();

            return orderedCity;
        }

        public List<City> GetAllCitiesInCountry(Country country)
        {
            return map.GetCities(country);
        }

        public int GetAvailableManpower(Country country)
        {
            int availableManpower = 0;
            List<City> cityList = GetAllCitiesInCountry(country);

            foreach (City city in cityList)
                availableManpower += city.population;

            return availableManpower;
        }

        public void CreateIntelligenceAgency(Country country, string name, int level, int budget, Texture2D flag)
        {
            IntelligenceAgency intelligenceAgency = new IntelligenceAgency();
            intelligenceAgency.IntelligenceAgencyName = name;
            intelligenceAgency.IntelligenceAgencyBudget = budget;

            intelligenceAgency.ReverseEnginering = level * 5;
            intelligenceAgency.Assassination = level * 5;
            intelligenceAgency.MilitaryCoup = level * 5;

            country.Intelligence_Agency = intelligenceAgency;
        }

        public void RemoveEnemyCountryAtWar(Country country, Country enemy)
        {
            foreach (War war in country.GetWarList().ToArray())
                if (war.GetEnemyCountry(country) == enemy)
                    country.GetWarList().Remove(war);
        }

        public List<Country> GetAtWarCountryList(Country country)
        {
            if (country.GetWarList() == null)
                return null;

            List<Country> temp = new List<Country>();

            foreach (War war in country.GetWarList())
                temp.Add(war.GetEnemyCountry(country));

            return temp;
        }

        public War GetWarCountry(Country country, Country enemy)
        {
            if (country.GetWarList() == null)
                return null;

            foreach (War war in country.GetWarList())
                if (war.GetEnemyCountry(country) == enemy)
                    return war;

            return null;
        }

        public List<Country> GetAllAllies(Country country)
        {
            List<Country> temp = new List<Country>();

            foreach (Country tempCountry in map.countries)
            {
                if (tempCountry != country && country.GetRelation(tempCountry.name) >= 50)
                {
                    temp.Add(tempCountry);
                }
            }
            return temp;
        }
        public List<Country> GetAllEnemies(Country country)
        {
            List<Country> temp = new List<Country>();

            foreach (Country tempCountry in map.countries)
            {
                if (tempCountry != country && country.GetRelation(tempCountry.name) <= -50)
                {
                    temp.Add(tempCountry);
                }
            }
            return temp;
        }

        public int GetAllEnemiesPower(Country country)
        {
            int power = 0;

            foreach (Country tempCountry in GetAllEnemies(country))
            {
                if (tempCountry.GetArmy() != null)
                    power += tempCountry.GetArmy().GetArmyPower();
            }
            return power;
        }

        public void SignPeaceOfTreaty(Country requestCountry, Country targetCountry)
        {
            requestCountry.SetRelations(targetCountry, -50);

            RemoveEnemyCountryAtWar(requestCountry, targetCountry);
            RemoveEnemyCountryAtWar(targetCountry, requestCountry);

            DivisionManager.Instance.RandomPositionDivisionsInCountry(requestCountry, targetCountry);
            DivisionManager.Instance.RandomPositionDivisionsInCountry(targetCountry, requestCountry);
        }

        public void VisibleAllDivisions(Country country, bool show)
        {
            if (country.GetArmy() == null)
                return;

            country.allDivisionsVisible = show;
            foreach (GameObjectAnimator division in country.GetArmy().GetAllDivisionInArmy())
            {
                if (division != null)
                {
                    division.enabled = show;
                    division.visible = show;
                }
            }
        }

        public int GetTotalDockyard(Country country)
        {
            int totalDockyard = 0;
            foreach (City city in GetAllCitiesInCountry(country))
                if (city.Dockyard != null)
                    totalDockyard += 1;

            return totalDockyard;
        }

    }
}
