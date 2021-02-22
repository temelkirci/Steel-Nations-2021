using UnityEngine;
using System.Collections.Generic;
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

        WMSK map;
        
        Player player;

        DateTime today;
        bool gameStarted = false;
        private IEnumerator coroutine;

        void Start()
        {
            instance = this;

            // Get a reference to the World Map API:
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

        private IEnumerator IncreaseDay(float waitTime)
        {
            while(map.paused == false)
            {
                yield return new WaitForSeconds(waitTime);

                today = today.AddDays(1);

                if(GetDayOfMonth() == 1 && GetMonthOfYear() == 1)
                {
                    // new year
                }

                CountryManager.Instance.DailyUpdateAllCountries();

                if (GetDayOfMonth() == 1)
                {
                    // save game
                }
                if (GetDayOfMonth() == 5)
                {
                    
                }
                if (GetDayOfMonth() == 10)
                {

                }
                if (GetDayOfMonth() == 15)
                {
                    CountryManager.Instance.MonthlyUpdateAllCountries();
                }
                if (GetDayOfMonth() == 20)
                {
                    
                }

                if (GetDayOfMonth() == 25)
                {
                    foreach (Organization org in OrganizationManager.Instance.GetAllOrganizations())
                        org.ResultForApply();
                }

                HUDManager.Instance.UpdateHUD();
                UIManager.Instance.UpdatePanels();
                NotificationManager.Instance.ShowNews();
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
            today = new DateTime(2020, 1, 1);
            AudioManager.Instance.PlayVoice(VOICE_TYPE.GAME_BACKGROUND, true);

            gameStarted = true;

            HUDManager.Instance.ShowHUD();
            BuildingManager.Instance.CreateBuildings();

            MapManager.Instance.StartEventListener();                
            MapManager.Instance.ListenVehicleEvents();

            MapManager.Instance.StartListeningCountries();
            UIManager.Instance.StartButtonListeners();
            coroutine = IncreaseDay(2.0f);
            StartCoroutine(coroutine);
        }

        public int GetCurrentYear()
        {
            return today.Year;
        }

        
    }

}

