using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Heartucate.Core.Presentation;
using Heartucate.TUD;

namespace Heartucate.Tracking
{
    public class CalibrationManager : MonoBehaviour
    {
        public static CalibrationManager instance;
        public static bool trackingResetHintWasShown = false;

        // Delegate invoked when the active tracked image changes. Parameter is the tracked image's reference name.
        public static Action<string> OnTrackedImageActiveChanged;

        public bool calibrated;
        public bool allowCalibration;
        public bool showTrackingSuccessPopup;
        public float thresholdAngle = 5f;
        public LayerMask calibrationLayers;
        public LayerMask allLayers;

        [SerializeField] private ARSession _arSession;
        [SerializeField] private Camera _camera;
        [SerializeField] private ARTrackedImageManager _trackedImageManager; // assign in inspector or found at runtime
        [SerializeField] private Transform calibrationProxy; // optional: world object that marks desired calibration center/orientation

        [Header("Recalibration settings")]
        [Tooltip("If the calibration target moves more than this distance (meters) after calibration, re-run calibration")]
        [SerializeField] private float recalibrationPositionThreshold = 0.005f; // 0.5 cm
        [Tooltip("If the calibration target rotates more than this (degrees) after calibration, re-run calibration")]
        [SerializeField] private float recalibrationRotationThresholdDeg = 0.5f;

        // Stability requirements: how similar pose must be across time to be considered stable
        [Tooltip("How close position must be (meters) between samples to count as stable")]
        [SerializeField] private float stabilityPositionTolerance = 0.01f; // 1 cm
        [Tooltip("How close yaw (degrees) must be between samples to count as stable")]
        [SerializeField] private float stabilityRotationToleranceDeg = 3f;
        [Tooltip("Duration in seconds the tracked image must remain stable before we accept it for (re)calibration")]
        [SerializeField] private float requiredStableDuration = 0.15f;

        [Header("Smoothing")]
        [Tooltip("Duration in seconds to smoothly interpolate the origin to the calibrated pose")]
        [SerializeField] private float smoothingDuration = 0.15f;

        private bool _calibrationInProgress;
        private Coroutine _applyRoutine;

        // Track last calibrated target pose so we can detect movement and re-calibrate
        private Vector3 _lastCalibratedTargetPos = Vector3.zero;
        private float _lastCalibratedTargetYaw = 0f;

        // Stability tracking state for the currently observed tracked image
        private ARTrackedImage _stableTrackedImage = null;
        private Vector3 _stableReferencePos = Vector3.zero;
        private float _stableReferenceYaw = 0f;
        private float _stableStartTime = 0f;

