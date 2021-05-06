using UnityEngine;
using TMPro;

namespace WorldMapStrategyKit
{
    public class IntelligenceAgencyPanel : MonoBehaviour
    {
        private static IntelligenceAgencyPanel instance;
        public static IntelligenceAgencyPanel Instance
        {
            get { return instance; }
        }

        public GameObject intelligenceAgencyPanel;
        public TextMeshProUGUI intelligenceAgencyName;
        public TextMeshProUGUI intelligenceAgencyBudget;

        public TextMeshProUGUI assassination;
        public TextMeshProUGUI reverseEngineering;
        public TextMeshProUGUI militaryCoup;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void ShowIntelligenceAgency()
        {
            intelligenceAgencyPanel.SetActive(true);

            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            intelligenceAgencyName.text = myCountry.Intelligence_Agency.IntelligenceAgencyName;
            intelligenceAgencyBudget.text = myCountry.Intelligence_Agency.IntelligenceAgencyBudget.ToString();

            assassination.text = "% " + myCountry.Intelligence_Agency.Assassination.ToString();
            reverseEngineering.text = "% " + myCountry.Intelligence_Agency.ReverseEnginering.ToString();
            militaryCoup.text = "% " + myCountry.Intelligence_Agency.MilitaryCoup.ToString();

        }

        public void HideIntelligenceAgency()
        {
            intelligenceAgencyPanel.SetActive(false);
        }
    }
}
