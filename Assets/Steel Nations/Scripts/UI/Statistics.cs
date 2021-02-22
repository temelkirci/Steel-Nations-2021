using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        void Start()
        {
            instance = this;
        }

        public void HidePanel()
        {
            statisticsPanel.SetActive(false);
        }

        public void ShowEconomy()
        {
            statisticsPanel.SetActive(true);

            foreach (Transform child in economyContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach(Country country in CountryManager.Instance.GetAllCountries())
            {
                if(country != null)
                {
                    GameObject temp = Instantiate(economyItem, economyContent.transform);
                    temp.gameObject.transform.GetChild(3).transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
                    temp.gameObject.transform.GetChild(3).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = country.name;
                    temp.gameObject.transform.GetChild(3).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "$ " + string.Format("{0:#,0}", float.Parse(country.GetBudget().ToString())) + "M";
                    temp.gameObject.transform.GetChild(3).transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$ " + string.Format("{0:#,0}", float.Parse(country.attrib["Previous GDP"])) + "M";
                    temp.gameObject.transform.GetChild(3).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "$ " + string.Format("{0:#,0}", float.Parse(country.attrib["Previous Export"])) + "M";
                    temp.gameObject.transform.GetChild(3).transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "$ " + string.Format("{0:#,0}", float.Parse(country.attrib["Previous Import"])) + "M";
                    temp.gameObject.transform.GetChild(3).transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = country.attrib["Trade Ratio"];
                    temp.gameObject.transform.GetChild(3).transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "$ " + string.Format("{0:#,0}", float.Parse(country.attrib["Debt"])) + "M";
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}