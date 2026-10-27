// FaceCamera.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;

namespace Heartucate.TUD
{
    public class FaceCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform _targetCameraTransform;
        [SerializeField] private bool _fixY = true;
        [SerializeField] private bool _useLerp = false;
        [SerializeField] private float _lerpSpeed = 5f;

        private void Start()
        {
            if (GameState.CurrentMode != GameState.GameMode.SIMULATION)
            {
                if (TrackingSystemManager.ArCamera != null)
                {
                    _targetCameraTransform = TrackingSystemManager.ArCamera.transform;
                }
            }
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = _targetCameraTransform.position;

            if (_fixY)
            {
                targetPosition.y = transform.position.y;
            }

            Vector3 direction = targetPosition - transform.position;

            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            if (_useLerp)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lerpSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
    }
}