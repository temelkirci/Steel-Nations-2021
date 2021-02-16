// World Strategy Kit for Unity - Main Script
// (C) 2016-2020 by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WorldMapStrategyKit {
	public enum EARTH_STYLE {
		Natural = 0,
		Alternate1 = 1,
		Alternate2 = 2,
		Alternate3 = 3,
		SolidColor = 4,
		NaturalHighRes = 5,
		NaturalScenic = 6,
		NaturalScenicPlus = 7,
		NaturalScenicPlusAlternate1 = 8,
		NaturalScenicPlus16K = 9,
		Texture = 10,
		NaturalHighRes16K = 11,
		Custom = 99
	}

	public static class EarthStyleExtensions {
		public static bool isScenicPlus (this EARTH_STYLE earthStyle) {
			return earthStyle == EARTH_STYLE.NaturalScenicPlus || earthStyle == EARTH_STYLE.NaturalScenicPlusAlternate1 || earthStyle == EARTH_STYLE.NaturalScenicPlus16K;
		}

		public static int numTextures(this EARTH_STYLE earthStyle) {
			if (earthStyle == EARTH_STYLE.NaturalHighRes16K || earthStyle == EARTH_STYLE.NaturalScenicPlus16K) {
				return 4;
			}
			return 1;
		}


		public static bool supportsBumpMap (this EARTH_STYLE earthStyle) {
			return earthStyle == EARTH_STYLE.Natural || earthStyle == EARTH_STYLE.NaturalHighRes || earthStyle == EARTH_STYLE.Alternate1 || earthStyle == EARTH_STYLE.Alternate2 || earthStyle == EARTH_STYLE.Alternate3 || earthStyle == EARTH_STYLE.NaturalScenic;
		}
	}


	public partial class WMSK : MonoBehaviour {

		#region Public properties

		[SerializeField]
		bool _showWorld = true;

		/// <summary>
		/// Toggle Earth visibility.
		/// </summary>
		public bool showEarth { 
			get {
				return _showWorld; 
			}
			set {
				if (value != _showWorld) {
					_showWorld = value;
					isDirty = true;
					gameObject.GetComponent<MeshRenderer>().enabled = _showWorld;
				}
			}
		}

		[SerializeField]
		EARTH_STYLE
			_earthStyle = EARTH_STYLE.Natural;

		/// <summary>
		/// Earth map style.
		/// </summary>
		public EARTH_STYLE earthStyle {
			get {
				return _earthStyle;
			}
			set {
				if (value != _earthStyle) {
					_earthStyle = value;
					isDirty = true;
					RestyleEarth();
				}
			}
		}


		/// <summary>
		/// Reloads Earth style and clears any customization
		/// </summary>
		public void ResetEarthStyle() {
			earthMat = null;
			if (_earthStyle != EARTH_STYLE.Texture) {
				_earthTexture = null;
			}
			_earthBumpMapTexture = null;
			_heightMapTexture = null;
			_waterMask = null;
			_waterLevel = 0.1f;
			RestyleEarth ();
		}


		[SerializeField]
		Color
			_earthColor = Color.black;

		/// <summary>
		/// Color for Earth (for SolidColor style)
		/// </summary>
		public Color earthColor {
			get {
				return _earthColor;
			}
			set {
				if (value != _earthColor) {
					_earthColor = value;
					isDirty = true;

					if (_earthStyle == EARTH_STYLE.SolidColor || _earthStyle == EARTH_STYLE.Texture) {
						Material mat = GetComponent<Renderer>().sharedMaterial;
						mat.color = _earthColor;
					}
				}
			}
		}


		[SerializeField]
		bool _earthBumpEnabled = false;

		public bool earthBumpEnabled {
			get { return _earthBumpEnabled; }
			set {
				if (_earthBumpEnabled != value) {
					_earthBumpEnabled = value;
					isDirty = true;
					RestyleEarth();
				}
			}
		}

		[SerializeField]
		float _earthBumpAmount = 0.5f;

		public float earthBumpAmount {
			get { return _earthBumpAmount; }
			set {
				if (_earthBumpAmount != value) {
					_earthBumpAmount = value;
					isDirty = true;
					RestyleEarth();
				}
			}
		}

		[SerializeField]
		Texture2D _earthBumpMapTexture;

		public Texture2D earthBumpMapTexture {
			get { return _earthBumpMapTexture; }
			set {
				if (_earthBumpMapTexture != value) {
					earthMat = null;
					_earthBumpMapTexture = value;
					isDirty = true;
					RestyleEarth();
				}
			}
		}



		[SerializeField]
		Texture2D
			_earthTexture;

		/// <summary>
		/// Texture for Earth background
		/// </summary>
		public Texture2D earthTexture {
			get {
				return _earthTexture;
			}
			set {
				if (value != _earthTexture) {
					_earthTexture = value;
					if (_earthTexture == null) {
						earthMat = null;
					}
					_earthColor = Color.white; // ensure texture is visible in original colors
					isDirty = true;
					RestyleEarth();
				}
			}
		}


        [SerializeField]
        Vector2
            _earthTextureScale = Misc.Vector2one;

        /// <summary>
        /// Earth texture scale
        /// </summary>
        public Vector2 earthTextureScale {
            get {
                return _earthTextureScale;
            }
            set {
                if (value != _earthTextureScale) {
                    _earthTextureScale = value;
                    isDirty = true;
                    RestyleEarth();
                }
            }
        }



        [SerializeField]
        Vector2
            _earthTextureOffset;

        /// <summary>
        /// Earth texture offset
        /// </summary>
        public Vector2 earthTextureOffset {
            get {
                return _earthTextureOffset;
            }
            set {
                if (value != _earthTextureOffset) {
                    _earthTextureOffset = value;
                    isDirty = true;
                    RestyleEarth();
                }
            }
        }

        /// <summary>
        /// Gets the currently active Earth material.
        /// </summary>
        /// <value>The Earth material.</value>
        public Material earthMaterial {
			get { return earthMat; }
		}

		[SerializeField]
		Color _waterColor = new Color(0, 106.0f / 255.0f, 148.0f / 255.0f);

		/// <summary>
		/// Defines the base water color used in Scenic Plus style.
		/// </summary>
		public Color waterColor {
			get {
				return _waterColor;
			}
			set {
				if (value != _waterColor) {
					_waterColor = value;
					isDirty = true;
					RestyleEarth();
				}
			}
		}

		
		[SerializeField]
		float _waterLevel = 0.1f;

		/// <summary>
		/// Water level for the Scenic Plus styles.
		/// </summary>
		public float waterLevel {
			get { return _waterLevel; }
			set {
				if (_waterLevel != value) { 
					_waterLevel = value;
					if (_earthStyle.isScenicPlus()) {
						RestyleEarth();
					}
					AdjustsGridAlpha();
					isDirty = true; 
				}
			}
		}

		[SerializeField]
		float _waterFoamThreshold = 0.1f;

		/// <summary>
		/// Foam threshold (amount) for the Scenic Plus styles.
		/// </summary>
		public float waterFoamThreshold {
			get { return _waterFoamThreshold; }
			set {
				if (_waterFoamThreshold != value) { 
					_waterFoamThreshold = value;
					if (_earthStyle.isScenicPlus())
						RestyleEarth();
					isDirty = true; 
				}
			}
		}

		[SerializeField]
		float _waterFoamIntensity = 30f;

		/// <summary>
		/// Foam intensity effect..
		/// </summary>
		public float waterFoamIntensity {
			get { return _waterFoamIntensity; }
			set {
				if (_waterFoamIntensity != value) { 
					_waterFoamIntensity = value;
					if (_earthStyle.isScenicPlus())
						RestyleEarth();
					isDirty = true; 
				}
			}
		}


		#endregion


		#region Earth related APIs

		/// <summary>
		/// Returns true if specified position is on water.
		/// </summary>
		public bool ContainsWater(Vector2 position) {
			position.x += 0.5f;
			position.y += 0.5f;
			if (position.x < 0 || position.x >= 1.0f || position.y < 0 || position.y >= 1.0f)
				return false;
			CheckRouteWaterMask();
			int jj = ((int)(position.y * earthWaterMaskHeight)) * earthWaterMaskWidth;
			int kk = (int)(position.x * earthWaterMaskWidth);
			bool hasWater = earthWaterMask.GetBit(jj + kk);
			return hasWater;
		}

		/// <summary>
		/// Returns true if specified area with center at "position" contains water.
		/// </summary>
		/// <param name="boxSize">Box size.</param>
		/// <param name="waterPosition">Exact position where water was found.</param>
		public bool ContainsWater(Vector2 position, float boxSize, out Vector2 waterPosition) {
			
			CheckRouteWaterMask();

			float halfSize = boxSize * 0.5f;
			float stepX = 1.0f / earthWaterMaskWidth;
			float stepY = 1.0f / earthWaterMaskHeight;

			position.x += 0.5f;
			position.y += 0.5f;
			float y0 = position.y - halfSize; // + 0.5f;
			float y1 = position.y + halfSize; // + 0.5f;
			float x0 = position.x - halfSize; // + 0.5f;
			float x1 = position.x + halfSize; // + 0.5f;
			for (float y = y0; y <= y1; y += stepY) {
				if (y < 0 || y >= 1.0f)
					continue;
				int jj = ((int)(y * earthWaterMaskHeight)) * earthWaterMaskWidth;
				for (float x = x0; x <= x1; x += stepX) {
					if (x < 0 || x >= 1.0f)
						continue;
					int kk = (int)(x * earthWaterMaskWidth);
					if (earthWaterMask.GetBit(jj + kk)) {
						waterPosition = new Vector2(x - 0.5f + stepX * 0.5f, y - 0.5f + stepY * 0.5f);
						return true;
					}
				}
			}
			waterPosition = Misc.Vector2zero;
			return false;
		}

		/// <summary>
		/// Returns true if specified area with center at "position" contains water.
		/// </summary>
		/// <param name="boxSize">Box size in cell units.</param>
		bool ContainsWater(Vector2 position, int boxSize, out Vector2 waterPosition) {
			
			CheckRouteWaterMask();
			
			int halfSize = boxSize / 2;

			int yc = (int)((position.y + 0.5f) * earthWaterMaskHeight);
			int xc = (int)((position.x + 0.5f) * earthWaterMaskWidth);
			int y0 = yc - halfSize;
			int y1 = yc + halfSize;
			int x0 = xc - halfSize;
			int x1 = xc + halfSize;
			for (int y = y0; y <= y1; y++) {
				if (y < 0 || y >= earthWaterMaskHeight)
					continue;
				int yy = y * earthWaterMaskWidth;
				for (int x = x0; x <= x1; x++) {
					if (x < 0 || x >= earthWaterMaskWidth)
						continue;
					if (earthWaterMask.GetBit(yy + x)) {
						waterPosition = new Vector2((float)x / earthWaterMaskWidth - 0.5f, (float)y / earthWaterMaskHeight - 0.5f);
						return true;
					}
				}
			}
			waterPosition = Misc.Vector2zero;
			return false;
		}



		#endregion

	}

}