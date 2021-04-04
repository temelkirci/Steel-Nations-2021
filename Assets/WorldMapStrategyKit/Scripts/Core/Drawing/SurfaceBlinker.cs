using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {
	public class SurfaceBlinker : MonoBehaviour {

		public float duration;
		public Color color1, color2;
		public float speed;
		public Material blinkMaterial;
		public Region customizableSurface;
		public bool smoothBlink;

		Material oldMaterial;
		float startTime, lapTime;
		bool whichColor;
		WMSK map;

		void Start () {
			oldMaterial = GetComponent<Renderer> ().sharedMaterial;
			GenerateMaterial ();
			map = WMSK.GetInstance (transform);
			startTime = map.time;
			lapTime = startTime - speed;
		}

        
        // Update is called once per frame
        void Update () {
			float elapsed = map.time - startTime;
			if (elapsed > duration) {
				// Restores material
				Material goodMat;
				if (customizableSurface.customMaterial != null) {
					goodMat = customizableSurface.customMaterial;
				} else {
					goodMat = oldMaterial;
				}
				GetComponent<Renderer> ().sharedMaterial = goodMat;
				// Hide surface?
				if (customizableSurface.customMaterial == null) {
					gameObject.SetActive (false);
				}
				Destroy (this);
				return;
			}
			if (smoothBlink) {
				Material mat = GetComponent<Renderer> ().sharedMaterial;
				if (mat != blinkMaterial)
					GenerateMaterial ();

				float t = Mathf.PingPong (map.time * speed, 1f);
				blinkMaterial.color = Color.Lerp (color1, color2, t);

			} else if (map.time - lapTime > speed) {
				lapTime = map.time;
				Material mat = GetComponent<Renderer> ().sharedMaterial;
				if (mat != blinkMaterial)
					GenerateMaterial ();
				whichColor = !whichColor;
				if (whichColor) {
					blinkMaterial.color = color1;
				} else {
					blinkMaterial.color = color2;
				}
			}
		}

		void GenerateMaterial () {
			blinkMaterial = Instantiate (blinkMaterial);
			blinkMaterial.hideFlags = HideFlags.DontSave;
			GetComponent<Renderer> ().sharedMaterial = blinkMaterial;
		}
	}

}