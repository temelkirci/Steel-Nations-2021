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

                if (GameSettings.Instance.GetSelectedGameMode() == GameSettings.GAME_MODE.QUIZ)
                {

                }
                else
                {
                    CountryManager.Instance.DailyUpdateForAllCountries();

                    if (GetDayOfMonth() == 5)
                    {
                        HUDManager.Instance.PrivateNotification("Auto Saving !... ");
                        //SaveLoadManager.Instance.SaveGame();
                        CountryManager.Instance.MonthlyUpdateForAllCountries();

                        if (GetMonthOfYear() == 1)
                        {
                            // new year
                        }
                    }

                    if (GetDayOfMonth() == 15)
                    {
                        int index = UnityEngine.Random.Range(0, OrganizationManager.Instance.GetAllOrganizations().Count);

                        OrganizationManager.Instance.GetAllOrganizations()[index].ResultForApply();
                    }

                    if (GetDayOfMonth() == 28)
                    {
                        //CountryManager.Instance.MonthlyUpdateForAllCountries();
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

            today = new DateTime(2020, 1, 1);

            HUDManager.Instance.ShowHUD();

            if (GameSettings.Instance.GetSelectedGameMode() == GameSettings.GAME_MODE.QUIZ)
            {
                MapManager.Instance.ColorizeCountry(GetPlayer().GetMyCountry());
                //MapManager.Instance.TextureCountry(GetPlayer().GetMyCountry());

                QuizManager.Instance.Init();
                HUDManager.Instance.bottomButtons.SetActive(false);

                GameSettings.Instance.SetGameSpeed_X8();
            }
            else
            {
                GameSettings.Instance.SetGameSpeed_X1();
                ArmyPanel.Instance.Init();

                ActionManager.Instance.Init();

                if (GetPlayer().GetMyCountry().IsAllDivisionsCreated() == false)
                    GetPlayer().GetMyCountry().CreateAllDivisions();

                MapManager.Instance.StartEventListener();
                MapManager.Instance.ListenVehicleEvents();

                MapManager.Instance.StartListeningCountries();
                UIManager.Instance.StartButtonListeners();           

                /*
                if (GetPlayer().GetMyCountry().GetArmy() != null)
                {
                    Dictionary<WeaponTemplate, int> allWeapons = GetPlayer().GetMyCountry().GetArmy().GetAllWeaponsInArmyInventory();
                    foreach (WeaponTemplate weaponTemplate in WeaponManager.Instance.GetWeaponTemplateList())
                    {
                        int number = 0;
                        allWeapons.TryGetValue(weaponTemplate, out number);

                        if (number > 0)
                        {
                            Debug.Log(weaponTemplate.weaponName + " -> " + number);
                        }
                    }
                }
                */
            }

            coroutine = IncreaseDay();
            StartCoroutine(coroutine);
        }
    }

}

