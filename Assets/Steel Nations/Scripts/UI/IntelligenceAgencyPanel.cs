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

        public void ShowIntelligenceAgency()
        {
            intelligenceAgencyPanel.SetActive(true);

            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            intelligenceAgencyName.text = myCountry.GetIntelligenceAgency().GetIntelligenceAgencyName();
            intelligenceAgencyLevel.text = myCountry.GetIntelligenceAgency().GetIntelligenceAgencyLevel().ToString();
            intelligenceAgencyBudget.text = myCountry.GetIntelligenceAgency().GetIntelligenceAgencyBudget().ToString();
        }

        public void HideIntelligenceAgency()
        {
            intelligenceAgencyPanel.SetActive(false);
        }
    }
}