        void Start()
        {
            if (_camera == null)
                _camera = Camera.main;

            if (_camera != null)
                _camera.cullingMask = calibrationLayers;

            allowCalibration = true;
            calibrated = false;

            instance = this;

            // Try to find a tracked image manager if none assigned
            if (_trackedImageManager == null)
                _trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            if (_trackedImageManager != null)
                _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        private void OnDisable()
        {
            if (_trackedImageManager != null)
                _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                CalibrateAsync(true);

            // Only auto-calibrate once when upright
            if (allowCalibration && !calibrated && IsUpright())
            {
                var cal = CalibrationTarget.instance;
                if (cal != null)
                {
                    var trackedImage = cal.GetComponentInParent<ARTrackedImage>();
                    if (trackedImage != null && trackedImage.trackingState == TrackingState.Tracking)
                    {
                        CalibrateAsync(false);
                    }
                }
            }
        }

        async void CalibrateAsync(bool force)
        {
            if (_calibrationInProgress)
                return;

            _calibrationInProgress = true;
            if (!trackingResetHintWasShown && MenuPopupManager.instance != null)
            {
                if (showTrackingSuccessPopup)
                    await MenuPopupManager.instance.ShowPopup(TrackingResetHintPopupData());

                trackingResetHintWasShown = true;
            }

            if (!force)
            {
                var cal = CalibrationTarget.instance;
                if (cal != null)
                {
                    // Compute calibration by solving for the new origin transform T' so that:
                    // T' * calLocal = desiredWorld
                    // Where calLocal is the calibration target's pose relative to the current origin (this.transform).

                    // Get cal pose relative to current origin (local to this.transform)
                    Vector3 calLocalPos = Quaternion.Inverse(transform.rotation) * (cal.transform.position - transform.position);
                    Quaternion calLocalRot = Quaternion.Inverse(transform.rotation) * cal.transform.rotation;

                    // Desired world pose for the calibration target
                    Vector3 desiredWorldPos = (calibrationProxy != null) ? calibrationProxy.position : Vector3.zero;
                    Quaternion desiredWorldRot = (calibrationProxy != null) ? Quaternion.Euler(0f, calibrationProxy.eulerAngles.y, 0f) : Quaternion.identity;

                    // Solve for new origin rotation and position
                    Quaternion newOriginRot = desiredWorldRot * Quaternion.Inverse(calLocalRot);
                    Vector3 newOriginPos = desiredWorldPos - newOriginRot * calLocalPos;

                    // Determine magnitude of correction (relative to current origin)
                    float posMag = Vector3.Distance(transform.position, newOriginPos);
                    float rotAngle = Quaternion.Angle(transform.rotation, newOriginRot);

                    // Tolerances to ignore tiny jitter
                    const float minTranslationThreshold = 0.0005f; // 0.5 mm low-level noise guard
                    const float minRotationThreshold = 0.05f; // degrees

                    if (posMag < minTranslationThreshold && rotAngle < minRotationThreshold)
                    {
                        // Skip applying micro-noise corrections
                        Debug.Log($"Calibration skipped micro-noise: posMag={posMag:F6} rotAngle={rotAngle:F4}");
                    }
                    else
                    {
                        // If the correction is very small but above micro-noise, still smooth gently.
                        // Stop previous apply routine if running
                        if (_applyRoutine != null)
                            StopCoroutine(_applyRoutine);

                        _applyRoutine = StartCoroutine(ApplyCalibrationSmooth(newOriginPos, newOriginRot, smoothingDuration));

                        Debug.Log($"Calibration starting smooth apply: posMag={posMag:F4}, rotAngle={rotAngle:F3}, duration={smoothingDuration:F3}");
                    }

                    // Record the calibrated target pose so we can detect future movement
                    _lastCalibratedTargetPos = cal.transform.position;
                    _lastCalibratedTargetYaw = cal.transform.eulerAngles.y;
                }
            }

            if (_camera == null)
                _camera = Camera.main;

            if (_camera != null)
            {
                _camera.cullingMask = allLayers;
                Debug.Log($"Calibration: set camera cullingMask to { _camera.cullingMask } (allLayers={(int)allLayers})");
            }

            calibrated = true;
            _calibrationInProgress = false;
        }

        private System.Collections.IEnumerator ApplyCalibrationSmooth(Vector3 targetPos, Quaternion targetRot, float duration)
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            if (duration <= 0f)
            {
                transform.position = targetPos;
                transform.rotation = targetRot;
                _applyRoutine = null;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float ease = Mathf.SmoothStep(0f, 1f, t);
                transform.position = Vector3.Lerp(startPos, targetPos, ease);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, ease);
                yield return null;
            }

            transform.position = targetPos;
            transform.rotation = targetRot;
            _applyRoutine = null;
        }

        bool IsUpright()
        {
            if (CalibrationTarget.instance == null)
                return false;

            float dot = Vector3.Dot(Vector3.up, CalibrationTarget.instance.transform.up);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            return allowCalibration && angle < thresholdAngle;
        }

