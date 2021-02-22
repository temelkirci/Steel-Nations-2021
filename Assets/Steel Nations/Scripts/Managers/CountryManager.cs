using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class CountryManager : Singleton<CountryManager>
    {
        List<Country> countryList = new List<Country>();

        public void AddCountry(Country country)
        {
            countryList.Add(country);
        }
        public List<Country> GetAllCountries()
        {
            return countryList;
        }

        public List<Country> GetAllCountriesWhichHaveArmy()
        {
            List<Country> countryHaveArmyList = new List<Country>();

            foreach (Country country in GetAllCountries())
                if(country.GetArmy() != null)
                    countryHaveArmyList.Add(country);

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

        public void DailyUpdateAllCountries()
        {
            foreach (Country country in countryList)
            {
                if (country != null)
                {
                    country.DailyUpdateForCountry();
                    country.UpdateAllConstructionInCountry();
                    country.UpdateResearchInProgress();
                    country.UpdateProductionInProgress();
                }
            }
        }
        public void MonthlyUpdateAllCountries()
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
                    tempCountry.UpdateResourcesInCountry();
                }
            }
        }
        void UpdateEconomy()
        {
            foreach (Country country in countryList)
            {
                if (country != null)
                {
                    country.UpdateEconomy();
                }
            }
        }

        void UpdateBirthRate()
        {
            foreach (Country tempCountry in countryList)
            {
                if (tempCountry != null)
                {
                    tempCountry.UpdateNatality();
                }
            }
        }
    }
}
