using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WorldMapStrategyKit
{
    public class HUDManager : MonoBehaviour
    {
        private static HUDManager instance;
        public static HUDManager Instance
        {
            get { return instance; }
        }

        WMSK map;

        public GameObject HUD;
        public GameObject selectCountryText;

        public GameObject chatPanel;
        public GameObject minimap;
        public GameObject searchInputText;

        public TextMeshProUGUI myIron;
        public TextMeshProUGUI mySteel;
        public TextMeshProUGUI myUranium;
        public TextMeshProUGUI myAluminium;
        public TextMeshProUGUI myOil;

        public Button search;

        public TextMeshProUGUI myBudget;
        public TextMeshProUGUI date;
        public TextMeshProUGUI tension;

        public TextMeshProUGUI infoText;

        public RawImage MyCountryFlag;

        bool showMinimap = false;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;
        }

        public void ShowHUD()
        {
            HUD.SetActive(true);
            CreateListeners();

            ShowMyCountryFlag();
            UpdateHUD();
            UpdateTension();
            ShowHideChatPanel(false);
            ShowHideMinimap(true);
        }

        public void UpdateHUD()
        {
            UpdateDefenseBudget();
            UpdateMyResources();
            UpdateDate();
            UpdateTension();
        }

        public void UpdateDate()
        {
            date.text = GameEventHandler.Instance.GetToday().ToShortDateString();
        }
        public void UpdateDefenseBudget()
        {
            myBudget.text = "$ " + string.Format("{0:#,0}", float.Parse(GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy().GetDefenseBudget().ToString())) + " M";
        }
        public void ShowMyCountryFlag()
        {
            MyCountryFlag.texture = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetCountryFlag();
        }
        public void UpdateTension()
        {
            tension.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetTension().ToString();
        }

        public void ShowInfoText(string info)
        {
            infoText.text = info;
        }
        public void UpdateMyResources()
        {
            myOil.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetOil().ToString();
            myIron.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetIron().ToString();
            myUranium.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetUranium().ToString();
            mySteel.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetSteel().ToString();
            myAluminium.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetAluminium().ToString();
        }

        public void ShowHideChatPanel(bool show)
        {
            Vector3 pos = chatPanel.transform.localPosition;

            if (show)
            {
                pos.x = pos.x - 850;
            }
            else
            {
                pos.x = pos.x + 850;
            }

            chatPanel.transform.localPosition = pos;
        }

        public void ShowSelectCountryText(bool show)
        {
            selectCountryText.SetActive(show);
        }

        public void ShowHideMinimap(bool show)
        {
            showMinimap = show;
            ShowHideMinimap();
        }
        public void ShowHideMinimap()
        {
            Vector3 pos = minimap.transform.localPosition;

            if (showMinimap == false)
            {
                pos.x = pos.x + 550;
            }
            else
            {
                pos.x = pos.x - 600;
            }
            showMinimap = !showMinimap;

            minimap.transform.localPosition = pos;
        }

        void CreateListeners()
        {
            search.onClick.AddListener(() => SearchCountryOrCity());
        }

        public void SearchCountryOrCity()
        {
            string text = searchInputText.GetComponent<TMP_InputField>().text;
            Country country = map.GetCountry(text);

            if (country != null)
            {
                map.BlinkCountry(text, Color.green, Color.black, 0.8f, 0.1f);
            }
            else
            {
                City city = map.GetCity(text);

                if (city != null)
                {
                    // Fly to the location with provided zoom level
                    map.FlyToLocation(city.unity2DLocation, 2.0f, 0.05f);
                    map.BlinkCity(text, map.GetCityCountryName(city), Color.green, Color.black, 0.8f, 0.1f);
                }
            }
        }
    }
}