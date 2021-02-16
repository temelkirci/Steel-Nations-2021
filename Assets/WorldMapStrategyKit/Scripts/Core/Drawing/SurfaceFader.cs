using UnityEngine;
using System.Collections;

namespace WorldMapStrategyKit {

	public enum FADER_STYLE
	{
		FadeOut = 0,
		Blink = 1,
		Flash = 2
	}

	public class SurfaceFader : MonoBehaviour {

		public float duration;
		Material fadeMaterial;
		float highlightFadeStart;
		public IFader fadeEntity;
		FADER_STYLE style;
		Color color, initialColor;
		Renderer _renderer;
		WMSK map;

		public static void Animate (FADER_STYLE style, IFader fadeEntity, Renderer renderer, Color initialColor, Color color, float duration)
		{
			SurfaceFader fader = renderer.GetComponent<SurfaceFader> ();
			if (fader != null)
				DestroyImmediate (fader);
			fader = renderer.gameObject.AddComponent<SurfaceFader> ();
			fader.duration = duration + 0.0001f;
			fader.color = color;
			fader.style = style;
			fader._renderer = renderer;
			fader.fadeEntity = fadeEntity;
			fader.initialColor = initialColor;
		}

		void Start () {
			GenerateMaterial ();
			map = WMSK.GetInstance (transform);
			highlightFadeStart = map.time;
			if (fadeEntity!=null) fadeEntity.isFading = true;
			Update ();
		}
	
		// Update is called once per frame
		void Update () {
			float elapsed = map.time - highlightFadeStart;
			switch (style) {
			case FADER_STYLE.FadeOut:
				UpdateFadeOut (elapsed);
				break;
			case FADER_STYLE.Blink:
				UpdateBlink (elapsed);
				break;
			case FADER_STYLE.Flash:
				UpdateFlash (elapsed);
				break;
			}
		}


		void UpdateFadeOut(float elapsed) {
			SetFadeOutColor(elapsed / duration);
			if (elapsed > duration) {
				if (fadeEntity!=null) {
					fadeEntity.isFading = false;
					fadeEntity.customMaterial = null;
				}
				_renderer.enabled = false;
				Destroy (this);
			}
		}

		void SetFadeOutColor(float t) {
			Color newColor = Color.Lerp (color, Misc.ColorClear, t);
			fadeMaterial.color = newColor;
			if (_renderer.sharedMaterial != fadeMaterial) {
				fadeMaterial.mainTexture = _renderer.sharedMaterial.mainTexture;
				_renderer.sharedMaterial = fadeMaterial;
			}
		}

		#region Flash effect
		
		void UpdateFlash (float elapsed)
		{
			SetFlashColor (elapsed / duration);
			if (elapsed >= duration) {
				if (fadeEntity!=null) {
					fadeEntity.isFading = false;
					if (fadeEntity.customMaterial!=null) {
						_renderer.sharedMaterial = fadeEntity.customMaterial;
					} else {
						_renderer.enabled = false;
					}
				}
				Destroy (this);
			}
		}
		
		void SetFlashColor (float t)
		{
			Color newColor = Color.Lerp (color, initialColor, t);
			fadeMaterial.color = newColor;
			if (_renderer.sharedMaterial != fadeMaterial) {
				fadeMaterial.mainTexture = _renderer.sharedMaterial.mainTexture;
				_renderer.sharedMaterial = fadeMaterial;
			}
		}
		
		#endregion
		
		#region Blink effect
		
		void UpdateBlink (float elapsed)
		{
			SetBlinkColor (elapsed / duration);
			if (elapsed >= duration) {
				SetBlinkColor (0);
				if (fadeEntity!=null) {
					fadeEntity.isFading = false;
					if (fadeEntity.customMaterial!=null) {
						_renderer.sharedMaterial = fadeEntity.customMaterial;
					} else {
						_renderer.enabled = false;
					}
				}
				Destroy (this);
			}
		}
		
		void SetBlinkColor (float t)
		{
			Color newColor;
			if (t < 0.5f) {
				newColor = Color.Lerp (initialColor, color, t * 2f);
			} else {
				newColor = Color.Lerp (color, initialColor, (t - 0.5f) * 2f);
			}
			fadeMaterial.color = newColor;
			if (_renderer.sharedMaterial != fadeMaterial) {
				fadeMaterial.mainTexture = _renderer.sharedMaterial.mainTexture;
				_renderer.sharedMaterial = fadeMaterial;
			}
		}
		
		#endregion


		void GenerateMaterial () {
			fadeMaterial = Instantiate (GetComponent<Renderer> ().sharedMaterial);
			fadeMaterial.hideFlags = HideFlags.DontSave;
			_renderer.sharedMaterial = fadeMaterial;
		}
	}

}
