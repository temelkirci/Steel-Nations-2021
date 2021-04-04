// World Map Strategy Kit for Unity - Main Script
// (C) 2016-2020 by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WorldMapStrategyKit.Poly2Tri;
using WorldMapStrategyKit.ClipperLib;


namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        const string PROVINCE_ATTRIB_DEFAULT_FILENAME = "provincesAttrib";

        // resources
        Material provincesMat;
        Material hudMatProvince;

        // gameObjects
        GameObject provincesObj, provinceRegionHighlightedObj;

        // caché and gameObject lifetime control
        int countryProvincesDrawnIndex;
        Dictionary<Province, int> _provinceLookup;
        int lastProvinceLookupCount = -1;
        bool provinceNeighboursComputed;

        Dictionary<Province, int> provinceLookup {
            get {
                if (_provinceLookup != null && provinces.Length == lastProvinceLookupCount)
                    return _provinceLookup;
                if (_provinceLookup == null) {
                    _provinceLookup = new Dictionary<Province, int>();
                } else {
                    _provinceLookup.Clear();
                }
                for (int k = 0; k < provinces.Length; k++) {
                    _provinceLookup[provinces[k]] = k;
                }
                lastProvinceLookupCount = provinces.Length;
                return _provinceLookup;
            }
        }

        /// <summary>
        /// Used internally by the Map Editor. It will recalculate de boundaries and optimize frontiers based on new data of provinces array
        /// </summary>
        public void RefreshProvinceDefinition(int provinceIndex, bool justComputeBorders) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return;
            RefreshProvinceGeometry(provinceIndex);
            DrawProvinces(provinces[provinceIndex].countryIndex, true, true, justComputeBorders);
        }

        /// <summary>
        /// Used internally by the Map Editor. It will recalculate de boundaries and optimize frontiers based on new data of provinces array
        /// </summary>
        public void RefreshProvinceGeometry(int provinceIndex) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return;
            RefreshProvinceGeometry(provinces[provinceIndex]);
        }

        /// <summary>
        /// Used internally by the Map Editor. It will recalculate de boundaries and optimize frontiers based on new data of provinces array
        /// </summary>
        public void RefreshProvinceGeometry(IAdminEntity province) {
            lastProvinceLookupCount = -1;
            float maxVol = 0;
            if (province.regions == null) {
                ReadProvincePackedString(province);
            }
            if (province.regions == null)
                return;
            int regionCount = province.regions.Count;
            Vector2 minProvince = Misc.Vector2one * 10;
            Vector2 maxProvince = -minProvince;
            Vector2 min = Misc.Vector2one * 10f;
            Vector2 max = -min;

            for (int r = 0; r < regionCount; r++) {
                Region provinceRegion = province.regions[r];
                provinceRegion.entity = province;   // just in case one country has been deleted
                provinceRegion.regionIndex = r;             // just in case a region has been deleted
                int coorCount = provinceRegion.points.Length;
                min.x = min.y = 10f;
                max.x = max.y = -10f;
                for (int c = 0; c < coorCount; c++) {
                    float x = provinceRegion.points[c].x;
                    float y = provinceRegion.points[c].y;
                    if (x < min.x)
                        min.x = x;
                    if (x > max.x)
                        max.x = x;
                    if (y < min.y)
                        min.y = y;
                    if (y > max.y)
                        max.y = y;
                }
                FastVector.Average(ref min, ref max, ref provinceRegion.center);

                if (min.x < minProvince.x)
                    minProvince.x = min.x;
                if (min.y < minProvince.y)
                    minProvince.y = min.y;
                if (max.x > maxProvince.x)
                    maxProvince.x = max.x;
                if (max.y > maxProvince.y)
                    maxProvince.y = max.y;
                provinceRegion.rect2D = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
                provinceRegion.rect2DArea = provinceRegion.rect2D.width * provinceRegion.rect2D.height;
                float vol = FastVector.SqrDistance(ref max, ref min); // (max - min).sqrMagnitude;
                if (vol > maxVol) {
                    maxVol = vol;
                    province.mainRegionIndex = r;
                    province.center = provinceRegion.center;
                }
            }
            province.regionsRect2D = new Rect(minProvince.x, minProvince.y, Math.Abs(maxProvince.x - minProvince.x), Mathf.Abs(maxProvince.y - minProvince.y));
        }


        /// <summary>
        /// Loads and cache province data. This is automatically called when showProvinces is set to true.
        /// </summary>
        void ReadProvincesPackedString() {
            lastProvinceLookupCount = -1;

            string frontiersFileName = _geodataResourcesPath + "/provinces10";
            TextAsset ta = Resources.Load<TextAsset>(frontiersFileName);

            if (ta != null) {
                SetProvincesGeoData(ta.text);
                ReloadProvincesAttributes();
            }
        }

        /* Used internally in migration processes */
        /*
        void SolveProvincesData() {
            TextAsset ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/countries10-2d");
            SetCountryGeoData(ta.text);
            Country[] newCountries = this.countries;
            ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/provinces10-2d");
            SetProvincesGeoData(ta.text);
            Province[] newProvinces = this.provinces;
            foreach (Province prov in newProvinces) ReadProvincePackedString(prov);
            ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/cities10-2d");
            SetCityGeoData(ta.text);
            List<City> newCities = new List<City>(cities);

            ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/countries10");
            SetCountryGeoData(ta.text);
            ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/provinces10");
            SetProvincesGeoData(ta.text);
            foreach (Province prov in provinces) ReadProvincePackedString(prov);
            ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/cities10");
            SetCityGeoData(ta.text);

            int totalDiff = 0;
            int totalNew = 0;

            Debug.Log("Old provinces: " + provinces.Length + " New provinces: " + newProvinces.Length);
            for (int k = 0; k < newProvinces.Length; k++) {
                Province newProvince = newProvinces[k];
                int similarId = 0;
                int bestScore = 0;
                string newCountryName = newCountries[newProvince.countryIndex].name;
                if (string.IsNullOrEmpty(newCountryName)) continue;

                float minDist = float.MaxValue;
                int nearest = -1;
                for (int j = 0; j < provinces.Length; j++) {
                    Province oldProvince = provinces[j];
                    string oldCountryName = countries[oldProvince.countryIndex].name;
                    if (newCountryName != oldCountryName) continue;

                    if (oldProvince.name == newProvince.name) {
                        similarId = nearest = j;
                        break;
                    }

                    float dist = Vector2.Distance(newProvince.center, oldProvince.center);
                    if (dist < minDist) {
                        minDist = dist;
                        nearest = j;
                    }

                    int score = SimilarScore(newProvince.name, oldProvince.name);
                    if (score > bestScore) {
                        bestScore = score;
                        similarId = j;
                    }
                }
                if (nearest < 0) {
                    for (int u = 0; u < this.countries.Length; u++) {
                        if (this.countries[u].name == newCountryName) {
                            if (this.countries[u].provinces == null) {
                                int newCountryIndex = newProvince.countryIndex;
                                Country newCountry = newCountries[newCountryIndex];
                                // Añade todas las provincias
                                foreach (Province prov in newCountry.provinces) {
                                    Debug.Log("$$$ New! " + prov.name + " (Country: " + newCountryName + ") | Old country has no provinces");
                                    // Add province
                                    prov.countryIndex = u;
                                    ProvinceAdd(prov);
                                    // Añade ciudades
                                    for (int c = 0; c < newcities.Length; c++) {
                                        City newCity = newCities[c];
                                        if (newCity.countryIndex == newCountryIndex && newCity.province == prov.name) {
                                            newCity.countryIndex = u;
                                            cities.Add(newCity);
                                            Debug.Log("New City: " + newCity.name + " (" + newCity.province + ") ");
                                        }
                                    }
                                }
                                newCountry.name = "";
                                totalNew++;
                            }
                            break;
                        }
                    }
                } else if (newProvince.name != provinces[similarId].name) {
                    totalDiff++;
                    int countryIndex = provinces[similarId].countryIndex;
                    string oldCountryName = countries[countryIndex].name;
                    string bestByName = provinces[similarId].name;
                    string bestByDist = provinces[nearest].name;
                    if (bestByName == bestByDist) {
                        Debug.Log("*** Diff: " + newProvince.name + " (Country: " + newCountryName + ") | Similar: " + bestByName);
                        string oldProvinceName = provinces[nearest].name;
                        // update cities ref
                        for (int c = 0; c < cities.Length; c++) {
                            City city = cities[c];
                            if (city.countryIndex == countryIndex && city.province == oldProvinceName) {
                                Debug.Log("City updated: " + city.name + " (old province name: " + city.province + ", new name: " + newProvince.name + ", ");
                                city.province = newProvince.name;
                            }
                        }
                        provinces[nearest].name = newProvince.name;
                    }
                }
            }
            Debug.Log("Total Unkwnon: " + totalDiff + ", Total New: " + totalNew);

            string newData = GetProvinceGeoData();
            string fullPathName = "Assets/WorldMapStrategyKit/Resources/WMSK/Geodata/provinces10-new.txt";
            File.WriteAllText(fullPathName, newData, System.Text.Encoding.UTF8);

            newData = GetCityGeoData();
            fullPathName = "Assets/WorldMapStrategyKit/Resources/WMSK/Geodata/cities10-new.txt";
            File.WriteAllText(fullPathName, newData, System.Text.Encoding.UTF8);

            UnityEditor.AssetDatabase.Refresh();
        }

        int SimilarScore(string s1, string s2) {
            int min = Mathf.Min(s1.Length, s2.Length);
            int score = 0;
            for (int k = 0; k < min; k++) {
                if (s2[k] == s1[k]) {
                    score += 5;
                } else {
                    for (int j = k + 1; j < min; j++) {
                        if (s2[j] == s1[k]) {
                            score++;
                            break;
                        }
                    }
                }
            }
            if (s1.Substring(s1.Length - 2) == s2.Substring(s2.Length - 2)) {
                score += 5;
            }
            return score;
        }

        */

        void ReloadProvincesAttributes() {
            TextAsset ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/" + _provinceAttributeFile);
            if (ta == null)
                return;
            SetProvincesAttributes(ta.text);
        }

        /// <summary>
        /// Unpacks province geodata information. Used by Map Editor.
        /// </summary>
        /// <param name="province">Province.</param>
        public void ReadProvincePackedString(IAdminEntity entity) {
            Province province = (Province)entity;
            if (province == null || province.packedRegions == null)
                return;
            string[] regions = province.packedRegions.Split(SPLIT_SEP_ASTERISK, StringSplitOptions.RemoveEmptyEntries);
            int regionCount = regions.Length;
            province.regions = new List<Region>(regionCount);
            float maxVol = float.MinValue;
            Vector2 minProvince = Misc.Vector2one * 10;
            Vector2 maxProvince = -minProvince;
            for (int r = 0; r < regionCount; r++) {
                string[] coordinates = regions[r].Split(SPLIT_SEP_SEMICOLON, StringSplitOptions.RemoveEmptyEntries);
                int coorCount = coordinates.Length;
                Vector2 min = Misc.Vector2one * 10;
                Vector2 max = -min;
                Region provinceRegion = new Region(province, province.regions.Count);
                provinceRegion.points = new Vector2[coorCount];
                for (int c = 0; c < coorCount; c++) {
                    float x, y;
                    GetPointFromPackedString(ref coordinates[c], out x, out y);
                    if (x < min.x)
                        min.x = x;
                    if (x > max.x)
                        max.x = x;
                    if (y < min.y)
                        min.y = y;
                    if (y > max.y)
                        max.y = y;
                    provinceRegion.points[c].x = x;
                    provinceRegion.points[c].y = y;
                }
                FastVector.Average(ref min, ref max, ref provinceRegion.center);
                provinceRegion.sanitized = true;
                province.regions.Add(provinceRegion);

                // Calculate province bounding rect
                if (min.x < minProvince.x)
                    minProvince.x = min.x;
                if (min.y < minProvince.y)
                    minProvince.y = min.y;
                if (max.x > maxProvince.x)
                    maxProvince.x = max.x;
                if (max.y > maxProvince.y)
                    maxProvince.y = max.y;
                provinceRegion.rect2D = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
                provinceRegion.rect2DArea = provinceRegion.rect2D.width * provinceRegion.rect2D.height;
                float vol = FastVector.SqrDistance(ref min, ref max); // (max - min).sqrMagnitude;
                if (vol > maxVol) {
                    maxVol = vol;
                    province.mainRegionIndex = r;
                    province.center = provinceRegion.center;
                }
            }
            province.regionsRect2D = new Rect(minProvince.x, minProvince.y, Math.Abs(maxProvince.x - minProvince.x), Mathf.Abs(maxProvince.y - minProvince.y));
        }

        #region Drawing stuff


        /// <summary>
        /// Draws all countries provinces.
        /// </summary>
        void DrawAllProvinceBorders(bool forceRefresh, bool justComputeBorders) {

            if (!gameObject.activeInHierarchy)
                return;
            if (provincesObj != null && !forceRefresh)
                return;
            HideProvinces();
            if (!_showProvinces || !_drawAllProvinces)
                return;

            int numCountries = _countries.Length;
            List<Country> targetCountries = new List<Country>(numCountries);
            for (int k = 0; k < numCountries; k++) {
                if (!_countries[k].hidden || !Application.isPlaying) {
                    targetCountries.Add(_countries[k]);
                }
            }
            DrawProvinces(targetCountries, justComputeBorders);
        }


        /// <summary>
        /// Draws the provinces for specified country and optional also neighbours'
        /// </summary>
        /// <returns><c>true</c>, if provinces was drawn, <c>false</c> otherwise.</returns>
        /// <param name="countryIndex">Country index.</param>
        /// <param name="includeNeighbours">If set to <c>true</c> include neighbours.</param>
        /// <param name="forceRefresh">If set to <c>true</c> borders will be computed and redrawn again for the given country even though it was the last country whose provinces were drawn.</param>
        /// <param name="justComputeBorders">If set to <c>true</c> only province borders will be computed but no meshes will be created. This is useful when calling multiple times this function from editor functions.</param>
        bool mDrawProvinces(int countryIndex, bool includeNeighbours, bool forceRefresh, bool justComputeBorders) {

            if (!gameObject.activeInHierarchy || provinces == null) // asset not ready - return
                return false;

            if (!countries[countryIndex].showProvinces)
                return false;

            if (countryProvincesDrawnIndex == countryIndex && provincesObj != null && !forceRefresh)    // existing gameobject containing province borders?
                return false;

            bool res;
            if (_drawAllProvinces) {
                DrawAllProvinceBorders(forceRefresh, justComputeBorders);
                res = true;
            } else {
                // prepare a list with the countries to be drawn
                countryProvincesDrawnIndex = countryIndex;
                List<Country> targetCountries = new List<Country>(20);
                // add selected country
                targetCountries.Add(_countries[countryIndex]);
                // add neighbour countries?
                if (includeNeighbours) {
                    int rCount = _countries[countryIndex].regions.Count;
                    for (int k = 0; k < rCount; k++) {
                        List<Region> neighbours = _countries[countryIndex].regions[k].neighbours;
                        int nCount = neighbours.Count;
                        for (int n = 0; n < nCount; n++) {
                            Country c = (Country)neighbours[n].entity;
                            if (!targetCountries.Contains(c))
                                targetCountries.Add(c);
                        }
                    }
                }
                res = DrawProvinces(targetCountries, justComputeBorders);
            }

            if (res && _showOutline && !justComputeBorders) {
                Country country = _countries[countryIndex];
                Region region = country.regions[country.mainRegionIndex];
                int cacheIndex = GetCacheIndexForCountryRegion(countryIndex, country.mainRegionIndex);
                DrawCountryRegionOutline(cacheIndex.ToString(), region, true, 1f);
            }

            return res;

        }


        void TestProvinceNeighbourSegmentAny(Region region, int i, int j) {
            Vector2 p0 = region.points[i];
            Vector2 p1 = region.points[j];
            double v = (p0.x + p1.x) + MAP_PRECISION * (p0.y + p1.y);
            Region neighbour;
            if (frontiersCacheHit.TryGetValue(v, out neighbour)) {
                if (neighbour != region) {
                    if (!region.neighbours.Contains(neighbour)) {
                        region.neighbours.Add(neighbour);
                        neighbour.neighbours.Add(region);
                    }
                }
            } else {
                frontiersCacheHit.Add(v, region);
                frontiersPoints.Add(p0);
                frontiersPoints.Add(p1);
            }
        }

        void TestProvinceNeighbourSegmentInland(Region region, int i, int j) {
            Vector2 p0 = region.points[i];
            Vector2 p1 = region.points[j];
            double v = (p0.x + p1.x) + MAP_PRECISION * (p0.y + p1.y);
            Region neighbour;
            if (frontiersCacheHit.TryGetValue(v, out neighbour)) {
                if (neighbour != region) {
                    if (!region.neighbours.Contains(neighbour)) {
                        region.neighbours.Add(neighbour);
                        neighbour.neighbours.Add(region);
                    }
                    frontiersPoints.Add(p0);
                    frontiersPoints.Add(p1);
                }
            } else {
                frontiersCacheHit.Add(v, region);
            }
        }


        bool DrawProvinces(List<Country> targetCountries, bool justComputeBorders) {

            // optimize required lines
            if (frontiersCacheHit == null) {
                frontiersCacheHit = new Dictionary<double, Region>(500000);
            } else {
                frontiersCacheHit.Clear();
            }
            int tcCount = targetCountries.Count;

            if (justComputeBorders) {
                provinceNeighboursComputed = true;
                // Compute borders but ignore mesh building
                for (int c = 0; c < tcCount; c++) {
                    Country targetCountry = targetCountries[c];
                    if (targetCountry.provinces == null || targetCountry.hidden || !targetCountry.showProvinces)
                        continue;
                    for (int p = 0; p < targetCountry.provinces.Length; p++) {
                        Province province = targetCountry.provinces[p];
                        if (province.regions == null) { // read province data the first time we need it
                            ReadProvincePackedString(province);
                        }
                        int prCount = province.regions.Count;
                        for (int r = 0; r < prCount; r++) {
                            Region region = province.regions[r];
                            region.entity = province;
                            region.regionIndex = r;
                            region.neighbours.Clear();
                            int numPoints = region.points.Length - 1;
                            for (int i = 0; i < numPoints; i++) {
                                Vector2 p0 = region.points[i];
                                Vector2 p1 = region.points[i + 1];
                                double v = (p0.x + p1.x) + MAP_PRECISION * (p0.y + p1.y);
                                Region neighbour;
                                if (frontiersCacheHit.TryGetValue(v, out neighbour)) {
                                    if (neighbour != region) {
                                        if (!region.neighbours.Contains(neighbour)) {
                                            region.neighbours.Add(neighbour);
                                            neighbour.neighbours.Add(region);
                                        }
                                    }
                                } else {
                                    frontiersCacheHit[v] = region;
                                }
                            }
                        }
                    }
                }
                return true; // ignore mesh building
            }

            if (_showTiles && _currentZoomLevel > _tileLinesMaxZoomLevel)
                return false;

            if (frontiersPoints == null) {
                frontiersPoints = new List<Vector2>(1000000);
            } else {
                frontiersPoints.Clear();
            }

            TestNeighbourSegment testFunction;
            if (_provincesCoastlines) {
                testFunction = TestProvinceNeighbourSegmentAny;
            } else {
                testFunction = TestProvinceNeighbourSegmentInland;
            }

            for (int c = 0; c < tcCount; c++) {
                Country targetCountry = targetCountries[c];
                if (targetCountry.provinces == null || targetCountry.hidden || !targetCountry.showProvinces)
                    continue;
                for (int p = 0; p < targetCountry.provinces.Length; p++) {
                    Province province = targetCountry.provinces[p];
                    if (province.regions == null) { // read province data the first time we need it
                        ReadProvincePackedString(province);
                    }
                    int prCount = province.regions.Count;
                    for (int r = 0; r < prCount; r++) {
                        Region region = province.regions[r];
                        region.entity = province;
                        region.regionIndex = r;
                        region.neighbours.Clear();
                        int numPoints = region.points.Length - 1;
                        for (int i = 0; i < numPoints; i++) {
                            testFunction(region, i, i + 1);
                        }
                        // Close the polygon
                        testFunction(region, numPoints, 0);
                    }
                }
            }

            int meshGroups = (frontiersPoints.Count / 65000) + 1;
            int meshIndex = -1;
            int[][] provincesIndices = new int[meshGroups][];
            Vector3[][] provincesBorders = new Vector3[meshGroups][];
            int fpCount = frontiersPoints.Count;
            for (int k = 0; k < fpCount; k += 65000) {
                int max = Mathf.Min(fpCount - k, 65000);
                provincesBorders[++meshIndex] = new Vector3[max];
                provincesIndices[meshIndex] = new int[max];
                for (int j = k; j < k + max; j++) {
                    provincesBorders[meshIndex][j - k].x = frontiersPoints[j].x;
                    provincesBorders[meshIndex][j - k].y = frontiersPoints[j].y;
                    provincesIndices[meshIndex][j - k] = j - k;
                }
            }

            // Create province layer if needed
            if (provincesObj != null) {
                DestroyRecursive(provincesObj);
            }

            if (provincesBorders.Length > 0) {
                provincesObj = new GameObject("Provinces");
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(provincesObj); //provincesObj.hideFlags = HideFlags.DontSave;
                provincesObj.transform.SetParent(transform, false);
                provincesObj.transform.localPosition = Misc.Vector3back * 0.002f;
                provincesObj.layer = gameObject.layer;

                for (int k = 0; k < provincesBorders.Length; k++) {
                    GameObject flayer = new GameObject("flayer");
                    if (disposalManager != null)
                        disposalManager.MarkForDisposal(flayer); // flayer.hideFlags = HideFlags.DontSave;
                    flayer.hideFlags |= HideFlags.HideInHierarchy;
                    flayer.transform.SetParent(provincesObj.transform, false);
                    flayer.transform.localPosition = Misc.Vector3zero;
                    flayer.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
                    flayer.layer = provincesObj.layer;

                    Mesh mesh = new Mesh();
                    mesh.vertices = provincesBorders[k];
                    mesh.SetIndices(provincesIndices[k], MeshTopology.Lines, 0);
                    mesh.RecalculateBounds();
                    if (disposalManager != null)
                        disposalManager.MarkForDisposal(mesh); // mesh.hideFlags = HideFlags.DontSave;

                    MeshFilter mf = flayer.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;

                    MeshRenderer mr = flayer.AddComponent<MeshRenderer>();
                    mr.receiveShadows = false;
                    mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.sharedMaterial = provincesMat;
                }
            }
            return true;
        }

        #endregion



        #region Province highlighting

        int GetCacheIndexForProvinceRegion(int provinceIndex, int regionIndex) {
            return 1000000 + provinceIndex * 1000 + regionIndex;
        }

        /// <summary>
        /// Highlights the province region specified. Returns the generated highlight surface gameObject.
        /// Internally used by the Map UI and the Editor component, but you can use it as well to temporarily mark a country region.
        /// </summary>
        /// <param name="refreshGeometry">Pass true only if you're sure you want to force refresh the geometry of the highlight (for instance, if the frontiers data has changed). If you're unsure, pass false.</param>
        public GameObject HighlightProvinceRegion(int provinceIndex, int regionIndex, bool refreshGeometry) {
            if (!refreshGeometry && _provinceHighlightedIndex == provinceIndex && _provinceRegionHighlightedIndex == regionIndex)
                return provinceRegionHighlightedObj;
            if (provinceRegionHighlightedObj != null)
                HideProvinceRegionHighlight();
            if (provinceIndex < 0 || provinceIndex >= provinces.Length || provinces[provinceIndex].regions == null || regionIndex < 0 || regionIndex >= provinces[provinceIndex].regions.Count)
                return null;

            if (OnProvinceHighlight != null) {
                bool allowHighlight = true;
                OnProvinceHighlight(provinceIndex, regionIndex, ref allowHighlight);
                if (!allowHighlight)
                    return null;
            }

            int cacheIndex = GetCacheIndexForProvinceRegion(provinceIndex, regionIndex);
            GameObject obj;
            bool existsInCache = surfaces.TryGetValue(cacheIndex, out obj);
            if (refreshGeometry && existsInCache) {
                surfaces.Remove(cacheIndex);
                DestroyImmediate(obj);
                existsInCache = false;
            }

            bool doHighlight = true;
            if (_highlightMaxScreenAreaSize < 1f) {
                // Check screen area size
                Region region = provinces[provinceIndex].regions[regionIndex];
                doHighlight = CheckScreenAreaSizeOfRegion(region);
            }
            if (doHighlight) {
                if (_enableProvinceHighlight && _provinces[provinceIndex].allowHighlight && _countries[_provinces[provinceIndex].countryIndex].allowProvincesHighlight) {

                    if (existsInCache) {
                        provinceRegionHighlightedObj = surfaces[cacheIndex];
                        if (provinceRegionHighlightedObj == null) {
                            surfaces.Remove(cacheIndex);
                        } else {
                            if (!provinceRegionHighlightedObj.activeSelf)
                                provinceRegionHighlightedObj.SetActive(true);
                            Renderer[] rr = provinceRegionHighlightedObj.GetComponentsInChildren<Renderer>(true);
                            for (int k = 0; k < rr.Length; k++) {
                                if (rr[k].sharedMaterial != hudMatProvince && rr[k].sharedMaterial != outlineMatSimple) {
                                    rr[k].enabled = true;
                                    rr[k].sharedMaterial = hudMatProvince;
                                }
                            }
                        }
                    } else {
                        provinceRegionHighlightedObj = GenerateProvinceRegionSurface(provinceIndex, regionIndex, hudMatProvince);
                        // Add rest of regions?
                        if (_highlightAllProvinceRegions) {
                            Province province = provinces[provinceIndex];
                            for (int r = 0; r < province.regions.Count; r++) {
                                if (r != regionIndex) {
                                    Region otherRegion = province.regions[r];
                                    // Triangulate to get the polygon vertex indices
                                    Poly2Tri.Polygon poly = new Poly2Tri.Polygon(otherRegion.points);
                                    P2T.Triangulate(poly);
                                    GameObject otherSurf = Drawing.CreateSurface(provinceRegionHighlightedObj.name, poly, hudMatProvince, otherRegion.rect2D, Misc.Vector2zero, Misc.Vector2zero, 0, disposalManager);

                                    otherSurf.transform.SetParent(provinceRegionHighlightedObj.transform, false);
                                    otherSurf.transform.localPosition = Misc.Vector3zero;
                                    otherSurf.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
                                    otherSurf.layer = gameObject.layer;
                                }
                            }
                        }
                    }
                } else {
                    provinceRegionHighlightedObj = null;
                }
            }

            _provinceHighlighted = provinces[provinceIndex];
            _provinceHighlightedIndex = provinceIndex;
            _provinceRegionHighlighted = _provinceHighlighted.regions[regionIndex];
            _provinceRegionHighlightedIndex = regionIndex;
            return provinceRegionHighlightedObj;
        }

        void HideProvinceRegionHighlight() {
            if (provinceRegionHighlighted == null)
                return;
            // Hides province surface
            if (provinceRegionHighlightedObj != null) {
                if (provinceRegionHighlighted.customMaterial != null) {
                    ApplyMaterialToSurface(provinceRegionHighlightedObj, provinceRegionHighlighted.customMaterial);
                } else {
                    provinceRegionHighlightedObj.SetActive(false);
                }
                provinceRegionHighlightedObj = null;
            }

            // Raise exit event
            if (_provinceHighlightedIndex >= 0 && _provinceRegionHighlightedIndex >= 0) {
                if (OnProvinceExit != null) {
                    OnProvinceExit(_provinceHighlightedIndex, _provinceRegionHighlightedIndex);
                }
                if (OnRegionExit != null) {
                    OnRegionExit(_provinces[_provinceHighlightedIndex].regions[_provinceRegionHighlightedIndex]);
                }
            }

            _provinceHighlighted = null;
            _provinceHighlightedIndex = -1;
            _provinceRegionHighlighted = null;
            _provinceRegionHighlightedIndex = -1;
        }

        GameObject GenerateProvinceRegionSurface(int provinceIndex, int regionIndex, Material material) {
            return GenerateProvinceRegionSurface(provinceIndex, regionIndex, material, Misc.Vector2one, Misc.Vector2zero, 0);
        }

        void ProvinceSubstractProvinceEnclaves(int provinceIndex, Region region, Poly2Tri.Polygon poly) {
            List<Region> negativeRegions = new List<Region>();
            for (int oc = 0; oc < _countries.Length; oc++) {
                Country ocCountry = _countries[oc];
                if (ocCountry.hidden || ocCountry.provinces == null)
                    continue;
                Region mainCountryRegion = ocCountry.regions[ocCountry.mainRegionIndex];
                if (!mainCountryRegion.rect2D.Overlaps(region.rect2D))
                    continue;
                for (int op = 0; op < ocCountry.provinces.Length; op++) {
                    Province opProvince = ocCountry.provinces[op];
                    if (opProvince == provinces[provinceIndex])
                        continue;
                    if (opProvince.regions == null)
                        ReadProvincePackedString(opProvince);
                    if (opProvince.regions == null)
                        continue;
                    if (opProvince.regionsRect2D.Overlaps(region.rect2D, true)) {
                        Region oProvRegion = opProvince.regions[opProvince.mainRegionIndex];
                        if (region.Contains(oProvRegion)) { // just check main region of province for speed purposes
                            negativeRegions.Add(oProvRegion.Clone());
                        }
                    }
                }
            }
            // Collapse negative regions in big holes
            for (int nr = 0; nr < negativeRegions.Count - 1; nr++) {
                for (int nr2 = nr + 1; nr2 < negativeRegions.Count; nr2++) {
                    if (negativeRegions[nr].Intersects(negativeRegions[nr2])) {
                        Clipper clipper = new Clipper();
                        int control = negativeRegions[nr].points.Length;
                        clipper.AddPath(negativeRegions[nr], PolyType.ptSubject);
                        clipper.AddPath(negativeRegions[nr2], PolyType.ptClip);
                        clipper.Execute(ClipType.ctUnion);
                        negativeRegions.RemoveAt(nr2);
                        nr = -1;
                        break;
                    }
                }
            }

            // Substract holes
            int negativeRegionsCount = negativeRegions.Count;
            for (int r = 0; r < negativeRegionsCount; r++) {
                int pointCount = negativeRegions[r].points.Length;
                Vector2[] pp = new Vector2[pointCount];
                for (int p = 0; p < pointCount; p++) {
                    Vector2 point = negativeRegions[r].points[p];
                    Vector2 midPoint = negativeRegions[r].center;
                    pp[p] = point + (midPoint - point) * 0.0001f; // prevents Poly2Tri issues when enclave boarders are to near from region borders
                }
                Poly2Tri.Polygon polyHole = new Poly2Tri.Polygon(pp);
                poly.AddHole(polyHole);
            }
        }


        GameObject GenerateProvinceRegionSurface(int provinceIndex, int regionIndex, Material material, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return null;
            if (provinces[provinceIndex].regions == null)
                ReadProvincePackedString(provinces[provinceIndex]);
            if (provinces[provinceIndex].regions == null || regionIndex < 0 || regionIndex >= provinces[provinceIndex].regions.Count)
                return null;

            Province province = provinces[provinceIndex];
            Region region = province.regions[regionIndex];
            if (region.points.Length < 3)
                return null;

            // Triangulate to get the polygon vertex indices
            Poly2Tri.Polygon poly = new Poly2Tri.Polygon(region.points);

            if (_enableEnclaves && regionIndex == province.mainRegionIndex) {
                ProvinceSubstractProvinceEnclaves(provinceIndex, region, poly);
            }

            P2T.Triangulate(poly);

            // Prepare surface cache entry and deletes older surface if exists
            int cacheIndex = GetCacheIndexForProvinceRegion(provinceIndex, regionIndex);
            string cacheIndexSTR = cacheIndex.ToString();
            Transform t = surfacesLayer.transform.Find(cacheIndexSTR);
            if (t != null)
                DestroyImmediate(t.gameObject); // Deletes potential residual surface

            // Creates surface mesh
            GameObject surf = Drawing.CreateSurface(cacheIndexSTR, poly, material, region.rect2D, textureScale, textureOffset, textureRotation, disposalManager);
            surf.transform.SetParent(surfacesLayer.transform, false);
            surf.transform.localPosition = Misc.Vector3zero;
            surf.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
            surf.layer = gameObject.layer;
            surfaces[cacheIndex] = surf;
            return surf;
        }

        #endregion

        #region Province operations

        int ProvinceSizeComparer(Province p1, Province p2) {
            if (p1 == null || p2 == null || p1.mainRegionIndex < 0 || p2.mainRegionIndex < 0 || p1.regions == null || p2.regions == null)
                return 0;
            Region r1 = p1.regions[p1.mainRegionIndex];
            Region r2 = p2.regions[p2.mainRegionIndex];
            if (r1.rect2DArea < r2.rect2DArea) {
                return -1;
            } else if (r1.rect2DArea > r2.rect2DArea) {
                return 1;
            } else {
                return 0;
            }
        }


        /// <summary>
        /// Removes a province
        /// </summary>
        /// <param name="provinceIndex">Province index.</param>
        public bool ProvinceDelete(int provinceIndex) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return false;

            // Clears references from mount points
            if (mountPoints != null) {
                int mountPointsCount = mountPoints.Count;
                for (int k = 0; k < mountPointsCount; k++) {
                    if (mountPoints[k].provinceIndex == provinceIndex) {
                        mountPoints[k].provinceIndex = -1;
                    }
                }
            }

            List<Province> newProvinces = new List<Province>(_provinces.Length);
            // Clears references from cities
            int countryIndex = _provinces[provinceIndex].countryIndex;
            if (countryIndex >= 0 && countryIndex < _countries.Length) {
                string provinceName = _provinces[provinceIndex].name;
                if (cities != null) {
                    int citiesCount = _cities.Length;
                    for (int k = 0; k < citiesCount; k++) {
                        if (_cities[k].countryIndex == countryIndex && _cities[k].province.Equals(provinceName)) {
                            _cities[k].province = "";
                        }
                    }
                }

                // Remove it from the country array
                Country country = _countries[countryIndex];
                if (country.provinces != null) {
                    for (int k = 0; k < country.provinces.Length; k++) {
                        Province prov = country.provinces[k];
                        if (prov.regions.Count > 0 && !prov.name.Equals(provinceName))
                            newProvinces.Add(country.provinces[k]);
                    }
                    newProvinces.Sort(ProvinceSizeComparer);
                    country.provinces = newProvinces.ToArray();
                }
            }

            // Remove from the global array
            newProvinces.Clear();
            for (int k = 0; k < _provinces.Length; k++) {
                if (k != provinceIndex) {
                    newProvinces.Add(_provinces[k]);
                }
            }
            provinces = newProvinces.ToArray();

            return true;
        }

        #endregion

    }

}