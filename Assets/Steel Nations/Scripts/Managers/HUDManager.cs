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
        public GameObject bottomButtons;

        public TextMeshProUGUI myBudget;
        public TextMeshProUGUI date;
        public TextMeshProUGUI tension;

        public GameObject privateNotificationGO;

        public RawImage MyCountryFlag;
        public Button FlagButton;
        public GameObject selectedDivisions;
        public GameObject selectedDivisionItem;

        bool showMinimap = false;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;

            FlagButton.onClick.AddListener(FocusOnMyCountry);
        }

        public void Init()
        {
        }

        public void ShowHUD()
        {
            HUD.SetActive(true);
            bottomButtons.SetActive(true);

            CreateListeners();

            ShowMyCountryFlag();
            UpdateHUD();
            UpdateTension();
            ShowHideChatPanel(false);
            ShowHideMinimap(true);
        }

        public void UpdateHUD()
        {
            if(GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy() != null)
            {
                UpdateDefenseBudget();
            }

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
            //myBudget.text = "$ " + string.Format("{0:#,0}", float.Parse(GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy().Defense_Budget.ToString())) + " M";
            myBudget.text = "$ " + string.Format("{0:#,0}", float.Parse(GameEventHandler.Instance.GetPlayer().GetMyCountry().Budget.ToString())) + " M";
        }
        public void ShowMyCountryFlag()
        {
            Texture2D flag = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetCountryFlag();

            MyCountryFlag.texture = flag;
        }
        public void UpdateTension()
        {
            tension.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().Tension.ToString();
        }

        public void UpdateMyResources()
        {
            Country country = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            int oil = country.GetMineral(MINERAL_TYPE.OIL);
            int iron = country.GetMineral(MINERAL_TYPE.IRON);
            int uranium = country.GetMineral(MINERAL_TYPE.URANIUM);
            int steel = country.GetMineral(MINERAL_TYPE.STEEL);
            int aluminium = country.GetMineral(MINERAL_TYPE.ALUMINIUM);

            myOil.text = ThausandConvertToString(oil);
            myIron.text = ThausandConvertToString(iron);
            myUranium.text = ThausandConvertToString(uranium);
            mySteel.text = ThausandConvertToString(steel);
            myAluminium.text = ThausandConvertToString(aluminium);
        }

        public string ThausandConvertToString(int value)
        {
            string str = string.Empty;

            if (value > 999)
                str = (value / 1000) + "K";
            else
                str = value.ToString();

            return str;
        }

        public void PrivateNotification(string news)
        {
            privateNotificationGO.SetActive(true);
            privateNotificationGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = news;

            Invoke("HidePrivateNotification", 5);
        }

        void HidePrivateNotification()
        {
            privateNotificationGO.SetActive(false);
        }

        public void FocusOnMyCountry()
        {
            int capitalIndex = map.GetCountryIndex(GameEventHandler.Instance.GetPlayer().GetMyCountry());
            City city = map.GetCountryCapital(capitalIndex);
            map.FlyToLocation(city.unity2DLocation, 2f, 0.1f);
        }

        public void ShowSelectedDivisions()
        {
            selectedDivisions.SetActive(true);

            foreach (GameObjectAnimator GOA in GameEventHandler.Instance.GetPlayer().GetSelectedDivisions())
            {
                GameObject temp = Instantiate(selectedDivisionItem, selectedDivisions.transform);
                temp.GetComponent<RawImage>().texture = GOA.GetDivision().GetDivisionIcon();

                Division division = GOA.GetDivision();

                string line1 = division.divisionName;
                string line2 = "Speed : " + division.GetDivisionSpeed().ToString();
                //string line3 = "Soldier : " + division.currentSoldier.ToString();
                string line4 = "Attack Power : " + division.GetDivisionPower().ToString();
                string line5 = "Defense : " + division.GetDivisionDefense().ToString();
                string line6 = "Minimum Attack Range : " + division.GetDivisionMinimumAttackRange().ToString();
                string line7 = "Maximum Attack Range : " + division.GetDivisionMaximumAttackRange().ToString();

                string tooltipText = line1 + "\n" + "\n" + line2 + "\n" + line4 + "\n" + line5 + "\n" + line6 + "\n" + line6 + "\n";

                temp.GetComponent<SimpleTooltip>().infoLeft = tooltipText + "\n";

                temp.GetComponent<Button>().onClick.AddListener(() => ShowAndFocusDivision(GOA));
            }
        }

        public void ShowAndFocusDivision(GameObjectAnimator GOA)
        {
            DivisionManagerPanel.Instance.ShowDivisionPanel(GOA.GetDivision());
            // Fly to the destination and see the building created
            map.FlyToLocation(GOA.currentMap2DLocation, 2f, 0.02f);
        }

        public void ClearSelectedDivisions()
        {
            selectedDivisions.SetActive(false);
            foreach (Transform child in selectedDivisions.transform)
            {
                Destroy(child.gameObject);
            }
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
                pos.x = pos.x + 600;
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