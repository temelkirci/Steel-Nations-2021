﻿using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {
				
				public class DemoTerrainFunctions : MonoBehaviour {

								WMSK map;

								void Start () {
												map = WMSK.instance;
								}

								public void FlyToAustralia () {
												FlyToCountry (map.GetCountryIndex ("Australia"));
								}

								void FlyToCountry (int countryIndex) {
												// Get zoom level for the extents of the country
												float zoomLevel = map.GetCountryRegionZoomExtents (countryIndex);
												map.FlyToCountry (countryIndex, 2.0f, zoomLevel);
												map.BlinkCountry (countryIndex, Color.green, Color.black, 3.0f, 0.2f);
								}

								public void FlyToMadrid () {
												map.FlyToCity ("Madrid", "Spain", 2.0f, 0.05f);
								}

								public void ToggleFreeCamera () {
												FreeCameraMove freeCameraScript = FindObjectOfType<FreeCameraMove> ();
												if (freeCameraScript != null) {
																freeCameraScript.enabled = !freeCameraScript.enabled;
																map.enableFreeCamera = freeCameraScript.enabled;
												}
								}

								public void ColorizeEurope() {
												for (int countryIndex = 0; countryIndex < map.countries.Length; countryIndex++) {
																if (map.countries [countryIndex].continent.Equals ("Europe")) {
																				Color color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
																				map.ToggleCountrySurface (countryIndex, true, color);
																}
												}

								}

				}
}