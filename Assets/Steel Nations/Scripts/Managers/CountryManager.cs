using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class CountryManager : Singleton<CountryManager>
    {
        List<Country> countryList = new List<Country>();
        WMSK map = WMSK.instance;

        public void AddCountry(Country country)
        {
            if (countryList.Contains(country) == false)
                countryList.Add(country);
        }
        public List<Country> GetAllCountries()
        {
            return countryList;
        }

        public List<Country> GetAllCountriesWhichHaveArmy(bool includedMyCountry)
        {
            List<Country> countryHaveArmyList = new List<Country>();

            foreach (Country country in GetAllCountries())
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
            foreach (Country country in countryList)
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
        public void MonthlyUpdateForAllCountries()
        {
            UpdateResources();
            UpdateEconomy();
            UpdateBirthRate();
        }

        void UpdateResources()
        {
            foreach (Country tempCountry in countryList)
            {
                if (tempCountry != null)
                {
                    UpdateResourcesInCountry(tempCountry);
                }
            }
        }
        void UpdateEconomy()
        {
            foreach (Country country in countryList)
            {
                if (country != null)
                {
                    UpdateEconomy(country);
                }
            }
        }

        void UpdateBirthRate()
        {
            foreach (Country tempCountry in countryList)
            {
                if (tempCountry != null)
                {
                    UpdateNatality(tempCountry);
                }
            }
        }

        public bool IsCountryDefeated()
        {

            return false;
        }

        public void CountryAI()
        {
            foreach (Country country in countryList.ToList())
            {
                if (country == null)
                    countryList.Remove(country);
                else
                {
                    if (IsCountryDefeated() == true)
                    {
                        if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                        {
                            // game over
                        }
                    }
                    else
                    {
                        DecreaseMilitaryAccess(country);
                        UpdateAllConstructionInCountry(country);
                        UpdateResearchInProgress(country);
                        UpdateProductionInProgress(country);
                        ActionManager.Instance.UpdateActionsInProgress(country);

                        int random = Random.Range(0, 100);
                        int enemyNumber = GetAllEnemies(country).Count;
                        /*
                        if (random <= enemyNumber)
                        {
                            ActionManager.Instance.CreateAction(country, GetAllEnemies(country)[random], ACTION_TYPE.Declare_War, MINERAL_TYPE.NONE,
                            0,
                            null,
                            0,
                            null,
                            null,
                            0,
                            0,
                            0);
                        }
                        */
                        if (country.GetWarList().Count > 0)
                        {
                            WarManager.Instance.WarStrategy(country);
                        }
                        else
                        {
                            if (enemyNumber > 0)
                            {

                            }
                            else
                            {

                            }
                        }
                    }
                }

            }
        }

        public int GetWeaponPrice(Country country1, Country country2, WeaponTemplate template, int amount)
        {
            int weaponCost = (template.weaponCost * amount) - ((template.weaponCost * amount) * country2.attrib[country1.name]) / 100;

            return weaponCost;
        }

        public Production ProductWeapon(Country country, WeaponTemplate weapon)
        {
            if (country.GetArmy().Defense_Budget >= weapon.weaponCost)
            {
                if (country.GetMineral(MINERAL_TYPE.IRON) >= weapon.requiredIron &&
                country.GetMineral(MINERAL_TYPE.STEEL) >= weapon.requiredSteel &&
                country.GetMineral(MINERAL_TYPE.ALUMINIUM) >= weapon.requiredAluminium)
                {
                    country.GetArmy().Defense_Budget = country.GetArmy().Defense_Budget - weapon.weaponCost;

                    country.AddMineral(MINERAL_TYPE.IRON, -weapon.requiredIron);
                    country.AddMineral(MINERAL_TYPE.STEEL, -weapon.requiredSteel);
                    country.AddMineral(MINERAL_TYPE.ALUMINIUM, -weapon.requiredAluminium);

                    int researchSpeedRotio = (GetTotalBuildings(country, BUILDING_TYPE.MILITARY_FACTORY) / 10) + country.Production_Speed;

                    int weaponProductionTime = (weapon.weaponProductionTime - ((weapon.weaponProductionTime * researchSpeedRotio) / 100));
                    if (weaponProductionTime <= 1)
                        weaponProductionTime = 1;

                    Production production = new Production();
                    production.techWeapon = weapon;
                    production.productionCountries.Add(country);
                    production.leftDays = weapon.weaponProductionTime;

                    country.GetAllProductionsInProgress().Add(production);

                    if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                        HUDManager.Instance.PrivateNotification("Production has started " + weapon.weaponName);

                    return production;
                }
                else
                {
                    if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                        HUDManager.Instance.PrivateNotification("Resources are not enough for " + weapon.weaponName);

                    return null;
                }

            }
            else
            {
                HUDManager.Instance.PrivateNotification("Not enough budget for " + weapon.weaponName);

                return null;
            }
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
                    HUDManager.Instance.PrivateNotification("Production has been completed " + production.techWeapon.weaponName);
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
            if (country1.GetMilitaryAccess().ContainsKey(country2) == true)
            {
                int leftMilitaryAccess = 0;

                country1.GetMilitaryAccess().TryGetValue(country2, out leftMilitaryAccess);

                return leftMilitaryAccess;
            }
            return 0;
        }

        public void SetStartingResources(int multipler)
        {
            foreach (Country country in GetAllCountries())
            {
                country.AddMineral(MINERAL_TYPE.OIL, multipler * country.GetMineral(MINERAL_TYPE.OIL));
                country.AddMineral(MINERAL_TYPE.IRON, multipler * country.GetMineral(MINERAL_TYPE.IRON));
                country.AddMineral(MINERAL_TYPE.ALUMINIUM, multipler * country.GetMineral(MINERAL_TYPE.ALUMINIUM));
                country.AddMineral(MINERAL_TYPE.URANIUM, multipler * country.GetMineral(MINERAL_TYPE.URANIUM));
                country.AddMineral(MINERAL_TYPE.STEEL, multipler * country.GetMineral(MINERAL_TYPE.STEEL));
            }
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

        #region Nuclear War
        public void BeginNuclearWar(Country country_1, Country country_2)
        {
            WarManager.Instance.BeginNuclearWar(country_1, country_2);
            NotificationManager.Instance.CreatePublicNotification(country_1.name + " begun nuclear war against to " + country_2.name);
        }
        #endregion

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

        public float GetGDPTradeBonus(Country country)
        {
            int gdp = GetTotalGDPInWorld();

            if (gdp == 0)
                return 0;
            return (country.Current_GDP * 100) / gdp;
        }

        public int GetTotalTradeBonus(Country country)
        {
            float totalTradeBonus = 0;

            float organizationTradeBonus = 0;
            float politiksTradeBonus = 0;

            foreach (Organization organization in OrganizationManager.Instance.GetAllOrganizations())
            {
                if (organization.isTrade)
                {
                    if (organization.isFullMemberCountry(country))
                        organizationTradeBonus = organizationTradeBonus + organization.GetTradeBonus();
                }
            }

            foreach (Country embargo in country.GetTradeEmbargo())
            {
                organizationTradeBonus = organizationTradeBonus - GetGDPTradeBonus(embargo);
            }
            foreach (Country embargo in country.GetTradeTreaty())
            {
                organizationTradeBonus = organizationTradeBonus + GetGDPTradeBonus(embargo) * 10;
            }
            foreach (Policy policy in country.GetAcceptedPolicyList())
            {
                politiksTradeBonus = politiksTradeBonus + policy.tradeBonus;
            }

            totalTradeBonus = organizationTradeBonus + politiksTradeBonus;

            return (int)totalTradeBonus;
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

            int tradeBonus = GetTotalTradeBonus(country);
            int export = (cityIncome * tradeBonus) / 100;

            country.Current_GDP = country.Current_GDP + cityIncome + export + taxIncome;

            Debug.Log(country.name + " -> " + country.Current_GDP + "   City Income -> " + cityIncome + "   Tax Income -> " + taxIncome + "   tradeBonus -> " + tradeBonus);
        }

        public int GetTotalGDPInWorld()
        {
            int totalGDP = 0;

            foreach (Country country in countryList)
            {
                totalGDP = totalGDP + country.Current_GDP;
            }

            return totalGDP;
        }

        public int GetIndividualTaxIncomeMonthly(Country country)
        {
            ulong countryTaxIncome = ((ulong)GetAvailableManpower(country) * (ulong)country.Individual_Tax) / 1000000;
            //Debug.Log("Individual_Tax -> " + country.Individual_Tax + "   Country_Tax_Income -> " + countryTaxIncome);
            if (countryTaxIncome < 1)
                countryTaxIncome = 1;

            return (int)countryTaxIncome;
        }

        public void AddMineralIncome(Country country, int income)
        {
            country.Mineral_Income += income;
        }

        public void SetReligion(Country country, int christian, int muslim, int irreligion, int hindu, int buddist, int folk, int jewish, string religion)
        {
            /*
            country.attrib["Christian"] = christian;
            country.attrib["Muslim"] = muslim;
            country.attrib["Irreligion"] = irreligion;
            country.attrib["Hindu"] = hindu;
            country.attrib["Buddist"] = buddist;
            country.attrib["Folk Religion"] = folk;
            country.attrib["Jewish"] = jewish;
            */
            country.Religion = religion;
        }

        public void UpdateResourcesInCountry(Country country)
        {
            List<City> cityList = GetAllCitiesInCountry(country);

            foreach (City city in cityList)
            {
                int mineralFactory = city.GetBuildingNumber(BUILDING_TYPE.MINERAL_FACTORY);

                if (mineralFactory > 0)
                {
                    country.AddMineral(MINERAL_TYPE.ALUMINIUM, city.GetMineralResources(MINERAL_TYPE.ALUMINIUM) * mineralFactory);
                    country.AddMineral(MINERAL_TYPE.IRON, city.GetMineralResources(MINERAL_TYPE.IRON) * mineralFactory);
                    country.AddMineral(MINERAL_TYPE.STEEL, city.GetMineralResources(MINERAL_TYPE.STEEL) * mineralFactory);
                    country.AddMineral(MINERAL_TYPE.URANIUM, city.GetMineralResources(MINERAL_TYPE.URANIUM) * mineralFactory);
                }

                int oilRafinery = city.GetBuildingNumber(BUILDING_TYPE.OIL_RAFINERY);

                if (oilRafinery > 0)
                {
                    country.AddMineral(MINERAL_TYPE.OIL, city.GetMineralResources(MINERAL_TYPE.OIL) * oilRafinery);
                }
            }
        }

        public void UpdateAllConstructionInCountry(Country country)
        {
            foreach (City city in GetAllCitiesInCountry(country))
            {
                //city.UpdateConstructionInCity();
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

                country.GetArmy().Defense_Budget = country.GetArmy().Defense_Budget - weapon.weaponResearchCost;
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
                city.population = city.population + ((city.population * country.attrib["Country Natality"]) / 5000);
                city.population = city.population - ((city.population * country.attrib["Corona"]) / 10000);
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
                return UpdateMaximumDockyardAreaInCountry() > 0 && country.IsWeaponProducible(weapon.weaponID) && country.GetArmy().Defense_Budget >= weapon.weaponResearchCost;
            }
            else
            {
                return country.IsWeaponProducible(weapon.weaponID) && country.GetArmy().Defense_Budget >= weapon.weaponResearchCost;
            }
        }

        public int UpdateMaximumDockyardAreaInCountry()
        {
            int totalDockyardArea = 0;

            foreach (City city in map.GetCities())
            {
                if (city.Constructible_Dockyard_Area > 0)
                    totalDockyardArea = totalDockyardArea + 1;
            }

            return totalDockyardArea;
        }
        /*
        public void PayDebt(Country country)
        {
            if (country.Debt >= country.attrib["Debt Per Week"] && country.Debt >= 0)
            {
                country.attrib["Debt"] = country.attrib["Debt"] - country.attrib["Debt Per Week"];

                if (country.attrib["Debt"] <= 0)
                    country.attrib["Debt"] = 0;

                country.attrib["Debt"] = country.attrib["Debt"] - country.attrib["Debt Per Week"];
            }
        }
        */
        public void CalculateCountryPerCapitaIncome(Country country)
        {
            int manpower = GetAvailableManpower(country);

            if (manpower > 0)
            {
                if (country.Previous_GDP > 0)
                {
                    ulong gdp = (ulong)country.Previous_GDP * 1000000;
                    ulong total = gdp / (ulong)manpower;

                    if (total < 0)
                        total = 0;
                    country.Previous_GDP_per_Capita = (int)total;
                    country.Individual_Tax = country.Previous_GDP_per_Capita / 100;
                }
                else
                {
                    country.Previous_GDP_per_Capita = 1;
                    country.Individual_Tax = 1;
                }
            }
            else
            {
                country.Previous_GDP_per_Capita = 0;
                country.Individual_Tax = 0;
            }

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
            if (map == null)
                map = WMSK.instance;

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
            intelligenceAgency.IntelligenceAgencyLevel = level;
            intelligenceAgency.IntelligenceAgencyBudget = budget;

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
        public List<Country> GetAllAllies(Country country)
        {
            List<Country> temp = new List<Country>();

            foreach (Country tempCountry in GetAllCountries())
            {
                if (tempCountry != country && country.attrib[tempCountry.name] >= 50)
                {
                    temp.Add(tempCountry);
                }
            }
            return temp;
        }
        public List<Country> GetAllEnemies(Country country)
        {
            List<Country> temp = new List<Country>();

            foreach (Country tempCountry in GetAllCountries())
            {
                if (tempCountry != country && country.attrib[tempCountry.name] <= -50)
                {
                    temp.Add(tempCountry);
                }
            }
            return temp;
        }

        public void SignPeaceOfTreaty(Country requestCountry, Country targetCountry)
        {
            UpdateRelation(requestCountry, targetCountry, -50);

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
        public void UpdateRelation(Country country_1, Country country_2, int relation)
        {
            country_1.attrib[country_2.name] = relation;
            country_2.attrib[country_1.name] = relation;
        }
        public void AddRelation(Country country_1, Country country_2, int relation)
        {
            country_1.attrib[country_2.name] += relation;
            country_2.attrib[country_1.name] += relation;
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