        public void ResetSession()
        {
            if (!enabled)
                return;

            //THIS MIGHT BE NEEDED
            //if (CalibrationTarget.instance != null)
            //    Destroy(CalibrationTarget.instance.gameObject);

            if (_arSession != null)
                _arSession.Reset();

            Start();
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            // Handle newly added images (only consider those that are actively tracked)
            foreach (var trackedImage in eventArgs.added)
            {
                if (trackedImage != null && trackedImage.trackingState == TrackingState.Tracking)
                    TryAssignCalibrationTargetFromTrackedImage(trackedImage);
            }

            // Handle updated images (only consider those that are actively tracked)
            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage != null && trackedImage.trackingState == TrackingState.Tracking)
                    TryAssignCalibrationTargetFromTrackedImage(trackedImage);
            }

            // Optionally handle removed images: if the removed image was the current calibration target, clear it
            foreach (var trackedImage in eventArgs.removed)
            {
                var cal = trackedImage.GetComponentInChildren<CalibrationTarget>();
                if (cal != null && CalibrationTarget.instance == cal)
                {
                    CalibrationTarget.instance = null;
                    OnTrackedImageActiveChanged?.Invoke(string.Empty);
                }
            }
        }

        private void TryAssignCalibrationTargetFromTrackedImage(ARTrackedImage trackedImage)
        {
            if (trackedImage == null)
                return;

            // Only proceed when the image is actively tracked (in view). Ignore Limited/None states to avoid jitter/recalibrate when out of view.
            if (trackedImage.trackingState != TrackingState.Tracking)
                return;

            // Get image name (if available) and ensure it matches current game state
            string imageName = trackedImage.referenceImage != null ? trackedImage.referenceImage.name : string.Empty;
            if (!DoesImageMatchCurrentGameState(imageName))
                return;

            // Look for a CalibrationTarget component on the spawned content for this tracked image
            var cal = trackedImage.GetComponentInChildren<CalibrationTarget>();
            if (cal != null)
            {
                // Make this the current instance so CalibrationManager can use it
                CalibrationTarget.instance = cal;

                // Notify listeners which image is active now (keep immediate for selection flow)
                string imageNameNotify = imageName;
                OnTrackedImageActiveChanged?.Invoke(imageNameNotify);

                // ----- Stability-based (re)calibration logic -----
                // We require the tracked image pose to remain approximately stable over requiredStableDuration
                // (within stabilityPositionTolerance and stabilityRotationToleranceDeg) before we perform calibration
                // or trigger a recalibration after the image has moved since last calibration.

                Vector3 currentPos = cal.transform.position;
                float currentYaw = cal.transform.eulerAngles.y;

                // If we started observing a different tracked image, reset stability tracking
                if (_stableTrackedImage != trackedImage)
                {
                    _stableTrackedImage = trackedImage;
                    _stableReferencePos = currentPos;
                    _stableReferenceYaw = currentYaw;
                    _stableStartTime = Time.time;
                }
                else
                {
                    float posDelta = Vector3.Distance(_stableReferencePos, currentPos);
                    float yawDelta = Mathf.Abs(Mathf.DeltaAngle(_stableReferenceYaw, currentYaw));

                    if (posDelta <= stabilityPositionTolerance && yawDelta <= stabilityRotationToleranceDeg)
                    {
                        // Pose remains within tolerances — check if it has been stable long enough
                        if (Time.time - _stableStartTime >= requiredStableDuration)
                        {
                            // Confirmed stable
                            // If not yet calibrated, and conditions permit, perform calibration
                            if (!calibrated && allowCalibration && IsUpright())
                            {
                                // Reset stability tracking to avoid duplicate triggers
                                _stableTrackedImage = null;
                                _stableStartTime = 0f;

                                CalibrateAsync(false);
                                return;
                            }

                            // If already calibrated, check whether the target has moved relative to the last calibrated pose
                            if (calibrated)
                            {
                                float movedDistSinceCal = Vector3.Distance(_lastCalibratedTargetPos, currentPos);
                                float yawDiffSinceCal = Mathf.Abs(Mathf.DeltaAngle(_lastCalibratedTargetYaw, currentYaw));

                                if (movedDistSinceCal > recalibrationPositionThreshold || yawDiffSinceCal > recalibrationRotationThresholdDeg)
                                {
                                    // Reset stability tracking to avoid duplicate triggers
                                    _stableTrackedImage = null;
                                    _stableStartTime = 0f;

                                    // Trigger recalibration (CalibrateAsync will set calibrated back to true once complete)
                                    calibrated = false;
                                    CalibrateAsync(false);
                                    return;
                                }
                            }

                            // If stable but no action required, keep monitoring but do not constantly retrigger.
                            // Update reference to current pose so further small drifts restart timing if needed.
                            _stableReferencePos = currentPos;
                            _stableReferenceYaw = currentYaw;
                            _stableStartTime = Time.time;
                        }
                        // else: still within tolerances but not yet long enough — wait
                    }
                    else
                    {
                        // Pose deviated beyond tolerances — restart stability window using current pose
                        _stableReferencePos = currentPos;
                        _stableReferenceYaw = currentYaw;
                        _stableStartTime = Time.time;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true when the tracked image's name matches the currently selected GameState.
        /// If GameState.CurrentMode == NONE we accept any image (keeps selection flow working).
        /// Mapping: 'H' => HYBRID, 'A' => FULLAR, 'S' => SIMULATION.
        /// </summary>
        private bool DoesImageMatchCurrentGameState(string imageName)
        {
            // Accept any image while no game mode is selected (selection phase)
            if (GameState.CurrentMode == GameState.GameMode.NONE)
                return true;

            if (string.IsNullOrEmpty(imageName))
                return false;

            char key = char.ToUpperInvariant(imageName[0]);
            switch (key)
            {
                case 'H':
                    return GameState.CurrentMode == GameState.GameMode.HYBRID;
                case 'A':
                    return GameState.CurrentMode == GameState.GameMode.FULLAR;
                case 'S':
                    return GameState.CurrentMode == GameState.GameMode.SIMULATION;
                default:
                    return false;
            }
        }

        private PopupData TrackingResetHintPopupData() =>
           new PopupData("Super, das hat geklappt! Bei Tracking-Problemen: Tablet auf den Tracker halten oder Menü \u2192 Tracking Reset nutzen.", new List<ButtonData>
           {
                new ButtonData("Verstanden", ButtonData.ButtonStyle.Action, null, true)
           }, iconId: "Check");
    }
}