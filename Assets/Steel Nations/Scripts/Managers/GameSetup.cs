using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class GameSetup : MonoBehaviour
    {
        private static GameSetup instance;
        public static GameSetup Instance
        {
            get { return instance; }
        }

        WMSK map;

        // Location of database - tempCountry will be set during Awake as to stop Unity 5.4 error regarding initialization before scene is set
        // file should show up in the Unity inspector after a few seconds of running it the first time
        private string _sqlDBLocation = "";
        

        /// <summary>
        /// Table name and DB actual file name -- tempCountry is the name of the actual file on the filesystem
        /// </summary>
        private const string SQL_DB_NAME = "Database";

        /// <summary>
        /// DB objects
        /// </summary>
        private IDbConnection _connection = null;
        private IDbCommand _command = null;
        private IDataReader _reader = null;
    
        /// <summary>
        /// Awake will initialize the connection.  
        /// RunAsyncInit is just for show.  You can do the normal SQLiteInit to ensure that it is
        /// initialized during the Awake() phase and everything is ready during the Start() phase
        /// </summary>
        void Awake()
        {
            map = WMSK.instance;
            instance = this;

            // here is where we set the file location
            // ------------ IMPORTANT ---------
            // - during builds, tempCountry is located in the project root - same level as Assets/Library/obj/ProjectSettings
            // - during runtime (Windows at least), tempCountry is located in the SAME directory as the executable
            // you can play around with the path if you like, but build-vs-run locations need to be taken into account
            _sqlDBLocation = "URI=file:" + SQL_DB_NAME + ".db";
            SQLiteInitForNewGame();
        }

        void Start()
        {
            InitGame();
        }

        /// <summary>
        /// Clean up SQLite Connections, anything else
        /// </summary>
        void OnDestroy()
        {
            SQLiteClose();
        }

        /// <summary>
        /// Example using the Loom to run an asynchronous method on another thread so SQLite lookups
        /// do not block the main Unity thread
        /// </summary>
        public void RunAsyncInit()
        {
            LoomManager.Loom.QueueOnMainThread(() =>
            {
                SQLiteInitForNewGame();
            });
        }

        /// <summary>
        /// Basic initialization of SQLite
        /// </summary>
        private void SQLiteInitForNewGame()
        {
            try
            {
                _connection = new SqliteConnection(_sqlDBLocation);
                _command = _connection.CreateCommand();
                _connection.Open();

                // WAL = write ahead logging, very huge speed increase
                _command.CommandText = "PRAGMA journal_mode = WAL;";
                _command.ExecuteNonQuery();

                // journal mode = look it up on google, I don't remember
                _command.CommandText = "PRAGMA journal_mode";
                _reader = _command.ExecuteReader();

                _reader.Close();

                // more speed increases
                _command.CommandText = "PRAGMA synchronous = OFF";
                _command.ExecuteNonQuery();

                // and some more
                _command.CommandText = "PRAGMA synchronous";
                _reader = _command.ExecuteReader();

                _reader.Close();

                // close connection
                _connection.Close();

                Debug.Log("SQLiter - Opening SQLite succesfull");
            }
            catch
            {
                Debug.Log("SQLiter - Opening SQLite FAIL");
            }
        }

        public void InitGame()
        {
            //MapManager.Instance.ColorizeWorld();

            _connection.Open();


            //if (GameManager.Instance.GameType == GAME_TYPE.NEW_GAME)
                Invoke("NewGame", 1f);

            //if (GameManager.Instance.GameType == GAME_TYPE.LOAD_GAME)
                //Invoke("LoadGame", 1f);
        }

        public void PreLoad()
        {
            LoadBuildings();
            LoadActions();
            LoadDivisions();
            LoadPolitks();
            LoadWeapons();
        }

        public void LoadGame()
        {
            PreLoad();

            SaveLoadManager.Instance.LoadGame();
        }

        /// <summary>
        /// Quick method to show how you can query everything.  Expland on the query parameters to limit what you're looking for, etc.
        /// </summary>
        public void NewGame()
        {
            UpdatePopulation();

            if (GameSettings.Instance.GetSelectedGameMode() == GameSettings.GAME_MODE.QUIZ)
            {
                LoadEconomy();
                //LoadOrganizations();
                //LoadReligion();
                //LoadMilitary();
            }
            else
            {
                PreLoad();

                LoadEconomy();
                LoadGovernments();
                LoadOrganizations();
                LoadReligion();
                LoadMilitary();
            }

            GameSettings.Instance.ShowGameOptionsPanel(true);


            _reader.Close();
            _connection.Close();

            SQLiteClose();
        }

        public void UpdatePopulation()
        {
            foreach(Country country in map.countries)
            {
                string countryName = country.name;

                if (countryName == "China" || 
                    countryName == "Afghanistan" || 
                    countryName == "Algeria" || 
                    countryName == "Angola" || 
                    countryName == "Azerbaijan" ||
                    countryName == "Bosnia and Herzegovina" || 
                    countryName == "Cameroon" ||
                    countryName == "Central African Republic" ||
                    countryName == "Czech Republic" ||
                    countryName == "Eritrea" ||
                    countryName == "Honduras" ||
                    countryName == "Iran" ||
                    countryName == "Ireland" ||
                    countryName == "Ivory Coast" ||
                    countryName == "Kenya" ||
                    countryName == "Kyrgyzstan" ||
                    countryName == "Liberia" ||
                    countryName == "Mali" ||
                    countryName == "Moldova" ||
                    countryName == "Nigeria" ||
                    countryName == "North Korea" ||
                    countryName == "Pakistan" ||
                    countryName == "Poland" ||
                    countryName == "Romania" ||
                    countryName == "Slovenia" ||
                    countryName == "Somalia" ||
                    countryName == "South Africa" ||
                    countryName == "Tajikistan" ||
                    countryName == "Thailand" ||
                    countryName == "Ukraine" ||
                    countryName == "United Arab Emirates" ||
                    countryName == "Uzbekistan" ||
                    countryName == "Vietnam" ||
                    countryName == "Yemen" ||
                    countryName == "Zambia" ||
                    countryName == "Zimbabwe")
                {
                    foreach(City city in map.GetCities(country))
                    {
                        city.population = city.population * 2;
                    }
                }

                if (countryName == "Bangladesh" || 
                    countryName == "Ghana" || 
                    countryName == "Guatemala" || 
                    countryName == "India" || 
                    countryName == "Indonesia" ||
                    countryName == "Mozambique" ||
                    countryName == "Philippines" ||
                    countryName == "Slovakia" ||
                    countryName == "Sudan")
                {
                    foreach (City city in map.GetCities(country))
                    {
                        city.population = city.population * 3;
                    }
                }

                if (countryName == "Burkina Faso" || 
                    countryName == "Cambodia" || 
                    countryName == "Chad" || 
                    countryName == "Laos" || 
                    countryName == "Madagascar" ||
                    countryName == "Myanmar" ||
                    countryName == "Niger" ||
                    countryName == "Sri Lanka" ||
                    countryName == "Uganda")
                {
                    foreach (City city in map.GetCities(country))
                    {
                        city.population = city.population * 4;
                    }
                }

                if (countryName == "Nepal" || countryName == "South Sudan")
                {
                    foreach (City city in map.GetCities(country))
                    {
                        city.population = city.population * 5;
                    }
                }

                if (countryName == "Ethiopia")
                {
                    foreach (City city in map.GetCities(country))
                    {
                        city.population = city.population * 8;
                    }
                }
            }
        }

        public void LoadActions()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Actions";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string actionName = _reader.GetString(0);
                int SuccessRate = _reader.GetInt32(1);
                int effectOnCountryTensionOnSuccess = _reader.GetInt32(2);
                int effectOnCountryTensionOnFailure = _reader.GetInt32(3);
                int atWar = _reader.GetInt32(4);
                int enemy = _reader.GetInt32(5);
                int neutral = _reader.GetInt32(6);
                int ally = _reader.GetInt32(7);
                int evolotionTime = _reader.GetInt32(8);
                string actionCategoryName = _reader.GetString(9);

                ACTION_TYPE type = ActionManager.Instance.GetActionTypeByActionName(actionName);
                ACTION_CATEGORY actionCategoryType = ActionManager.Instance.GetActionCategoryTypeByCategoryName(actionCategoryName);

                Action action = new Action();

                action.SetAction(type, actionName, null, SuccessRate, effectOnCountryTensionOnSuccess, effectOnCountryTensionOnFailure, atWar, enemy, neutral, ally, evolotionTime, actionCategoryType);
                ActionManager.Instance.AddAction(action);
            }
        }

        public void LoadEconomy()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Economy";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string countryName = _reader.GetString(0);
                Country country = map.GetCountry(countryName);

                int GDP = _reader.GetInt32(1);
                int debt = _reader.GetInt32(2);
                int money = _reader.GetInt32(3);

                country.Current_GDP = 0;
                country.Previous_GDP = GDP;
                country.Previous_GDP_Monthly = GDP / 20;

                country.Debt = debt;
                country.Budget = money;

                country.Individual_Tax = 1;

                CountryManager.Instance.AddCountry(country);
            }
        }

        public void LoadGovernments()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Governments";
            _reader = _command.ExecuteReader();


            while (_reader.Read())
            {
                string country = _reader.GetString(0);
                Country tempCountry = map.GetCountry(country);

                if (tempCountry == null)
                {
                    Debug.Log(country + " -> is null");
                }
                else
                {
                    int rank = _reader.GetInt32(1);
                    string friend = _reader.GetString(2);
                    string enemy = _reader.GetString(3);

                    int oil = _reader.GetInt32(4);
                    int uranium = _reader.GetInt32(5);
                    int iron = _reader.GetInt32(6);
                    int aluminium = _reader.GetInt32(7);
                    int steel = _reader.GetInt32(8);
                    int tansion = _reader.GetInt32(9);

                    int nuclearWarHead = _reader.GetInt32(10);

                    float country_natality = _reader.GetFloat(11);
                    float corona = _reader.GetFloat(12);
                    string system_government = _reader.GetString(13);
                    string pandemicBeginDate = _reader.GetString(14);

                    if (system_government == string.Empty)
                        system_government = "Republic";

                    int unemploymentRate = _reader.GetInt32(15);
                    string intelligenceAgency = _reader.GetString(16);

                    if (intelligenceAgency != string.Empty)
                    {
                        int intelligenceAgencyLevel = _reader.GetInt32(17);
                        int intelligenceAgencyBudget = _reader.GetInt32(18);

                        CountryManager.Instance.CreateIntelligenceAgency(tempCountry, intelligenceAgency, intelligenceAgencyLevel, intelligenceAgencyBudget, null);
                        tempCountry.Intelligence_Agency.Init();

                        tempCountry.Intelligence_Agency_Budget_By_GDP = (intelligenceAgencyBudget * 100.0f) / (float)tempCountry.Previous_GDP;
                    }

                    tempCountry.SetCountryFlag();

                    if (nuclearWarHead > 0)
                        tempCountry.Nuclear_Power = true;
                    else
                        tempCountry.Nuclear_Power = false;

                    foreach (Country temp in CountryManager.Instance.GetAllCountries())
                    {
                        CountryManager.Instance.UpdateRelation(tempCountry, temp, 0);
                    }

                    string[] friend_countries = friend.Split('~');
                    string[] enemy_countries = enemy.Split('~');

                    foreach (string index in friend_countries)
                    {
                        Country allyCountry = map.GetCountry(index);
                        if (allyCountry != null)
                        {
                            CountryManager.Instance.UpdateRelation(tempCountry, allyCountry, 50);
                            tempCountry.AddTradeTreaty(allyCountry);
                        }
                    }

                    foreach (string index in enemy_countries)
                    {
                        Country enemyCountry = map.GetCountry(index);
                        if (enemyCountry != null)
                        {
                            tempCountry.PlaceTradeEmbargo(enemyCountry);
                            CountryManager.Instance.UpdateRelation(tempCountry, enemyCountry, -50);
                        }
                    }


                    tempCountry.attrib["My Country"] = 0;

                    tempCountry.AddMineral(MINERAL_TYPE.OIL, oil);
                    tempCountry.AddMineral(MINERAL_TYPE.IRON, iron);
                    tempCountry.AddMineral(MINERAL_TYPE.ALUMINIUM, aluminium);
                    tempCountry.AddMineral(MINERAL_TYPE.URANIUM, uranium);
                    tempCountry.AddMineral(MINERAL_TYPE.STEEL, steel);

                    tempCountry.Military_Rank = rank;
                    tempCountry.Tension = tansion;
                    tempCountry.Unemployment_Rate = unemploymentRate;

                    tempCountry.Production_Speed = 10;
                    tempCountry.Research_Speed = 10;
                    tempCountry.Fertility_Rate_PerWeek = country_natality;
                    tempCountry.Pandemic_Death_Rate_Monthly = corona;
                    tempCountry.System_Of_Government = system_government;

                    List<City> cities = CountryManager.Instance.GetAllCitiesInCountry(tempCountry);
                    int manpower = CountryManager.Instance.GetAvailableManpower(tempCountry);

                    foreach (City city in cities)
                    {
                        int mineralMultipler = 1;

                        city.Constructible_Dockyard_Area = 0;

                        if (MapManager.Instance.CityHasCoast(city))
                            city.Constructible_Dockyard_Area = 1;

                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            mineralMultipler = 3;
                        if (city.cityClass == CITY_CLASS.REGION_CAPITAL)
                            mineralMultipler = 2;

                        
                        city.SetMineralResources(MINERAL_TYPE.OIL, oil / mineralMultipler);
                        city.SetMineralResources(MINERAL_TYPE.IRON, iron / mineralMultipler);
                        city.SetMineralResources(MINERAL_TYPE.ALUMINIUM, aluminium / mineralMultipler);
                        city.SetMineralResources(MINERAL_TYPE.URANIUM, uranium / mineralMultipler);
                        city.SetMineralResources(MINERAL_TYPE.STEEL, steel / mineralMultipler);

                        city.CityLevel = (city.population * 100.0f) / manpower;

                        if (city.CityLevel < 0)
                            city.CityLevel = 0;

                        city.CityIncome = (int)((tempCountry.Previous_GDP_Monthly * city.CityLevel) / 100.0f);

                        BuildingManager.Instance.SetBuildingsInCity(city);
                    }

                    CountryManager.Instance.CalculateCountryPerCapitaIncome(tempCountry);
                }
            }
        }

        public void LoadOrganizations()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Organizations";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string organizationName = _reader.GetString(0);

                if (organizationName != string.Empty)
                {
                    int isActive = Int32.Parse(_reader.GetString(1));
                    int isTradeBonus = Int32.Parse(_reader.GetString(2));
                    int isMilitaryBonus = Int32.Parse(_reader.GetString(3));
                    string description = _reader.GetString(4);
                    int isTerroristOrganization = Int32.Parse(_reader.GetString(5));
                    int isAttackToProtect = Int32.Parse(_reader.GetString(6));
                    string logoPath = _reader.GetString(7);

                    string founder = _reader.GetString(8);

                    string fullMember = _reader.GetString(9);
                    string observers = _reader.GetString(10);
                    string dialoguePartners = _reader.GetString(11);

                    string appliedForFullMember = _reader.GetString(12);
                    string appliedForObservor = _reader.GetString(13);
                    string appliedForDialoguePartner = _reader.GetString(14);


                    OrganizationManager.Instance.CreateOrganization(organizationName,
                        isActive,
                        isTradeBonus,
                        isMilitaryBonus,
                        isTerroristOrganization,
                        isAttackToProtect,
                        description,
                        logoPath,
                        founder,
                        fullMember,
                        observers,
                        dialoguePartners,
                        appliedForFullMember,
                        appliedForObservor,
                        appliedForDialoguePartner);
                }

            }
        }

        public void LoadReligion()
        {
            _command.CommandText = "SELECT * FROM Religion";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string country = _reader.GetString(0);
                Country tempCountry = map.GetCountry(country);

                //int christian = _reader.GetInt32(1);
                //int muslim = _reader.GetInt32(2);
                //int irreligion = _reader.GetInt32(3);
                //int hindu = _reader.GetInt32(4);
                //int buddist = _reader.GetInt32(5);
                //int folk = _reader.GetInt32(6);
                //int jewish = _reader.GetInt32(7);
                string religion = _reader.GetString(8);

                CountryManager.Instance.SetReligion(tempCountry, 0, 0, 0, 0, 0, 0, 0, religion);
            }
        }

        public void LoadDivisions()
        {
            _command.CommandText = "SELECT * FROM Division";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string divisionType = _reader.GetString(0);
                int soldierMinimum = _reader.GetInt32(1);
                int soldierMaximum = _reader.GetInt32(2);
                string mainUnit = _reader.GetString(3);
                int maxMainUnit = _reader.GetInt32(4);
                string secondUnit = _reader.GetString(5);
                int maxSecondUnit = _reader.GetInt32(6);
                string thirdUnit = _reader.GetString(7);
                int maxThirdUnit = _reader.GetInt32(8);

                DivisionManagerPanel.CreateDivisionTemplate(mainUnit,
                    secondUnit,
                    thirdUnit,
                    maxMainUnit,
                    maxSecondUnit,
                    maxThirdUnit,
                    soldierMinimum,
                    soldierMaximum,
                    divisionType);
            }
        }

        public void LoadPolitks()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Policy";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                int policyID = _reader.GetInt32(0);
                string policyName = _reader.GetString(1);
                string policyDescription = _reader.GetString(2);
                string policyBonus = _reader.GetString(3);
                int policyBonusValue = _reader.GetInt32(4);
                int requiredDefenseBudget = _reader.GetInt32(5);
                int requiredGSYH = _reader.GetInt32(6);
                int tradeBonus = _reader.GetInt32(7);
                int costPermenant = _reader.GetInt32(8);
                string policyType = _reader.GetString(9);

                Policy policy = new Policy();

                policy.policyID = policyID;
                policy.policyName = policyName;
                policy.policyDescription = policyDescription;
                policy.policyBonusValue = policyBonusValue;
                policy.policyBonus = policyBonus;
                policy.requiredDefenseBudget = requiredDefenseBudget;
                policy.requiredGSYH = requiredGSYH;
                policy.tradeBonus = tradeBonus;
                policy.costPermenant = costPermenant;

                policy.SetPolicyTypeByName(policyType);

                PolicyPanel.Instance.AddPolicy(policy);
            }
        }

        public void LoadWeapons()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Weapon";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                int weapon_id = _reader.GetInt32(0);
                string weapon_name = _reader.GetString(1);
                int weapon_speed = _reader.GetInt32(2);
                int weapon_attack = _reader.GetInt32(3);
                int weapon_defense = _reader.GetInt32(4);
                int weapon_operational_range = _reader.GetInt32(5);
                int weapon_cost = _reader.GetInt32(6);
                int weapon_level = _reader.GetInt32(7);
                int weapon_terrain_capability = _reader.GetInt32(8);
                int weapon_duration = _reader.GetInt32(9);
                int weapon_research_time = _reader.GetInt32(10);
                int weapon_research_cost = _reader.GetInt32(11);
                int weapon_research_year = _reader.GetInt32(12);
                string weapon_description = _reader.GetString(13);
                string weapon_image_directory = _reader.GetString(14);

                int weapon_required_oil = _reader.GetInt32(15);
                int weapon_required_uranium = _reader.GetInt32(16);

                int weapon_required_iron = _reader.GetInt32(17);
                int weapon_required_steel = _reader.GetInt32(18);
                int weapon_required_aluminium = _reader.GetInt32(19);

                WeaponManager.Instance.CreateWeaponTemplate(weapon_id,
                    weapon_name,
                    weapon_speed,
                    weapon_attack,
                    weapon_defense,
                    weapon_operational_range,
                    weapon_cost,
                    weapon_level,
                    weapon_terrain_capability,
                    weapon_duration,
                    weapon_research_year,
                    weapon_research_time,
                    weapon_research_cost,
                    weapon_description,
                    weapon_image_directory,
                    weapon_required_oil,
                    weapon_required_uranium,
                    weapon_required_iron,
                    weapon_required_steel,
                    weapon_required_aluminium);
            }
        }

        public void LoadMilitary()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Military";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string countryName = _reader.GetString(0);
                Country country = map.GetCountry(countryName);

                int defense_budget = _reader.GetInt32(1);

                int soldier = _reader.GetInt32(2);
                int tank = _reader.GetInt32(3);
                int armored_vehicle = _reader.GetInt32(4);
                int self_propelled_artillery = _reader.GetInt32(5);
                int towed_artillery = _reader.GetInt32(6);
                int rocket_projector = _reader.GetInt32(7);

                int helicopter = _reader.GetInt32(8);
                int fighter = _reader.GetInt32(9);
                int drone = _reader.GetInt32(10);

                int carrier = _reader.GetInt32(11);
                int submarine = _reader.GetInt32(12);
                int frigate = _reader.GetInt32(13);
                int corvette = _reader.GetInt32(14);
                int destroyer = _reader.GetInt32(15);
                int patrol = _reader.GetInt32(16);

                int missile = _reader.GetInt32(17);


                int soldier_equipment_tech = _reader.GetInt32(18);
                int tank_tech = _reader.GetInt32(19);
                int armored_vehicle_tech = _reader.GetInt32(20);
                int self_propelled_artillery_tech = _reader.GetInt32(21);
                int towed_artillery_tech = _reader.GetInt32(22);
                int rocket_projector_tech = _reader.GetInt32(23);

                int helicopter_tech = _reader.GetInt32(24);
                int fighter_tech = _reader.GetInt32(25);
                int drone_tech = _reader.GetInt32(26);

                int carrier_tech = _reader.GetInt32(27);
                int submarine_tech = _reader.GetInt32(28);
                int frigate_tech = _reader.GetInt32(29);
                int corvette_tech = _reader.GetInt32(30);
                int destroyer_tech = _reader.GetInt32(31);
                int patrol_tech = _reader.GetInt32(32);

                int missile_tech = _reader.GetInt32(33);

                country.CreateArmy();
                country.GetArmy().Defense_Budget = defense_budget;
                country.GetArmy().SetSoldierNumber(soldier);

                country.Defense_Budget_By_GDP = (defense_budget * 100.0f) / (float)country.Previous_GDP;

                InitWeaponInventory(country, WEAPON_TYPE.TANK, tank, tank_tech);
                InitWeaponInventory(country, WEAPON_TYPE.TOWED_ARTILLERY, towed_artillery, towed_artillery_tech);
                InitWeaponInventory(country, WEAPON_TYPE.ROCKET_PROJECTOR, rocket_projector, rocket_projector_tech);
                InitWeaponInventory(country, WEAPON_TYPE.SELF_PROPELLED_ARTILLERY, self_propelled_artillery, self_propelled_artillery_tech);
                InitWeaponInventory(country, WEAPON_TYPE.ARMORED_VEHICLE, armored_vehicle, armored_vehicle_tech);

                InitWeaponInventory(country, WEAPON_TYPE.FIGHTER, fighter, fighter_tech);
                InitWeaponInventory(country, WEAPON_TYPE.DRONE, drone, drone_tech);
                InitWeaponInventory(country, WEAPON_TYPE.HELICOPTER, helicopter, helicopter_tech);

                InitWeaponInventory(country, WEAPON_TYPE.AIRCRAFT_CARRIER, carrier, carrier_tech);
                InitWeaponInventory(country, WEAPON_TYPE.DESTROYER, destroyer, destroyer_tech);
                InitWeaponInventory(country, WEAPON_TYPE.FRIGATE, frigate, frigate_tech);
                InitWeaponInventory(country, WEAPON_TYPE.CORVETTE, corvette, corvette_tech);
                InitWeaponInventory(country, WEAPON_TYPE.SUBMARINE, submarine, submarine_tech);
                InitWeaponInventory(country, WEAPON_TYPE.COASTAL_PATROL, patrol, patrol_tech);

                InitWeaponInventory(country, WEAPON_TYPE.ICBM, missile, missile_tech);
                InitWeaponInventory(country, WEAPON_TYPE.ANTI_MATTER_BOMB, 0, 0);
                InitWeaponInventory(country, WEAPON_TYPE.NEUTRON_BOMB, 0, 0);

            }
        }

        void InitWeaponInventory(Country country, WEAPON_TYPE weaponType, int unitNumber, int unitTech)
        {
            bool addWeapon = true;

            country.SetWeaponTech(weaponType, unitTech);

            WeaponTemplate tempWeapon = WeaponManager.Instance.GetWeaponByWeaponTypeAndTech(weaponType, unitTech);

            if (tempWeapon == null)
                return;

            if (unitTech == 0)
            {
                addWeapon = false;
                unitTech = WeaponManager.Instance.GetLowestGenerationByWeaponName(tempWeapon.weaponName);
            }

            if (tempWeapon.weaponLevel < unitTech)
            {
                if (addWeapon)
                    country.AddProducibleWeaponToInventory(tempWeapon.weaponID);
            }
            else if (tempWeapon.weaponLevel == unitTech)
            {
                if (addWeapon)
                    country.AddProducibleWeaponToInventory(tempWeapon.weaponID);

                for (int i = 0; i < unitNumber; i++)
                {
                    Weapon weapon = new Weapon();
                    weapon.weaponTemplateID = tempWeapon.weaponID;
                    weapon.weaponLeftHealth = tempWeapon.weaponDefense;

                    if (tempWeapon.weaponTerrainType == 1)
                        country.GetArmy().GetLandForces().AddWeaponToMilitaryForces(weapon);
                    if (tempWeapon.weaponTerrainType == 2)
                        country.GetArmy().GetNavalForces().AddWeaponToMilitaryForces(weapon);
                    if (tempWeapon.weaponTerrainType == 3 || tempWeapon.weaponTerrainType == 4)
                        country.GetArmy().GetAirForces().AddWeaponToMilitaryForces(weapon);
                }
            }

        }

        public void LoadBuildings()
        {
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Buildings";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string Building_Name = _reader.GetString(0);
                int Construction_Time = _reader.GetInt32(1);
                int Construction_Cost = _reader.GetInt32(2);
                int Income_Monthly = _reader.GetInt32(3);
                int Required_Employee = _reader.GetInt32(4);
                int Required_Manpower = _reader.GetInt32(5);
                int Max_Building = _reader.GetInt32(6);
                string Description = _reader.GetString(7);

                Building building = new Building(Building_Name, Construction_Time, Construction_Cost, Income_Monthly, Required_Employee, Required_Manpower , Max_Building, Description);

                BuildingManager.Instance.AddBuilding(building);
            }
        }

        /// <summary>
        /// Basic execute command - open, create command, execute, close
        /// </summary>
        /// <param name="commandText"></param>
        public void ExecuteNonQuery(string commandText)
        {
            _connection.Open();
            _command.CommandText = commandText;
            _command.ExecuteNonQuery();
            _connection.Close();
        }

        /// <summary>
        /// Clean up everything for SQLite
        /// </summary>
        private void SQLiteClose()
        {
            if (_reader != null && !_reader.IsClosed)
                _reader.Close();
            _reader = null;

            if (_command != null)
                _command.Dispose();
            _command = null;

            if (_connection != null && _connection.State != ConnectionState.Closed)
                _connection.Close();
            _connection = null;
        }               
    }
}
