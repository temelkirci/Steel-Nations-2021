using UnityEngine;
using System;
using System.Collections;

namespace WorldMapStrategyKit
{
    public class GameEventHandler : MonoBehaviour
    {
        private static GameEventHandler instance;
        public static GameEventHandler Instance
        {
            get { return instance; }
        }
        
        Player player;
        WMSK map;

        DateTime today;
        bool gameStarted = false;
        IEnumerator coroutine;

        void Start()
        {
            instance = this;

            map = WMSK.instance;
            map.paused = false;
            map.timeSpeed = 1;
        }

        public int GetDayOfMonth()
        {
            return today.Day;
        }

        public int GetMonthOfYear()
        {
            return today.Month;
        }

        public int GetCurrentYear()
        {
            return today.Year;
        }

        IEnumerator IncreaseDay()
        {
            while(map.paused == false)
            {
                yield return new WaitForSeconds(GameSettings.Instance.GetGameSpeed());

                today = today.AddDays(1);

                HUDManager.Instance.UpdateHUD();
                NotificationManager.Instance.ShowNews();

                if (GameSettings.Instance.GetSelectedGameMode() != GameSettings.GAME_MODE.QUIZ)
                {
                    CountryManager.Instance.DailyUpdateForAllCountries();

                    if (GetDayOfMonth() == 1)
                    {
                        if (GetMonthOfYear() == 1)
                        {
                            // stop game
                            Debug.Log("New Year");
                            if (GameSettings.Instance.GetFinishYear() == GetCurrentYear())
                            {
                                // finish game
                            }
                            else
                            {
                                foreach (Country country in map.countries)
                                    CountryManager.Instance.InitBudget(country);
                            }
                        }
                        else
                        {
                            //SaveLoadManager.Instance.SaveGame();
                        }
                    }
                    else
                    {
                        if (GetDayOfMonth() == 7)
                        {
                            CountryManager.Instance.UpdateBirthRate();

                            foreach (Country country in CountryManager.Instance.GetAllEnemies(GetPlayer().GetMyCountry()))
                                CountryManager.Instance.WarDecision(GetPlayer().GetMyCountry(), country);
                        }

                        if (GetDayOfMonth() == 14)
                        {
                            CountryManager.Instance.UpdateResources();
                        }

                        if (GetDayOfMonth() == 21)
                        {
                            int index = UnityEngine.Random.Range(0, OrganizationManager.Instance.GetAllOrganizations().Count);

                            OrganizationManager.Instance.GetAllOrganizations()[index].ResultForApply();
                        }

                        if (GetDayOfMonth() == 28)
                        {
                            CountryManager.Instance.UpdateEconomy();
                        }
                    }

                    UIManager.Instance.UpdatePanels();
                }

            }
        }

        public bool IsGameStarted()
        {
            return gameStarted;
        }
        public DateTime GetToday()
        {
            return today;
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
        }
        public Player GetPlayer()
        {                
            return player;
        }

        public void GameStarted()
        {
            gameStarted = true;

            //today = new DateTime(2019, 12, 28);
            today = new DateTime(2020, 1, 1);

            HUDManager.Instance.ShowHUD();

            if (GameSettings.Instance.GetSelectedGameMode() == GameSettings.GAME_MODE.QUIZ)
            {
                QuizManager.Instance.Init();
                HUDManager.Instance.bottomButtons.SetActive(false);

                GameSettings.Instance.SetGameSpeed_X8();
            }
            else
            {
                GameSettings.Instance.SetGameSpeed_X1();

                ArmyPanel.Instance.Init();
                ActionManager.Instance.Init();
                ResearchPanel.Instance.Init();

                if (GetPlayer().GetMyCountry().IsAllDivisionsCreated() == false)
                    GetPlayer().GetMyCountry().CreateAllDivisions();

                MapManager.Instance.StartEventListener();

                MapManager.Instance.StartListeningCountries();
                UIManager.Instance.StartButtonListeners();                         
            }

            coroutine = IncreaseDay();
            StartCoroutine(coroutine);
        }
    }

}

