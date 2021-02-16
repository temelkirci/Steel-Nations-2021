// World Map Strategy Kit for Unity - Main Script
// (C) 2016-2020 by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        // viewport game objects
        Dictionary<int, GameObjectAnimator> vgosDict;
        GameObjectAnimator[] vgos;
        int vgosCount;
        bool vgosArrayIsDirty;

        // Water effects
        float buoyancyCurrentAngle;

        void SetupVGOs() {
            if (vgos == null) {
                vgosDict = new Dictionary<int, GameObjectAnimator>();
            }
            if (vgos == null || vgos.Length < vgosCount) {
                vgos = new GameObjectAnimator[vgosCount > 100 ? vgosCount : 100];
            }
        }


        void CheckVGOsArrayDirty() {
            if (vgos == null || vgos.Length < vgosCount) {
                vgos = new GameObjectAnimator[vgosCount];
            }
            if (!vgosArrayIsDirty) return;
            for (int k = 0; k < vgosCount; k++) {
                if (vgos[k] == null) {
                    vgosCount--;
                    Array.Copy(vgos, k + 1, vgos, k, vgosCount);
                }
            }
            vgosArrayIsDirty = false;
        }

        void FixedUpdateViewportObjectsLoop() {
            // Update animators
            CheckVGOsArrayDirty();
            for (int k = 0; k < vgosCount; k++) {
                GameObjectAnimator vgo = vgos[k];
                if (vgo.isMoving || (vgo.lastKnownPosIsOnWater && vgo.enableBuoyancyEffect)) {
                    vgo.PerformUpdateLoop();
                }
            }
        }


        void UpdateViewportObjectsVisibility() {
            // Update animators
            CheckVGOsArrayDirty();
            for (int k = 0; k < vgosCount; k++) {
                GameObjectAnimator vgo = vgos[k];
                vgo.UpdateTransformAndVisibility();
            }
        }

        void RepositionViewportObjects() {
            if (renderViewportIsEnabled) {
                for (int k = 0; k < vgosCount; k++) {
                    GameObjectAnimator go = vgos[k];
                    go.transform.SetParent(null, true);
                    go.UpdateTransformAndVisibility(true);
                }
            } else {
                for (int k = 0; k < vgosCount; k++) {
                    GameObjectAnimator go = vgos[k];
                    go.transform.localScale = go.originalScale;
                    go.UpdateTransformAndVisibility(true);
                }
            }
        }

        void UpdateViewportObjectsBuoyancy() {
            buoyancyCurrentAngle = Mathf.Sin(time) * VGOBuoyancyAmplitude * Mathf.Rad2Deg;
        }



    }
}
