using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace WorldMapStrategyKit
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance
        {
            get { return instance; }
        }

        WMSK map;

        public Button economyButton;
        public Button researchButton;
        public Button productionButton;
        public Button organizationButton;
        public Button politiksButton;
        public Button armyButton;
        public Button agentButton;
        public Button statisticsButton;

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
            DockyardPanel.Instance.ShowProductionProgress();
            CityInfoPanel.Instance.UpdateAllBuildings();
        }

        public void PauseGame()
        {
            map.paused = !map.paused;

            pauseGO.SetActive(map.paused);
        }

        public void ExitToMainMenu()
        {
            SceneManager.LoadScene("Menu Scene");
        }

        public void PublicNotification(string news)
        {
            newsGO.SetActive(true);
            newsGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = news;

            Invoke("HidePublicNotification", 5);
        }

        void HidePublicNotification()
        {
            newsGO.SetActive(false);
        }

    }
}