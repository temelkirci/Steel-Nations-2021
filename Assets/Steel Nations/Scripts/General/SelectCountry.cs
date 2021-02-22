using UnityEngine;
using UnityEngine.UI;

namespace WorldMapStrategyKit
{
    public class SelectCountry : MonoBehaviour
    {
        private static SelectCountry instance;
        public static SelectCountry Instance
        {
            get { return instance; }
        }

        WMSK map;
        Country selectedCountry;
        Player player;
        public GameObject startButton;

        // Start is called before the first frame update
        void Start()
        {
            // Get a reference to the World Map API:
            map = WMSK.instance;
            instance = this;

            map.OnCountryClick += OnCountryClick;
            startButton.GetComponent<Button>().onClick.AddListener(delegate { ChooseCountry(); });
        }

        void OnCountryClick(int countryIndex, int regionIndex, int buttonIndex)
        {
            if (buttonIndex == 0 && GameEventHandler.Instance.IsGameStarted() == false )
            {
                selectedCountry = map.GetCountry(countryIndex);

                GovernmentPanel.Instance.ShowSelectCountryPanel(selectedCountry);
                startButton.SetActive(true);
            }
        }

        void CreatePlayer()
        {
            Player player = new Player();
            player.SetMyCountry(selectedCountry);
            this.player = player;
        }

        public void ChooseCountry()
        {
            CreatePlayer();
            GameEventHandler.Instance.SetPlayer(player);
            HUDManager.Instance.ShowSelectCountryText(false);

            map.OnCountryClick -= OnCountryClick;

            GovernmentPanel.Instance.HidePanel();

            foreach(Country country in CountryManager.Instance.GetAllCountriesWhichHaveArmy())
                DivisionManager.Instance.CreateDivisions(country);

            GameEventHandler.Instance.GameStarted();
        }
    }
}