// World Map Strategy Kit for Unity - Main Script
// (C) 2016-2020 by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

//#define NGUI_SUPPORT

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using WorldMapStrategyKit.PolygonClipping;

namespace WorldMapStrategyKit {


    [Serializable]
    [ExecuteInEditMode]
    public partial class WMSK : MonoBehaviour {

        public const float MAP_PRECISION = 5000000f;
        public const string SURFACE_LAYER = "Surfaces";

        const float TAP_THRESHOLD = 0.25f;
        const string OVERLAY_BASE = "OverlayLayer";
        const string SKW_BUMPMAP_ENABLED = "WMSK_BUMPMAP_ENABLED";
        char[] SPLIT_SEP_SEMICOLON = { ';' };
        char[] SPLIT_SEP_ASTERISK = { '*' };


        public static float mapWidth { get { return WMSK.instanceExists ? WMSK.instance.transform.localScale.x : 200.0f; } }

        public static float mapHeight { get { return WMSK.instanceExists ? WMSK.instance.transform.localScale.y : 100.0f; } }

        #region Internal variables

        // resources
        Material coloredMat, coloredAlphaMat, texturizedMat;
        Material outlineMatSimple, outlineMatTextured, cursorMatH, cursorMatV, imaginaryLinesMat;
        Material markerMat, markerLineMat;
        Material earthMat;
        Material rectangleSelectionMat;

        // gameObjects
        GameObject _surfacesLayer;

        GameObject surfacesLayer {
            get {
                if (_surfacesLayer == null)
                    CreateSurfacesLayer();
                return _surfacesLayer;
            }
        }

        GameObject cursorLayerHLine, cursorLayerVLine, latitudeLayer, longitudeLayer;
        GameObject markersLayer;

        // caché and gameObject lifetime control
        Dictionary<int, GameObject> surfaces;
        Dictionary<Color, Material> coloredMatCache;
        Dictionary<Color, Material> markerMatCache;
        Dictionary<double, Region> frontiersCacheHit;
        List<Vector3> frontiersPoints;
        DisposalManager disposalManager;
        int lastRegionIndex;

        // FlyTo functionality
        Quaternion flyToStartQuaternion, flyToEndQuaternion;
        Vector3 flyToStartLocation, flyToEndLocation;
        bool flyToActive;
        float flyToStartTime, flyToDuration;
        Vector3 flyToCallParamsPoint;
        float flyToCallZoomDistance;

        // UI interaction variables
        Vector3 mouseDragStart, dragDirection, mouseDragStartHitPos;
        float dragDampingStart, dragSpeed, maxFrustumDistance, lastDistanceFromCamera, distanceFromCameraStartingFrame;
        float wheelAccel, zoomDampingStart;
        bool pinching, dragging, hasDragged, lastMouseMapHitPosGood;
        float clickTime;
        float lastCamOrtographicSize;
        Vector3 lastMapPosition, lastCamPosition;
        Vector3 lastMouseMapHitPos;
        bool shouldCheckBoundaries;

        // raycasting reusable buffer
        RaycastHit[] tempHits;

        bool canInteract = true;

        // used in viewport mode changes
        Vector3 lastKnownMapCoordinates;


        /// <summary>
        /// The last known zoom level. Updated when zooming in/out.
        /// </summary>
        public float lastKnownZoomLevel;

        // Overlay (Labels, tickers, ...)
        GameObject overlayLayer;

        // Earth effects
        RenderTexture earthBlurred;

        int layerMask {
            get {
                if (Application.isPlaying && renderViewportIsEnabled)
                    return 1 << renderViewport.layer;
                else
                    return 1 << gameObject.layer;
            }
        }

        bool miniMapChecked;
        bool _isMiniMap;

        /// <summary>
        /// Returns true if this is a minimap. Set internally by minimap component.
        /// </summary>
        bool isMiniMap {
            get {
                if (!miniMapChecked) {
                    _isMiniMap = GetComponent<WMSKMiniMap>() != null;
                    miniMapChecked = true;
                }
                return _isMiniMap;
            }
        }


        bool updateDoneThisFrame;

        public bool isDirty;
        // internal variable used to confirm changes - don't change its value

        Material outlineMat {
            get {
                if (_outlineDetail == OUTLINE_DETAIL.Textured)
                    return outlineMatTextured;
                else
                    return outlineMatSimple;
            }
        }

        #endregion



        #region Game loop events

        void OnEnable() {

            if (_countries == null) {
                Init();
            }
        }

        void OnDisable() {
            if (isMiniMap)
                return;
            ToggleWrapCamera(false);
            if (_currentCamera != null && _currentCamera.name.Equals(MAPPER_CAM))
                _currentCamera.enabled = false;
            ToggleUIPanel(true);
        }

        void OnDestroy() {
            if (_surfacesLayer != null) {
                GameObject.DestroyImmediate(_surfacesLayer);
            }
            overlayLayer = null;
            DestroyMapperCam();
            DestroyTiles();
            disposalManager.DisposeAll();
        }

        void Reset() {
            Redraw();
        }

        void Update() {

            if (currentCamera == null || !Application.isPlaying) {
                // For some reason, when saving the scene, the renderview port loses the attached rendertexture.
                // No event is fired, except for Update(), so we need to refresh the attached rendertexture of the render viewport here.
                SetupViewport();
                CheckRectConstraints();
                return;
            }

            if (updateDoneThisFrame)
                return;
            updateDoneThisFrame = true;

            // Updates mapperCam to reflect current main camera position and rotation (if main camera has moved)
            if (_renderViewportIsTerrain) {
                SyncMapperCamWithMainCamera();
            }

            // Check if navigateTo... has been called and in this case scrolls the map until the country is centered
            if (flyToActive) {
                MoveCameraToDestination();
            }

            // Check Viewport scale
            CheckViewportScaleAndCurvature();

            // Check whether the points is on an UI element, then avoid user interaction
            if (respectOtherUI && !hasDragged) {
                if (UnityEngine.EventSystems.EventSystem.current != null) {
                    if (Application.isMobilePlatform && Input.touchCount > 0 && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                        canInteract = false;
                    } else if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1)) {
                        canInteract = false;
                    }
                }

#if NGUI_SUPPORT
																if (UICamera.hoveredObject != null && !UICamera.hoveredObject.name.Equals("UI Root")) {
																				canInteract = false;
																}
#endif

                if (!canInteract) {
                    mouseIsOverUIElement = true;
                }
            }

            Vector3 prevCamPos = _currentCamera.transform.position;
            if (canInteract) {
                CheckCursorVisibility();
                mouseIsOverUIElement = false;
                PerformUserInteraction();
            } else if (!Input.GetMouseButton(0)) {
                canInteract = true;
            }

            if (isMiniMap)
                return;

            // Check boundaries
            if (transform.position != lastMapPosition || _currentCamera.transform.position != lastCamPosition || _currentCamera.orthographicSize != lastCamOrtographicSize) {
                shouldCheckBoundaries = true;
            }

            if (shouldCheckBoundaries) {
                shouldCheckBoundaries = false;

                shouldCheckTiles = true;
                resortTiles = true;

                // Last distance
                if (_currentCamera.orthographic) {
                    _currentCamera.orthographicSize = Mathf.Clamp(_currentCamera.orthographicSize, 1, maxFrustumDistance);
                    // updates frontiers LOD
                    frontiersMat.shader.maximumLOD = _currentCamera.orthographicSize < 2.2 ? 100 : (_currentCamera.orthographicSize < 8 ? 200 : 300);
                    provincesMat.shader.maximumLOD = _currentCamera.orthographicSize < 8 ? 200 : 300;
                } else {
                    if (!_enableFreeCamera && _allowUserZoom && (_zoomMinDistance > 0 || _zoomMaxDistance > 0)) {
                        float minDistance = Mathf.Max(_currentCamera.nearClipPlane + 0.0001f, _zoomMinDistance * transform.localScale.y);
                        Plane plane = new Plane(transform.forward, transform.position);
                        lastDistanceFromCamera = Mathf.Abs(plane.GetDistanceToPoint(_currentCamera.transform.position));
                        if (lastDistanceFromCamera < minDistance) {
                            _currentCamera.transform.position = ClampDistanceToMap(prevCamPos, _currentCamera.transform.position, lastDistanceFromCamera, minDistance);
                            //_currentCamera.transform.position += _currentCamera.transform.forward * (lastDistanceFromCamera - minDistance);
                            lastDistanceFromCamera = minDistance;
                            wheelAccel = 0;
                        } else {
                            float maxDistance = Mathf.Min(maxFrustumDistance, _zoomMaxDistance * transform.localScale.y);
                            if (lastDistanceFromCamera > maxDistance) {
                                // Get intersection point from camera with plane
                                //_currentCamera.transform.position += _currentCamera.transform.forward * (lastDistanceFromCamera - maxDistance);
                                _currentCamera.transform.position = ClampDistanceToMap(prevCamPos, _currentCamera.transform.position, lastDistanceFromCamera, maxDistance);
                                lastDistanceFromCamera = maxDistance;
                                wheelAccel = 0;
                            }
                        }
                    }
                    // updates frontiers LOD
                    if (_thickerFrontiers) {
                        float fw = _frontiersWidth;
                        if (_frontiersDynamicWidth && !_renderViewportIsTerrain) {
                            frontiersMat.shader.maximumLOD = lastDistanceFromCamera < 25f ? 100 : 300;
                            if (lastDistanceFromCamera > 20f && lastDistanceFromCamera < 25f) { // smooth transition
                                float t = 1f - (lastDistanceFromCamera - 20f) / 5f;
                                fw = Mathf.Max(0.05f, _frontiersWidth * t);
                            }
                        } else {
                            frontiersMat.shader.maximumLOD = 100;
                        }
                        frontiersMat.SetFloat("_Thickness", fw);
                    } else {
                        int lod = 300;
                        if (_frontiersDynamicWidth && !_renderViewportIsTerrain) {
                            lod = lastDistanceFromCamera < 4.472f ? 100 : (lastDistanceFromCamera < 17.888f ? 200 : 300);
                        }
                        frontiersMat.shader.maximumLOD = lod;
                    }

                    // Provinces
                    provincesMat.shader.maximumLOD = lastDistanceFromCamera < 14.0 ? 200 : 300;
                }


                // Constraint to limits if user interaction is enabled
                if (!_enableFreeCamera && (_allowUserDrag || _allowUserZoom || _allowUserKeys)) {
                    CheckRectConstraints();
                }
                lastMapPosition = transform.position;
                lastCamPosition = _currentCamera.transform.position;
                lastCamOrtographicSize = _currentCamera.orthographicSize;
                lastMouseMapHitPosGood = false; // forces check again CheckMousePos()
                lastRenderViewportGood = false; // forces calculation of the viewport rect

                // Map has moved: apply changes
                if (distanceFromCameraStartingFrame != lastDistanceFromCamera) {
                    distanceFromCameraStartingFrame = lastDistanceFromCamera;

                    // Update distance param in ScenicPlus material
                    if (_earthStyle.isScenicPlus()) {
                        UpdateScenicPlusDistance();
                    }
                    // Fades country labels
                    if (_showCountryNames) {
                        FadeCountryLabels();
                    }
                    // Fades country labels
                    if (_showProvinceNames) {
                        FadeProvinceLabels();
                    }

                    // Check maximum screen area size for highlighted country
                    if (_highlightMaxScreenAreaSize < 1f) {
                        if (_countryRegionHighlighted != null && countryRegionHighlightedObj != null && countryRegionHighlightedObj.activeSelf) {
                            if (!CheckScreenAreaSizeOfRegion(_countryRegionHighlighted)) {
                                countryRegionHighlightedObj.SetActive(false);
                            }
                        }
                        if (_provinceRegionHighlighted != null && provinceRegionHighlightedObj != null && provinceRegionHighlightedObj.activeSelf) {
                            if (!CheckScreenAreaSizeOfRegion(_provinceRegionHighlighted)) {
                                provinceRegionHighlightedObj.SetActive(false);
                            }
                        }
                    }

                    if (_showTiles) {
                        if (frontiersLayer != null) {
                            if (_currentZoomLevel > _tileLinesMaxZoomLevel && frontiersLayer.activeSelf) {
                                frontiersLayer.SetActive(false);
                            } else if (_showFrontiers && _currentZoomLevel <= _tileLinesMaxZoomLevel && !frontiersLayer.activeSelf) {
                                frontiersLayer.SetActive(true);
                            }
                        }
                        if (provincesObj != null) {
                            if (_currentZoomLevel > _tileLinesMaxZoomLevel && provincesObj.activeSelf) {
                                provincesObj.SetActive(false);
                            } else if (_showProvinces && _currentZoomLevel <= _tileLinesMaxZoomLevel && !provincesObj.activeSelf) {
                                provincesObj.SetActive(true);
                            }
                        }
                        if (lastCountryOutlineRef != null) {
                            if (_currentZoomLevel > _tileLinesMaxZoomLevel && lastCountryOutlineRef.activeSelf) {
                                lastCountryOutlineRef.SetActive(false);
                            } else if (_currentZoomLevel <= _tileLinesMaxZoomLevel && !lastCountryOutlineRef.activeSelf) {
                                lastCountryOutlineRef.SetActive(true);
                            }
                        }
                    }
                }

                // Update everything related to viewport
                lastRenderViewportGood = false;
                if (renderViewportIsEnabled) {
                    UpdateViewport();
                }

                // Update grid
                if (_showGrid) {
                    CheckGridRect();
                }
            } else if (!_renderViewportIsTerrain) {
                // Map has not moved
                if (--viewportColliderNeedsUpdate == 1) {
                    Mesh ms;
                    if (viewportIndices.Length >= 25000) {
                        ms = flexQuad;
                    } else {
                        ms = _renderViewport.GetComponent<MeshFilter>().sharedMesh;
                    }
                    if (ms.vertexCount >= 4) {
                        MeshCollider mc = _renderViewport.GetComponent<MeshCollider>();
                        if (mc != null) {
                            if (mc.convex) {
                                mc.convex = false;
                            }
                            mc.sharedMesh = null;
                            mc.sharedMesh = ms;
                        }
                    }
                    viewportColliderNeedsUpdate = 0;
                }

                // Check if viewport rotation has changed or has moved
                if (renderViewportIsEnabled) {
                    if (_renderViewport.transform.localRotation.eulerAngles != lastRenderViewportRotation || _renderViewport.transform.position != lastRenderViewportPosition) {
                        lastRenderViewportRotation = _renderViewport.transform.localRotation.eulerAngles;
                        lastRenderViewportPosition = _renderViewport.transform.position;
                        UpdateViewportObjectsVisibility();
                    }
                    if (VGOBuoyancyAmplitude > 0) {
                        UpdateViewportObjectsBuoyancy();
                    }
                }
            }

