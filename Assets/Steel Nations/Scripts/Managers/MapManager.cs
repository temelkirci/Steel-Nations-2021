using System.Collections;
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

        WMSK map;

        public GameObject NuclearWar;
        public GameObject Nuke;
        public GameObject tankBullet;

        public Texture2D borderTexture;

        public GameObject endCapSprite;
        public Material endCapMaterial;
        LineMarkerAnimator pathLine;
        GameObject circle;
        Material lineMaterialAerial, lineMaterialGround;

        int principalCountryIndex = -1;

        float pathDrawingDuration = 0f;
        // 0 means instant drawing.
        float pathArcElevation = 0f;
        // 0 means ground-level (path will be part of the viewport texture). If you provide a value, path will be 3D.
        float pathLineWidth = 0.01f;
        float pathDashInterval = 0;
        float pathDashAnimationDuration = 0;
        float circleRadius = 25f, circleRingStart = 0, circleRingEnd = 1f;
        Color circleColor = new Color(0.2f, 1f, 0.2f, 0.75f);


        // Start is called before the first frame update
        void Start()
        {
            map = WMSK.instance;

            instance = this;

            // Prepare line texture
            lineMaterialAerial = Instantiate(Resources.Load<Material>("PathLine/aerialPath"));
            lineMaterialGround = Instantiate(Resources.Load<Material>("PathLine/groundPath"));
        }

        void Update()
        {
            if (GameEventHandler.Instance.IsGameStarted() == false)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.PauseGame();
            }

            //if (GetPlayer().GetMouseOverUnit() != null)
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

        void rectangleSelectionCallback(Rect rect, bool finishRectangleSelection)
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            if (finishRectangleSelection)
            {
                if(map.VGOGet(rect).Count > 0)
                {
                    List<GameObjectAnimator> divisions = new List<GameObjectAnimator>();

                    foreach (GameObjectAnimator GOA in map.VGOGet(rect))
                    {
                        if (GOA.isDivision() && player.IsMyDivision(GOA))
                        {
                            divisions.Add(GOA);
                            GOA.GetComponentInChildren<Renderer>().material.color = Color.blue;
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

        public void BeginNuclearWar(Country attackCountry, Country defenseCountry)
        {
            StartCoroutine(War(attackCountry, defenseCountry, 1));
        }

        void ClearCurrentSelection()
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            foreach (GameObjectAnimator go in player.GetSelectedDivisions())
            {
                go.GetComponentInChildren<Renderer>().material.color = go.attrib["Color"];
            }

            DestroyPathLine();
            player.ClearSelectedDivisions();
        }

        public void StartListeningCountries()
        {
            /* Register events: this is optionally but allows your scripts to be informed instantly as the mouse enters or exits a country, province or city */
            //map.OnCityEnter += OnCityEnter;
            //map.OnCityExit += OnCityExit;
            map.OnCityClick += OnCityClick;
            map.OnCountryEnter += (int countryIndex, int regionIndex) => ShowBorderTexture(countryIndex, true);
            map.OnCountryExit += (int countryIndex, int regionIndex) => ShowBorderTexture(countryIndex, false);
            map.OnCountryClick += OnCountryClick;
            map.OnProvinceEnter += OnProvinceEnter;
            map.OnProvinceExit += OnProvinceExit;
            map.OnProvinceClick += OnProvinceClick;
        }


        void OnCityClick(int cityIndex, int buttonIndex)
        {
            ClearCurrentSelection();

            if (buttonIndex == 0)
            {
                City city = map.GetCity(cityIndex);
                               
                if(GameEventHandler.Instance.GetPlayer().GetMyCountry().GetAllCitiesInCountry().Contains(city) == true)
                {
                    GameEventHandler.Instance.GetPlayer().SetSelectedCity(city);

                    CityInfoPanel.Instance.ShowCityMenu();
                }            
            }
        }

        void OnProvinceClick(int provinceIndex, int regionIndex, int buttonIndex)
        {
            //ClearCurrentSelection();
        }

        void OnProvinceEnter(int provinceIndex, int regionIndex)
        {
            Province province = map.GetProvince(provinceIndex);

            HUDManager.Instance.ShowInfoText(province.name);
        }

        void OnProvinceExit(int provinceIndex, int regionIndex)
        {
            Province province = map.GetProvince(provinceIndex);

            HUDManager.Instance.ShowInfoText("");
        }


        void OnCountryClick(int countryIndex, int regionIndex, int buttonIndex)
        {
            //ClearCurrentSelection();

            if (buttonIndex == 0)
            {
                Country tempCountry = map.GetCountry(countryIndex);

                //if (tempCountry == GameEventHandler.Instance.GetPlayer().GetMyCountry())
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
                if(GOA.isDivision())
                    player.AddSelectedDivisions(GOA);
            }

            if(player.GetSelectedDivisionNumber() == 1)
                DivisionManagerPanel.Instance.ShowDivisionPanel();
        }

        void SingleDivisionSelection(GameObjectAnimator GOA)
        {
            Player player = GameEventHandler.Instance.GetPlayer();
            ClearCurrentSelection();

            if (GOA.isDivision())
                player.AddSelectedDivisions(GOA);

            DivisionManagerPanel.Instance.ShowDivisionPanel();
        }

        public void StartEventListener()
        {
            Player player = GameEventHandler.Instance.GetPlayer();
            float distance = 0f;

            // plug our mouse move listener - it received the x,y map position of the mouse
            map.OnMouseMove += (float x, float y) =>
            {
                if (player.GetSelectedDivisions() != null)
                {
                    if (player.GetSelectedDivisionNumber() == 1)
                    {
                        distance = map.calc.Distance(x, y, player.GetSelectedDivisions()[0].currentMap2DLocation.x, player.GetSelectedDivisions()[0].currentMap2DLocation.y);
                        //Debug.Log("Distance : " + distance);

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
                    if (country == player.GetMyCountry()) // move
                    {
                        foreach (GameObjectAnimator division in player.GetSelectedDivisions())
                        {
                            if (IsDivisionReadyToMove(division))
                            {
                                Debug.Log(division.name + " Number -> " + division.GetDivision().GetWeaponsInDivision().Count + " -> Speed"+  division.GetDivision().GetDivisionSpeed());

                                if (division.GetDivision().divisionTemplate.divisionType == DIVISION_TYPE.AIR_DIVISION)
                                {
                                    AudioManager.Instance.PlayVoice(VOICE_TYPE.ATTACK);

                                    StartFlight(division, targetPosition);
                                }
                                else
                                {
                                    AudioManager.Instance.PlayVoice(VOICE_TYPE.ROGER_THAT);

                                    division.MoveTo(targetPosition, division.GetDivision().GetDivisionSpeed());
                                }
                            }
                        }
                    }
                    else // attack
                    {
                        foreach (GameObjectAnimator division in player.GetSelectedDivisions())
                        {
                            if (IsDivisionReadyToAttack(division))
                            {
                                PrepareToAttack(division, targetPosition);
                                AudioManager.Instance.PlayVoice(VOICE_TYPE.ATTACK);
                            }
                        }
                    }

                    AudioManager.Instance.PlayVoice(VOICE_TYPE.YES_SIR);
                }

            };
            map.CenterMap();
        }

        public bool IsDivisionReadyToAttack(GameObjectAnimator division)
        {
            if (division != null && division.isDivision())
                return true;
            else
                return false;
        }

        public bool IsDivisionReadyToMove(GameObjectAnimator division)
        {
            if (division != null && division.isDivision())
                return true;
            else
                return false;
        }

        public void ListenVehicleEvents()
        {
            Player player = GameEventHandler.Instance.GetPlayer();

            foreach (GameObjectAnimator vehicle in player.GetMyCountry().GetArmy().GetAllDivisionInArmy())
            {
                // Listen to unit-level events (if you need unit-level events...)
                vehicle.OnPointerEnter += (GameObjectAnimator anim) => player.SetMouseOverUnit(anim);
                vehicle.OnPointerExit += (GameObjectAnimator anim) => player.SetMouseOverUnit(null);
                vehicle.OnPointerUp += (GameObjectAnimator anim) => SingleDivisionSelection(anim);
                vehicle.OnPointerDown += (GameObjectAnimator anim) => SingleDivisionSelection(anim);
            }

            foreach (City city in player.GetMyCountry().GetAllCitiesInCountry())
            {
                if (city.GetDockyard() != null)
                {
                    // Listen to unit-level events (if you need unit-level events...)
                    city.GetDockyard().OnPointerEnter += (GameObjectAnimator anim) => player.SetMouseOverUnit(anim);
                    city.GetDockyard().OnPointerExit += (GameObjectAnimator anim) => player.SetMouseOverUnit(null);
                    city.GetDockyard().OnPointerDown += (GameObjectAnimator anim) => BuildingSelection(anim);
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
                    string line1 = player.GetMouseOverUnit().name;
                    string line2 = "Speed : " + GOA.GetDivision().GetDivisionSpeed().ToString();
                    tooltipText = line1 + "\n" + line2 + "\n";

                    GOA.gameObject.GetComponent<SimpleTooltip>().infoLeft = tooltipText + "\n";
                }
            }
        }

        void StartFlight(GameObjectAnimator fleet, Vector2 destination)
        {
            fleet.arcMultiplier = 5f;     // tempCountry is the arc for the plane trajectory
            fleet.easeType = EASE_TYPE.SmootherStep;    // make it an easy-in-out movement

            fleet.MoveTo(destination, fleet.GetDivision().GetDivisionSpeed());

            Vector2 startingPos = fleet.startingMap2DLocation;
            fleet.comeBack = true;
            fleet.altitudeEnd = 5;
            fleet.altitudeStart = 0.1f;

            fleet.OnCountryEnter += (GameObjectAnimator anim) =>
            {

            };
            fleet.OnKilled += (GameObjectAnimator anim) =>
            {

            };
            fleet.OnMoveStart += (GameObjectAnimator anim) =>
            {
                AudioManager.Instance.PlayVoice(VOICE_TYPE.FIGHTER);

                if (fleet.comeBack == false && startingPos != fleet.destination)
                {
                    fleet.altitudeEnd = 0.1f;
                    fleet.altitudeStart = 5f;
                }
            };

            fleet.OnMoveEnd += (GameObjectAnimator anim) =>
            {
                if (startingPos != fleet.destination && fleet.comeBack == true)
                {
                    fleet.comeBack = false;

                    fleet.altitudeEnd = 0.1f;
                    fleet.altitudeStart = 5;

                    fleet.MoveTo(startingPos, fleet.GetDivision().GetDivisionSpeed());
                    ShowExplosion(fleet.currentMap2DLocation);
                }
                else
                {
                    fleet.altitudeStart = 0.1f;
                    fleet.currentAltitude = 0.1f;
                    fleet.altitudeEnd = 0.1f;
                }
            };    // once the movement has finished, stop following the unit
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
                    pathLine = map.AddLine(route.ToArray(), Color.yellow, pathArcElevation, pathLineWidth);
                }
                pathLine.drawingDuration = pathDrawingDuration;
                pathLine.dashInterval = pathDashInterval;
                pathLine.dashAnimationDuration = pathDashAnimationDuration;

                UpdateCircle(destination);
            }   
            else
            {
                DestroyImmediate(pathLine.gameObject);
            }
        }

        void DestroyPathLine()
        {
            if (pathLine != null)
                DestroyImmediate(pathLine.gameObject);
        }

        void UpdateCircle(Vector2 position)
        {
            if (circle != null)
                Destroy(circle);

            circle = map.AddCircle(position, circleRadius, circleRingStart, circleRingEnd, circleColor);
        }

        public void ShowBorderTexture(int countryIndex, bool visible)
        {
            map.ToggleCountryOutline(countryIndex, visible, borderTexture, 0.1f, Color.green);
        }

        public IEnumerator War(Country attackCountry, Country defenseCountry, int wave)
        {
            float start = Time.time;
            while (Time.time - start < wave)
            {
                yield return null;
            }

            StartCoroutine(LaunchMissile(2f, attackCountry.name, defenseCountry.name, Color.yellow));
            //StartCoroutine (LaunchMissile (3f, defenseCountry.name, attackCountry.name, Color.black));
        }


        IEnumerator LaunchMissile(float delay, string countryOrigin, string countryDest, Color color)
        {
            float start = Time.time;
            while (Time.time - start < delay)
            {
                yield return null;
            }

            // Initiates line animation
            int cityOrigin = map.GetCityIndexRandom(map.GetCountry(countryOrigin));
            int cityDest = map.GetCityIndexRandom(map.GetCountry(countryDest));
            if (cityOrigin < 0 || cityDest < 0)
                yield break;

            Vector2 origin = map.cities[cityOrigin].unity2DLocation;
            Vector2 dest = map.cities[cityDest].unity2DLocation;
            float elevation = 10f;
            float width = 0.5f;
            LineMarkerAnimator lma = map.AddLine(origin, dest, color, elevation, width);
            lma.dashInterval = 0.0003f;
            lma.dashAnimationDuration = 0.5f;
            lma.drawingDuration = 4f;
            lma.autoFadeAfter = 1f;

            // Add flashing target
            GameObject sprite = Instantiate(NuclearWar) as GameObject;
            sprite.GetComponent<SpriteRenderer>().material.color = color * 0.9f;
            map.AddMarker2DSprite(sprite, dest, 0.003f);
            MarkerBlinker.AddTo(sprite, 4, 0.1f, 0.5f, true);

            // Triggers explosion
            StartCoroutine(AddCircleExplosion(4f, dest, Color.yellow));
        }


        IEnumerator AddCircleExplosion(float delay, Vector2 mapPos, Color color)
        {
            float start = Time.time;
            while (Time.time - start < delay)
            {
                yield return null;
            }

            GameObject circleObj = null;
            float radius = UnityEngine.Random.Range(80f, 100f);
            for (int k = 0; k < 100; k++)
            {
                if (circleObj != null)
                    DestroyImmediate(circleObj);
                float ringStart = Mathf.Clamp01((k - 50f) / 50f);
                float ringEnd = Mathf.Clamp01(k / 50f);
                circleObj = map.AddCircle(mapPos, radius, ringStart, ringEnd, color);
                yield return new WaitForSeconds(1 / 60f);
            }
            Destroy(circleObj);
        }

        /// <summary>
        /// tempCountry function adds a standard sphere primitive to the map. The difference here is that the pivot of the sphere is centered in the sphere. So we make use of pivotY property to specify it and
        /// tempCountry way the positioning over the terrain will work. Otherwise, the sphere will be cut by the terrain (the center of the sphere will be on the ground - and we want the sphere on top of the terrain).
        /// </summary>
        public void ShowExplosion(Vector3 position)
        {
            GameObject explosion = Instantiate(Nuke);
            explosion.transform.localScale = Misc.Vector3one * 0.1f;

            //map.AddMarker3DObject(explosion, position, 1);

            GameObjectAnimator anim = explosion.WMSK_MoveTo(position);
            anim.pivotY = 0.5f; // model is not ground based (which has a pivoty = 0, the default value, so setting the pivot to 0.5 will center vertically the model)
            AudioManager.Instance.PlayVoice(VOICE_TYPE.BOMB);

            Destroy(explosion, 5);
        }

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

        public void PrepareToAttack(GameObjectAnimator division, Vector2 targetAttackPosition)
        {
            // Create bullet
            GameObject bullet = Instantiate(tankBullet);
            bullet.GetComponent<Renderer>().material.color = Color.black;
            bullet.transform.localScale = Misc.Vector3one * 0.05f;

            // Animate bullet!
            Vector3 tankCannonAnchor = new Vector3(0f, 1.55f, 0.85f);   // this is the position relative to the tank pivot (note that the tank pivot is at bottom of tank per model definition)
            float bulletSpeed = 0.1f;
            float bulletArc = 1.1f;
            GameObjectAnimator bulletAnim = division.gameObject.WMSK_Fire(bullet, tankCannonAnchor, targetAttackPosition, bulletSpeed, bulletArc);
            //bulletAnim.type = (int)MY_UNIT_TYPE.AIRPLANE;              // this is completely optional, just used in the demo scene to differentiate this unit from other tanks and ships
            bulletAnim.terrainCapability = TERRAIN_CAPABILITY.Any;  // ignores path-finding and can use a straight-line from start to destination
            bulletAnim.pivotY = 0.5f; // model is not ground based (which has a pivoty = 0, the default value, so setting the pivot to 0.5 will center vertically the model)
            bulletAnim.autoRotation = true;  // auto-head to destination when moving
            bulletAnim.rotationSpeed = 0.5f;  // speed of the rotation of auto-head to destination

            // We use the OnMoveEnd event of the bullet to destroy it once it reaches its destination
            bulletAnim.OnMoveStart += BulletFired;
            bulletAnim.OnMoveEnd += BulletImpact;
        }

        /// <summary>
        /// You can use tempCountry event to draw some special effect on bullet position (like tiny explosion)
        /// </summary>
        void BulletFired(GameObjectAnimator bulletAnim)
        {
            Debug.Log("Bullet fired!");
        }

        /// <summary>
        /// You can use tempCountry event to process the impact damage
        /// </summary>
        void BulletImpact(GameObjectAnimator bulletAnim)
        {
            ShowExplosion(bulletAnim.destination);
            Destroy(bulletAnim.gameObject);
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
		void FindFranceCoast()
        {

            int franceIndex = map.GetCountryIndex("France");
            Debug.Log("France Index : " + franceIndex);

            List<Vector2> points = map.GetCountryCoastalPoints(franceIndex);
            Debug.Log("France points : " + points.Count);
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
        {            for (int countryIndex = 0; countryIndex < map.countries.Length; countryIndex++)
            {
                Color color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
                color.a = 0.4f;
                map.ToggleCountrySurface(countryIndex, true, color);
            }
        }

        /// <summary>
        /// Locates common frontiers points between France and Germany and add custom sprites over that line
        /// </summary>
        void FindFranceGermanyLine()
        {
            int franceIndex = map.GetCountryIndex("Turkey");
            int germanyIndex = map.GetCountryIndex("Armenia");
            List<Vector2> points = map.GetCountryFrontierPoints(franceIndex, germanyIndex);
        }

        void FindFranceGermanyProvinces()
        {
            map.showProvinces = true;
            int franceIndex = map.GetCountryIndex("Turkey");
            int germanyIndex = map.GetCountryIndex("Syria");
            List<Vector2> points = map.GetCountryFrontierPoints(franceIndex, germanyIndex);
            if (points.Count > 0)
                map.FlyToLocation(points[0], 2, 0.2f);

            for (int i = 0; i < points.Count; i++)
            {
                int provIndex = map.GetProvinceIndex(points[i] + new Vector2(UnityEngine.Random.value * 0.0001f - 0.00005f, UnityEngine.Random.value * 0.0001f - 0.00005f));
                if (provIndex >= 0 && (map.provinces[provIndex].countryIndex == franceIndex || map.provinces[provIndex].countryIndex == germanyIndex))
                {
                    map.ToggleProvinceSurface(provIndex, true, new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
                }
            }
        }


    }
}