// World Strategy Kit for Unity - Main Script
// (C) 2016-2020 by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace WorldMapStrategyKit {
	public delegate void OnCityEnter(int cityIndex);
	public delegate void OnCityExit(int cityIndex);
	public delegate void OnCityClick(int cityIndex,int buttonIndex);

	public partial class WMSK : MonoBehaviour {

		public const int CITY_CLASS_FILTER_ANY = 255;
		public const int CITY_CLASS_FILTER_NORMAL_CITY = 1;
		public const int CITY_CLASS_FILTER_REGION_CAPITAL = 2;
		public const int CITY_CLASS_FILTER_COUNTRY_CAPITAL = 4;

		#region Public properties

		List<City> _cities;

		/// <summary>
		/// Complete list of cities with their names and country names.
		/// </summary>
		public List<City> cities {
			get { 
				if (_cities == null)
					ReadCitiesPackedString();
				return _cities;
			}
			set {
				_cities = value;
				lastCityLookupCount = -1;
			}
		}

		/// <summary>
		/// Returns City under mouse position or null if none.
		/// </summary>
		City _cityHighlighted;

		/// <summary>
		/// Returns City under mouse position or null if none.
		/// </summary>
		public City cityHighlighted { get { return _cityHighlighted; } }

		int _cityHighlightedIndex = -1;

		/// <summary>
		/// Returns City index mouse position or null if none.
		/// </summary>
		public int cityHighlightedIndex { get { return _cityHighlightedIndex; } }

		int _cityLastClicked = -1;

		/// <summary>
		/// Returns the last clicked city index.
		/// </summary>
		public int cityLastClicked { get { return _cityLastClicked; } }

		public event OnCityEnter OnCityEnter;
		public event OnCityEnter OnCityExit;
		public event OnCityClick OnCityClick;

		[SerializeField]
		bool
			_showCities = true;

		/// <summary>
		/// Toggle cities visibility.
		/// </summary>
		public bool showCities { 
			get {
				return _showCities; 
			}
			set {
				if (_showCities != value) {
					_showCities = value;
					isDirty = true;
					if (citiesLayer != null) {
						citiesLayer.SetActive(_showCities);
					} else if (_showCities) {
						DrawCities();
					}
				}
			}
		}

		[SerializeField]
		Color
			_citiesColor = Color.white;

		/// <summary>
		/// Global color for cities.
		/// </summary>
		public Color citiesColor {
			get {
				if (citiesNormalMat != null) {
					return citiesNormalMat.color;
				} else {
					return _citiesColor;
				}
			}
			set {
				if (value != _citiesColor) {
					_citiesColor = value;
					isDirty = true;
					
					if (citiesNormalMat != null && _citiesColor != citiesNormalMat.color) {
						citiesNormalMat.color = _citiesColor;
					}
				}
			}
		}

		[SerializeField]
		Color
			_citiesRegionCapitalColor = Color.cyan;

		/// <summary>
		/// Global color for region capitals.
		/// </summary>
		public Color citiesRegionCapitalColor {
			get {
				if (citiesRegionCapitalMat != null) {
					return citiesRegionCapitalMat.color;
				} else {
					return _citiesRegionCapitalColor;
				}
			}
			set {
				if (value != _citiesRegionCapitalColor) {
					_citiesRegionCapitalColor = value;
					isDirty = true;
					
					if (citiesRegionCapitalMat != null && _citiesRegionCapitalColor != citiesRegionCapitalMat.color) {
						citiesRegionCapitalMat.color = _citiesRegionCapitalColor;
					}
				}
			}
		}

		
		[SerializeField]
		Color
			_citiesCountryCapitalColor = Color.yellow;

		/// <summary>
		/// Global color for country capitals.
		/// </summary>
		public Color citiesCountryCapitalColor {
			get {
				if (citiesCountryCapitalMat != null) {
					return citiesCountryCapitalMat.color;
				} else {
					return _citiesCountryCapitalColor;
				}
			}
			set {
				if (value != _citiesCountryCapitalColor) {
					_citiesCountryCapitalColor = value;
					isDirty = true;
					
					if (citiesCountryCapitalMat != null && _citiesCountryCapitalColor != citiesCountryCapitalMat.color) {
						citiesCountryCapitalMat.color = _citiesCountryCapitalColor;
					}
				}
			}
		}

		[SerializeField]
		float _cityIconSize = 1.0f;

		/// <summary>
		/// The size of the cities icon (dot).
		/// </summary>
		public float cityIconSize { 
			get {
				return _cityIconSize; 
			}
			set {
				if (value != _cityIconSize) {
					_cityIconSize = value;
					ScaleCities(); 
					ScaleMountPoints();	// for the Editor's icon: mount points are invisible at runtime
					isDirty = true;
				}
			}
		}

		[SerializeField]
		GameObject _citySpot;

		/// <summary>
		/// Allows you to change default icon for normal cities. This must be a 2D game object (you may duplicate and modify city prefabs in WMSK/Resources/Prefabs folder).
		/// </summary>
		public GameObject citySpot {
			get {
				return _citySpot;
			}
			set {
				if (value != _citySpot) {
					_citySpot = value;
					isDirty = true;
					DrawCities();
				}
			}
		}

		[SerializeField]
		GameObject _citySpotCapitalRegion;

		/// <summary>
		/// Allows you to change default icon for region capitals. This must be a 2D game object (you may duplicate and modify city prefabs in WMSK/Resources/Prefabs folder).
		/// </summary>
		public GameObject citySpotCapitalRegion {
			get {
				return _citySpotCapitalRegion;
			}
			set {
				if (value != _citySpotCapitalRegion) {
					_citySpotCapitalRegion = value;
					isDirty = true;
					DrawCities();
				}
			}
		}

		
		[SerializeField]
		GameObject _citySpotCapitalCountry;

		/// <summary>
		/// Allows you to change default icon for country capitals. This must be a 2D game object (you may duplicate and modify city prefabs in WMSK/Resources/Prefabs folder).
		/// </summary>
		public GameObject citySpotCapitalCountry {
			get {
				return _citySpotCapitalCountry;
			}
			set {
				if (value != _citySpotCapitalCountry) {
					_citySpotCapitalCountry = value;
					isDirty = true;
					DrawCities();
				}
			}
		}


		[Range(0, 17000)]
		[SerializeField]
		int
			_minPopulation = 0;

		public int minPopulation {
			get {
				return _minPopulation;
			}
			set {
				if (value != _minPopulation) {
					_minPopulation = value;
					isDirty = true;
					DrawCities();
				}
			}
		}

		
		[SerializeField]
		int _cityClassAlwaysShow;

		/// <summary>
		/// Flags for specifying the class of cities to always show irrespective of other filters like minimum population. Can assign a combination of bit flags defined by CITY_CLASS_FILTER* 
		/// </summary>
		public int cityClassAlwaysShow {
			get { return _cityClassAlwaysShow; }
			set {
				if (_cityClassAlwaysShow != value) {
					_cityClassAlwaysShow = value;
					isDirty = true;
					DrawCities();
				}
			}
		}

		[NonSerialized]
		public int
			numCitiesDrawn = 0;


		string _cityAttributeFile = CITY_ATTRIB_DEFAULT_FILENAME;

		public string cityAttributeFile {
			get { return _cityAttributeFile; }
			set {
				if (value != _cityAttributeFile) {
					_cityAttributeFile = value;
					if (_cityAttributeFile == null)
						_cityAttributeFile = CITY_ATTRIB_DEFAULT_FILENAME;
					isDirty = true;
					ReloadCitiesAttributes();
				}
			}
		}

		#endregion

		#region Public API area

		/// <summary>
		/// Deletes all cities of current selected country's continent
		/// </summary>
		public void CitiesDeleteFromContinent(string continentName) {
			HideCityHighlights();
			int k = -1;
			int cityCount = cities.Count;
			while (++k < cityCount) {
				int cindex = _cities[k].countryIndex;
				if (cindex >= 0) {
					string cityContinent = _countries[cindex].continent;
					if (cityContinent.Equals(continentName)) {
						_cities.RemoveAt(k);
						k--;
						cityCount--;
					}
				}
			}
		}

		/// <summary>
		/// Returns the index of a city in the global cities collection. Note that country name needs to be supplied due to repeated city names.
		/// </summary>
		public int GetCityIndex(string cityName, string countryName) {
			int countryIndex = GetCountryIndex(countryName);
			return GetCityIndex(cityName, countryIndex);
		}

		/// <summary>
		/// Returns the index of a city in the global cities collection. Note that country and province indices should be supplied due to repeated city names.
		/// </summary>
		public int GetCityIndex(string cityName, int countryIndex = -1, int provinceIndex = -1) {
			int cityCount = cities.Count;
			string provinceName = "";
			if (provinceIndex >= 0 && provinceIndex<provinces.Length) {
				provinceName = GetProvince (provinceIndex).name;
			}

			if (countryIndex >= 0 && countryIndex < _countries.Length) {
				for (int k = 0; k < cityCount; k++) {
					if (_cities[k].countryIndex == countryIndex && (provinceIndex<0 || provinceName.Equals(_cities[k].province)) && _cities[k].name.Equals(cityName))
						return k;
				}
			} else {
				// Try to select city by its name alone
				for (int k = 0; k < cityCount; k++) {
					if (_cities[k].name.Equals(cityName))
						return k;
				}
			}
			return -1;
		}

		/// <summary>
		/// Flashes specified city by name in the global city collection.
		/// Returns false if city is not found
		/// </summary>
		public bool BlinkCity(string cityName, string countryName, Color color1, Color color2, float duration, float blinkingSpeed) {
			int cityIndex = GetCityIndex(cityName, countryName);
			if (cityIndex < 0)
				return false;
			BlinkCity(cityIndex, color1, color2, duration, blinkingSpeed);
			return true;
		}


		/// <summary>
		/// Flashes specified city by index in the global city collection.
		/// </summary>
		public void BlinkCity(int cityIndex, Color color1, Color color2, float duration, float blinkingSpeed) {
			if (citiesLayer == null)
				return;

			string cobj = GetCityHierarchyName(cityIndex);
			Transform t = transform.Find(cobj);
			if (t == null)
				return;
			if (t.GetComponent<Renderer>() == null) {
				Debug.Log("City game object needs a renderer component for blinking effect.");
				return;
			}
			CityBlinker sb = t.gameObject.AddComponent<CityBlinker>();
			sb.blinkMaterial = t.GetComponent<Renderer>().sharedMaterial;
			sb.color1 = color1;
			sb.color2 = color2;
			sb.duration = duration;
			sb.speed = blinkingSpeed;
		}


		/// <summary>
		/// Starts navigation to target city. Returns false if not found.
		/// </summary>
		public bool FlyToCity(string cityName, string countryName) {
			return FlyToCity(name, countryName, _navigationTime);
		}

		/// <summary>
		/// Starts navigation to target city with duration provided. Returns false if not found.
		/// </summary>
		public bool FlyToCity(string name, string countryName, float duration) {
			int cityIndex = GetCityIndex(name, countryName);
			if (cityIndex >= 0) {
				FlyToCity(cityIndex);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Starts navigation to target city. with specified duration and zoom level, ignoring NavigationTime property.
		/// Set duration to zero to go instantly.
		/// Returns false if city is not found. 
		/// </summary>
		public bool FlyToCity(string name, string countryName, float duration, float zoomLevel) {
			int cityIndex = GetCityIndex(name, countryName);
			if (cityIndex >= 0) {
				FlyToCity(cityIndex, duration, zoomLevel);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Starts navigation to target city by index in the cities collection.
		/// </summary>
		public void FlyToCity(int cityIndex) {
			FlyToCity(cityIndex, _navigationTime);
		}

		
		/// <summary>
		/// Starts navigation to target city by index with provided duration
		/// </summary>
		public void FlyToCity(int cityIndex, float duration) {
			SetDestination(cities[cityIndex].unity2DLocation, duration);
		}

		
		/// <summary>
		/// Starts navigation to target city by index with provided duration and zoom level
		/// </summary>
		public void FlyToCity(int cityIndex, float duration, float zoomLevel) {
			if (cities == null || cityIndex < 0 || cityIndex >= cities.Count)
				return;
			SetDestination(cities[cityIndex].unity2DLocation, duration, zoomLevel);
		}

		/// <summary>
		/// Starts navigation to target city by passing a City object with provided duration and zoom level
		/// </summary>
		public void FlyToCity(City city, float duration, float zoomLevel) {
			int cityIndex = GetCityIndex(city);
			FlyToCity(cityIndex, duration, zoomLevel);
		}

		/// <summary>
		/// Returns the index of a city in the cities collection by its reference.
		/// </summary>
		public int GetCityIndex(City city) {
			int res;
			if (cityLookup.TryGetValue(city, out res)) {
				return res;
			} else {
				return -1;
			}
		}

		/// <summary>
		/// Returns the city object by its name belonging to provided country and province.
		/// </summary>
		public City GetCity(string cityName, string countryName = "", string provinceName = "") {
			int countryIndex = string.IsNullOrEmpty(countryName) ? -1 : GetCountryIndex(countryName);
			int provinceIndex = string.IsNullOrEmpty(provinceName) ? -1: GetProvinceIndex (countryName, provinceName);
			int cityIndex = GetCityIndex(cityName, countryIndex, provinceIndex);
			if (cityIndex >= 0)
				return cities[cityIndex];
			return null;
		}


		/// <summary>
		/// Returns city object by its index.
		/// </summary>
		public City GetCity(int cityIndex) {
			if (cityIndex < 0 || cityIndex >= _cities.Count)
				return null;
			return _cities[cityIndex];
		}


		/// <summary>
		/// Gets a random city from a given country
		/// </summary>
		/// <returns>The city.</returns>
		/// <param name="country">Country object.</param>
		public City GetCityRandom(Country country) {
			int cityCount = cities.Count;
			int countryIndex = GetCountryIndex(country);
			List<City> cc = new List<City>(100);
			for (int k = 0; k < cityCount; k++) {
				if (_cities[k].countryIndex == countryIndex) {
					cc.Add(_cities[k]);
				}
			}
			int count = cc.Count;
			if (count == 0)
				return null;
			return cc[UnityEngine.Random.Range(0, count)];
		}


		/// <summary>
		/// Gets a random city from a given province
		/// </summary>
		/// <returns>The city.</returns>
		/// <param name="province">Province object.</param>
		public City GetCityRandom(Province province) {
			int cityCount = cities.Count;
			int countryIndex = province.countryIndex;
			List<City> cc = new List<City>(100);
			for (int k = 0; k < cityCount; k++) {
				if (_cities[k].countryIndex == countryIndex && _cities[k].province.Equals(province.name)) {
					cc.Add(_cities[k]);
				}
			}
			int count = cc.Count;
			if (count == 0)
				return null;
			return cc[UnityEngine.Random.Range(0, count)];
		}


		/// <summary>
		/// Gets a random visible city
		/// </summary>
		/// <returns>The city.</returns>
		public City GetCityRandom() {
			int count = visibleCities.Length;
			if (count == 0)
				return null;
			return visibleCities[UnityEngine.Random.Range(0, count)];
		}

		/// <summary>
		/// Gets a random city index from a given country
		/// </summary>
		/// <returns>The city random.</returns>
		/// <param name="country">Country object.</param>
		public int GetCityIndexRandom(Country country) {
			City city = GetCityRandom(country);
			if (city == null)
				return -1;
			return GetCityIndex(city);
		}


		/// <summary>
		/// Gets a random city index from a given province
		/// </summary>
		/// <returns>The city random.</returns>
		/// <param name="province">Province object.</param>
		public int GetCityIndexRandom(Province province) {
			City city = GetCityRandom(province);
			if (city == null)
				return -1;
			return GetCityIndex(city);
		}


		/// <summary>
		/// Gets the city index with that unique Id.
		/// </summary>
		public int GetCityIndex(int uniqueId) {
			int cityCount = cities.Count;
			for (int k = 0; k < cityCount; k++) {
				if (cities[k].uniqueId == uniqueId)
					return k;
			}
			return -1;
		}


		/// <summary>
		/// Returns the city index by screen position
		/// </summary>
		public bool GetCityIndex(Ray ray, out int cityIndex) {
			return GetCityIndex(ray, -1, out cityIndex);

		}

		/// <summary>
		/// Returns the city index by screen position optionally limited to a given country index.
		/// </summary>
		public bool GetCityIndex(Ray ray, int countryIndex, out int cityIndex) {
			int hitCount = Physics.RaycastNonAlloc(ray, tempHits, 500, layerMask);
			if (hitCount > 0) {
				for (int k = 0; k < hitCount; k++) {
					if (tempHits[k].collider.gameObject == gameObject) {
						Vector3 localHit = transform.InverseTransformPoint(tempHits[k].point);
						int c = GetCityNearPoint(localHit, countryIndex);
						if (c >= 0) {
							cityIndex = c;
							return true;
						}
					}
				}
			}
			cityIndex = -1;
			return false;
		}


		/// <summary>
		/// Gets the name of the country of the city.
		/// </summary>
		public string GetCityCountryName(City city) {
			Country country = GetCountry(city.countryIndex);
			if (country != null)
				return country.name;
			else
				return "";
		}

		/// <summary>
		/// Gets the name of the province of the city.
		/// </summary>
		public string GetCityProvinceName(City city) {
			return city.province;
		}

		/// <summary>
		/// Clears any city highlighted (color changed) and resets them to default city color
		/// </summary>
		public void HideCityHighlights() {
			DrawCities();
		}

		/// <summary>
		/// Returns the index of the country's capital city in the cities array
		/// </summary>
		/// <returns>The country capital index.</returns>
		/// <param name="countryIndex">Country index.</param>
		public int GetCountryCapitalIndex(int countryIndex) {
			if (countryIndex >= 0 && countryIndex < countries.Length) {
				return _countries [countryIndex].capitalCityIndex;
			}
			return -1;
		}

		/// <summary>
		/// Returns the index of the country's capital city in the cities array
		/// </summary>
		/// <returns>The country capital index.</returns>
		/// <param name="countryName">Country name.</param>
		public int GetCountryCapitalIndex(string countryName) {
			int countryIndex = GetCountryIndex (countryName);
			if (countryIndex >= 0 && countryIndex < countries.Length) {
				return _countries [countryIndex].capitalCityIndex;
			}
			return -1;
		}


		/// <summary>
		/// Returns the country's capital city
		/// </summary>
		/// <returns>The country capital index.</returns>
		/// <param name="countryIndex">Country index.</param>
		public City GetCountryCapital(int countryIndex) {
			int cityIndex = GetCountryCapitalIndex (countryIndex);
			if (cityIndex >= 0) {
				return cities [cityIndex];
			} else {
				return null;
			}
		}

		/// <summary>
		/// Returns the country's capital city
		/// </summary>
		/// <returns>The country capital index.</returns>
		/// <param name="countryName">Country name.</param>
		public City GetCountryCapital(string countryName) {
			int cityIndex = GetCountryCapitalIndex (countryName);
			if (cityIndex >= 0) {
				return cities [cityIndex];
			} else {
				return null;
			}
		}


		/// <summary>
		/// Toggles the city highlight.
		/// </summary>
		/// <param name="cityIndex">City index.</param>
		/// <param name="color">Color.</param>
		/// <param name="highlighted">If set to <c>true</c> the color of the city will be changed. If set to <c>false</c> the color of the city will be reseted to default color</param>
		public void ToggleCityHighlight(int cityIndex, Color color, bool highlighted) {
			if (citiesLayer == null)
				return;
			string cobj = GetCityHierarchyName(cityIndex);
			Transform t = transform.Find(cobj);
			if (t == null)
				return;
			Renderer rr = t.gameObject.GetComponent<Renderer>();
			if (rr == null)
				return;
			Material mat;
			if (highlighted) {
				mat = Instantiate(rr.sharedMaterial);
				if (disposalManager!=null) disposalManager.MarkForDisposal(mat); // mat.hideFlags = HideFlags.DontSave;
				mat.color = color;
				rr.sharedMaterial = mat;
			} else {
				switch (cities[cityIndex].cityClass) {
					case CITY_CLASS.COUNTRY_CAPITAL:
						mat = citiesCountryCapitalMat;
						break;
					case CITY_CLASS.REGION_CAPITAL:
						mat = citiesRegionCapitalMat;
						break;
					default:
						mat = citiesNormalMat;
						break;
				}
				rr.sharedMaterial = mat;
			}
		}

		/// <summary>
		/// Returns an array with the city names.
		/// </summary>
		public string[] GetCityNames() {
			int cityCount = cities.Count;
			List<string> c = new List<string>(cityCount);
			for (int k = 0; k < cityCount; k++) {
				c.Add(_cities[k].name + " (" + k + ")");
			}
			c.Sort();
			return c.ToArray();
		}

		/// <summary>
		/// Returns an array with the city names.
		/// </summary>
		public string[] GetCityNames(int countryIndex) {
			int cityCount = cities.Count;
			List<string> c = new List<string>(cityCount);
			for (int k = 0; k < cityCount; k++) {
				if (_cities[k].countryIndex == countryIndex) {
					c.Add(_cities[k].name + " (" + k + ")");
				}
			}
			c.Sort();
			return c.ToArray();
		}


		/// <summary>
		/// Returns a list of cities whose attributes matches predicate
		/// </summary>
		public List<City> GetCities(AttribPredicate predicate) {
			List <City> selectedCities = new List<City>();
			int cityCount = cities.Count;
			for (int k = 0; k < cityCount; k++) {
				City city = _cities[k];
				if (predicate(city.attrib))
					selectedCities.Add(city);
			}
			return selectedCities;
		}


		/// <summary>
		/// Returns a list of cities
		/// </summary>
		/// <returns>The cities.</returns>
		/// <param name="cityFilters">Optional city class filter. Use WMSK.CITY_CLASS_FILTER constants to filter by specific types. You can combine several filters using | (OR) operator.</param>
		public List<City>GetCities(bool onlyVisible = false, int cityFilters = WMSK.CITY_CLASS_FILTER_ANY) {
			List<City> theCities = new List<City>();
			int count = cities.Count;
			for (int k = 0; k < count; k++) {
				City city = _cities[k];
				if ((city.isVisible || !onlyVisible) && ((int)city.cityClass & cityFilters) != 0) {
					theCities.Add(city);
				}
			}
			return theCities;
		}

		/// <summary>
		/// Returns a list of cities belonging to a country
		/// </summary>
		/// <returns>The cities.</returns>
		/// <param name="country">country.</param>
		/// <param name="cityFilters">Optional city class filter. Use WMSK.CITY_CLASS_FILTER constants to filter by specific types. You can combine several filters using | (OR) operator.</param>
		public List<City>GetCities(Country country, bool onlyVisible = false, int cityFilters = WMSK.CITY_CLASS_FILTER_ANY) {
			if (country == null)
				return null;
			List<City> countryCities = new List<City>();
			int countryIndex = GetCountryIndex(country);
			int count = cities.Count;
			for (int k = 0; k < count; k++) {
				City city = _cities[k];
				if (city.countryIndex == countryIndex && (city.isVisible || !onlyVisible) && ((int)city.cityClass & cityFilters) != 0) {
					countryCities.Add(city);
				}
			}
			return countryCities;
		}


		/// <summary>
		/// Returns a list of cities belonging to a province
		/// </summary>
		/// <returns>The cities.</returns>
		/// <param name="province">Country.</param>
		/// <param name="cityFilters">Optional city class filter. Use WMSK.CITY_CLASS_FILTER constants to filter by specific types. You can combine several filters using | (OR) operator.</param>
		public List<City>GetCities(Province province, bool onlyVisible = false, int cityFilters = WMSK.CITY_CLASS_FILTER_ANY) {
			if (province == null)
				return null;
			int countryIndex = province.countryIndex;
			List<City> provinceCities = new List<City>();
			int count = cities.Count;
			for (int k = 0; k < count; k++) {
				City city = _cities[k];
				if (city.countryIndex == countryIndex && (city.isVisible || !onlyVisible) && province.name.Equals(city.province) && ((int)city.cityClass & cityFilters) != 0) {
					provinceCities.Add(city);
				}
			}
			return provinceCities;
		}


		/// <summary>
		/// Returns a list of cities contained in a given region
		/// <param name="cityFilters">Optional city class filter. Use WMSK.CITY_CLASS_FILTER constants to filter by specific types. You can combine several filters using | (OR) operator.</param>
		/// </summary>
		public List<City> GetCities(Region region, bool onlyVisible = false, int cityFilters = WMSK.CITY_CLASS_FILTER_ANY) {
			int citiesCount = cities.Count;
			List<City> cc = new List<City>();
			for (int k = 0; k < citiesCount; k++) {
				City city = _cities[k];
				if ((city.isVisible || !onlyVisible) && region.Contains(city.unity2DLocation) && ((int)city.cityClass & cityFilters) != 0) {
					cc.Add(_cities[k]);
				}
			}
			return cc;
		}

		#endregion

		#region IO functions area

		/// <summary>
		/// Exports the geographic data in packed string format.
		/// </summary>
		public string GetCityGeoData() {
			StringBuilder sb = new StringBuilder();
			for (int k = 0; k < cities.Count; k++) {
				City city = cities[k];
				if (k > 0)
					sb.Append("|");
				sb.Append(city.name);
				sb.Append("$");
				sb.Append(city.province);
				sb.Append("$");
				sb.Append(countries[city.countryIndex].name);
				sb.Append("$");
				sb.Append(city.population.ToString(Misc.InvariantCulture));
				sb.Append("$");
				sb.Append(city.unity2DLocation.x.ToString(Misc.InvariantCulture));
				sb.Append("$");
				sb.Append(city.unity2DLocation.y.ToString(Misc.InvariantCulture));
				sb.Append("$");
				sb.Append(((int)city.cityClass).ToString(Misc.InvariantCulture));
				sb.Append("$");
				sb.Append(city.uniqueId);
				
			}
			return sb.ToString();
		}

		/// <summary>
		/// Loads cities information from a string
		/// </summary>
		public void SetCityGeoData(string s) {
			lastCityLookupCount = -1;
			
			string[] cityList = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			int cityCount = cityList.Length;
			_cities = new List<City>(cityCount);
			char[] separatorCities = new char[] { '$' };
			int cityIndex = 0;
			for (int k = 0; k < cityCount; k++) {
				string[] cityInfo = cityList[k].Split(separatorCities);
				string country = cityInfo[2];
				int countryIndex = GetCountryIndex(country);
				if (countryIndex >= 0) {
					string name = cityInfo[0];
					string province = cityInfo[1];
					int population = int.Parse(cityInfo[3], Misc.InvariantCulture);
					float x = float.Parse(cityInfo[4], Misc.InvariantCulture);
					float y = float.Parse(cityInfo[5], Misc.InvariantCulture);
					CITY_CLASS cityClass = (CITY_CLASS)int.Parse(cityInfo[6], Misc.InvariantCulture);
					int uniqueId;
					if (cityInfo.Length >= 8) {
						uniqueId = int.Parse(cityInfo[7], Misc.InvariantCulture);
					} else {
						uniqueId = GetUniqueId (new List<IExtendableAttribute>(_cities.ToArray()));
					}
					if (cityClass == CITY_CLASS.COUNTRY_CAPITAL) {
						_countries [countryIndex].capitalCityIndex = cityIndex;
					}
					City city = new City(name, province, countryIndex, population, new Vector3(x, y), cityClass, uniqueId);
					_cities.Add(city);
					cityIndex++;
				}
			}
		}

		/// <summary>
		/// Gets XML attributes of all cities in jSON format.
		/// </summary>
		public string GetCitiesAttributes(bool prettyPrint = true) {
			return GetCitiesAttributes(new List<City>(cities), prettyPrint);
		}

		/// <summary>
		/// Gets XML attributes of provided cities in jSON format.
		/// </summary>
		public string GetCitiesAttributes(List<City> cities, bool prettyPrint = true) {
			JSONObject composed = new JSONObject();
			int cityCount = cities.Count;
			for (int k = 0; k < cityCount; k++) {
				City city = _cities[k];
				if (city.attrib.keys != null)
					composed.AddField(city.uniqueId.ToString(), city.attrib);
			}
			return composed.Print(prettyPrint);
		}

		/// <summary>
		/// Sets cities attributes from a jSON formatted string.
		/// </summary>
		public void SetCitiesAttributes(string jSON) {
			JSONObject composed = new JSONObject(jSON);
			if (composed.keys == null)
				return;
			int keyCount = composed.keys.Count;
			for (int k = 0; k < keyCount; k++) {
				int uniqueId = int.Parse(composed.keys[k]);
				int cityIndex = GetCityIndex(uniqueId);
				if (cityIndex >= 0) {
					cities[cityIndex].attrib = composed[k];
				}
			}
		}

		#endregion


	}

}