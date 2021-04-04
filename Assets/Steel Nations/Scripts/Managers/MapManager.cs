using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager instance;
        public static MapManager Instance
        {
            get { return instance; }
        }

        public Texture2D borderTexture;

        LineMarkerAnimator pathLine;
        Material lineMaterialAerial, lineMaterialGround;

        int principalCountryIndex = -1;

        float pathDrawingDuration = 0f;
        // 0 means instant drawing.
        float pathArcElevation = 0f;
        // 0 means ground-level (path will be part of the viewport texture). If you provide a value, path will be 3D.
        float pathLineWidth = 0.01f;
        float pathDashInterval = 0;
        float pathDashAnimationDuration = 0;

        public GameObject marker3D;
        GameObject marker;
        WMSK map;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;

            // Prepare line texture
            lineMaterialAerial = Instantiate(Resources.Load<Material>("PathLine/aerialPath"));
            lineMaterialGround = Instantiate(Resources.Load<Material>("PathLine/groundPath"));

            marker = Instantiate(marker3D);
            AddMouseCursor();
        }

        void Update()
        {
            if (GameSettings.Instance.GetSelectedGameMode() != GameSettings.GAME_MODE.QUIZ)
            {
                if (GameEventHandler.Instance.IsGameStarted() == true)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        UIManager.Instance.PauseGame();
                    }

                    //Debug.Log(map.GetZoomLevel());
                    if (map.GetZoomLevel() < 0.1)
                    {
                        foreach (Country country in map.GetVisibleCountries())
                        {
                            //if (CountryManager.Instance.GetAtWarCountryList(GameEventHandler.Instance.GetPlayer().GetMyCountry()).Contains(country) || country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                            {
                                if (country.IsVisibleAllBuildings() == false)
                                    CountryManager.Instance.HideShowAllBuildings(country, true);
                                if (country.IsVisibleAllDivisions() == false)
                                    CountryManager.Instance.VisibleAllDivisions(country, true);
                            }
                        }
                    }
                    else
                    {
                        foreach (Country country in CountryManager.Instance.GetAllCountries())
                        {
                            if (country.IsVisibleAllBuildings() == true)
                                CountryManager.Instance.HideShowAllBuildings(country, false);
                            if (country.IsVisibleAllDivisions() == true)
                                CountryManager.Instance.VisibleAllDivisions(country, false);

                        }
                    }


                    //if (GetPlayer().GetMouseOverUnit() != null)
                    {
                        ShowTooltip();
                    }

                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        Color rectangleFillColor = new Color(1f, 1f, 1f, 0.38f);
                        Color rectangleLineColor = Color.green;
                        map.RectangleSelectionInitiate(RectangleSelectionCallback, rectangleFillColor, rectangleLineColor);
                    }
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        ClearCurrentSelection();
                    }
                }
            }           
        }

        void AddMouseCursor()
        {
            marker.SetActive(false);
            map.AddMarker3DObject(marker, Vector3.zero);
        }

        void UpdateMouseCursor(Vector3 location)
        {
            marker.SetActive(true);

            map.UpdateMarker3DObjectPosition(marker, location);
        }

        void RectangleSelectionCallback(Rect rect, bool finishRectangleSelection)
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            if (finishRectangleSelection)
            {
                if(map.VGOGet(rect).Count > 0)
                {
                    List<GameObjectAnimator> divisions = new List<GameObjectAnimator>();

                    foreach (GameObjectAnimator GOA in map.VGOGet(rect))
                    {
                        if (GOA.isDivision() && player.IsMyDivision(GOA) && GOA.visible == true)
                        {
                            divisions.Add(GOA);
                        }
                    }

                    MultipleDivisionSelection(divisions);
                }
                else
                {
                    ClearCurrentSelection();
                }
            }
        }

        void ClearCurrentSelection()
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            DestroyPathLine();

            foreach(GameObjectAnimator GOA in player.GetSelectedDivisions())
                GOA.DestroyShield();

            HUDManager.Instance.ClearSelectedDivisions();

            player.ClearSelectedDivisions();
        }

        public void StartListeningCountries()
        {
            /* Register events: this is optionally but allows your scripts to be informed instantly as the mouse enters or exits a country, province or city */
            //map.OnCityEnter += OnCityEnter;
            //map.OnCityExit += OnCityExit;
            map.OnCityClick += OnCityClick;
            //map.OnCountryEnter += (int countryIndex, int regionIndex) => ShowBorderTexture(countryIndex, true);
            //map.OnCountryExit += (int countryIndex, int regionIndex) => ShowBorderTexture(countryIndex, false);
            map.OnCountryClick += OnCountryClick;
            //map.OnProvinceEnter += OnProvinceEnter;
            //map.OnProvinceExit += OnProvinceExit;
            //map.OnProvinceClick += OnProvinceClick;
        }


        void OnCityClick(int cityIndex, int buttonIndex)
        {
            ClearCurrentSelection();

            if (buttonIndex == 0)
            {
                City city = map.GetCity(cityIndex);
                GameEventHandler.Instance.GetPlayer().SetSelectedCity(city);

                if (CountryManager.Instance.GetAllCitiesInCountry(GameEventHandler.Instance.GetPlayer().GetMyCountry()).Contains(city) == true)
                {
                    CityInfoPanel.Instance.ShowCityMenu();
                }     
                else
                {
                    CityInfoPanel.Instance.ShowCityInfo();
                }
            }
        }

        /*
        void OnProvinceClick(int provinceIndex, int regionIndex, int buttonIndex)
        {
            //ClearCurrentSelection();
        }

        void OnProvinceEnter(int provinceIndex, int regionIndex)
        {
            //Province province = map.GetProvince(provinceIndex);

            //HUDManager.Instance.ShowInfoText(province.name);
        }

        void OnProvinceExit(int provinceIndex, int regionIndex)
        {
            //Province province = map.GetProvince(provinceIndex);

            //HUDManager.Instance.ShowInfoText("");
        }
        */

        void OnCountryClick(int countryIndex, int regionIndex, int buttonIndex)
        {
            //ClearCurrentSelection();

            if (buttonIndex == 0)
            {
                Country tempCountry = map.GetCountry(countryIndex);
                if (tempCountry != null)
                {
                    GameEventHandler.Instance.GetPlayer().SetSelectedCountry(tempCountry);

                    GovernmentPanel.Instance.ShowGovernmentPanel();
                }
            }
        }

        void BuildingSelection(GameObjectAnimator anim)
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            if (anim.isBuilding())
            {
                player.SetSelectedBuilding(anim);
                player.ClearSelectedDivisions();

                DockyardPanel.Instance.ShowDockyardPanel();
            }
        }
        void MultipleDivisionSelection(List<GameObjectAnimator> anim)
        {
            Player player = GameEventHandler.Instance.GetPlayer();
            ClearCurrentSelection();

            foreach (GameObjectAnimator GOA in anim)
            {
                if (GOA.isDivision())
                {
                    player.AddSelectedDivisions(GOA);
                }
            }

            if(player.GetSelectedDivisions().Count > 0)
                HUDManager.Instance.ShowSelectedDivisions();
        }

        void SingleDivisionSelection(GameObjectAnimator GOA)
        {
            Player player = GameEventHandler.Instance.GetPlayer();
            ClearCurrentSelection();

            if (GOA.isDivision())
                player.AddSelectedDivisions(GOA);

            GOA.CreateShield(DivisionManager.Instance.shield);

            DivisionManagerPanel.Instance.ShowDivisionPanel(GOA.GetDivision());
        }

        public void StartEventListener()
        {
            Player player = GameEventHandler.Instance.GetPlayer();
            //int distance = 0;

            // plug our mouse move listener - it received the x,y map position of the mouse
            map.OnMouseMove += (float x, float y) =>
            {
                Vector2 targetPos = new Vector2(x, y);

                if (player.GetSelectedDivisions() != null)
                {
                    if (player.GetSelectedDivisionNumber() == 1)
                    {
                        Vector2 divisionPos = player.GetSelectedDivisions()[0].currentMap2DLocation;

                        //distance = WarManager.Instance.GetDistance(divisionPos, targetPos);
                        //Debug.Log("Distance : " + distance + " km");

                        UpdateRoutePathLine(player.GetSelectedDivisions()[0], x, y);
                    }
                    else
                    {
                        DestroyPathLine();
                    }
                }
                else
                {
                    DestroyPathLine();
                }
            };

            map.OnClick += (float x, float y, int buttonIndex) =>
            {
                Vector2 targetPosition = new Vector2(x, y);

                bool isContainWater = map.ContainsWater(targetPosition);
                Country country = map.GetCountry(targetPosition);

                if (buttonIndex == 0) // left click
                {
                    
                }

                else if (buttonIndex == 1) // right click
                {
                    if (isContainWater)
                    {
                        foreach (GameObjectAnimator division in player.GetSelectedDivisions())
                        {
                            if (division != null)
                            {
                                AudioManager.Instance.PlayVoice(VOICE_TYPE.ROGER_THAT);
                                WarManager.Instance.MoveAndAttack(division, null, false, targetPosition);
                            }
                        }
                    }
                    else
                    {
                        if (country == player.GetMyCountry()) // move
                        {
                            foreach (GameObjectAnimator division in player.GetSelectedDivisions())
                            {
                                if (division != null)
                                {
                                    if (division.GetDivision().divisionTemplate.divisionType == DIVISION_TYPE.AIR_DIVISION)
                                    {
                                        AudioManager.Instance.PlayVoice(VOICE_TYPE.AFFIRMATIVE);
                                    }
                                    else
                                    {
                                        AudioManager.Instance.PlayVoice(VOICE_TYPE.ROGER_THAT);
                                    }

                                    WarManager.Instance.MoveAndAttack(division, null, false, targetPosition);
                                }
                            }
                        }
                        else
                        {
                            if (CountryManager.Instance.GetAtWarCountryList(player.GetMyCountry()).Contains(country)) // it is my enemy
                            {
                                foreach (GameObjectAnimator division in player.GetSelectedDivisions())
                                {
                                    if (division != null)
                                    {
                                        WarManager.Instance.MoveAndAttack(division, null, true, targetPosition);
                                    }
                                }
                            }
                            else
                            {
                                player.SetSelectedCountry(country);
                                GovernmentPanel.Instance.ShowGovernmentPanel();
                                ActionManager.Instance.DeclareWarPanel();
                            }
                        }
                    }
                }

            };
            map.CenterMap();
        }

        public void ListenVehicleEvents()
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            if(player.GetMyCountry().GetArmy() != null)
            {
                foreach (GameObjectAnimator vehicle in player.GetMyCountry().GetArmy().GetAllDivisionInArmy())
                {
                    // Listen to unit-level events (if you need unit-level events...)
                    vehicle.OnPointerEnter += (GameObjectAnimator anim) => player.SetMouseOverUnit(anim);
                    vehicle.OnPointerExit += (GameObjectAnimator anim) => player.SetMouseOverUnit(null);
                    vehicle.OnPointerUp += (GameObjectAnimator anim) => SingleDivisionSelection(anim);
                    vehicle.OnPointerDown += (GameObjectAnimator anim) => SingleDivisionSelection(anim);
                }
            }

            foreach (City city in CountryManager.Instance.GetAllCitiesInCountry(player.GetMyCountry()))
            {
                if (city.Dockyard != null)
                {
                    // Listen to unit-level events (if you need unit-level events...)
                    city.Dockyard.OnPointerEnter += (GameObjectAnimator anim) => player.SetMouseOverUnit(anim);
                    city.Dockyard.OnPointerExit += (GameObjectAnimator anim) => player.SetMouseOverUnit(null);
                    city.Dockyard.OnPointerDown += (GameObjectAnimator anim) => BuildingSelection(anim);
                }
            }
        }

        public void ShowTooltip()
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            string tooltipText = string.Empty;

            GameObjectAnimator GOA = player.GetMouseOverUnit();

            if (GOA != null)
            {
                if (GOA.isBuilding() == true)
                {
                    tooltipText = GOA.name;
                    GOA.gameObject.GetComponent<SimpleTooltip>().infoLeft = tooltipText + "\n";
                }

                if (GOA.isDivision() == true)
                {
                    Division division = player.GetMouseOverUnit().GetDivision();

                    string line1 = division.divisionName;
                    string line2 = "Speed : " + division.GetDivisionSpeed().ToString();
                    //string line3 = "Soldier : " + division.currentSoldier.ToString();
                    string line4 = "Attack Power : " + division.GetDivisionPower().ToString();
                    string line5 = "Defense : " + division.GetDivisionDefense().ToString();
                    string line6 = "Minimum Attack Range : " + division.GetDivisionMinimumAttackRange().ToString();
                    string line7 = "Maximum Attack Range : " + division.GetDivisionMaximumAttackRange().ToString();

                    tooltipText = line1 + "\n" + "\n" + line2 + "\n" + line4 + "\n" + line5 + "\n" + line6 + "\n" + line7 + "\n";

                    GOA.gameObject.GetComponent<SimpleTooltip>().infoLeft = tooltipText + "\n";
                }
            }
        }

        void ExpandRegion(Country sourceCountry, Region region)
        {           
            if (sourceCountry == null || region == null)
                return;

            // Store principal country id which is immutable - country index will change because the countries array is modified after one country merges another (one of them disappears).
            int countryUniqueId = map.countries[principalCountryIndex].uniqueId;
            //string countryName = region.entity.name;

            if (map.CountryTransferCountryRegion(principalCountryIndex, region, true))
            {
                // Restore principal id before countries array changed
                SelectPrincipalCountry(map.GetCountryIndex(countryUniqueId));
                Debug.Log("Country " + sourceCountry.name + " conquered by " + map.countries[principalCountryIndex].name + "!");
            }           
        }

        void ExpandCountry(Country source, Country target)
        {            
            if (source == null || target == null)
                return;

            // Store principal country id which is immutable - country index will change because the countries array is modified after one country merges another (one of them disappears).
            int sourceCountryIndex = map.GetCountryIndex(source);
            int targetCountryIndex = map.GetCountryIndex(target);

            if (map.CountryTransferCountry(sourceCountryIndex, targetCountryIndex, true))
            {

            }
        }

        void SelectPrincipalCountry(int newCountryIndex)
        {
            principalCountryIndex = newCountryIndex;
        }
        /*
        /// <summary>
        /// Used when show Linear Path toggle is checked
        /// </summary>
        public void UpdateLinearPathLine(GameObjectAnimator division, float x, float y)
        {
            if (division != null)
            {
                if (pathLine != null)
                {   // remove existing line
                    DestroyImmediate(pathLine.gameObject);
                }

                // destination of linear path
                Vector2 destination = new Vector2(x, y);

                // optionally choose a material for the line (you may simply pass a color instead)
                Material lineMat = pathArcElevation > 0 ? lineMaterialAerial : lineMaterialGround;

                // draw the line
                pathLine = map.AddLine(division.currentMap2DLocation, destination, lineMat, pathArcElevation, pathLineWidth);
                pathLine.drawingDuration = pathDrawingDuration;
                pathLine.dashInterval = pathDashInterval;
                pathLine.dashAnimationDuration = pathDashAnimationDuration;

                pathLine.endCap = endCapSprite;
                pathLine.endCapMaterial = endCapMaterial;
                pathLine.endCapScale = new Vector3(1f, 1f, 2.5f);
                pathLine.endCapOffset = 4f;
                pathLine.endCapFlipDirection = true;

                UpdateCircle(destination);
            }           
        }
        */
        /// <summary>
        /// Used when show Route Path toggle is checked
        /// </summary>
        public void UpdateRoutePathLine(GameObjectAnimator division, float x, float y)
        {
            if (division != null)
            {
                if (pathLine != null)
                {   // remove existing line
                    DestroyImmediate(pathLine.gameObject);
                }

                // Find a route for tempCountry tank to destination
                Vector2 destination = new Vector2(x, y);
                List<Vector2> route = division.FindRoute(destination);

                if (route == null)
                {
                    // Draw a straight red line if no route is available
                    pathLine = map.AddLine(division.currentMap2DLocation, destination, Color.red, pathArcElevation, pathLineWidth);
                }
                else
                {
                    pathLine = map.AddLine(route.ToArray(), Color.gray, pathArcElevation, pathLineWidth);
                }
                pathLine.drawingDuration = pathDrawingDuration;
                pathLine.dashInterval = pathDashInterval;
                pathLine.dashAnimationDuration = pathDashAnimationDuration;

                UpdateMouseCursor(destination);
            }
            else
            {
                //DestroyImmediate(pathLine.gameObject);
            }
        }

        void DestroyPathLine()
        {
            if (pathLine != null)
                DestroyImmediate(pathLine.gameObject);
        }
        /*
        public void ShowBorderTexture(int countryIndex, bool visible)
        {
            map.ToggleCountryOutline(countryIndex, visible, borderTexture, 0.1f, Color.green);
        }
        */
       

        /*
        void AddMarker3DObjectOnCity(City city)
        {
            // Every marker is put on a plane-coordinate (in the range of -0.5..0.5 on both x and y)
            Vector2 planeLocation;

            planeLocation = city.unity2DLocation;

            // or... choose a city by its name:
            //		int cityIndex = map.GetCityIndex("Moscow");
            //		planeLocation = map.cities[cityIndex].unity2DLocation;

            // or... use the centroid of a country
            //		int countryIndex = map.GetCountryIndex("Greece");
            //		planeLocation = map.countries[countryIndex].center;

            // or... use a custom location lat/lon. Example put the building over New York:
            //		map.calc.fromLatDec = 40.71f;	// 40.71 decimal degrees north
            //		map.calc.fromLonDec = -74.00f;	// 74.00 decimal degrees to the west
            //		map.calc.fromUnit = UNIT_TYPE.DecimalDegrees;
            //		map.calc.Convert();
            //		planeLocation = map.calc.toPlaneLocation;

            // Send the prefab to the AddMarker API setting a scale of 0.1f (this depends on your marker scales)
            GameObject tower = Instantiate(Resources.Load<GameObject>("Tower/tower"));

            map.AddMarker3DObject(tower, planeLocation, 1f);

            // Fly to the destination and see the building created
            map.FlyToLocation(planeLocation, 2f, 0.2f);

            // Optionally add a blinking effect to the marker
            MarkerBlinker.AddTo(tower, 3, 0.2f);
        }
        */
        
        public bool CityHasCoast(City city)
        {
            Vector2 waterPos;
            if (map.ContainsWater(city.unity2DLocation, 0.01f, out waterPos))
            {
                return true;
            }

            return false;

            /*
            // For each found city, add a sphere marker on its position
            citiesNearWater.ForEach((City city) => {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                sphere.WMSK_MoveTo(city.unity2DLocation, false);
            });
            */
        }

        void AddSpriteAtPosition(City city)
        {
            /*
            // Instantiate the sprite, face it to up and position it into the map
            GameObject star = Instantiate(airportSprite);
            star.transform.localRotation = Quaternion.Euler(90, 0, 0);
            star.transform.localScale = Misc.Vector3one * 0.1f;
            star.WMSK_MoveTo(city.unity2DLocation, true, 0.25f);
            */
        }

        /// <summary>
		/// Locates coastal points for a sample country and add custom sprites over that line
		/// </summary>
		public void Coastline(Country country)
        {
            int countryIndex = map.GetCountryIndex(country);

            List<Vector2> points = map.GetCountryCoastalPoints(countryIndex);
            /*
            points.ForEach((point) => AddRandomSpriteAtPosition(point));
            if (points.Count > 0)
                map.FlyToLocation(points[0], 2, 0.2f);
            */
        }
        
        /// <summary>
		/// Creates a tower instance and adds it to given coordinates
		/// </summary>
		void AddTowerAtPosition(float x, float y)
        {
            /*
            // Instantiate game object and position it instantly over the city
            GameObject tower = Instantiate(tankPrefab);
            GameObjectAnimator anim = tower.WMSK_MoveTo(x, y);
            anim.autoScale = false;
            */
        }

        public void ColorizeWorld()
        {   
            foreach(Country country in map.countries)
            {
                ColorizeCountry(country);
            }
        }

        public void TextureCountry(Country country)
        {
            Debug.Log("Start");
            // Assign a flag texture to country
            ///string countryName = country.name;
            //CountryDecorator decorator = new CountryDecorator();
            //decorator.isColorized = true;

            if(country.GetCountryFlag() == null)
                Debug.Log("Flag is null");

            //decorator.texture = country.GetCountryFlag();
            //map.decorator.SetCountryDecorator(0, countryName, decorator);

            map.ToggleCountryMainRegionSurface(map.GetCountryIndex(country), true, country.GetCountryFlag());

            map.Redraw();
            Debug.Log("Finish");

        }

        public void ColorizeCountry(Country country)
        {
            Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            color.a = 0.15f;

            country.SurfaceColor = color;

            map.ToggleCountrySurface(map.GetCountryIndex(country), true, country.SurfaceColor);
        }

        /// <summary>
        /// Locates common frontiers points between France and Germany and add custom sprites over that line
        /// </summary>
        public List<Vector2> GetLineBetween2Country(Country country1, Country country2)
        {
            int countryIndex_1 = map.GetCountryIndex(country1);
            int countryIndex_2 = map.GetCountryIndex(country2);
            List<Vector2> points = map.GetCountryFrontierPoints(countryIndex_1, countryIndex_2);

            return points;
        }

        public List<Province> FindBorderProvinces(Country country1, Country enemy)
        {
            List<Province> tempProvinces = new List<Province>();

            if (country1 == null || enemy == null)
            {

            }
            else
            {
                map.showProvinces = true;
                int country1_Index = map.GetCountryIndex(country1.name);
                int country2_Index = map.GetCountryIndex(enemy.name);

                List<Vector2> points = map.GetCountryFrontierPoints(country1_Index, country2_Index);

                for (int i = 0; i < points.Count; i++)
                {
                    int provIndex = map.GetProvinceIndex(points[i] + new Vector2(UnityEngine.Random.value * 0.0001f - 0.00005f, UnityEngine.Random.value * 0.0001f - 0.00005f));
                    if (provIndex >= 0 && (map.provinces[provIndex].countryIndex == country1_Index || map.provinces[provIndex].countryIndex == country2_Index))
                    {
                        if (map.provinces[provIndex].countryIndex == country1_Index && tempProvinces.Contains(map.GetProvince(provIndex)) == false)
                        {
                            Province province = map.GetProvince(provIndex);

                            tempProvinces.Add(province);
                        }

                        //map.ToggleProvinceSurface(provIndex, true, new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
                    }
                }
            }

            return tempProvinces;
        }


    }
}