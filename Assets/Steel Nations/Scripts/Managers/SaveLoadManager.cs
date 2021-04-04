using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        WMSK map = WMSK.instance;

        const string CONTRY_ATTRIBUTES_FILENAME = "CountriesAttributes";
        const string CITY_ATTRIBUTES_FILENAME = "CitiesAttributes";

        string countryGeoData, countryAttributes, provinceGeoData, provinceAttributes, cityGeoData, cityAttributes, mountPointGeoData, mountPointAttributes;


        void SaveCountry()
        {
            foreach(Country country in CountryManager.Instance.GetAllCountries())
            {
                if (GameSettings.Instance.GetSelectedGameMode() == GameSettings.GAME_MODE.QUIZ)
                {
                    SaveCities(country);
                    SaveWar(country);
                }
                else
                {
                    SaveCities(country);
                    SaveDivisions(country);
                    SaveIntelligenceAgency(country);
                    SaveActions(country);
                    SaveWar(country);
                    SaveProducibleWeapons(country);
                    SaveTradeEmbargo(country);
                    SaveArmsEmbargo(country);
                    SavePolicy(country);
                    SaveMilitaryAccess(country);
                    SaveProductionInProgress(country);
                    SaveResearchInProgress(country);
                }
            }
        }

        void LoadCountry()
        {
            foreach (Country country in CountryManager.Instance.GetAllCountries())
            {
                LoadCities(country);
                LoadDivisions(country);
                LoadIntelligenceAgency(country);
                LoadActions(country);
                LoadWar(country);
                LoadProducibleWeapons(country);
                LoadTradeEmbargo(country);
                LoadArmsEmbargo(country);
                LoadPolicy(country);
                LoadMilitaryAccess(country);
                LoadProductionInProgress(country);
                LoadResearchInProgress(country);
            }
        }

        void LoadCities(Country country)
        {

        }

        void LoadDivisions(Country country)
        {

        }

        void LoadIntelligenceAgency(Country country)
        {

        }

        void LoadActions(Country country)
        {

        }
        void LoadWar(Country country)
        {

        }

        void LoadProducibleWeapons(Country country)
        {

        }

        void LoadTradeEmbargo(Country country)
        {

        }
        void LoadArmsEmbargo(Country country)
        {

        }

        void LoadPolicy(Country country)
        {

        }

        void LoadMilitaryAccess(Country country)
        {

        }

        void LoadProductionInProgress(Country country)
        {

        }

        void LoadResearchInProgress(Country country)
        {

        }

        void SaveDivisions(Country country)
        {
            if (country.GetArmy() == null)
                return;

            foreach(GameObjectAnimator GOA in country.GetArmy().GetAllDivisionInArmy())
            {
                Division division = GOA.GetDivision();

                GOA.attrib["Division Name"] = GOA.name;
                GOA.attrib["Division Position X"] = GOA.currentMap2DLocation.x;
                GOA.attrib["Division Position Y"] = GOA.currentMap2DLocation.y;
                GOA.attrib["Division Soldier Number"] = division.currentSoldier;

                GOA.attrib["Division Main Gun"] = division.MainWeapon;
                GOA.attrib["Division Main Gun Number"] = division.MainWeaponNumber;

                GOA.attrib["Division Second Gun"] = division.SecondWeapon;
                GOA.attrib["Division Second Gun Number"] = division.SecondWeaponNumber;

                GOA.attrib["Division Third Gun"] = division.ThirdWeapon;
                GOA.attrib["Division Third Gun Number"] = division.ThirdWeaponNumber;
            }
        }

        void SaveCities(Country country)
        {
            foreach(City city in CountryManager.Instance.GetAllCitiesInCountry(country))
            {
                city.attrib["Constructible Dockyard Area"] = city.Constructible_Dockyard_Area;
                city.attrib["Dockyard Number"] = 0;

                if (city.Dockyard != null)
                    city.attrib["Dockyard Number"] = 1;


                string saveBuilding = string.Empty;
                foreach(var building in city.GetAllBuildings())
                {
                    saveBuilding = saveBuilding + BuildingManager.Instance.GetBuildingByBuildingType(building.Key).buildingName + "&" + building.Value + "~";
                }

                city.attrib["Building"] = saveBuilding;
            }
        }

        void SaveResearchInProgress(Country country)
        {
            string saveResearch = string.Empty;

            foreach (Research research in country.GetAllResearchsInProgress())
            {
                saveResearch = saveResearch + research.techWeapon.weaponID + "&" + research.totalResearchDay + "&" + research.leftDays + "~";
            }

            country.attrib["Research"] = saveResearch;
        }


        void SaveProductionInProgress(Country country)
        {
            string saveProduction = string.Empty;

            foreach(Production production in country.GetAllProductionsInProgress())
            {
                saveProduction = saveProduction + production.techWeapon.weaponID + "&" + production.number + "&" + production.leftDays + "~";
            }

            country.attrib["Production"] = saveProduction;
        }

        void SaveMilitaryAccess(Country country)
        {
            string saveMilitaryAccess = string.Empty;

            foreach (var militaryAccess in country.GetMilitaryAccess())
                saveMilitaryAccess = saveMilitaryAccess + militaryAccess.Key.name + "&" + militaryAccess.Value;

            country.attrib["Military Access"] = saveMilitaryAccess;
        }

        void SavePolicy(Country country)
        {
            string savePolicy = string.Empty;

            foreach (Policy policy in country.GetAcceptedPolicyList())
                savePolicy = savePolicy + policy.policyID + "~";

            country.attrib["Approved Policy"] = savePolicy;
        }

        void SaveArmsEmbargo(Country country)
        {
            string armsEmbargo = string.Empty;

            foreach (Country embargo in country.GetArmsEmbargo())
                armsEmbargo = armsEmbargo + embargo.name + "~";

            country.attrib["Place Arms Embargo"] = armsEmbargo;
        }

        void SaveTradeEmbargo(Country country)
        {
            string tradeEmbargo = string.Empty;

            foreach(Country embargo in country.GetTradeEmbargo())
                tradeEmbargo = tradeEmbargo + embargo.name + "~";

            country.attrib["Place Trade Embargo"] = tradeEmbargo;
        }


        void SaveProducibleWeapons(Country country)
        {
            string weapons = string.Empty;

            foreach (int ID in country.GetProducibleWeapons())
                weapons = weapons + ID + "~";

            country.attrib["Producible Weapons"] = weapons;
        }

        void SaveWar(Country country)
        {
            string warSave = string.Empty;

            foreach (War war in country.GetWarList())
            {
                warSave = warSave + war.GetEnemyCountry(country).name + "~";
            }

            country.attrib["War"] = warSave;
        }

        void SaveActions(Country country)
        {
            string actionParse = string.Empty;

            foreach(Action action in country.GetActionList())
            {
                string actionSave = action.ActionName + "&" +
                    action.Country.name + "&" +
                    action.Division.divisionName + "&" +
                    action.EarnMoney + "&" +
                    action.EventFinishTime + "&" +
                    action.MilitaryAccess + "&" +
                    action.PayMoney + "&" +
                    action.Province.name + "&" +
                    action.Weapon.weaponID + "&" +
                    action.WeaponAmount;

                actionParse = actionParse + actionSave + "~";
            }

            country.attrib["Action List"] = actionParse;
        }

        void SaveIntelligenceAgency(Country country)
        {
            IntelligenceAgency intelligenceAgency = country.Intelligence_Agency;

            if(intelligenceAgency != null)
            {
                country.attrib["Intelligence Agency Name"] = intelligenceAgency.IntelligenceAgencyName;
                country.attrib["Intelligence Agency Level"] = intelligenceAgency.IntelligenceAgencyLevel;
                country.attrib["Intelligence Agency Budget"] = intelligenceAgency.IntelligenceAgencyBudget;

                country.attrib["Intelligence Agency Military Coup"] = intelligenceAgency.MilitaryCoup;
                country.attrib["Intelligence Agency Reverse Enginering"] = intelligenceAgency.ReverseEnginering;
                country.attrib["Intelligence Agency Assassination"] = intelligenceAgency.Assassination;
            }
        }

        public void SaveGame()
        {
            Debug.Log("Game Saving...");

            SaveCountry();

            string countryAttributes = map.GetCountriesAttributes(true);
            string cityAttributes = map.GetCitiesAttributes(true);

            File.WriteAllText(CONTRY_ATTRIBUTES_FILENAME, countryAttributes, Encoding.UTF8);
            File.WriteAllText(CITY_ATTRIBUTES_FILENAME, cityAttributes, Encoding.UTF8);

            SaveAllData();

            Debug.Log("Game Saved");
        }

        public void LoadGame()
        {
            if (File.Exists(CONTRY_ATTRIBUTES_FILENAME))
            {
                Debug.Log("Game Loading...");

                LoadCountry();

                string data = File.ReadAllText(CONTRY_ATTRIBUTES_FILENAME);
                map.SetCountriesAttributes(data);
                LoadAllData();

                Debug.Log("Game Loaded...");
            }
        }

        void SaveAllData()
        {
            // Store current countries information and frontiers data in string variables
            countryGeoData = map.GetCountryGeoData();
            countryAttributes = map.GetCountriesAttributes();

            // Same for provinces. This wouldn't be neccesary if you are not using provinces in your app.
            provinceGeoData = map.GetProvinceGeoData();
            provinceAttributes = map.GetProvincesAttributes();

            // Same for cities. This wouldn't be neccesary if you are not using cities in your app.
            cityGeoData = map.GetCityGeoData();
            cityAttributes = map.GetCitiesAttributes();

            // Same for mount points. This wouldn't be neccesary if you are not using mount points in your app.
            mountPointGeoData = map.GetMountPointsGeoData();
            mountPointAttributes = map.GetMountPointsAttributes();
        }

        void LoadAllData()
        {
            // Load country information from a string.
            map.SetCountryGeoData(countryGeoData);
            map.SetCountriesAttributes(countryAttributes);

            // Same for provinces. This wouldn't be neccesary if you are not using provinces in your app.
            map.SetProvincesGeoData(provinceGeoData);
            map.SetProvincesAttributes(provinceAttributes);

            // Same for cities. This wouldn't be neccesary if you are not using cities in your app.
            map.SetCityGeoData(cityGeoData);
            map.SetCitiesAttributes(cityAttributes);

            // Same for mount points. This wouldn't be neccesary if you are not using mount points in your app.
            map.SetMountPointsGeoData(mountPointGeoData);
            map.SetMountPointsAttributes(mountPointAttributes);

            // Redraw everything
            map.Redraw();
        }
    }
}