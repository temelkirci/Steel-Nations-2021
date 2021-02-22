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

            Debug.Log("_sqlDBLocation : " + _sqlDBLocation);

            SQLiteInitForNewGame();
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
            Debug.Log( "SQLiter - Opening SQLite Connection" );

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

        /// <summary>
        /// Quick method to show how you can query everything.  Expland on the query parameters to limit what you're looking for, etc.
        /// </summary>
        public void NewGame()
        {
            Debug.Log("NewGame");
            MapManager.Instance.ColorizeWorld();

            _connection.Open();

            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Settings";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string key = _reader.GetString(0);
                int value = _reader.GetInt32(1);

                GameSettings.SetGameSettings(key, value);
            }

            Debug.Log("Settings Loaded");

            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Buildings";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string Building_Name = _reader.GetString(0);
                int Construction_Time = _reader.GetInt32(1);
                int Construction_Cost = _reader.GetInt32(2);
                int Income_Monthly = _reader.GetInt32(3);
                int Expense_Monthly = _reader.GetInt32(4);
                string Description = _reader.GetString(5);

                BUILDING_TYPE type = CityInfoPanel.Instance.GetBuildingTypeByBuildingName(Building_Name);

                Building building = new Building(Building_Name, 1, type, 0, Income_Monthly, Expense_Monthly, Construction_Cost, Construction_Time, Description);

                CityInfoPanel.Instance.AddBuilding(building);
            }
            Debug.Log("Buildings Loaded");

            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Economy";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string country = _reader.GetString(0);
                Country tempCountry = WMSK.instance.GetCountry(country);

                int national_income = _reader.GetInt32(1);
                int export = _reader.GetInt32(2);
                int import = _reader.GetInt32(3);
                int debt = _reader.GetInt32(4);
                int money = _reader.GetInt32(5);

                tempCountry.SetPreviousGDP(national_income);
                tempCountry.SetPreviousExport(export);
                tempCountry.SetPreviousImport(import);
                tempCountry.SetDebt(debt);
                tempCountry.SetBudget(money);

                if (debt < 1000)
                    tempCountry.attrib["Debt Per Week"] = debt / 100;
                else
                    tempCountry.attrib["Debt Per Week"] = debt / 1000;

                tempCountry.SetCurrentGDP(0);
                tempCountry.SetIndividualTax(10);// between 0-100
                tempCountry.SetCorporationTax(100000); // between 100.000-1.000.000

                int exportMonthly = export / 10;
                int importMonthly = import / 10;

                tempCountry.SetImportMonthly(importMonthly);
                tempCountry.SetExportMonthly(exportMonthly);

                tempCountry.SetPreviousGDPAnnualGrowthRate(1);

                CountryManager.Instance.AddCountry(tempCountry);
            }

            Debug.Log("Economy Loaded");


            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Governments";
            _reader = _command.ExecuteReader();
           

            while ( _reader.Read() )
            {
                string country = _reader.GetString( 0 );
                Country tempCountry = WMSK.instance.GetCountry(country);

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

                    if (system_government == string.Empty)
                        system_government = "Başkanlık Sistemi";

                    int military_factory = _reader.GetInt32(15);
                    int harbor = _reader.GetInt32(16);
                    int university = _reader.GetInt32(17);
                    int airport = _reader.GetInt32(18);
                    int unemploymentRate = _reader.GetInt32(19);

                    string intelligenceAgency = _reader.GetString(20);

                    if(intelligenceAgency != string.Empty)
                    {
                        int intelligenceAgencyLevel = _reader.GetInt32(21);
                        int intelligenceAgencyBudget = _reader.GetInt32(22);

                        tempCountry.CreateIntelligenceAgency(intelligenceAgency, intelligenceAgencyLevel, intelligenceAgencyBudget, null);
                    }

                    tempCountry.SetCountryFlag(Resources.Load("flags/" + tempCountry.name) as Texture2D);

            
                    string[] friend_countries = friend.Split('~');
                    string[] enemy_countries = enemy.Split('~');

                    List<Country> allyList = new List<Country>();
                    List<Country> enemyList = new List<Country>();

                    foreach (string index in friend_countries)
                    {
                        Country allyCountry = map.GetCountry(index);
                        allyList.Add(allyCountry);
                    }

                    foreach (string index in enemy_countries)
                    {
                        enemyList.Add(map.GetCountry(index));
                    }

                    foreach(Country temp in CountryManager.Instance.GetAllCountries())
                    {
                        if(temp != tempCountry)
                        {
                            if(allyList.Contains(temp))
                            {
                                tempCountry.attrib[temp.name] = 50;
                            }
                            else if (enemyList.Contains(temp))
                            {
                                tempCountry.attrib[temp.name] = -50;
                            }
                            else
                            {
                                tempCountry.attrib[temp.name] = 0;
                            }
                        }
                    }

                    tempCountry.attrib["My Country"] = 0;

                    tempCountry.SetMilitaryRank(rank);
                    tempCountry.SetNuclearWarHead(nuclearWarHead);
                    tempCountry.DistributeResources(oil, iron, steel, aluminium, uranium);
                    tempCountry.SetTension(tansion);
                    tempCountry.SetUnemploymentRate(unemploymentRate);                 

                    tempCountry.SetProductionSpeed(0);
                    tempCountry.SetResearchSpeed(0);
                    tempCountry.SetFertilityRatePerWeek(country_natality*4);
                    tempCountry.SetPandemicDeathRatePerWeek(corona*4);
                    tempCountry.SetSystemOfGovernment(system_government);

                    List<City> cities = map.GetCities(tempCountry, false, WMSK.CITY_CLASS_FILTER_ANY);

                    int totalMineral = iron + steel + aluminium;
                    int totalFactory = tempCountry.GetPreviousExport() / 1000;

                    foreach (City tempCity in cities)
                    {                     
                        Vector2 waterPos;
                        if (map.ContainsWater(tempCity.unity2DLocation, 0.001f, out waterPos))
                        {
                            //citiesNearWater.Add(tempCity);
                            tempCity.SetConstructibleDockyardAreaNumber(1);
                        }

                        /*
                        // For each found city, add a sphere marker on its position
                        citiesNearWater.ForEach((City city) => {
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                            sphere.WMSK_MoveTo(city.unity2DLocation, false);
                        });
                        */

                        tempCity.CalculateCityLevel();

                        float buildingNumber = (military_factory * tempCity.GetCityLevel()) / 100f;
                        tempCity.CreateBuilding(BUILDING_TYPE.MILITARY_FACTORY, Convert.ToInt32(buildingNumber), military_factory);

                        buildingNumber = (university * tempCity.GetCityLevel()) / 100f;
                        tempCity.CreateBuilding(BUILDING_TYPE.UNIVERSITY, Convert.ToInt32(buildingNumber), university);

                        buildingNumber = (airport * tempCity.GetCityLevel()) / 100f;
                        tempCity.CreateBuilding(BUILDING_TYPE.AIRPORT, Convert.ToInt32(buildingNumber), airport);

                        buildingNumber = (totalMineral * tempCity.GetCityLevel()) / 100f;
                        tempCity.CreateBuilding(BUILDING_TYPE.MINERAL_FACTORY, Convert.ToInt32(buildingNumber), totalMineral);


                        buildingNumber = (totalFactory * tempCity.GetCityLevel()) / 100;
                        tempCity.CreateBuilding(BUILDING_TYPE.FACTORY, Convert.ToInt32(buildingNumber), totalFactory);


                        if (tempCity.GetConstructibleDockyardAreaNumber() > 0)
                            tempCity.CreateBuilding(BUILDING_TYPE.TRADE_PORT, 1, 1);


                        tempCity.SetReserveResources(oil, iron, steel, aluminium, uranium);

                        tempCity.CreateBuilding(BUILDING_TYPE.HOSPITAL, (tempCity.population / 500000), (tempCity.population / 1000000));
                        tempCity.CreateBuilding(BUILDING_TYPE.GARRISON, 5,5);


                        if (tempCity.GetOilReserves() > 0 && (tempCity.cityClass == CITY_CLASS.REGION_CAPITAL || tempCity.cityClass == CITY_CLASS.COUNTRY_CAPITAL))
                            tempCity.CreateBuilding(BUILDING_TYPE.OIL_RAFINERY, 1, 1);
                        else
                            tempCity.CreateBuilding(BUILDING_TYPE.OIL_RAFINERY, 0, 0);


                        if (tempCity.GetUraniumReserves() > 0 && (tempCity.cityClass == CITY_CLASS.REGION_CAPITAL || tempCity.cityClass == CITY_CLASS.COUNTRY_CAPITAL))
                            tempCity.CreateBuilding(BUILDING_TYPE.NUCLEAR_FACILITY, 1, 1);
                        else
                            tempCity.CreateBuilding(BUILDING_TYPE.NUCLEAR_FACILITY, 0, 0);

                    }
                    
                    tempCountry.GetCountryPerCapitaIncome();
                }
            }

            Debug.Log("Governments Loaded");


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

            Debug.Log("Organizations Loaded");

            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Religion";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string country = _reader.GetString(0);
                Country tempCountry = map.GetCountry(country);

                int christian = _reader.GetInt32(1);
                int muslim = _reader.GetInt32(2);
                int irreligion = _reader.GetInt32(3);
                int hindu = _reader.GetInt32(4);
                int buddist = _reader.GetInt32(5);
                int folk = _reader.GetInt32(6);
                int jewish = _reader.GetInt32(7);
                string religion = _reader.GetString(8);

                tempCountry.SetReligion(christian, muslim, irreligion, hindu, buddist, folk, jewish, religion);
            }
          
            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Division";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string divisionType = _reader.GetString(0);
                int type = _reader.GetInt32(1);
                int soldierMinimum = _reader.GetInt32(2);
                int soldierMaximum = _reader.GetInt32(3);
                string mainUnit = _reader.GetString(4);
                int maxMainUnit = _reader.GetInt32(5);
                string secondUnit = _reader.GetString(6);
                int maxSecondUnit = _reader.GetInt32(7);
                string thirdUnit = _reader.GetString(8);
                int maxThirdUnit = _reader.GetInt32(9);

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

            Debug.Log("Divisions Loaded");

            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Doctrine";
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
                int costPerWeek = _reader.GetInt32(7);
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
                policy.costPerWeek = costPerWeek;
                policy.costPermenant = costPermenant;

                policy.SetPolicyTypeByName(policyType);

                PolicyPanel.Instance.AddPolicy(policy);
            }

            Debug.Log("Politiks Loaded");

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

            Debug.Log("Weapons Loaded");

            // if you have a bunch of stuff, tempCountry is going to be inefficient and a pain.  it's just for testing/show
            _command.CommandText = "SELECT * FROM Military";
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                string country = _reader.GetString(0);
                Country tempCountry = map.GetCountry(country);

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

                tempCountry.CreateArmy();
                tempCountry.GetArmy().SetDefenseBudget(defense_budget);
                tempCountry.GetArmy().SetSoldierNumber(soldier);

                tempCountry.attrib["Soldier Equipment"] = soldier;
                tempCountry.attrib["Tank"] = tank;
                tempCountry.attrib["Armored Vehicle"] = armored_vehicle;
                tempCountry.attrib["Self-Propelled Artillery"] = self_propelled_artillery;
                tempCountry.attrib["Towed Artillery"] = towed_artillery;
                tempCountry.attrib["Rocket Projector"] = rocket_projector;

                tempCountry.attrib["Helicopter"] = helicopter;
                tempCountry.attrib["Fighter"] = fighter;
                tempCountry.attrib["Drone"] = drone;
               

                tempCountry.attrib["Frigate"] = frigate;
                tempCountry.attrib["Corvette"] = corvette;
                tempCountry.attrib["Coastal Patrol"] = patrol;
                tempCountry.attrib["Destroyer"] = destroyer;
                tempCountry.attrib["Aircraft Carrier"] = carrier;
                tempCountry.attrib["Submarine"] = submarine;

                tempCountry.attrib["Ballistic Missile"] = missile;
                tempCountry.attrib["Neutron Bomb"] = 0;
                tempCountry.attrib["Anti Matter Bomb"] = 0;

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

                tempCountry.attrib["Soldier Equipment Tech"] = soldier_equipment_tech;
                tempCountry.attrib["Tank Tech"] = tank_tech;
                tempCountry.attrib["Armored Vehicle Tech"] = armored_vehicle_tech;
                tempCountry.attrib["Self-Propelled Artillery Tech"] = self_propelled_artillery_tech;
                tempCountry.attrib["Towed Artillery Tech"] = towed_artillery_tech;
                tempCountry.attrib["Rocket Projector Tech"] = rocket_projector_tech;

                tempCountry.attrib["Helicopter Tech"] = helicopter_tech;
                tempCountry.attrib["Fighter Tech"] = fighter_tech;
                tempCountry.attrib["Drone Tech"] = drone_tech;

                tempCountry.attrib["Frigate Tech"] = frigate_tech;
                tempCountry.attrib["Corvette Tech"] = corvette_tech;
                tempCountry.attrib["Coastal Patrol Tech"] = patrol_tech;
                tempCountry.attrib["Destroyer Tech"] = destroyer_tech;
                tempCountry.attrib["Aircraft Carrier Tech"] = carrier_tech;
                tempCountry.attrib["Submarine Tech"] = submarine_tech;

                tempCountry.attrib["Ballistic Tech"] = missile_tech;
                tempCountry.attrib["Neutron Bomb Tech"] = missile_tech;
                tempCountry.attrib["Anti Matter Bomb Tech"] = missile_tech;

                tempCountry.UpdateWeaponTechnology();
            }

            _reader.Close();
            _connection.Close();
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
