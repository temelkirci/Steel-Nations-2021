using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace WorldMapStrategyKit {

	public delegate void OnMarkerPointerClickEvent (int buttonIndex);
    public delegate void OnMarkerEvent();

    public class MarkerClickHandler : MonoBehaviour {

		public OnMarkerPointerClickEvent OnMarkerMouseDown;
		public OnMarkerPointerClickEvent OnMarkerMouseUp;
        public OnMarkerEvent OnMarkerMouseEnter;
        public OnMarkerEvent OnMarkerMouseExit;
        public WMSK map;
        public bool respectOtherUI;
        bool wasInside;

		void Start () {
            // Get a reference to the World Map API:
            if (map == null) {
                map = WMSK.instance;
            }
            wasInside = SpriteRectContainsPointer();
		}


		void LateUpdate () {
			bool leftButtonPressed = Input.GetMouseButtonDown (0);
			bool rightButtonPressed = Input.GetMouseButtonDown (1);
			bool leftButtonReleased = Input.GetMouseButtonUp (0);
			bool rightButtonReleased = Input.GetMouseButtonUp (1);
            bool checkEnterExit = OnMarkerMouseEnter != null || OnMarkerMouseExit != null;
			if (checkEnterExit || leftButtonPressed || rightButtonPressed || leftButtonReleased || rightButtonReleased) {
                // Check if cursor location is inside marker rect
                bool inside = SpriteRectContainsPointer();
				if (inside) {
                    if (leftButtonPressed && OnMarkerMouseDown != null)
						OnMarkerMouseDown (0);
					if (rightButtonPressed && OnMarkerMouseDown != null)
						OnMarkerMouseDown (1);
					if (leftButtonReleased && OnMarkerMouseUp != null)
						OnMarkerMouseUp (0);
					if (rightButtonReleased && OnMarkerMouseUp != null)
						OnMarkerMouseUp (1);
                    if (!wasInside && OnMarkerMouseEnter != null) {
                        OnMarkerMouseEnter();
                    }
				} else {
                    if (wasInside && OnMarkerMouseExit != null) {
                        OnMarkerMouseExit();
                    }
                }
                wasInside = inside;
            }
        }

        bool SpriteRectContainsPointer() {
            // Check if cursor location is inside marker rect
            if (map == null)
                return false;
            Vector3 cursorLocation;
            if (respectOtherUI) {
                cursorLocation = map.cursorLocation;
            } else {
                map.GetLocalHitFromScreenPos(Input.mousePosition, out cursorLocation, false);
            }
            Rect rect = new Rect(transform.localPosition - transform.localScale * 0.5f, transform.localScale);
            return rect.Contains(cursorLocation);
        }
	}

}

