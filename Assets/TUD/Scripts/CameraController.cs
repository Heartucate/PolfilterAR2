// CameraController.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System.Collections;
using UnityEngine;

namespace Heartucate.TUD
{
    public class CameraController : MonoBehaviour
    {
        [Header("Rotation Einstellungen")]
        [SerializeField] private float degreeValue = 15f;

        [Header("Lerp Einstellungen")]
        [SerializeField] private float lerpTime = 0.3f;

        private Coroutine rotateRoutine;

        private float targetYAngle;

        private void Start()
        {
            targetYAngle = transform.eulerAngles.y;
        }

        public void IncreaseRot()
        {
            SetTargetAngle(targetYAngle + degreeValue);
        }

        public void DecreaseRot()
        {
            SetTargetAngle(targetYAngle - degreeValue);
        }

        private void SetTargetAngle(float newAngle)
        {
            targetYAngle = newAngle;

            if (rotateRoutine != null)
                StopCoroutine(rotateRoutine);

            rotateRoutine = StartCoroutine(LerpToTarget());
        }

        private IEnumerator LerpToTarget()
        {
            Quaternion startRot = transform.rotation;
            Quaternion endRot = Quaternion.Euler(0f, targetYAngle, 0f);

            float time = 0f;

            while (time < lerpTime)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / lerpTime);

                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }

            transform.rotation = endRot;
            rotateRoutine = null;
        }
    }
}