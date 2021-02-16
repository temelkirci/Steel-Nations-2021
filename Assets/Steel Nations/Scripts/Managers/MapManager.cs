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

        public void StartListeningCountries()
        {
            /* Register events: this is optionally but allows your scripts to be informed instantly as the mouse enters or exits a country, province or city */
            //map.OnCityEnter += (int cityIndex) => Debug.Log("Entered city " + map.cities[cityIndex].name);
            //map.OnCityExit += (int cityIndex) => Debug.Log("Exited city " + map.cities[cityIndex].name);
            //map.OnCityClick += OnCityClick;
            map.OnCountryEnter += (int countryIndex, int regionIndex) => ShowBorderTexture(countryIndex, true);
            map.OnCountryExit += (int countryIndex, int regionIndex) => ShowBorderTexture(countryIndex, false);
            map.OnCountryClick += OnCountryClick;
            //map.OnProvinceEnter += (int provinceIndex, int regionIndex) => Debug.Log("Entered province " + map.provinces[provinceIndex].name);
            //map.OnProvinceExit += (int provinceIndex, int regionIndex) => Debug.Log("Exited province " + map.provinces[provinceIndex].name);
            map.OnProvinceClick += OnProvinceClick;
        }

        void OnProvinceClick(int provinceIndex, int regionIndex, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                Province province = map.GetProvince(provinceIndex);
                Country country = map.GetCountry(province.countryIndex);

                if(GameEventHandler.Instance.GetPlayer().GetMyCountry() == country)
                {
                    foreach (City city in country.GetAllCitiesInCountry())
                    {
                        if (city.province == province.name)
                        {
                            GameEventHandler.Instance.GetPlayer().SetSelectedCity(city);

                            CityInfoPanel.Instance.ShowCityMenu();
                            break;
                        }
                    }
                }
                else
                {

                }
                
            }
        }

        void OnCountryClick(int countryIndex, int regionIndex, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                Country tempCountry = map.GetCountry(countryIndex);

                if (tempCountry != GameEventHandler.Instance.GetPlayer().GetMyCountry())
                {
                    GameEventHandler.Instance.GetPlayer().SetSelectedCountry(tempCountry);

                    GovernmentPanel.Instance.ShowGovernmentPanel();
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

        /// <summary>
        /// Used when show Linear Path toggle is checked
        /// </summary>
        public void UpdateLinearPathLine(float x, float y)
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
            pathLine = map.AddLine(GameEventHandler.Instance.GetPlayer().GetSelectedDivision().currentMap2DLocation, destination, lineMat, pathArcElevation, pathLineWidth);
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

        /// <summary>
        /// Used when show Route Path toggle is checked
        /// </summary>
        public void UpdateRoutePathLine(float x, float y)
        {
            if (GameEventHandler.Instance.GetPlayer().GetSelectedDivision() == null)
                return;

            if (pathLine != null)
            {   // remove existing line
                DestroyImmediate(pathLine.gameObject);
            }

            // Find a route for tempCountry tank to destination
            Vector2 destination = new Vector2(x, y);
            List<Vector2> route = GameEventHandler.Instance.GetPlayer().GetSelectedDivision().FindRoute(destination);

            if (route == null)
            {
                // Draw a straight red line if no route is available
                pathLine = map.AddLine(GameEventHandler.Instance.GetPlayer().GetSelectedDivision().currentMap2DLocation, destination, Color.red, pathArcElevation, pathLineWidth);
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


        public void PrepareToAttack(Vector2 targetAttackPosition)
        {
            // Create bullet
            GameObject bullet = Instantiate(tankBullet);
            bullet.GetComponent<Renderer>().material.color = Color.black;
            bullet.transform.localScale = Misc.Vector3one * 0.05f;

            // Animate bullet!
            Vector3 tankCannonAnchor = new Vector3(0f, 1.55f, 0.85f);   // this is the position relative to the tank pivot (note that the tank pivot is at bottom of tank per model definition)
            float bulletSpeed = 0.1f;
            float bulletArc = 1.1f;
            GameObjectAnimator bulletAnim = GameEventHandler.Instance.GetPlayer().GetSelectedDivision().gameObject.WMSK_Fire(bullet, tankCannonAnchor, targetAttackPosition, bulletSpeed, bulletArc);
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

        public void ToggleFreeCamera()
        {
            FreeCameraMove freeCameraScript = FindObjectOfType<FreeCameraMove>();
            if (freeCameraScript != null)
            {
                freeCameraScript.enabled = !freeCameraScript.enabled;
                map.enableFreeCamera = freeCameraScript.enabled;
            }
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