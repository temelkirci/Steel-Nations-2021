using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WorldMapStrategyKit
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance
        {
            get { return instance; }
        }

        public Button economyButton;
        public Button researchButton;
        public Button productionButton;
        public Button organizationButton;
        public Button politiksButton;
        public Button armyButton;
        public Button agentButton;
        public Button statisticsButton;

        WMSK map;
        public GameObject newsGO;
        public GameObject pauseGO;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;
        }

        public void StartButtonListeners()
        {
            economyButton.onClick.AddListener(delegate { EconomyPanel.Instance.ShowEconomyPanel(); });
            researchButton.onClick.AddListener(delegate { ResearchPanel.Instance.ShowResearchPanel(); });
            productionButton.onClick.AddListener(delegate { ProductionPanel.Instance.ShowProductionPanel(); });
            organizationButton.onClick.AddListener(delegate { OrganizationPanel.Instance.ShowOrganizationPanel(); });
            politiksButton.onClick.AddListener(delegate { PolicyPanel.Instance.ShowPolicyPanel(); });
            armyButton.onClick.AddListener(delegate { ArmyPanel.Instance.ShowArmyPanel(); });
            agentButton.onClick.AddListener(delegate { IntelligenceAgencyPanel.Instance.ShowIntelligenceAgency(); });
            statisticsButton.onClick.AddListener(delegate { Statistics.Instance.ShowEconomy(); });
        }

        public void UpdatePanels()
        {
            EconomyPanel.Instance.UpdateEconomyPanel();
            ResearchPanel.Instance.ShowResearchProgress();
            ProductionPanel.Instance.ShowProductionProgress();
            // update organization panel
            // update policy panel
            // update army panel
            // update agent panel
            // update statistics panel
        }

        public void PauseGame()
        {
            map.paused = !map.paused;

            pauseGO.SetActive(map.paused);
        }

        public void News(string news)
        {
            newsGO.SetActive(true);
            newsGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = news;

            Invoke("HideNews", 5);
        }

        void HideNews()
        {
            newsGO.SetActive(false);
        }

    }
}