// AutoFocusTouchZone.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Heartucate.TUD
{
    public class AutoFocusTouchZone : MonoBehaviour
    {
        [SerializeField] private ARCameraManager cameraManager;

        // Size of the touch zone (1/10 of screen width & height)
        private float zoneWidth;
        private float zoneHeight;

        void Start()
        {
            zoneWidth = Screen.width * 0.1f;
            zoneHeight = Screen.height * 0.1f;

            Debug.Log($"[AutoFocusTouchZone] Bottom-left touch zone: {zoneWidth} x {zoneHeight}");
        }

        void Update()
        {
#if UNITY_EDITOR
            // Mouse click for testing in editor
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(Input.mousePosition);
            }
#else
        // Real touch on device
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTouch(Input.GetTouch(0).position);
        }
#endif
        }

        void HandleTouch(Vector2 pos)
        {
            Debug.Log($"[AutoFocusTouchZone] Touch at: {pos}");

            // Bottom-left zone check
            bool inX = pos.x <= zoneWidth;
            bool inY = pos.y <= zoneHeight;

            if (inX && inY)
            {
                Debug.Log("[AutoFocusTouchZone] Touch in focus zone ? Nudging autofocus");
                NudgeAutoFocus();
            }
        }

        void NudgeAutoFocus()
        {
            if (cameraManager == null)
            {
                Debug.LogWarning("[AutoFocusTouchZone] No ARCameraManager assigned!");
                return;
            }

            Debug.Log("[AutoFocusTouchZone] Turning autofocus OFF");
            cameraManager.autoFocusRequested = false;

            Debug.Log("[AutoFocusTouchZone] Turning autofocus ON");
            cameraManager.autoFocusRequested = true;
        }
    }
}