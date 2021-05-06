using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace WorldMapStrategyKit
{
    public class Statistics : MonoBehaviour
    {
        private static Statistics instance;
        public static Statistics Instance
        {
            get { return instance; }
        }

        public GameObject statisticsPanel;
        public GameObject economyItem;
        public GameObject economyContent;

        WMSK map;

        void Start()
        {
            instance = this;
            map = WMSK.instance;
        }

        public void HidePanel()
        {
            statisticsPanel.SetActive(false);
        }

        public void UpdateRanks()
        {
            UpdateManpowerRank();

            UpdateGDPRank();
            UpdateBudgetRank();
            UpdateDefenseBudgetRank();
            UpdateDebtRank();

            UpdateMilitaryRank();
            UpdateTaxRank();

            //UpdateLandForcesRank();
            //UpdateAirForcesRank();
            //UpdateNavalForcesRank();
        }

        public void UpdateDebtRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.Debt).ToList();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.debtRank = rank;
            }
        }

        public void UpdateTaxRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.Individual_Tax).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.taxRank = rank;
            }
        }

        public void UpdateGDPRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.Current_GDP).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.GDPRank = rank;
            }
        }

        public void UpdateManpowerRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => CountryManager.Instance.GetAvailableManpower(x)).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.manpowerRank = rank;
            }
        }

        public void UpdateLandForcesRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.GetArmy().GetLandForces().GetMilitaryPower()).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.landPowerRank = rank;
            }
        }

        public void UpdateAirForcesRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.GetArmy().GetAirForces().GetMilitaryPower()).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.airPowerRank = rank;
            }
        }

        public void UpdateNavalForcesRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.GetArmy().GetNavalForces().GetMilitaryPower()).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.navalPowerRank = rank;
            }
        }

        public void UpdateDefenseBudgetRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.Defense_Budget).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.defenseBudgetRank = rank;
            }
        }

        public void UpdateBudgetRank()
        {
            List<Country> sortedCountry = null;

            sortedCountry = map.countries.OrderBy(x => x.Budget).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.budgetRank = rank;
            }
        }
        public void UpdateMilitaryRank()
        {
            List<Country> sortedCountry = map.countries.OrderBy(x => x.GetArmy().GetArmyPower()).ToList();
            sortedCountry.Reverse();

            int rank = 0;
            foreach (Country country in sortedCountry)
            {
                rank++;
                country.militaryRank = rank;
            }
        }

        public void ShowEconomy()
        {
            statisticsPanel.SetActive(true);

            UpdateRanks();

            foreach (Transform child in economyContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach(Country country in map.countries)
            {
                if(country != null)
                {
                    GameObject temp = Instantiate(economyItem, economyContent.transform);
                    temp.gameObject.transform.GetChild(3).transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
                    temp.gameObject.transform.GetChild(3).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = country.name;
                    temp.gameObject.transform.GetChild(3).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = country.manpowerRank.ToString();
                    temp.gameObject.transform.GetChild(3).transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = country.GDPRank.ToString();
                    temp.gameObject.transform.GetChild(3).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = country.budgetRank.ToString();
                    temp.gameObject.transform.GetChild(3).transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = country.defenseBudgetRank.ToString();
                    temp.gameObject.transform.GetChild(3).transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = country.debtRank.ToString();
                    temp.gameObject.transform.GetChild(3).transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = country.militaryRank.ToString();
                }
            }
        }
    }
}