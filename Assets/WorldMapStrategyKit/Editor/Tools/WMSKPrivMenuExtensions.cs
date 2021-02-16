using UnityEngine;
using UnityEditor;
using System.Collections;

namespace WorldMapStrategyKit {

	public static class WMSKMenuExtensions {
		[MenuItem ("GameObject/3D Object/World Map Strategy Kit Map")]
		static void CreateWMSKMap () {
			GameObject wmsk = GameObject.Instantiate (Resources.Load<GameObject> ("WMSK/Prefabs/WorldMapStrategyKit"));
			wmsk.name = "WorldMapStrategyKit";
		}


		[MenuItem ("GameObject/3D Object/World Map Strategy Kit Viewport")]
		static void CreateWMSKViewport () {
			GameObject viewport = GameObject.Instantiate (Resources.Load<GameObject> ("WMSK/Prefabs/Viewport"));
			viewport.name = "Viewport";
			if (!WMSK.instanceExists) {
				GameObject wmsk = GameObject.Instantiate (Resources.Load<GameObject> ("WMSK/Prefabs/WorldMapStrategyKit"));
				wmsk.name = "WorldMapStrategyKit";
			}
		}
	}
}
