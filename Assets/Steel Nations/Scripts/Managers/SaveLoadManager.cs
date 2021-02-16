using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class SaveLoadManager
    {
        private static SaveLoadManager instance;
        public static SaveLoadManager Instance
        {
            get { return instance; }
        }

        const string ATTRIBUTES_FILENAME = "CountriesAttributes";
        string countryGeoData, countryAttributes, provinceGeoData, provinceAttributes, cityGeoData, cityAttributes, mountPointGeoData, mountPointAttributes;

        WMSK map;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;

            // Get a reference to the World Map API:
            map = WMSK.instance;
        }

        public void SaveButtonClick()
        {
            Debug.Log("Game Saving...");

            string countryAttributes = map.GetCountriesAttributes(true);
            File.WriteAllText(ATTRIBUTES_FILENAME, countryAttributes, Encoding.UTF8);
            SaveAllData();

            Debug.Log("Game Saved");
        }

        public void LoadButtonClick()
        {
            if (File.Exists(ATTRIBUTES_FILENAME))
            {
                Debug.Log("Game Loading...");

                string data = File.ReadAllText(ATTRIBUTES_FILENAME);
                map.SetCountriesAttributes(data);
                //LoadAllData();

                Debug.Log("Game Loaded...");
            }
        }

        public void SaveAllData()
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