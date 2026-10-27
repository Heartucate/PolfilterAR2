// MaxResolutionCameraConfig.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Heartucate.TUD
{
    [RequireComponent(typeof(ARCameraManager))]
    public class MaxResolutionCameraConfig : MonoBehaviour
    {
        ARCameraManager cam;

        void Awake() => cam = GetComponent<ARCameraManager>();

        void OnEnable()
        {
            // Wait a frame so the subsystem is up and has configs
            StartCoroutine(ApplyMaxConfigNextFrame());
        }

        System.Collections.IEnumerator ApplyMaxConfigNextFrame()
        {
            yield return null;

            if (cam.subsystem == null)
                yield break;

            using var configs = cam.GetConfigurations(Allocator.Temp);
            if (!configs.IsCreated || configs.Length == 0)
                yield break;

            XRCameraConfiguration best = configs[0];
            int bestArea = best.width * best.height;

            for (int i = 1; i < configs.Length; i++)
            {
                var c = configs[i];
                int area = c.width * c.height;

                // Prefer larger resolution; as a tie-breaker prefer higher FPS if available.
                bool better =
                    area > bestArea ||
                    (area == bestArea && c.framerate.HasValue && best.framerate.HasValue && c.framerate.Value > best.framerate.Value);

                if (better)
                {
                    best = c;
                    bestArea = area;
                }
            }

            cam.currentConfiguration = best; // this drives camera background quality
            Debug.Log($"Selected camera config: {best.width}x{best.height} @ {best.framerate?.ToString() ?? "?"}fps");
        }
    }
}