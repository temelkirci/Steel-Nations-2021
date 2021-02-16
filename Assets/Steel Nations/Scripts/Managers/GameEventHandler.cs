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

        Vector2 targetPosition;
        
        List<Country> countryList = new List<Country>();
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
        void Update()
        {         
            if (gameStarted == false)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.PauseGame();
            }
            
            if(GetPlayer().GetMouseOverUnit() != null)
            {
                ShowTooltip();
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Color rectangleFillColor = new Color(1f, 1f, 1f, 0.38f);
                Color rectangleLineColor = Color.green;
                map.RectangleSelectionInitiate(rectangleSelectionCallback, rectangleFillColor, rectangleLineColor);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearCurrentSelection();
            }
            
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

                DailyUpdateForCountry();

                if (GetDayOfMonth() == 1)
                {
                    // save game
                }
                if (GetDayOfMonth() == 5)
                {
                    UpdateResources();
                }
                if (GetDayOfMonth() == 10)
                {

                }
                if (GetDayOfMonth() == 15)
                {
                    Natality();
                }
                if (GetDayOfMonth() == 20)
                {
                    Economy();
                }

                if (GetDayOfMonth() == 25)
                {
                    foreach (Organization org in OrganizationManager.Instance.GetAllOrganizations())
                    {
                        org.ResultForApply();
                    }
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
        public void AddCountry(Country country)
        {
            countryList.Add(country);
        }
        public List<Country> GetAllCountries()
        {
            return countryList;
        }
        public void SetPlayer(Player player)
        {
            this.player = player;
        }
        public Player GetPlayer()
        {
            return player;
        }

        void StartEventListener()
        {
            // plug our mouse move listener - it received the x,y map position of the mouse
            map.OnMouseMove += (float x, float y) => 
            {
                if (GetPlayer().GetSelectedDivision() != null)
                {
                    MapManager.Instance.UpdateRoutePathLine(x, y);
                }
            };

            map.OnClick += ( float x, float y, int buttonIndex ) =>
            {
                targetPosition = new Vector2(x, y);

                //bool isContainWater = map.ContainsWater(targetPosition);

                if (buttonIndex == 0) // left click
				{
                    
                }

                else if(buttonIndex == 1) // right click
                {
                    if (isReadyToMove())
                    {
                        AudioManager.Instance.PlayVoice(VOICE_TYPE.ROGER_THAT);

                        if (GetPlayer().GetSelectedDivision().GetDivision().divisionTemplate.divisionType == DIVISION_TYPE.AIR_DIVISION)
                            StartFlight(targetPosition);
                        else
                            GetPlayer().GetSelectedDivision().MoveTo(targetPosition, GetPlayer().GetSelectedDivision().GetDivision().GetDivisionSpeed());
                    }
                    if (GetPlayer().GetSelectedDivision().GetDivision().divisionTemplate.divisionType == DIVISION_TYPE.AIR_DIVISION)
                    {
                        StartFlight(targetPosition);
                        AudioManager.Instance.PlayVoice(VOICE_TYPE.ATTACK);
                    }

                    else
                    {
                        if (isReadyToAttack())
                        {
                            MapManager.Instance.PrepareToAttack(targetPosition);
                            AudioManager.Instance.PlayVoice(VOICE_TYPE.ATTACK);
                        }
                    }

                    AudioManager.Instance.PlayVoice(VOICE_TYPE.YES_SIR);
                }

            };
            map.CenterMap();
        }

        public Country GetCountryByName(string countryName)
        {
            foreach (Country country in countryList)
            {
                if (country.name == countryName)
                {
                    return country;
                }
            }
            return null;
        }

        
        void rectangleSelectionCallback(Rect rect, bool finishRectangleSelection)
        {
            if (finishRectangleSelection)
            {
                GetPlayer().SetSelectedUnits(map.VGOGet(rect));

                foreach (GameObjectAnimator GOA in GetPlayer().GetSelectedUnits())
                    if (GOA.isBuilding())
                        GetPlayer().GetSelectedUnits().Remove(GOA);

                if (GetPlayer().GetSelectedUnits().Count > 0)
                {
                    foreach (GameObjectAnimator go in GetPlayer().GetSelectedUnits())
                    {
                        Debug.Log(go.name);
                        //go.GetComponentInChildren<Renderer>().material.color = Color.blue;
                    }
                }
                else
                {
                    ClearCurrentSelection();
                }
            }
        }

        void ClearCurrentSelection()
        {
            foreach (GameObjectAnimator go in GetPlayer().GetSelectedUnits())
            {
                go.GetComponentInChildren<Renderer>().material.color = go.attrib["Color"];
            }
            GetPlayer().GetSelectedUnits().Clear();
        }
       
        

        public bool isReadyToAttack()
        {
            if (GetPlayer().GetSelectedDivision() != null && GetPlayer().GetSelectedDivision().isDivision())
                return true;
            else
                return false;
        }

        public bool isReadyToMove()
        {
            if (GetPlayer().GetSelectedDivision() != null && GetPlayer().GetSelectedDivision().isDivision())
                return true;
            else
                return false;
        }

        public void GameStarted()
        {
            today = new DateTime(2020, 1, 1);
            AudioManager.Instance.PlayVoice(VOICE_TYPE.GAME_BACKGROUND, true);

            gameStarted = true;

            HUDManager.Instance.ShowHUD();
            BuildingManager.Instance.CreateBuildings();
            DivisionManager.Instance.CreateDivisions();

            StartEventListener();                
            ListenVehicleEvents();

            MapManager.Instance.StartListeningCountries();
            UIManager.Instance.StartButtonListeners();
            coroutine = IncreaseDay(2.0f);
            StartCoroutine(coroutine);
        }

        public int GetCurrentYear()
        {
            return today.Year;
        }

        void DailyUpdateForCountry()
        {
            foreach (Country country in countryList)
            {
                if (country != null)
                {
                    country.DailyUpdateForCountry();
                    country.UpdateAllConstructionInCountry();
                    country.UpdateResearchInProgress();
                    country.UpdateProductionInProgress();

                    foreach(Country tempCountry in country.GetNuclearWar())
                    {
                        // Launch 5 waves of attacks and counter-attacks
                        //for (int wave = 0; wave < 1; wave++)
                        //{
                            StartCoroutine(MapManager.Instance.War(country, tempCountry, 1));
                        //}
                    }

                    country.GetNuclearWar().Clear();
                }
            }
        }

        void UpdateResources()
        {
            foreach (Country tempCountry in countryList)
            {
                if (tempCountry != null)
                {
                    tempCountry.UpdateResourcesInCountry();
                }
            }
        }
        void Economy()
        {
            foreach (Country country in countryList)
            {
                if (country != null)
                {
                    country.UpdateEconomy();
                }
            }
        }

        void Natality()
        {
            foreach (Country tempCountry in countryList)
            {
                if (tempCountry != null)
                {
                    tempCountry.UpdateNatality();
                }
            }
        }

		
        void UnitSelection(GameObjectAnimator anim)
        {
            GetPlayer().SetSelectedBuilding(null);
            GetPlayer().SelectDivision(anim);
            DivisionManagerPanel.Instance.ShowDivisionPanel();

            if (anim.isBuilding())
            {
                GetPlayer().SetSelectedBuilding(anim);
                GetPlayer().SelectDivision(null);
            }
        }        

        void ListenVehicleEvents()
        {
        	foreach(GameObjectAnimator vehicle in GetPlayer().GetMyCountry().GetArmy().GetAllDivisionInArmy())
        	{
	        	// Listen to unit-level events (if you need unit-level events...)
				vehicle.OnPointerEnter += (GameObjectAnimator anim) => GetPlayer().SetMouseOverUnit(anim);       
                vehicle.OnPointerExit += (GameObjectAnimator anim) => GetPlayer().SetMouseOverUnit(null);
                vehicle.OnPointerUp += (GameObjectAnimator anim) => UnitSelection(anim);
                vehicle.OnPointerDown += (GameObjectAnimator anim) => UnitSelection(anim);
            }

            foreach(City city in GetPlayer().GetMyCountry().GetAllCitiesInCountry())
        	{
                if (city.GetDockyard() != null)
                {
                    // Listen to unit-level events (if you need unit-level events...)
                    city.GetDockyard().OnPointerEnter += (GameObjectAnimator anim) => GetPlayer().SetMouseOverUnit(anim);
                    city.GetDockyard().OnPointerExit += (GameObjectAnimator anim) => GetPlayer().SetMouseOverUnit(null);
                    city.GetDockyard().OnPointerDown += (GameObjectAnimator anim) => UnitSelection(anim);
                }
            }          			
        }
        
        public void ShowTooltip()
        {
            string tooltipText = string.Empty;

            if (GetPlayer().GetMouseOverUnit().isBuilding() == true)
            {
                tooltipText = GetPlayer().GetMouseOverUnit().name;
            }
            else
            {       
                string line1 = GetPlayer().GetMouseOverUnit().name;
                string line2 = "Speed : " + GetPlayer().GetMouseOverUnit().GetDivision().GetDivisionSpeed().ToString();
                tooltipText = line1 + "\n" + line2 + "\n";
            }

            GetPlayer().GetMouseOverUnit().gameObject.GetComponent<SimpleTooltip>().infoLeft = tooltipText + "\n";
        }
      
        void StartFlight(Vector2 destination)
        {
            GetPlayer().GetSelectedDivision().arcMultiplier = 5f;     // tempCountry is the arc for the plane trajectory
            GetPlayer().GetSelectedDivision().easeType = EASE_TYPE.SmootherStep;    // make it an easy-in-out movement

            GetPlayer().GetSelectedDivision().MoveTo(destination, GetPlayer().GetSelectedDivision().GetDivision().GetDivisionSpeed());

            Vector2 startingPos = GetPlayer().GetSelectedDivision().startingMap2DLocation;
            GetPlayer().GetSelectedDivision().comeBack = true;
            GetPlayer().GetSelectedDivision().altitudeEnd = 5;
            GetPlayer().GetSelectedDivision().altitudeStart = 0.1f;

            GetPlayer().GetSelectedDivision().OnCountryEnter += (GameObjectAnimator anim) => 
            {

            };
            GetPlayer().GetSelectedDivision().OnKilled += (GameObjectAnimator anim) =>
            {

            };
            GetPlayer().GetSelectedDivision().OnMoveStart += (GameObjectAnimator anim) =>
            {
                AudioManager.Instance.PlayVoice(VOICE_TYPE.FIGHTER);

                if(GetPlayer().GetSelectedDivision().comeBack == false && startingPos != GetPlayer().GetSelectedDivision().destination)
                {
                    GetPlayer().GetSelectedDivision().altitudeEnd = 0.1f;
                    GetPlayer().GetSelectedDivision().altitudeStart = 5f;
                }
            };

            GetPlayer().GetSelectedDivision().OnMoveEnd += (GameObjectAnimator anim) => 
            {
                if (startingPos != GetPlayer().GetSelectedDivision().destination && GetPlayer().GetSelectedDivision().comeBack == true)
                {
                    GetPlayer().GetSelectedDivision().comeBack = false;

                    GetPlayer().GetSelectedDivision().altitudeEnd = 0.1f;
                    GetPlayer().GetSelectedDivision().altitudeStart = 5;

                    GetPlayer().GetSelectedDivision().MoveTo(startingPos, GetPlayer().GetSelectedDivision().GetDivision().GetDivisionSpeed());
                    MapManager.Instance.ShowExplosion(GetPlayer().GetSelectedDivision().destination);
                }
                else
                {
                    GetPlayer().GetSelectedDivision().altitudeStart = 0.1f;
                    GetPlayer().GetSelectedDivision().currentAltitude = 0.1f;
                    GetPlayer().GetSelectedDivision().altitudeEnd = 0.1f;
                }
            };    // once the movement has finished, stop following the unit
        }
        
    }

}

