using UnityEngine;
using System.Collections.Generic;
using System;

namespace WorldMapStrategyKit {
	public partial class Country: AdminEntity {

		Army army = null;
        // -100/-50 = at war
        // -50/-25 = enemy
        // -25/25 = neutral
        // 25/50 = ally
        // 50/100 = friend

        public bool allDivisionsCreated = false;
        public bool allDivisionsVisible = false;

        public bool allBuildingsVisible = false;

        int previousGDPPerCapita = 0;

        IntelligenceAgency intelligenceAgency;

        List<Action> actionList = new List<Action>();
        List<War> atWarList = new List<War>();

        List<Person> ministerList = new List<Person>();

        List<Country> tradeTreaty = new List<Country>();

        List<Country> tradeEmbargo = new List<Country>();
        List<Country> armsEmbargo = new List<Country>();

        List<int> producibleWeapons = new List<int>();
        List<Policy> acceptedPolicyList = new List<Policy>();

        Texture2D countryFlag;

        List<Research> researchProgress = new List<Research>();
        List<Production> productionProgress = new List<Production>();

        Dictionary<WEAPON_TYPE, int> weaponTech = new Dictionary<WEAPON_TYPE, int>();
        Dictionary<MINERAL_TYPE, int> mineral = new Dictionary<MINERAL_TYPE, int>();

        Dictionary<Country, int> militaryAccess = new Dictionary<Country, int>();

        Person president;
        Person vicePresident;

        Color countryColor;
        float intelligenceBudgetByGDP;
        float defenseBudgetByGDP;
        int taxRate;

        string religion;
        int tension;
        float unemploymentRate;
        int productionSpeed;
        int researchSpeed;
        float fertilityRate;
        float pandemicDeadRateMonthly;
        string systemOfGovernment;
        int debt;
        int militaryRank;
        int tax;
        int currentGDP;
        int previousGDP;
        int previousGDPMonthly;
        float currentGDPAnnualGrowthRate;
        long budget;
        bool nuclearPower;
        int oilIncome;
        int gunIncome;
        int mineralIncome;

        public Person President
        {
            get { return president; }
            set { president = value; }
        }

        public Person VicePresident
        {
            get { return vicePresident; }
            set { vicePresident = value; }
        }

        public Color SurfaceColor
        {
            get { return countryColor; }
            set { countryColor = value; }
        }

        public void AddTradeTreaty(Country country)
        {
            tradeTreaty.Add(country);
        }

        public List<Country> GetTradeTreaty()
        {
            return tradeTreaty;
        }

        public float Intelligence_Agency_Budget_By_GDP
        {
            get { return intelligenceBudgetByGDP; }
            set { intelligenceBudgetByGDP = value; }
        }

        public float Defense_Budget_By_GDP
        {
            get { return defenseBudgetByGDP; }
            set { defenseBudgetByGDP = value; }
        }

        public int Previous_GDP_per_Capita
        {
            get { return previousGDPPerCapita; }
            set { previousGDPPerCapita = value; }
        }

        public string Religion
        {
            get { return religion; }
            set { religion = value; }
        }

        #region Arm Embargo
        public void PlaceArmsEmbargo(Country tempCountry)
        {
            armsEmbargo.Add(tempCountry);
        }
        #endregion

        #region Trade Embargo
        // Countries that have an economic embargo on you
        public void PlaceTradeEmbargo(Country tempCountry)
        {
            tradeEmbargo.Add(tempCountry);
        }
        #endregion

        public List<Country> GetTradeEmbargo()
        {
            return tradeEmbargo;
        }

        public List<Country> GetArmsEmbargo()
        {
            return armsEmbargo;
        }


        public void AddMinister(Person person)
        {
            ministerList.Add(person);
        }

        public void AddMineral(MINERAL_TYPE mineralType, int number)
        {
            if (mineral.ContainsKey(mineralType))
                mineral[mineralType] = number;
            else
                mineral.Add(mineralType, number);
        }
        public int GetMineral(MINERAL_TYPE mineralType)
        {
            int number = 0;
            mineral.TryGetValue(mineralType, out number);

            return number;
        }

        public void SetWeaponTech(WEAPON_TYPE weaponType, int techLevel)
        {
            if(weaponType != WEAPON_TYPE.NONE)
                weaponTech.Add(weaponType, techLevel);
        }
        public int GetWeaponTech(WEAPON_TYPE weaponType)
        {
            int tech = 0;
            weaponTech.TryGetValue(weaponType, out tech);

            return tech;
        }

        public bool CountryIsContainsInActionList(Country country, ACTION_TYPE actionType)
        {
            foreach (Action action in actionList)
                if (action.Country == country && action.ActionType == actionType)
                    return true;
            return false;
        }
        public List<Action> GetActionList()
        {
            return actionList;
        }
        public void AddAction(Action action)
        {
            actionList.Add(action);
        }
        public void RemoveAction(Action action)
        {
            if (actionList.Contains(action))
                actionList.Remove(action);
        }