            if (_showGrid) {
                GridUpdateHighlightFade();  // Fades current selection
            }
        }


        void FixedUpdate() {
            FixedUpdateViewportObjectsLoop();
        }

        void LateUpdate() {
            updateDoneThisFrame = false;

            if (_renderViewportIsTerrain) {
                if (_enableFreeCamera || !Application.isPlaying) {
                    SyncMapperCamWithMainCamera(); // catch any camera change between Update and LateUpdate
                }
                SyncMainCameraWithMapperCam();
            }

            if (_earthBumpEnabled && _earthStyle.supportsBumpMap() && _sun != null) {
                earthMat.SetVector("_SunLightDirection", -_sun.transform.forward);
            }

            if (_showTiles) {
                LateUpdateTiles();
            }

            FitViewportToUIPanel();

            if (!paused) {
                time += Time.deltaTime * timeSpeed;
            }
        }


        Vector3 ClampDistanceToMap(Vector3 prevPos, Vector3 currPos, float currDistance, float clampDistance) {

            Plane plane = new Plane(transform.forward, transform.position);
            float prevDistance = Mathf.Abs(plane.GetDistanceToPoint(prevPos));

            float ta = (clampDistance - currDistance) / (prevDistance - currDistance);
            return Vector3.Lerp(currPos, prevPos, ta);
        }


        void CheckRectConstraints() {

            if (isMiniMap)
                return;

            Camera cam = currentCamera;
            if (cam == null)
                return;

            float limitLeft, limitRight;
            if (_fitWindowWidth) {
                limitLeft = 0f;
                limitRight = 1f;
            } else {
                limitLeft = 0.9f;
                limitRight = 0.1f;
            }

            // Reduce floating-point errors
            Vector3 pos, apos = transform.position;
            if (renderViewportIsEnabled) {
                transform.position -= apos;
                cam.transform.position -= apos;
            }

            Vector3 posEdge;
            if (!_wrapHorizontally || _renderViewportIsTerrain) {
                // Clamp right
                posEdge = transform.TransformPoint(_windowRect.xMax, 0, 0);
                pos = cam.WorldToViewportPoint(posEdge);
                if (pos.x < limitRight) {
                    pos.x = limitRight;
                    pos = cam.ViewportToWorldPoint(pos);
                    cam.transform.position += (posEdge - pos);
                    dragDampingStart = 0;
                } else {
                    // Clamp left
                    posEdge = transform.TransformPoint(_windowRect.xMin, 0, 0);
                    pos = cam.WorldToViewportPoint(posEdge);
                    if (pos.x > limitLeft) {
                        pos.x = limitLeft;
                        pos = cam.ViewportToWorldPoint(pos);
                        cam.transform.position += (posEdge - pos);
                        dragDampingStart = 0;
                    }
                }
            }

            float limitTop, limitBottom;
            if (_fitWindowHeight) {
                limitTop = 1.0f;
                limitBottom = 0f;
            } else {
                limitTop = 0.1f;
                limitBottom = 0.9f;
            }

            // Clamp top
            posEdge = transform.TransformPoint(0, _windowRect.yMax, 0);
            pos = cam.WorldToViewportPoint(posEdge);
            if (pos.y < limitTop) {
                pos.y = limitTop;
                pos = cam.ViewportToWorldPoint(pos);
                cam.transform.position += (posEdge - pos);
                dragDampingStart = 0;
            } else {
                // Clamp bottom
                posEdge = transform.TransformPoint(0, _windowRect.yMin, 0);
                pos = cam.WorldToViewportPoint(posEdge);
                if (pos.y > limitBottom) {
                    pos.y = limitBottom;
                    pos = cam.ViewportToWorldPoint(pos);
                    cam.transform.position += (posEdge - pos);
                    dragDampingStart = 0;
                }
            }

            // Reduce floating-point errors
            if (renderViewportIsEnabled) {
                transform.position += apos;
                cam.transform.position += apos;
            }
        }

        /// <summary>
        /// Check controls (keys, mouse, ...) and react
        /// </summary>
        void PerformUserInteraction() {

            float deltaTime = Time.deltaTime * 60f;

            bool buttonLeftPressed = Input.GetMouseButton(0) || (Input.touchSupported && Input.touchCount == 1 && Input.touches[0].phase != TouchPhase.Ended);
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                clickTime = Time.time;

            // Use mouse wheel to zoom in and out
            if (!flyToActive && allowUserZoom && (mouseIsOver || wheelAccel != 0)) {
                float wheel = Input.GetAxis("Mouse ScrollWheel");
                if (wheel != 0) {
                    zoomDampingStart = Time.time;
                    wheelAccel += wheel * (_invertZoomDirection ? -1 : 1);
                }

                Vector3 zoomCenter;
                // Support for pinch on mobile
                if (Input.touchSupported && Input.touchCount == 2) {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    zoomCenter = (touchZero.position + touchOne.position) * 0.5f;

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // Pass the delta to the wheel accel
                    if (deltaMagnitudeDiff != 0) {
                        zoomDampingStart = Time.time;
                        wheelAccel += deltaMagnitudeDiff;
                    }

                    pinching = true;
                    dragDampingStart = 0;
                } else {
                    zoomCenter = Input.mousePosition;
                }

                if (wheelAccel != 0) {
                    wheelAccel = Mathf.Clamp(wheelAccel, -0.1f, 0.1f);
                    if (wheelAccel >= 0.001f || wheelAccel <= -0.001f) {
                        Vector3 dest;
                        if (GetLocalHitFromScreenPos(zoomCenter, out dest, true)) {
                            dest = transform.TransformPoint(dest);
                        } else {
                            dest = transform.position;
                        }

                        if (_currentCamera.orthographic) {
                            _currentCamera.orthographicSize += _currentCamera.orthographicSize * wheelAccel * _mouseWheelSensitivity * deltaTime;
                        } else {
                            _currentCamera.transform.Translate((_currentCamera.transform.position - dest) * wheelAccel * _mouseWheelSensitivity * deltaTime, Space.World);
                        }
                        if (zoomDampingStart > 0) {
                            float t = (Time.time - zoomDampingStart) / _zoomDampingDuration;
                            wheelAccel = Mathf.Lerp(wheelAccel, 0, t); 
                        } else {
                            wheelAccel = 0;
                        }
                    } else {
                        wheelAccel = 0;
                    }
                }
            }

            if (pinching && wheelAccel == 0 || Input.touchCount == 0) {
                pinching = false;
            }

            // Verify if mouse enter a country boundary - we only check if mouse is inside the map
            if (!pinching) {
                if (mouseIsOver) {

                    // Remember the last element clicked
                    int buttonIndex = -1;
                    bool releasing = true;
                    if (Input.GetMouseButtonUp(0)) {
                        buttonIndex = 0;
                        if (!hasDragged)
                            dragging = false;
                    } else if (Input.GetMouseButtonUp(1)) {
                        buttonIndex = 1;
                    } else if (Input.GetMouseButtonDown(0)) {
                        buttonIndex = 0;
                        releasing = false;
                    } else if (Input.GetMouseButtonDown(1)) {
                        buttonIndex = 1;
                        releasing = false;
                    }

                    // Check highlighting only if flyTo is not active to prevent hiccups during movement
                    if (!flyToActive) {

                        Vector3 localPoint;
                        bool goodHit = GetLocalHitFromMousePos(out localPoint);
                        bool positionMoved = false;
                        if (localPoint != lastMouseMapHitPos || !lastMouseMapHitPosGood) {
                            lastMouseMapHitPos = localPoint;
                            lastMouseMapHitPosGood = goodHit;
                            positionMoved = true;
                        }
                        if (Application.isMobilePlatform) {
                            if (Input.touchCount == 1) {
                                if (!dragging && releasing) {
                                    CheckMousePos();
                                    GridCheckMousePos();        // Verify if mouse enter a territory boundary - we only check if mouse is inside the sphere of world
                                }
                            }
                        } else {
                            if (positionMoved) {
                                if (!dragging) {
                                    CheckMousePos();
                                    GridCheckMousePos();        // Verify if mouse enter a territory boundary - we only check if mouse is inside the sphere of world
                                }
                            }
                        }
                        if (positionMoved && lastMouseMapHitPosGood) {
                            // Cursor follow
                            if (_cursorFollowMouse) {
                                cursorLocation = lastMouseMapHitPos;
                            }

                            // Raise mouse move event
                            if (OnMouseMove != null) {
                                OnMouseMove(lastMouseMapHitPos.x, lastMouseMapHitPos.y);
                            }
                        }
                    }

                    if (buttonIndex >= 0) {
                        if (releasing) {
                            if (OnMouseRelease != null)
                                OnMouseRelease(_cursorLocation.x, _cursorLocation.y, buttonIndex);
                            if (!hasDragged || Time.time - clickTime < TAP_THRESHOLD) {
                                _countryLastClicked = _countryHighlightedIndex;
                                _countryRegionLastClicked = _countryRegionHighlightedIndex;
                                _provinceLastClicked = _provinceHighlightedIndex;
                                _provinceRegionLastClicked = _provinceRegionHighlightedIndex;
                                _cityLastClicked = _cityHighlightedIndex;
                                _cellLastClickedIndex = _cellHighlightedIndex;
                                if (VGOLastHighlighted == null || !VGOLastHighlighted.blocksRayCast) {
                                    if (_countryLastClicked >= 0) {
                                        if (OnCountryClick != null) {
                                            OnCountryClick(_countryLastClicked, _countryRegionLastClicked, buttonIndex);
                                        }
                                        if (OnRegionClick != null) {
                                            OnRegionClick(_countries[_countryLastClicked].regions[_countryRegionLastClicked], buttonIndex);
                                        }
                                    }
                                    if (_provinceLastClicked >= 0) {
                                        if (OnProvinceClick != null) {
                                            OnProvinceClick(_provinceLastClicked, _provinceRegionLastClicked, buttonIndex);
                                        }
                                        if (OnRegionClick != null) {
                                            OnRegionClick(_provinces[_provinceLastClicked].regions[_provinceRegionLastClicked], buttonIndex);
                                        }

                                    }
                                    if (_cityLastClicked >= 0 && OnCityClick != null) {
                                        OnCityClick(_cityLastClicked, buttonIndex);
                                    }
                                    if (_cellLastClickedIndex >= 0 && OnCellClick != null) {
                                        OnCellClick(_cellLastClickedIndex, buttonIndex);
                                    }
                                    if (OnClick != null) {
                                        OnClick(_cursorLocation.x, _cursorLocation.y, buttonIndex);
                                    }
                                }
                            }
                        } else {
                            if (OnMouseDown != null)
                                OnMouseDown(_cursorLocation.x, _cursorLocation.y, buttonIndex);
                        }
                    }

                    if (hasDragged && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))) {
                        if (OnDragEnd != null)
                            OnDragEnd();
                    }

                    // if mouse/finger is over map, implement drag and zoom of the world
                    if (_allowUserDrag) {
                        // Use left mouse button to drag the map
                        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                            mouseDragStart = Input.mousePosition;
                            mouseDragStartHitPos = lastMouseMapHitPos;
                            dragging = true;
                            hasDragged = false;
                            flyToActive = false;
                        }

                        // Use right mouse button and fly and center on target country
                        if (Input.GetMouseButtonDown(1) && !Input.touchSupported) { // two fingers can be interpreted as right mouse button -> prevent this.
                            if (Input.GetMouseButtonDown(1) && _centerOnRightClick) {
                                if (VGOLastHighlighted != null) {
                                    FlyToLocation(VGOLastHighlighted.currentMap2DLocation, 0.8f);
                                } else if (_provinceHighlightedIndex >= 0) {
                                    Vector2 regionPos = provinces[_provinceHighlightedIndex].regions[_provinceRegionHighlightedIndex].center;
                                    FlyToLocation(regionPos, 0.8f);
                                } else if (_countryHighlightedIndex >= 0) {
                                    Vector2 regionPos = _countries[_countryHighlightedIndex].regions[_countryRegionHighlightedIndex].center;
                                    FlyToLocation(regionPos, 0.8f);
                                }
                            }
                        }
                    }
                }

                // Check special keys
                if (_allowUserKeys) {
                    Vector3 keyDragDirection = Misc.Vector3zero;
                    bool pressed = false;
                    if (Input.GetKey(_keyUp)) {
                        keyDragDirection = Misc.Vector3down;
                        pressed = true;
                    }
                    if (Input.GetKey(_keyDown)) {
                        keyDragDirection += Misc.Vector3up;
                        pressed = true;
                    }
                    if (Input.GetKey(_keyLeft)) {
                        keyDragDirection += Misc.Vector3right;
                        pressed = true;
                    }
                    if (Input.GetKey(_keyRight)) {
                        keyDragDirection += Misc.Vector3left;
                        pressed = true;
                    }
                    if (pressed) {
                        buttonLeftPressed = false;
                        dragDirection = keyDragDirection;
                        if (_currentCamera.orthographic) {
                            dragSpeed = _currentCamera.orthographicSize * 10.0f * _mouseDragSensitivity;
                        } else {
                            dragSpeed = lastDistanceFromCamera * _mouseDragSensitivity;
                        }
                        dragDirection *= _dragKeySpeedMultiplier * dragSpeed;
                        if (dragFlipDirection) {
                            dragDirection *= -1;
                        }
                        if (_dragConstantSpeed) {
                            dragDampingStart = Time.time + _dragDampingDuration;
                        } else {
                            dragDampingStart = Time.time;
                        }
                    }
                }

                if (dragging) {
                    if (buttonLeftPressed) {
                        if (_allowUserDrag || _allowUserKeys) {
                            if (_dragConstantSpeed) {
                                if (lastMouseMapHitPosGood && mouseIsOver) {
                                    dragDirection = lastMouseMapHitPos - mouseDragStartHitPos;
                                    dragDirection.x = ApplyDragThreshold(dragDirection.x);
                                    dragDirection.y = ApplyDragThreshold(dragDirection.y);
                                    if (dragDirection.x != 0 || dragDirection.y != 0) {
                                        dragDirection = transform.TransformVector(dragDirection);
                                        _currentCamera.transform.Translate(-dragDirection, Space.World);
                                        dragDampingStart = Time.time;
                                    }
                                }
                            } else {
                                dragDirection = (Input.mousePosition - mouseDragStart);
                                dragDirection.x = ApplyDragThreshold(dragDirection.x);
                                dragDirection.y = ApplyDragThreshold(dragDirection.y);
                                if (dragDirection.x != 0 || dragDirection.y != 0) {
                                    if (_currentCamera.orthographic) {
                                        dragSpeed = _currentCamera.orthographicSize * _mouseDragSensitivity * 0.00035f;
                                    } else {
                                        dragSpeed = lastDistanceFromCamera * _mouseDragSensitivity * 0.00035f;
                                    }
                                    dragDampingStart = Time.time;
                                    dragDirection *= dragSpeed;
                                    // Drag along the map plane
                                    _currentCamera.transform.Translate(-dragDirection * deltaTime, transform);
                                }
                            }
                        }
                    } else {
                        dragging = false;
                    }
                }

                if (!hasDragged) {
                    if (buttonLeftPressed && Time.time - clickTime > TAP_THRESHOLD && (dragDirection.x != 0 || dragDirection.y != 0)) {
                        hasDragged = true;
                        if (OnDragStart != null)
                            OnDragStart();
                    }
                } else if (!buttonLeftPressed) {
                    hasDragged = false;
                }
            }

            // Check scroll on borders
            if (_allowScrollOnScreenEdges && (_allowUserDrag || _allowUserKeys)) {
                bool onEdge = false;
                float mx = Input.mousePosition.x;
                float my = Input.mousePosition.y;
                if (mx >= 0 && mx < Screen.width && my >= 0 && my < Screen.height) {
                    if (my < _screenEdgeThickness) {
                        dragDirection = Misc.Vector3up;
                        onEdge = true;
                    }
                    if (my >= Screen.height - _screenEdgeThickness) {
                        dragDirection = Misc.Vector3down;
                        onEdge = true;
                    }
                    if (mx < _screenEdgeThickness) {
                        dragDirection = Misc.Vector3right;
                        onEdge = true;
                    }
                    if (mx >= Screen.width - _screenEdgeThickness) {
                        dragDirection = Misc.Vector3left;
                        onEdge = true;
                    }
                }
                if (onEdge) {
                    if (_currentCamera.orthographic) {
                        dragSpeed = _currentCamera.orthographicSize * 10.0f * _mouseDragSensitivity;
                    } else {
                        dragSpeed = lastDistanceFromCamera * _mouseDragSensitivity;
                    }
                    dragDirection *= 0.1f * dragSpeed;
                    if (dragFlipDirection) {
                        dragDirection *= -1;
                    }
                    dragDampingStart = Time.time;
                }
            }


            if (dragDampingStart > 0 && !buttonLeftPressed) {
                float t = 1f - (Time.time - dragDampingStart) / dragDampingDuration;
                if (t < 0) {
                    t = 0;
                    dragDampingStart = 0f;
                } else if (t > 1f) {
                    t = 1f;
                    dragDampingStart = 0f;
                }
                dragging = true;
                _currentCamera.transform.Translate(-dragDirection * (t * deltaTime), transform);
            }

        }

        public void OnMouseEnter() {
            mouseIsOver = true;
        }

        public void OnMouseExit() {
            // Make sure it's outside of map
            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x >= 0 && mousePos.x < Screen.width && mousePos.y >= 0 && mousePos.y < Screen.height) {
                Ray ray = cameraMain.ScreenPointToRay(mousePos);
                int hitCount = Physics.RaycastNonAlloc(ray.origin, ray.direction, tempHits, 2000);
                for (int k = 0; k < hitCount; k++) {
                    if (tempHits[k].collider.name.Equals(WMSKMiniMap.MINIMAP_NAME)) {
                        mouseIsOver = false;
                        return;
                    }
                }
                for (int k = 0; k < hitCount; k++) {
                    if (tempHits[k].collider.gameObject == _renderViewport)
                        return;
                }
            }

            mouseIsOver = false;
            HideCountryRegionHighlight();
        }

        public void DoOnMouseClick() {
            mouseIsOver = true;
            Update();
        }

        public void DoOnMouseRelease() {
            Update();
            mouseIsOver = false;
            HideCountryRegionHighlight();
        }

        #endregion

        #region System initialization

        public void Init() {

#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
            UnityEditor.PrefabInstanceStatus prefabInstanceStatus = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject);
            if (prefabInstanceStatus != UnityEditor.PrefabInstanceStatus.NotAPrefab) {
                UnityEditor.PrefabUtility.UnpackPrefabInstance(gameObject, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
            }
#else
            UnityEditor.PrefabType prefabType = UnityEditor.PrefabUtility.GetPrefabType(gameObject);
            if (prefabType != UnityEditor.PrefabType.None && prefabType != UnityEditor.PrefabType.DisconnectedPrefabInstance && prefabType != UnityEditor.PrefabType.DisconnectedModelPrefabInstance) {
                UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
            }
#endif
#endif

            if (disposalManager == null) {
                disposalManager = new DisposalManager();
            }

            // Conversion from old scales
            if (_renderViewportGOAutoScaleMultiplier > 10f) {
                _renderViewportGOAutoScaleMultiplier *= 0.01f;
                isDirty = true;
            }

            // Boot initialization
            tempHits = new RaycastHit[100];
            int mapLayer = gameObject.layer;
            foreach (Transform t in transform) {
                t.gameObject.layer = mapLayer;
            }
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.detectCollisions = false;

            SetupVGOs();

            SetupViewport();

            // Labels materials
            ReloadFont();
            ReloadProvinceFont();

            // Map materials
            frontiersMat = Instantiate(Resources.Load<Material>("WMSK/Materials/Frontiers"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(frontiersMat); // frontiersMat.hideFlags = HideFlags.DontSave;
            frontiersMat.shader.maximumLOD = 300;
            hudMatCountry = Instantiate(Resources.Load<Material>("WMSK/Materials/HudCountry"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(hudMatCountry); //.hideFlags = HideFlags.DontSave;
            hudMatProvince = Instantiate(Resources.Load<Material>("WMSK/Materials/HudProvince"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(hudMatProvince); //.hideFlags = HideFlags.DontSave;
            hudMatProvince.renderQueue++;   // render on top of country highlight
            CheckCityIcons();
            citiesNormalMat = Instantiate(Resources.Load<Material>("WMSK/Materials/Cities"));
            citiesNormalMat.name = "Cities";
            if (disposalManager != null)
                disposalManager.MarkForDisposal(citiesNormalMat); //.hideFlags = HideFlags.DontSave;
            citiesRegionCapitalMat = Instantiate(Resources.Load<Material>("WMSK/Materials/CitiesCapitalRegion"));
            citiesRegionCapitalMat.name = "CitiesCapitalRegion";
            if (disposalManager != null)
                disposalManager.MarkForDisposal(citiesRegionCapitalMat); //.hideFlags = HideFlags.DontSave;
            citiesCountryCapitalMat = Instantiate(Resources.Load<Material>("WMSK/Materials/CitiesCapitalCountry"));
            citiesCountryCapitalMat.name = "CitiesCapitalCountry";
            if (disposalManager != null)
                disposalManager.MarkForDisposal(citiesCountryCapitalMat); //.hideFlags = HideFlags.DontSave;

            provincesMat = Instantiate(Resources.Load<Material>("WMSK/Materials/Provinces"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(provincesMat); //.hideFlags = HideFlags.DontSave;
            provincesMat.shader.maximumLOD = 300;
            outlineMatSimple = Instantiate(Resources.Load<Material>("WMSK/Materials/Outline"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(outlineMatSimple); //.hideFlags = HideFlags.DontSave;
            outlineMatTextured = Instantiate(Resources.Load<Material>("WMSK/Materials/OutlineTex"));
            if (_outlineTexture == null)
                _outlineTexture = (Texture2D)outlineMatTextured.mainTexture;
            outlineMatTextured.mainTexture = _outlineTexture;
            outlineMatTextured.mainTextureScale = new Vector2(_outlineTilingScale, 1f);
            if (disposalManager != null)
                disposalManager.MarkForDisposal(outlineMatTextured); //.hideFlags = HideFlags.DontSave;
            coloredMat = Instantiate(Resources.Load<Material>("WMSK/Materials/ColorizedRegion"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(coloredMat); //.hideFlags = HideFlags.DontSave;
            coloredAlphaMat = Instantiate(Resources.Load<Material>("WMSK/Materials/ColorizedTranspRegion"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(coloredAlphaMat); //.hideFlags = HideFlags.DontSave;
            texturizedMat = Instantiate(Resources.Load<Material>("WMSK/Materials/TexturizedRegion"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(texturizedMat); //.hideFlags = HideFlags.DontSave;
            cursorMatH = Instantiate(Resources.Load<Material>("WMSK/Materials/CursorH"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(cursorMatH); // = HideFlags.DontSave;
            cursorMatV = Instantiate(Resources.Load<Material>("WMSK/Materials/CursorV"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(cursorMatV); //.hideFlags = HideFlags.DontSave;
            imaginaryLinesMat = Instantiate(Resources.Load<Material>("WMSK/Materials/ImaginaryLines"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(imaginaryLinesMat); //.hideFlags = HideFlags.DontSave;
            markerLineMat = Instantiate(Resources.Load<Material>("WMSK/Materials/MarkerLine"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(markerLineMat); //.hideFlags = HideFlags.DontSave;
            markerMat = Instantiate(markerLineMat);  //Resources.Load<Material>("WMSK/Materials/Marker")); // Marker shader is not compatible with LWRP so we use markerLineMat which serves the same purpose. Kept old shader for a while for compatibility reasons.
            if (disposalManager != null)
                disposalManager.MarkForDisposal(markerMat);
            mountPointSpot = Resources.Load<GameObject>("WMSK/Prefabs/MountPointSpot");
            mountPointsMat = Instantiate(Resources.Load<Material>("WMSK/Materials/Mount Points"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(mountPointsMat); //.hideFlags = HideFlags.DontSave;
            gridMat = Instantiate(Resources.Load<Material>("WMSK/Materials/Grid"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(gridMat); //.hideFlags = HideFlags.DontSave;
            gridMat.renderQueue++;
            hudMatCell = Instantiate(Resources.Load<Material>("WMSK/Materials/HudCell"));
            if (disposalManager != null)
                disposalManager.MarkForDisposal(hudMatCell); //.hideFlags = HideFlags.DontSave;
            hudMatCell.renderQueue++;
            extrudedMat = Instantiate(Resources.Load<Material>("WMSK/Materials/ExtrudedRegion"));
            SRP.Configure(extrudedMat);

            coloredMatCache = new Dictionary<Color, Material>();
            markerMatCache = new Dictionary<Color, Material>();

            if (_dontLoadGeodataAtStart) {
                countries = new Country[0];
                provinces = new Province[0];
                cities = new List<City>();
                mountPoints = new List<MountPoint>();
            } else {
                ReloadData();
            }

            if (_showTiles)
                InitTileSystem();


            // Redraw frontiers and cities -- destroy layers if they already exists
            if (!Application.isPlaying) {
                Redraw();
            }

            PostInit();

        }

        void PostInit() {
            // Additional setup executed only during initialization

            // Check material
            Renderer renderer = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
            if (renderer.sharedMaterial == null) {
                RestyleEarth();
            }

            if (hudMatCountry != null && hudMatCountry.color != _fillColor) {
                hudMatCountry.color = _fillColor;
            }
            UpdateFrontiersMaterial();
            if (hudMatProvince != null && hudMatProvince.color != _provincesFillColor) {
                hudMatProvince.color = _provincesFillColor;
            }
            if (provincesMat != null && provincesMat.color != _provincesColor) {
                provincesMat.color = _provincesColor;
            }
            if (citiesNormalMat.color != _citiesColor) {
                citiesNormalMat.color = _citiesColor;
            }
            if (citiesRegionCapitalMat.color != _citiesRegionCapitalColor) {
                citiesRegionCapitalMat.color = _citiesRegionCapitalColor;
            }
            if (citiesCountryCapitalMat.color != _citiesCountryCapitalColor) {
                citiesCountryCapitalMat.color = _citiesCountryCapitalColor;
            }
            if (outlineMat.color != _outlineColor) {
                outlineMat.color = _outlineColor;
            }
            if (cursorMatH.color != _cursorColor) {
                cursorMatH.color = _cursorColor;
            }
            if (cursorMatV.color != _cursorColor) {
                cursorMatV.color = _cursorColor;
            }
            if (imaginaryLinesMat.color != _imaginaryLinesColor) {
                imaginaryLinesMat.color = _imaginaryLinesColor;
            }
            if (hudMatCell != null && hudMatCell.color != _cellHighlightColor) {
                hudMatCell.color = _cellHighlightColor;
            }
            if (gridMat != null && gridMat.color != _gridColor) {
                gridMat.color = _gridColor;
            }
            if (_enableCellHighlight) {
                showLatitudeLines = showLongitudeLines = false;
            }

            // Unity 5.3.1 prevents raycasting in the scene view if rigidbody is present - we have to delete it in editor time but re-enable it here during play mode
            if (Application.isPlaying) {

                if (GetComponent<Rigidbody>() == null) {
                    Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }

                Redraw();

                if (_prewarm) {
                    CountriesPrewarmBigSurfaces();
                    PathFindingPrewarm();
                }
            }

            maxFrustumDistance = float.MaxValue;
            if (_fitWindowWidth || _fitWindowHeight)
                CenterMap();
        }

        /// <summary>
        /// Reloads the data of frontiers and cities from datafiles and redraws the map.
        /// </summary>
        public void ReloadData() {
            // Destroy surfaces layer
            DestroySurfaces();
            // read precomputed data
            ReadCountriesPackedString();
            if (_showCities || GetComponent<WMSK_Editor>() != null)
                ReadCitiesPackedString();
            if (_showProvinces || _enableProvinceHighlight || GetComponent<WMSK_Editor>() != null)
                ReadProvincesPackedString();
            ReadMountPointsPackedString();
        }


        void DestroySurfaces() {
            HideCountryRegionHighlights(true);
            HideProvinceRegionHighlight();
            if (frontiersCacheHit != null)
                frontiersCacheHit.Clear();
            InitSurfacesCache();
            if (_surfacesLayer != null)
                DestroyImmediate(_surfacesLayer);
            if (provincesObj != null)
                DestroyImmediate(provincesObj);
        }


        #endregion

        #region Drawing stuff

        float lastGCTime;

        /// <summary>
        /// Immediately destroys any gameobject and its children including dynamically created meshes
        /// </summary>
        /// <param name="go">Go.</param>
        void DestroyRecursive(GameObject go) {
            if (go == null)
                return;
            MeshFilter[] mm = go.GetComponentsInChildren<MeshFilter>(true);
            for (int k = 0; k < mm.Length; k++) {
                if (mm[k] != null) {
                    Mesh mesh = mm[k].sharedMesh;
                    mesh.Clear(false);
                    DestroyImmediate(mesh);
                    mm[k].sharedMesh = null;
                }
            }
            Transform[] gg = go.GetComponentsInChildren<Transform>(true);
            for (int k = 0; k < gg.Length; k++) {
                if (gg[k] != null && gg[k] != go.transform) {
                    DestroyImmediate(gg[k].gameObject);
                    gg[k] = null;
                }
            }
            DestroyImmediate(go);

            if (!Application.isPlaying || Time.time - lastGCTime > 10f) {
                lastGCTime = Time.time;
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
            }

        }

        /// <summary>
        /// Convenient function to organize any surface or outline into the hierarchy
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="categoryName">Category name.</param>
        /// <param name="obj">Object.</param>
        void ParentObjectToRegion(string entityId, string categoryName, GameObject obj) {
            Transform entityRoot = surfacesLayer.transform.Find(entityId);
            if (entityRoot == null) {
                GameObject aux = new GameObject(entityId);
                if (disposalManager != null) {
                    disposalManager.MarkForDisposal(aux);
                }
                aux.hideFlags |= HideFlags.HideInHierarchy;
                entityRoot = aux.transform;
                entityRoot.SetParent(surfacesLayer.transform, false);
            }
            // Check if childNode exists
            Transform category;
            if (string.IsNullOrEmpty(categoryName)) {
                category = entityRoot;
            } else {
                category = entityRoot.Find(categoryName);
                if (category == null) {
                    GameObject aux = new GameObject(categoryName);
                    if (disposalManager != null) {
                        disposalManager.MarkForDisposal(aux);
                    }
                    aux.hideFlags |= HideFlags.HideInHierarchy;
                    category = aux.transform;
                    category.SetParent(entityRoot, false);
                }
            }
            // Check if object already exists
            Transform t = category.Find(obj.name);
            if (t != null) {
                DestroyImmediate(t.gameObject);
            }
            obj.transform.SetParent(category, false);
            obj.layer = gameObject.layer;
        }

        /// <summary>
        /// Convenient function which destroys a previously created surface or outline related to a region
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="categoryName">Category name.</param>
        /// <param name="objectName">Object name.</param>
        void HideRegionObject(string entityId, string categoryName, string objectName) {
            Transform t = surfacesLayer.transform.Find(entityId);
            if (t == null)
                return;
            if (string.IsNullOrEmpty(categoryName)) {
                t = t.transform.Find(objectName);
            } else {
                t = t.transform.Find(categoryName + "/" + objectName);
            }
            if (t != null) {
                t.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Used internally and by other components to redraw the layers in specific moments. You shouldn't call this method directly.
        /// </summary>
        /// <param name="forceReconstructFrontiers">If set to <c>true</c> frontiers will be recomputed.</param>
        public void Redraw(bool forceReconstructFrontiers) {
            if (forceReconstructFrontiers)
                needOptimizeFrontiers = true;
            Redraw();
        }

        /// <summary>
        /// Redraws all map layers.
        /// </summary>
        public void Redraw() {
            if (!gameObject.activeInHierarchy)
                return;

            if (lastDistanceFromCamera == 0) {
                Camera cam = currentCamera;
                if (cam == null)
                    return;
                lastDistanceFromCamera = (transform.position - cam.transform.position).magnitude;
            }

            shouldCheckBoundaries = true;

            DestroySurfaces();  // Initialize surface cache, destroys already generated surfaces

            RestyleEarth(); // Apply texture to Earth

            DrawFrontiers();    // Redraw frontiers -- the next method is also called from width property when this is changed

            DrawAllProvinceBorders(needOptimizeFrontiers, false); // Redraw province borders

            needOptimizeFrontiers = false;

            DrawCities();       // Redraw cities layer

            DrawMountPoints();  // Redraw mount points (only in Editor time)

            DrawCursor();       // Draw cursor lines

            DrawImaginaryLines();       // Draw longitude & latitude lines

            DrawMapLabels();    // Destroy existing texts and draw them again

            DrawGrid();

            SetupViewport();

        }

        void InitSurfacesCache() {
            if (surfaces != null) {
                List<GameObject> cached = new List<GameObject>(surfaces.Values);
                for (int k = 0; k < cached.Count; k++) {
                    if (cached[k] != null)
                        DestroyImmediate(cached[k]);
                }
                surfaces.Clear();
            } else {
                surfaces = new Dictionary<int, GameObject>();
            }
        }

        void CreateSurfacesLayer() {
            Transform t = transform.Find(SURFACE_LAYER);
            if (t != null) {
                DestroyImmediate(t.gameObject);
                for (int k = 0; k < _countries.Length; k++)
                    for (int r = 0; r < _countries[k].regions.Count; r++)
                        _countries[k].regions[r].customMaterial = null;
            }
            _surfacesLayer = new GameObject(SURFACE_LAYER);
            _surfacesLayer.transform.SetParent(transform, false);
            _surfacesLayer.transform.localPosition = Misc.Vector3back * 0.001f;
            _surfacesLayer.layer = gameObject.layer;
        }

        void RestyleEarth() {
            if (gameObject == null)
                return;

            string materialName;
            switch (_earthStyle) {
                case EARTH_STYLE.Alternate1:
                    materialName = "Earth2";
                    break;
                case EARTH_STYLE.Alternate2:
                    materialName = "Earth4";
                    break;
                case EARTH_STYLE.Alternate3:
                    materialName = "Earth5";
                    break;
                case EARTH_STYLE.SolidColor:
                    materialName = "EarthSolidColor";
                    break;
                case EARTH_STYLE.Texture:
                    materialName = "EarthTexture";
                    break;
                case EARTH_STYLE.NaturalHighRes:
                    materialName = "EarthHighRes";
                    break;
                case EARTH_STYLE.NaturalHighRes16K:
                    materialName = "EarthHighRes16K";
                    break;
                case EARTH_STYLE.NaturalScenic:
                    materialName = "EarthScenic";
                    break;
                case EARTH_STYLE.NaturalScenicPlus:
                    materialName = "EarthScenicPlus";
                    break;
                case EARTH_STYLE.NaturalScenicPlusAlternate1:
                    materialName = "EarthScenicPlusAlternate1";
                    break;
                case EARTH_STYLE.NaturalScenicPlus16K:
                    materialName = "EarthScenicPlus16K";
                    break;
                default:
                    materialName = "Earth";
                    break;
            }

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (earthMat == null || renderer.sharedMaterial == null || !renderer.sharedMaterial.name.Equals(materialName)) {
                earthMat = Instantiate(Resources.Load<Material>("WMSK/Materials/" + materialName));
                if (disposalManager != null) {
                    disposalManager.MarkForDisposal(earthMat);
                }
                earthMat.name = materialName;
                renderer.material = earthMat;
                if (earthBlurred != null && RenderTexture.active != earthBlurred) {
                    RenderTexture.DestroyImmediate(earthBlurred);
                    earthBlurred = null;
                }
            }

            if (_earthStyle == EARTH_STYLE.SolidColor) {
                earthMat.color = _earthColor;
            } else if (_earthStyle == EARTH_STYLE.Texture) {
                earthMat.color = _earthColor;
            } else if (_earthStyle.isScenicPlus()) {
                earthMat.SetColor("_WaterColor", _waterColor);
                Vector3 waterInfo = new Vector3(_waterLevel, _waterFoamThreshold, _waterFoamIntensity);
                earthMat.SetVector("_WaterLevel", waterInfo);
                if (earthBlurred == null && _earthStyle != EARTH_STYLE.NaturalScenicPlus16K) {
                    EarthPrepareBlurredTexture();
                }
                if (_waterMask != null) {
                    earthMat.SetTexture("_TerrestrialMap", _waterMask);
                }
                UpdateScenicPlusDistance();
            }
            if (_earthTexture != null && earthMat.HasProperty("_MainTex")) {
                earthMat.mainTexture = _earthTexture;
            }
            earthMat.mainTextureOffset = -_earthTextureOffset;
            earthMat.mainTextureScale = new Vector2(1f / _earthTextureScale.x, 1f / _earthTextureScale.y); ;

            if (_earthBumpMapTexture != null && _earthStyle.supportsBumpMap()) {
                earthMat.SetTexture("_NormalMap", _earthBumpMapTexture);
            }

            if (_sun == null) {
                FindDirectionalLight();
            }
            if (_earthStyle.supportsBumpMap()) {
                if (_earthBumpEnabled) {
                    earthMat.EnableKeyword(SKW_BUMPMAP_ENABLED);
                    earthMat.SetFloat("_BumpAmount", _earthBumpAmount);
                    if (_sun != null) {
                        earthMat.SetVector("_SunLightDirection", -_sun.transform.forward);
                    } else {
                        earthMat.SetVector("_SunLightDirection", transform.forward);
                    }
                } else {
                    earthMat.DisableKeyword(SKW_BUMPMAP_ENABLED);
                }
            }
            if (_pathFindingVisualizeMatrixCost && earthMat != null && pathFindingCustomMatrixCostTexture != null) {
                earthMat.mainTexture = pathFindingCustomMatrixCostTexture;
            }

            if (_showTiles) {
                if (Application.isPlaying) {
                    if (tilesRoot == null) {
                        InitTileSystem();
                    } else {
                        ResetTiles();
                    }
                    if (!_tileTransparentLayer) {
                        renderer.enabled = false;
                    }
                } else if (tilesRoot != null) {
                    tilesRoot.gameObject.SetActive(false);
                }
            }


        }

        void FindDirectionalLight() {
            Light[] lights = FindObjectsOfType<Light>();
            if (lights == null)
                return;
            for (int k = 0; k < lights.Length; k++) {
                if (lights[k] != null && lights[k].isActiveAndEnabled && lights[k].type == LightType.Directional) {
                    _sun = lights[k].gameObject;
                    return;
                }
            }
        }


        void EarthPrepareBlurredTexture() {

            Texture2D earthTex = (Texture2D)earthMat.GetTexture("_MainTex");
            if (earthTex == null)
                return;

            if (earthBlurred == null) {
                earthBlurred = new RenderTexture(earthTex.width / 8, earthTex.height / 8, 0);
                earthBlurred.hideFlags = HideFlags.DontSave;
            }
            Material blurMat = new Material(Shader.Find("WMSK/Blur5Tap"));
            if (blurMat != null) {
                Graphics.Blit(earthTex, earthBlurred, blurMat);
            } else {
                Graphics.Blit(earthTex, earthBlurred);
            }
            earthMat.SetTexture("_EarthBlurred", earthBlurred);
        }


        #endregion



        #region Highlighting

        bool GetLocalHitFromMousePos(out Vector3 localPoint) {
            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height) {
                localPoint = Misc.Vector3zero;
                return false;
            }
            return GetLocalHitFromScreenPos(mousePos, out localPoint, false);
        }

        /// <summary>
        /// Check mouse hit on the map and return the local plane coordinate. Handles viewports.
        /// </summary>
        /// <returns><c>true</c>, if local hit from screen position was gotten, <c>false</c> otherwise.</returns>
        /// <param name="screenPos">Screen position.</param>
        /// <param name="localPoint">Local point.</param>
        /// <param name="nonWrap">If true is passed, then a local hit is returned either on the real map plane or in a assumed wrapped plane next to it (effectively returning an x coordinate from -1.5..1.5</param>
        public bool GetLocalHitFromScreenPos(Vector3 screenPos, out Vector3 localPoint, bool nonWrap) {
            Ray ray = cameraMain.ScreenPointToRay(screenPos);
            int hitCount = Physics.RaycastNonAlloc(ray.origin, ray.direction, tempHits, 2000, layerMask);
            if (hitCount > 0) {
                for (int k = 0; k < hitCount; k++) {
                    // Hit the map?
                    if (tempHits[k].collider.gameObject == _renderViewport) {
                        if (_renderViewportIsTerrain) {
                            localPoint = tempHits[k].point; // hit on terrain in world space
                            float terrainCenterX = terrain.transform.position.x + terrain.terrainData.size.x * 0.5f;
                            float terrainCenterZ = terrain.transform.position.z + terrain.terrainData.size.z * 0.5f;
                            localPoint.x = ((localPoint.x - terrainCenterX) / terrain.terrainData.size.x);
                            localPoint.y = ((localPoint.z - terrainCenterZ) / terrain.terrainData.size.z);
                            localPoint.z = 0;
                            return true;
                        }
                        localPoint = _renderViewport.transform.InverseTransformPoint(tempHits[k].point);
                        localPoint.z = 0;
                        // Is the viewport a render viewport or the map itself? If it's a render viewport projects hit into mapper cam space
                        if (renderViewportIsEnabled) {
                            // Get plane in screen space
                            localPoint.x += 0.5f;   // convert to viewport coordinates
                            localPoint.y += 0.5f;
                            Vector3 tl = _currentCamera.WorldToViewportPoint(transform.TransformPoint(new Vector3(-0.5f, 0.5f)));
                            Vector3 br = _currentCamera.WorldToViewportPoint(transform.TransformPoint(new Vector3(0.5f, -0.5f)));

                            if (nonWrap) {
                                localPoint.x = (localPoint.x - tl.x) / (br.x - tl.x) - 0.5f;
                                localPoint.y = (localPoint.y - br.y) / (tl.y - br.y) - 0.5f;
                                return true;
                            } else {
                                if (_wrapHorizontally) {    // enables wrapping mode location
                                    if (localPoint.x < tl.x)
                                        localPoint.x = br.x - (tl.x - localPoint.x);
                                    else if (localPoint.x > br.x) {
                                        localPoint.x = tl.x + localPoint.x - br.x;
                                    }
                                }
                                // Trace the ray from this position in mapper cam space
                                if (localPoint.x >= tl.x && localPoint.x <= br.x && localPoint.y >= br.y && localPoint.y <= tl.y) {
                                    localPoint.x = (localPoint.x - tl.x) / (br.x - tl.x) - 0.5f;
                                    localPoint.y = (localPoint.y - br.y) / (tl.y - br.y) - 0.5f;
                                    return true;
                                }
                            }
                        } else
                            return true;
                    }
                }
            }
            localPoint = Misc.Vector3zero;
            return false;
        }

        void CheckMousePos() {

            if (!lastMouseMapHitPosGood) {
                HideCountryRegionHighlight();
                if (!_drawAllProvinces)
                    HideProvinces();
                return;
            }

            // verify if hitPos is inside any country polygon
            int candidateCountryIndex = -1;
            int candidateCountryRegionIndex = -1;
            int candidateProvinceIndex = -1;
            int candidateProvinceRegionIndex = -1;
            float candidateProvinceRegionSize = float.MaxValue;

            // optimization: is inside current highlighted region?
            bool needCheckRegion = _enableEnclaves ||
                                   ((_showProvinces || _enableProvinceHighlight) && _provinceHighlightedIndex < 0 || _provinceRegionHighlightedIndex < 0) ||
                                   ((_showFrontiers || _enableCountryHighlight) && (_countryHighlightedIndex < 0 || _countryRegionHighlightedIndex < 0));

            if (!needCheckRegion) {
                if ((_showProvinces || _enableProvinceHighlight) &&
                    _provinceHighlightedIndex >= 0 && _provinceRegionHighlightedIndex >= 0 && !_provinces[provinceHighlightedIndex].regions[_provinceRegionHighlightedIndex].Contains(lastMouseMapHitPos)) {
                    needCheckRegion = true;
                }
                if (!needCheckRegion &&
                    (_showFrontiers || _enableCountryHighlight) &&
                    _countryHighlightedIndex >= 0 && _countryRegionHighlightedIndex >= 0 && !_countries[_countryHighlightedIndex].regions[_countryRegionHighlightedIndex].Contains(lastMouseMapHitPos)) {
                    needCheckRegion = true;
                }
            }

            if (needCheckRegion) {
                int countryCount = _countriesOrderedBySize.Count;
                for (int oc = 0; oc < countryCount; oc++) {
                    int c = _countriesOrderedBySize[oc];
                    Country country = _countries[c];
                    if (country.hidden)
                        continue;
                    if (!country.regionsRect2D.Contains(lastMouseMapHitPos))
                        continue;
                    int regionCount = country.regions.Count;
                    for (int cr = 0; cr < regionCount; cr++) {
                        Region countryRegion = country.regions[cr];
                        if (countryRegion.Contains(lastMouseMapHitPos)) {
                            candidateCountryIndex = c;
                            candidateCountryRegionIndex = cr;
                            if (_showProvinces || _enableProvinceHighlight) {
                                if (country.provinces != null) {
                                    for (int p = 0; p < country.provinces.Length; p++) {
                                        // and now, we check if the mouse if inside a province
                                        Province province = country.provinces[p];
                                        if (province.regions == null) { // read province data the first time we need it
                                            ReadProvincePackedString(province);
                                        }
                                        if (province.regionsRect2D.Contains(lastMouseMapHitPos)) {
                                            int regCount = province.regions.Count;
                                            for (int pr = 0; pr < regCount; pr++) {
                                                Region provRegion = province.regions[pr];
                                                if (provRegion.rect2DArea < candidateProvinceRegionSize && provRegion.Contains(lastMouseMapHitPos)) {
                                                    candidateProvinceRegionSize = provRegion.rect2DArea;
                                                    candidateProvinceIndex = GetProvinceIndex(province);
                                                    candidateProvinceRegionIndex = pr;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    if (candidateCountryIndex >= 0) 
                        break;
                }
                // If no candidate country found, try looking into provinces directly just in case some province doesn't have a country region on the same area
                if (candidateCountryIndex < 0 && (_showProvinces || _enableProvinceHighlight)) {
                    int provincesCount = provinces.Length;
                    for (int p = 0; p < provincesCount; p++) {
                        Province province = _provinces[p];
                        if (province.regionsRect2D.Contains(lastMouseMapHitPos)) {
                            if (province.regions == null) {
                                ReadProvincePackedString(province);
                            }
                            int regionsCount = province.regions.Count;
                            for (int pr = 0; pr < regionsCount; pr++) {
                                Region provRegion = province.regions[pr];
                                if (provRegion.rect2DArea < candidateProvinceRegionSize && provRegion.Contains(lastMouseMapHitPos)) {
                                    candidateProvinceRegionSize = provRegion.rect2DArea;
                                    candidateProvinceIndex = p;
                                    candidateProvinceRegionIndex = pr;
                                    candidateCountryIndex = province.countryIndex;
                                    candidateCountryRegionIndex = countries[candidateCountryIndex].mainRegionIndex; // fallback; we don't have a specific country region under this province so we link it to the main region
                                }
                            }
                        }
                    }
                }

            } else {
                candidateCountryIndex = _countryHighlightedIndex;
                candidateCountryRegionIndex = _countryRegionHighlightedIndex;
                candidateProvinceIndex = _provinceHighlightedIndex;
                candidateProvinceRegionIndex = _provinceRegionHighlightedIndex;
            }

            if (candidateCountryIndex != _countryHighlightedIndex || (candidateCountryIndex == _countryHighlightedIndex && candidateCountryRegionIndex != _countryRegionHighlightedIndex)) {
                if (candidateCountryIndex < 0) {
                    HideCountryRegionHighlight();
                } else {
                    // Raise enter event
                    if (candidateCountryIndex >= 0 && candidateCountryRegionIndex >= 0) {
                        if (OnCountryEnter != null) {
                            OnCountryEnter(candidateCountryIndex, candidateCountryRegionIndex);
                        }
                        if (OnRegionEnter != null) {
                            OnRegionEnter(_countries[candidateCountryIndex].regions[candidateCountryRegionIndex]);
                        }
                    }

                    HighlightCountryRegion(candidateCountryIndex, candidateCountryRegionIndex, false, _showOutline);

                    if (_showProvinces) {
                        DrawProvinces(candidateCountryIndex, false, false, false); // draw provinces borders if not drawn
                    }

                    // Draw province labels?
                    if (_showProvinceNames && _provinceLabelsVisibility == PROVINCE_LABELS_VISIBILITY.Automatic && _showAllCountryProvinceNames) {
                        RedrawProvinceLabels(_countries[candidateCountryIndex]);
                    }

                }
            }

            if (candidateProvinceIndex != _provinceHighlightedIndex || (candidateProvinceIndex == _provinceHighlightedIndex && candidateProvinceRegionIndex != _provinceRegionHighlightedIndex)) {
                // Raise enter event
                if (candidateProvinceIndex >= 0 && candidateProvinceRegionIndex >= 0) {
                    if (OnProvinceEnter != null) {
                        OnProvinceEnter(candidateProvinceIndex, candidateProvinceRegionIndex);
                    }

                    if (OnRegionEnter != null) {
                        OnRegionEnter(_provinces[candidateProvinceIndex].regions[candidateProvinceRegionIndex]);
                    }
                }

                HighlightProvinceRegion(candidateProvinceIndex, candidateProvinceRegionIndex, false);


                // Draw province labels?
                if (_showProvinceNames && _provinceLabelsVisibility == PROVINCE_LABELS_VISIBILITY.Automatic && !_showAllCountryProvinceNames) {
                    RedrawProvinceLabels(_countries[candidateCountryIndex]);
                }
            }

            // Verify if a city is hit inside selected country
            if (_showCities) {
                CheckMousePosCity(lastMouseMapHitPos);
            }
        }


        void CheckMousePosCity(Vector3 localPoint) {
            int ci = GetCityNearPoint(localPoint, _countryHighlightedIndex);
            if (ci >= 0) {
                if (ci != _cityHighlightedIndex) {
                    HideCityHighlight();
                    HighlightCity(ci);
                }
            } else if (_cityHighlightedIndex >= 0) {
                HideCityHighlight();
            }
        }

        #endregion

        #region Internal API

        float ApplyDragThreshold(float value) {
            if (_mouseDragThreshold > 0) {
                if (value < 0) {
                    value += _mouseDragThreshold;
                    if (value > 0)
                        value = 0;
                } else {
                    value -= _mouseDragThreshold;
                    if (value < 0)
                        value = 0;
                }
            }
            return value;

        }



        /// <summary>
        /// Returns the overlay base layer (parent gameObject), useful to overlay stuff on the map (like labels). It will be created if it doesn't exist.
        /// </summary>
        public GameObject GetOverlayLayer(bool createIfNotExists) {
            if (overlayLayer != null) {
                return overlayLayer;
            } else if (createIfNotExists) {
                return CreateOverlay();
            } else {
                return null;
            }
        }


        void SetDestination(Vector2 point, float duration) {
            SetDestination(point, duration, GetZoomLevel());
        }

        void SetDestination(Vector2 point, float duration, float zoomLevel) {
            float distance = GetZoomLevelDistance(zoomLevel);
            SetDestinationAndDistance(point, duration, distance);
        }

        void SetDestinationAndDistance(Vector2 point, float duration, float distance) {

            // if map is in world-wrapping mode, offset the point to the appropriate side of the map
            if (_wrapHorizontally) {
                float x = _cursorLocation.x;
                Vector3 localPos;
                if (GetCurrentMapLocation(out localPos))
                    x = localPos.x;
                float rightSide = point.x + 1f;
                float leftSide = point.x - 1f;
                float distNormal = Mathf.Abs(point.x - x);
                float distRightSide = Mathf.Abs(rightSide - x);
                float distLeftSide = Mathf.Abs(leftSide - x);
                if (distRightSide < distNormal) {
                    point.x = rightSide;
                } else if (distLeftSide < distNormal) {
                    point.x = leftSide;
                }
            }

            // save params call (used by RecalculateFlyToParams)
            flyToCallParamsPoint = point;
            flyToCallZoomDistance = distance;

            // setup lerping parameters
            flyToStartQuaternion = currentCamera.transform.rotation;
            flyToStartLocation = _currentCamera.transform.position;
            if (_enableFreeCamera) {
                flyToEndQuaternion = flyToStartQuaternion;
                flyToEndLocation = transform.TransformPoint(point) - _currentCamera.transform.forward * distance;
            } else {
                flyToEndQuaternion = transform.rotation;
                //				flyToEndLocation = transform.TransformPoint (point) - transform.forward * distance;

                Vector3 offset = _currentCamera.ViewportToWorldPoint(new Vector3(_flyToScreenCenter.x, _flyToScreenCenter.y, distance)) - _currentCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distance));
                flyToEndLocation = transform.TransformPoint(point) - transform.forward * distance - offset;
            }
            flyToDuration = duration;
            flyToActive = true;
            flyToStartTime = Time.time;
            if (OnFlyStart != null) {
                OnFlyStart();
            }
            if (flyToDuration == 0) {
                MoveCameraToDestination();
            }
        }

        /// <summary>
        /// Returns the distance from camera to map according to fit to window width/height parameters
        /// </summary>
        /// <returns>The frustum distance.</returns>
        float GetFrustumDistance() {
            Camera cam = currentCamera;
            if (cam == null)
                return 1;

            float fv = cam.fieldOfView;
            float aspect = cam.aspect;
            float radAngle = fv * Mathf.Deg2Rad;
            float distance, frustumDistanceW, frustumDistanceH;
            if (currentCamera.orthographic) {
                if (_fitWindowHeight) {
                    cam.orthographicSize = mapHeight * 0.5f * _windowRect.height;
                    maxFrustumDistance = cam.orthographicSize;
                } else if (_fitWindowWidth) {
                    cam.orthographicSize = mapWidth * 0.5f * _windowRect.width / aspect;
                    maxFrustumDistance = cam.orthographicSize;
                } else {
                    maxFrustumDistance = float.MaxValue;
                }
                distance = 1;

            } else {
                frustumDistanceH = mapHeight * _windowRect.height * 0.5f / Mathf.Tan(radAngle * 0.5f);
                frustumDistanceW = (mapWidth * _windowRect.width / aspect) * 0.5f / Mathf.Tan(radAngle * 0.5f);
                if (_fitWindowHeight) {
                    distance = Mathf.Min(frustumDistanceH, frustumDistanceW);
                    maxFrustumDistance = distance;
                } else if (_fitWindowWidth) {
                    distance = Mathf.Max(frustumDistanceH, frustumDistanceW);
                    maxFrustumDistance = distance;
                } else {
                    Plane plane = new Plane(-transform.forward, transform.position);
                    distance = plane.GetDistanceToPoint(cam.transform.position);
                    maxFrustumDistance = float.MaxValue;
                }
            }
            return distance;
        }

        /// <summary>
        /// Returns optimum distance between camera and a region of width/height
        /// </summary>
        float GetFrustumZoomLevel(float width, float height) {
            if (currentCamera == null)
                return 1;
            float fv = _currentCamera.fieldOfView;
            float aspect = _currentCamera.aspect;
            float radAngle = fv * Mathf.Deg2Rad;
            float distance, frustumDistanceW, frustumDistanceH;
            if (_currentCamera.orthographic) {
                distance = 1;
            } else {
                frustumDistanceH = height * 0.5f / Mathf.Tan(radAngle * 0.5f);
                frustumDistanceW = (width / aspect) * 0.5f / Mathf.Tan(radAngle * 0.5f);
                distance = Mathf.Max(frustumDistanceH, frustumDistanceW);
            }
            float referenceDistance = GetZoomLevelDistance(1f);
            return distance / referenceDistance;
        }

        /// <summary>
        /// Gets the distance according to the zoomLevel. The zoom level is a value between 0 and 1 which maps to 0-max zoom distance parameter.
        /// </summary>
        /// <returns>The zoom level distance.</returns>
        float GetZoomLevelDistance(float zoomLevel) {
            Camera cam = currentCamera;
            if (cam == null)
                return 0;

            zoomLevel = Mathf.Clamp01(zoomLevel);

            float fv = cam.fieldOfView;
            float radAngle = fv * Mathf.Deg2Rad;
            float aspect = cam.aspect;
            float frustumDistanceH = mapHeight * 0.5f / Mathf.Tan(radAngle * 0.5f);
            float frustumDistanceW = (mapWidth / aspect) * 0.5f / Mathf.Tan(radAngle * 0.5f);
            float distance;
            if (_fitWindowWidth) {
                distance = Mathf.Max(frustumDistanceH, frustumDistanceW);
            } else {
                distance = Mathf.Min(frustumDistanceH, frustumDistanceW);
            }
            return distance * zoomLevel;
        }

        /// <summary>
        /// Returns the current distance to map from the camera
        /// </summary>
        public float GetMapDistance() {
            Camera cam = currentCamera;
            if (cam == null)
                return 0;

            Plane p = new Plane(transform.forward, transform.position);
            return p.GetDistanceToPoint(cam.transform.position);
        }

        /// <summary>
        /// Used internally to translate the camera during FlyTo operations. Use FlyTo method.
        /// </summary>
        void MoveCameraToDestination() {
            float delta;
            Quaternion rotation;
            Vector3 destination;
            if (flyToDuration == 0) {
                delta = 0;
                rotation = flyToEndQuaternion;
                destination = flyToEndLocation;
            } else {
                delta = (Time.time - flyToStartTime);
                float t = delta / flyToDuration;
                float st = Mathf.SmoothStep(0, 1, t);
                rotation = Quaternion.Lerp(flyToStartQuaternion, flyToEndQuaternion, st);
                destination = Vector3.Lerp(flyToStartLocation, flyToEndLocation, st);
            }
            _currentCamera.transform.rotation = rotation;
            _currentCamera.transform.position = destination;

            if (delta >= flyToDuration) {
                flyToActive = false;
                if (OnFlyEnd != null) {
                    OnFlyEnd();
                }
            }
        }

        // Updates flyTo params due to a change in viewport mode
        void RepositionCamera() {
            if (_renderViewportIsTerrain) {
                Vector3 terrainCenter = terrain.GetPosition();
                terrainCenter.x += terrain.terrainData.size.x * 0.5f;
                terrainCenter.y += 250f;
                terrainCenter.z += terrain.terrainData.size.z * 0.5f;
                cameraMain.transform.position = terrainCenter;
                cameraMain.transform.forward = Vector3.down;
            }

            // When changing viewport mode, the asset changes cameras, so we take the current location and zoom level and updates the new frustrum distance and other things calling CenterMap()
            // then we move the new camera to that location and zoom level and optionally update flyTo params.
            CenterMap();

            // Changes new camera to current map position
            _currentCamera.transform.rotation = transform.rotation;
            _currentCamera.transform.position = transform.TransformPoint(lastKnownMapCoordinates) - transform.forward * GetZoomLevelDistance(lastKnownZoomLevel);

            // update lerping parameters based on the new camera setup and original destination and zoom level
            if (!flyToActive)
                return;
            flyToStartQuaternion = _currentCamera.transform.rotation;
            flyToStartLocation = _currentCamera.transform.position;
            flyToEndQuaternion = transform.rotation;
            flyToEndLocation = transform.TransformPoint(flyToCallParamsPoint) - transform.forward * flyToCallZoomDistance;
            MoveCameraToDestination();
        }


        Material GetColoredTexturedMaterial(Color color, Texture2D texture) {
            return GetColoredTexturedMaterial(color, texture, true);
        }


        Material GetColoredTexturedMaterial(Color color, Texture2D texture, bool autoChooseTransparentMaterial, int renderQueueIncrement = 0) {
            Material customMat;
            if (texture == null) {
                if (cacheMaterials && coloredMatCache.TryGetValue(color, out customMat)) {
                    customMat.renderQueue += renderQueueIncrement;
                    return customMat;
                }
            }
            if (texture != null) {
                customMat = Instantiate(texturizedMat);
                customMat.name = texturizedMat.name;
                customMat.mainTexture = texture;
            } else {
                if (color.a < 1.0f || !autoChooseTransparentMaterial) {
                    customMat = Instantiate(coloredAlphaMat);
                } else {
                    customMat = Instantiate(coloredMat);
                }
                customMat.name = coloredMat.name;
                if (cacheMaterials) {
                    coloredMatCache[color] = customMat;
                }
            }
            customMat.color = color;
            if (disposalManager != null) {
                disposalManager.MarkForDisposal(customMat);
            }
            customMat.renderQueue += renderQueueIncrement;
            return customMat;
        }

        Material GetColoredMarkerMaterial(Color color) {
            Material customMat;
            if (markerMatCache.TryGetValue(color, out customMat)) {
                return customMat;
            }
            customMat = Instantiate(markerMat);
            customMat.name = markerMat.name;
            markerMatCache[color] = customMat;
            customMat.color = color;
            if (disposalManager != null)
                disposalManager.MarkForDisposal(customMat); //.hideFlags = HideFlags.DontSave;
            return customMat;
        }


        void ApplyMaterialToSurface(GameObject obj, Material sharedMaterial) {
            if (obj != null) {
                Renderer[] rr = obj.GetComponentsInChildren<Renderer>(true);    // surfaces can be saved under parent when Include All Regions is enabled
                for (int k = 0; k < rr.Length; k++) {
                    rr[k].sharedMaterial = sharedMaterial;
                }
            }
        }

        void GetPointFromPackedString(ref string s, out float x, out float y) {
            //			int j = -1;
            //			for (int k = 0; k < s.Length; k++) {
            //				if (s [k] == ',') {
            //					j = k;
            //					break;
            //				}
            //			}
            //			if (j < 0) {
            //				x = 0;
            //				y = 0;
            //				return;
            //			}
            //			string sx = s.Substring (0, j);
            //			float.TryParse (sx, NumberStyles.Float, Misc.InvariantCulture, out x);
            //			x /= MAP_PRECISION;
            //			string sy = s.Substring (j + 1);
            //			float.TryParse (sy, NumberStyles.Float, Misc.InvariantCulture, out y);
            //			y /= MAP_PRECISION;


            int d = 1;
            float v = 0;
            y = 0;
            for (int k = s.Length - 1; k >= 0; k--) {
                char ch = s[k];
                if (ch >= '0' && ch <= '9') {
                    v += (ch - '0') * d;
                    d *= 10;
                } else if (ch == '.') {
                    v = v / d;
                } else if (ch == '-') {
                    v = -v;
                } else if (ch == ',') {
                    y = v / MAP_PRECISION;
                    v = 0;
                    d = 1;
                }
            }
            x = v / MAP_PRECISION;
        }


        /// <summary>
        /// Internal usage.
        /// </summary>
        public int GetUniqueId(List<IExtendableAttribute> list) {
            for (int k = 0; k < 1000; k++) {
                int rnd = UnityEngine.Random.Range(0, int.MaxValue);
                int listCount = list.Count;
                for (int o = 0; o < listCount; o++) {
                    IExtendableAttribute obj = list[o];
                    if (obj != null && obj.uniqueId == rnd) {
                        rnd = 0;
                        break;
                    }
                }
                if (rnd > 0)
                    return rnd;
            }
            return 0;
        }

        /// <summary>
        /// Internal usage. Checks quality of polygon points. Useful before using polygon clipping operations.
        /// Return true if there're changes.
        /// </summary>
        public bool RegionSanitize(Region region) {
            bool changes = false;
            Vector2[] points = region.points;
            // removes points which are too near from others
            for (int k = 0; k < points.Length; k++) {
                float x0 = points[k].x;
                float y0 = points[k].y;
                for (int j = k + 1; j < points.Length; j++) {
                    float x1 = points[j].x;
                    float y1 = points[j].y;
                    float distance = (x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0);
                    if (distance < 0.00000000001f) {
                        points = points.Purge<Vector2>(j);
                        j--;
                        changes = true;
                    }
                }
            }
            // remove crossing segments
            if (PolygonSanitizer.RemoveCrossingSegments(ref points)) {
                changes = true;
            }
            if (changes) {
                region.points = points;
            }
            region.sanitized = true;
            return changes;
        }

        /// <summary>
        /// Checks for the sanitized flag in regions list and invoke RegionSanitize on pending regions
        /// </summary>
        void RegionSanitize(List<Region> regions) {
            regions.ForEach((Region region) => {
                if (!region.sanitized)
                    RegionSanitize(region);
            });
        }


        /// <summary>
        /// Makes a region collapse with the neigbhours frontiers - needed when merging two adjacent regions
        /// </summary>
        void RegionMagnet(Region region, Region neighbourRegion) {

            const float tolerance = 1e-6f;
            int pointCount = region.points.Length;
            bool[] usedPoints = new bool[pointCount];
            int otherPointCount = neighbourRegion.points.Length;
            bool[] usedOtherPoints = new bool[otherPointCount];

            for (int i = 0; i < pointCount; i++) {
                float minDist = float.MaxValue;
                int selPoint = -1;
                int selOtherPoint = -1;
                for (int p = 0; p < pointCount; p++) {
                    if (usedPoints[p])
                        continue;
                    Vector2 point0 = region.points[p];
                    for (int o = 0; o < otherPointCount; o++) {
                        if (usedOtherPoints[o])
                            continue;
                        Vector2 point1 = neighbourRegion.points[o];
                        float dx = point0.x - point1.x;
                        if (dx < 0)
                            dx = -dx;
                        if (dx < tolerance) {
                            float dy = point0.y - point1.y;
                            if (dy < 0)
                                dy = -dy;
                            if (dy < tolerance) {
                                float dist = dx < dy ? dx : dy;
                                if (dist <= 0) {
                                    usedPoints[p] = true;
                                    usedOtherPoints[o] = true;
                                    break;
                                } else if (dist < minDist) {
                                    minDist = dist;
                                    selPoint = p;
                                    selOtherPoint = o;
                                }
                            }
                        }
                    }
                }
                if (selPoint >= 0) {
                    region.points[selPoint] = neighbourRegion.points[selOtherPoint];
                    region.sanitized = false;
                    usedPoints[selPoint] = true;
                    usedOtherPoints[selOtherPoint] = true;
                } else
                    break;
            }
        }


        /// <summary>
        /// Removes special characters from string.
        /// </summary>
        string DataEscape(string s) {
            s = s.Replace("$", "");
            s = s.Replace("|", "");
            return s;
        }

        #endregion

        #region World Gizmos

        void CheckCursorVisibility() {
            if (_showCursor) {
                if (cursorLayerHLine != null) {
                    bool visible = cursorLayerHLine.activeSelf;
                    if (_showTiles) {
                        if (_currentZoomLevel > TILE_MAX_CURSOR_ZOOM_LEVEL && cursorLayerHLine.activeSelf) {
                            visible = false;
                        } else if (_currentZoomLevel <= TILE_MAX_CURSOR_ZOOM_LEVEL && !cursorLayerHLine.activeSelf) {
                            visible = true;
                        }
                    }
                    if ((mouseIsOverUIElement || !mouseIsOver) && visible && !cursorAlwaysVisible) {    // not over map?
                        visible = false;
                    } else if (!mouseIsOverUIElement && mouseIsOver && !visible) {  // finally, should be visible?
                        visible = true;
                    }
                    if (cursorLayerHLine.activeSelf != visible) {
                        cursorLayerHLine.SetActive(visible);
                    }
                }
                if (cursorLayerVLine != null) {
                    bool visible = cursorLayerVLine.activeSelf;
                    if (_showTiles) {
                        if (_currentZoomLevel > TILE_MAX_CURSOR_ZOOM_LEVEL && cursorLayerVLine.activeSelf) {
                            visible = false;
                        } else if (_currentZoomLevel <= TILE_MAX_CURSOR_ZOOM_LEVEL && !cursorLayerVLine.activeSelf) {
                            visible = true;
                        }
                    }
                    if ((mouseIsOverUIElement || !mouseIsOver) && visible && !cursorAlwaysVisible) {    // not over map?
                        visible = false;
                    } else if (!mouseIsOverUIElement && mouseIsOver && !visible) {  // finally, should be visible?
                        visible = true;
                    }
                    if (cursorLayerVLine.activeSelf != visible) {
                        cursorLayerVLine.SetActive(visible);
                    }
                }
            }
        }


        void DrawCursor() {

            if (!_showCursor)
                return;

            // Generate line V **********************
            Vector3[] points = new Vector3[2];
            int[] indices = new int[2];
            indices[0] = 0;
            indices[1] = 1;
            points[0] = Misc.Vector3up * -0.5f;
            points[1] = Misc.Vector3up * 0.5f;

            Transform t = transform.Find("CursorV");
            if (t != null)
                DestroyImmediate(t.gameObject);
            cursorLayerVLine = new GameObject("CursorV");
            if (disposalManager != null)
                disposalManager.MarkForDisposal(cursorLayerVLine); //.hideFlags = HideFlags.DontSave;
            cursorLayerVLine.transform.SetParent(transform, false);
            cursorLayerVLine.transform.localPosition = Misc.Vector3back * 0.00001f; // needed for minimap
            cursorLayerVLine.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
            cursorLayerVLine.layer = gameObject.layer;
            cursorLayerVLine.SetActive(_showCursor);

            Mesh mesh = new Mesh();
            mesh.vertices = points;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            if (disposalManager != null)
                disposalManager.MarkForDisposal(mesh); //.hideFlags = HideFlags.DontSave;

            MeshFilter mf = cursorLayerVLine.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            MeshRenderer mr = cursorLayerVLine.AddComponent<MeshRenderer>();
            mr.receiveShadows = false;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //			mr.useLightProbes = false;
            mr.sharedMaterial = cursorMatV;


            // Generate line H **********************
            points[0] = Misc.Vector3right * -0.5f;
            points[1] = Misc.Vector3right * 0.5f;

            t = transform.Find("CursorH");
            if (t != null)
                DestroyImmediate(t.gameObject);
            cursorLayerHLine = new GameObject("CursorH");
            if (disposalManager != null)
                disposalManager.MarkForDisposal(cursorLayerHLine); //.hideFlags = HideFlags.DontSave;
            cursorLayerHLine.transform.SetParent(transform, false);
            cursorLayerHLine.transform.localPosition = Misc.Vector3back * 0.00001f; // needed for minimap
            cursorLayerHLine.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
            cursorLayerHLine.layer = gameObject.layer;
            cursorLayerHLine.SetActive(_showCursor);

            mesh = new Mesh();
            mesh.vertices = points;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            if (disposalManager != null)
                disposalManager.MarkForDisposal(mesh); //.hideFlags = HideFlags.DontSave;

            mf = cursorLayerHLine.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            mr = cursorLayerHLine.AddComponent<MeshRenderer>();
            mr.receiveShadows = false;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //			mr.useLightProbes = false;
            mr.sharedMaterial = cursorMatH;


        }

        void DrawImaginaryLines() {
            DrawLatitudeLines();
            DrawLongitudeLines();
        }

        void DrawLatitudeLines() {
            if (!_showLatitudeLines)
                return;

            // Generate latitude lines
            List<Vector3> points = new List<Vector3>();
            List<int> indices = new List<int>();
            float r = 0.5f;
            int idx = -1;

            for (float a = 0; a < 90; a += _latitudeStepping) {
                for (int h = 1; h >= -1; h--) {
                    if (h == 0)
                        continue;
                    float y = h * a / 90.0f * r;
                    points.Add(new Vector3(-r, y, 0));
                    points.Add(new Vector3(r, y, 0));
                    indices.Add(++idx);
                    indices.Add(++idx);
                    if (a == 0)
                        break;
                }
            }

            Transform t = transform.Find("LatitudeLines");
            if (t != null)
                DestroyImmediate(t.gameObject);
            latitudeLayer = new GameObject("LatitudeLines");
            if (disposalManager != null)
                disposalManager.MarkForDisposal(latitudeLayer); //.hideFlags = HideFlags.DontSave;
            latitudeLayer.transform.SetParent(transform, false);
            latitudeLayer.transform.localPosition = Misc.Vector3zero;
            latitudeLayer.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
            latitudeLayer.layer = gameObject.layer;
            latitudeLayer.SetActive(_showLatitudeLines);

            Mesh mesh = new Mesh();
            mesh.vertices = points.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            if (disposalManager != null)
                disposalManager.MarkForDisposal(mesh); //.hideFlags = HideFlags.DontSave;

            MeshFilter mf = latitudeLayer.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            MeshRenderer mr = latitudeLayer.AddComponent<MeshRenderer>();
            mr.receiveShadows = false;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //			mr.useLightProbes = false;
            mr.sharedMaterial = imaginaryLinesMat;

        }

        void DrawLongitudeLines() {
            if (!_showLongitudeLines)
                return;

            // Generate longitude lines
            List<Vector3> points = new List<Vector3>();
            List<int> indices = new List<int>();
            float r = 0.5f;
            int idx = -1;
            int step = 180 / _longitudeStepping;

            for (float a = 0; a < 90; a += step) {
                for (int h = 1; h >= -1; h--) {
                    if (h == 0)
                        continue;
                    float x = h * a / 90.0f * r;
                    points.Add(new Vector3(x, -r, 0));
                    points.Add(new Vector3(x, r, 0));
                    indices.Add(++idx);
                    indices.Add(++idx);
                    if (a == 0)
                        break;
                }
            }


            Transform t = transform.Find("LongitudeLines");
            if (t != null)
                DestroyImmediate(t.gameObject);
            longitudeLayer = new GameObject("LongitudeLines");
            if (disposalManager != null)
                disposalManager.MarkForDisposal(longitudeLayer); //.hideFlags = HideFlags.DontSave;
            longitudeLayer.transform.SetParent(transform, false);
            longitudeLayer.transform.localPosition = Misc.Vector3zero;
            longitudeLayer.transform.localRotation = Quaternion.Euler(Misc.Vector3zero);
            longitudeLayer.layer = gameObject.layer;
            longitudeLayer.SetActive(_showLongitudeLines);

            Mesh mesh = new Mesh();
            mesh.vertices = points.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            if (disposalManager != null)
                disposalManager.MarkForDisposal(mesh); //.hideFlags = HideFlags.DontSave;

            MeshFilter mf = longitudeLayer.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            MeshRenderer mr = longitudeLayer.AddComponent<MeshRenderer>();
            mr.receiveShadows = false;
            mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.sharedMaterial = imaginaryLinesMat;

        }

        #endregion

        #region Overlay

        public GameObject CreateOverlay() {

            // 2D labels layer
            Transform t = transform.Find(OVERLAY_BASE);
            if (t == null) {
                overlayLayer = new GameObject(OVERLAY_BASE);
                if (disposalManager != null)
                    disposalManager.MarkForDisposal(overlayLayer); //.hideFlags = HideFlags.DontSave;
                overlayLayer.transform.SetParent(transform, false);
                overlayLayer.transform.localPosition = Misc.Vector3back * 0.002f;
                overlayLayer.transform.localScale = Misc.Vector3one;
                overlayLayer.layer = gameObject.layer;
            } else {
                overlayLayer = t.gameObject;
                overlayLayer.SetActive(true);
            }
            return overlayLayer;
        }

        void UpdateScenicPlusDistance() {
            if (earthMat == null)
                return;
            float zoomLevel = GetZoomLevel();
            earthMat.SetFloat("_Distance", zoomLevel);
        }

        #endregion

        #region Markers support

        void CheckMarkersLayer() {
            if (markersLayer == null) { // try to capture an existing marker layer
                Transform t = transform.Find("Markers");
                if (t != null)
                    markersLayer = t.gameObject;
            }
            if (markersLayer == null) { // create it otherwise
                markersLayer = new GameObject("Markers");
                markersLayer.transform.SetParent(transform, false);
                markersLayer.layer = transform.gameObject.layer;
            }
        }


        #endregion

        #region Global Events handling

        internal void BubbleEvent<T>(Action<T> a, T o) {
            if (a != null)
                a(o);
        }


        #endregion



    }

}