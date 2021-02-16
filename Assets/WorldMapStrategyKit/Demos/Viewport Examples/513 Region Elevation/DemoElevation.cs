using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace WorldMapStrategyKit {
	public class DemoElevation : MonoBehaviour {

		WMSK map;
		GUIStyle labelStyle;

		void Start() {
			// Get a reference to the World Map API:
			map = WMSK.instance;

			// UI Setup - non-important, only for this demo
			labelStyle = new GUIStyle();
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.white;

			map.OnCountryClick += (int countryIndex, int regionIndex, int buttonIndex) => {
				map.RegionSetCustomElevation(map.GetCountry(countryIndex).regions, 0.7f);
			};
		}


		void OnGUI() {
 			GUIResizer.AutoResize();
			GUI.Box(new Rect(10, 10, 460, 40), "Click on a region to change its elevation", labelStyle);
		}

	}

}