        public Dictionary<Country, int> GetMilitaryAccess()
        {
            return militaryAccess;
        }

        public void AddWar(War war)
        {
            atWarList.Add(war);
        }

        public List<War> GetWarList()
        {
            return atWarList;
        }

        public bool IsAllDivisionsCreated()
        {
            return allDivisionsCreated;
        }
        public bool IsVisibleAllDivisions()
        {
            return allDivisionsVisible;
        }

        #region IntelligenceAgency
        public void CreateIntelligenceAgency(string name, int level, int budget, Texture2D flag)
        {
            intelligenceAgency = new IntelligenceAgency();
            intelligenceAgency.IntelligenceAgencyName = name;
            intelligenceAgency.IntelligenceAgencyLevel = level;
            intelligenceAgency.IntelligenceAgencyBudget = budget;
        }

        public IntelligenceAgency Intelligence_Agency
        {
            get { return intelligenceAgency; }
            set { intelligenceAgency = value; }
        }
        #endregion

        #region Army
        public void CreateArmy()
        {
            army = new Army();

            army.CreateArmy();
            army.Country = this;
        }
        public Army GetArmy()
        {
            return army;
        }
        #endregion

        #region Country Flag
        public Texture2D GetCountryFlag()
        {
            if (countryFlag == null)
                SetCountryFlag();

            return countryFlag;
        }
        public void SetCountryFlag()
        {
            countryFlag = ResourceManager.Instance.LoadTexture(RESOURCE_TYPE.FLAG, name);
        }
        #endregion

        #region Tension
        public int Tension
        {
            get { return tension; }
            set { tension = value; }
        }
        #endregion

        #region Unemployment
        public float Unemployment_Rate
        {
            get { return unemploymentRate; }
            set { unemploymentRate = value; }
        }
        #endregion

        #region Production Speed
        public int Production_Speed
        {
            get { return productionSpeed; }
            set { productionSpeed = value; }
        }
        #endregion

        #region Fertility Rate
        public float Fertility_Rate_PerWeek
        {
            get { return fertilityRate; }
            set { fertilityRate = value; }
        }
        #endregion

        #region Pandemic
        public float Pandemic_Death_Rate_Monthly
        {
            get { return pandemicDeadRateMonthly; }
            set { pandemicDeadRateMonthly = value; }
        }
        #endregion

        #region System Of Government

        public string System_Of_Government
        {
            get { return systemOfGovernment; }
            set { systemOfGovernment = value; }
        }
        #endregion

        #region Debt
        public int Debt
        {
            get { return debt; }
            set { debt = value; }
        }
        #endregion

        #region Military Rank

        public int Military_Rank
        {
            get { return militaryRank; }
            set { militaryRank = value; }
        }
        #endregion

        #region Taxes
        public int Individual_Tax
        {
            get { return tax; }
            set { tax = value; }
        }
        #endregion

        #region GDP
        public int Current_GDP
        {
            get { return currentGDP; }
            set { currentGDP = value; }
        }

        public int Previous_GDP
        {
            get { return previousGDP; }
            set { previousGDP = value; }
        }

        public int Previous_GDP_Monthly
        {
            get { return previousGDPMonthly; }
            set { previousGDPMonthly = value; }
        }

        public float Current_GDP_Annual_Growth_Rate
        {
            get { return currentGDPAnnualGrowthRate; }
            set { currentGDPAnnualGrowthRate = value; }
        }

        #endregion

        #region Budget
        public long Budget
        {
            get { return budget; }
            set { budget = value; }
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
        public int Research_Speed
        {
            get { return researchSpeed; }
            set { researchSpeed = value; }
        }
        #endregion

        #region Production
        public List<Production> GetAllProductionsInProgress()
        {
            return productionProgress;
        }
        #endregion

        #region Mineral Income
        public int Mineral_Income
        {
            get { return mineralIncome; }
            set { mineralIncome = value; }
        }
        #endregion

        #region Gun Income
        public int Gun_Income
        {
            get { return gunIncome; }
            set { gunIncome = value; }
        }
        #endregion

        #region Oil Income
        public int Oil_Income
        {
            get { return oilIncome; }
            set { oilIncome = value; }
        }
        #endregion

        public bool Nuclear_Power
        {
            get { return nuclearPower; }
            set { nuclearPower = value; }
        }

        public bool IsWeaponProducible(int weaponID)
        {
            if (producibleWeapons.Contains(weaponID))
                return true;
            else
                return false;
        }

        public List<int> GetProducibleWeapons()
        {
            return producibleWeapons;
        }

        public void AddProducibleWeaponToInventory(int weaponID)
        {
            if (IsWeaponProducible(weaponID) == false)
            {
                producibleWeapons.Add(weaponID);
            }
        }

        public void CreateAllDivisions()
        {
            DivisionManager.Instance.CreateDivisions(this);
            allDivisionsCreated = true;
        }

        public bool IsVisibleAllBuildings()
        {
            return allBuildingsVisible;
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
        /// Current number of visible cities of this country
        /// </summary>
		[NonSerialized]
        public int visibleCities;

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