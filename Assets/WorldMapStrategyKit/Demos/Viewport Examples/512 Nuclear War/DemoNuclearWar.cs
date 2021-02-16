using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace WorldMapStrategyKit {
	public class DemoNuclearWar : MonoBehaviour {

		WMSK map;
		public GameObject target;


		void Start () {
			// Get a reference to the World Map API:
			map = WMSK.instance;

			// Focus on middle ground
			Vector2 koreaPos = map.GetCity ("Pyongyang", "North Korea").unity2DLocation;
			Vector2 japanPos = map.GetCity ("Tokyo", "Japan").unity2DLocation;
			Vector2 midPos = (koreaPos + japanPos) / 2f;
			map.FlyToLocation (midPos, 3f, 0.08f);

			// Launch 5 waves of attacks and counter-attacks
			for (int wave = 0; wave < 5; wave++) {
				StartCoroutine (War (wave * 3));
			}
		}

		IEnumerator War (int wave) {

			float start = Time.time;
			while (Time.time - start < wave) {
				yield return null;
			}

			// N. Korea attacks Japan
			StartCoroutine (LaunchMissile (2f, "North Korea", "Japan", Color.red));
			StartCoroutine (LaunchMissile (3f, "North Korea", "Japan", Color.red));

			// Japan attacks N. Korea
			StartCoroutine (LaunchMissile (5f, "Japan", "North Korea", Color.white));
			StartCoroutine (LaunchMissile (6f, "Japan", "North Korea", Color.white));
			StartCoroutine (LaunchMissile (7f, "Japan", "North Korea", Color.white));
			StartCoroutine (LaunchMissile (8f, "Japan", "North Korea", Color.white));

			if (wave > 1) {
				// N. Korea attack S. Korea
				StartCoroutine (LaunchMissile (4f, "North Korea", "South Korea", Color.red));
				StartCoroutine (LaunchMissile (5f, "North Korea", "South Korea", Color.red));

				// S. Korea attack N. Korea
				StartCoroutine (LaunchMissile (6f, "South Korea", "North Korea", Color.green));
				StartCoroutine (LaunchMissile (7f, "South Korea", "North Korea", Color.green));
				StartCoroutine (LaunchMissile (8f, "South Korea", "North Korea", Color.green));

			}

		}


		IEnumerator LaunchMissile (float delay, string countryOrigin, string countryDest, Color color) {
			float start = Time.time;
			while (Time.time - start < delay) {
				yield return null;
			}

			// Initiates line animation
			int cityOrigin = map.GetCityIndexRandom (map.GetCountry(countryOrigin));
			int cityDest = map.GetCityIndexRandom (map.GetCountry(countryDest));
			if (cityOrigin < 0 || cityDest < 0)
				yield break;

			Vector2 origin = map.cities [cityOrigin].unity2DLocation;
			Vector2 dest = map.cities [cityDest].unity2DLocation;
			float elevation = 1f;
			float width = 0.25f;
			LineMarkerAnimator lma = map.AddLine (origin, dest, color, elevation, width);
			lma.dashInterval = 0.0003f;
			lma.dashAnimationDuration = 0.5f;
			lma.drawingDuration = 4f;
			lma.autoFadeAfter = 1f;

			// Add flashing target
			GameObject sprite = Instantiate (target) as GameObject;
			sprite.GetComponent<SpriteRenderer> ().material.color = color * 0.9f;
			map.AddMarker2DSprite (sprite, dest, 0.003f);
			MarkerBlinker.AddTo (sprite, 4, 0.1f, 0.5f, true);

			// Triggers explosion
			StartCoroutine (AddCircleExplosion (4f, dest, Color.yellow));
		}

		IEnumerator AddCircleExplosion (float delay, Vector2 mapPos, Color color) {
			float start = Time.time;
			while (Time.time - start < delay) {
				yield return null;
			}

			GameObject circleObj = null;
			float radius = UnityEngine.Random.Range (80f, 100f);
			for (int k = 0; k < 100; k++) {
				if (circleObj != null)
					DestroyImmediate (circleObj);
				float ringStart = Mathf.Clamp01 ((k - 50f) / 50f);
				float ringEnd = Mathf.Clamp01 (k / 50f);
				circleObj = map.AddCircle (mapPos, radius, ringStart, ringEnd, color);
				yield return new WaitForSeconds (1 / 60f);
			}
			Destroy (circleObj);
		}


	}
}

