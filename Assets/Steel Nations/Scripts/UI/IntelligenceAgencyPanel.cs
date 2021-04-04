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
        public TextMeshProUGUI intelligenceAgencyLevel;
        public TextMeshProUGUI intelligenceAgencyBudget;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }
        public void Init()
        {

        }

        public void ShowIntelligenceAgency()
        {
            intelligenceAgencyPanel.SetActive(true);

            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            intelligenceAgencyName.text = myCountry.Intelligence_Agency.IntelligenceAgencyName;
            intelligenceAgencyLevel.text = myCountry.Intelligence_Agency.IntelligenceAgencyLevel.ToString();
            intelligenceAgencyBudget.text = myCountry.Intelligence_Agency.IntelligenceAgencyBudget.ToString();
        }

        public void HideIntelligenceAgency()
        {
            intelligenceAgencyPanel.SetActive(false);
        }
    }
}
