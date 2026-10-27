// ObjectButtonTouch.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem.XR;

namespace Heartucate.TUD
{
    public class ObjectButtonTouch : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private OneShotPlayer _oneShotPlayer;

        [Header("Events")]
        public UnityEvent onClick;

        [Header("Animation Einstellungen")]
        public AnimationCurve fresnelCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 3f);
        public float duration = 1.5f;

        [Header("Hold Einstellungen")]
        [SerializeField] private float startDelay = 0.4f;
        [SerializeField] private float minDelay = 0.05f;
        [SerializeField] private float acceleration = 0.85f;


        private Renderer rend;
        private MaterialPropertyBlock mpb;
        private Coroutine animRoutine;

        private bool isHolding = false;
        private Coroutine holdRoutine;

        private void Start()
        {
            if (GameState.CurrentMode != GameState.GameMode.SIMULATION)
            {
                targetCamera = FindObjectOfType<TrackedPoseDriver>().GetComponent<Camera>();
            }
        }

        public void SetShaderColor(Color col)
        {
            if (rend == null)
                rend = GetComponent<Renderer>();

            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            rend.GetPropertyBlock(mpb);

            mpb.SetColor("_BaseColor", col);

            rend.SetPropertyBlock(mpb);
        }

        void Update()
        {
            // ---- Maus ----
            if (Input.GetMouseButtonDown(0))
                TryStartHold(Input.mousePosition);

            if (Input.GetMouseButtonUp(0))
                StopHold();

            // ---- Touch ----
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                    TryStartHold(touch.position);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    StopHold();
            }
        }

        void TryStartHold(Vector3 screenPosition)
        {
            if (targetCamera == null)
            {
                Debug.LogError("Keine Kamera zugewiesen!");
                return;
            }

            Ray ray = targetCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                StartHold();
            }
        }

        void StartHold()
        {
            _oneShotPlayer.PlayOS();
            if (isHolding)
                return;

            isHolding = true;
            holdRoutine = StartCoroutine(HoldRoutine());
        }

        void StopHold()
        {
            if (!isHolding)
                return;

            isHolding = false;

            if (holdRoutine != null)
                StopCoroutine(holdRoutine);

            holdRoutine = null;
        }

        private IEnumerator HoldRoutine()
        {
            float currentDelay = startDelay;

            while (isHolding)
            {
                OnClickAction();

                yield return new WaitForSeconds(currentDelay);

                // Beschleunigung
                currentDelay = Mathf.Max(minDelay, currentDelay * acceleration);
            }
        }

        public void PlayFresnelAnimation()
        {
            if (animRoutine != null)
                StopCoroutine(animRoutine);

            if (gameObject.activeInHierarchy)
                animRoutine = StartCoroutine(AnimateFresnel());
        }

        private IEnumerator AnimateFresnel()
        {
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);

                float fresnelPower = fresnelCurve.Evaluate(t);

                rend.GetPropertyBlock(mpb);
                mpb.SetFloat("_FresnelPower", fresnelPower);
                rend.SetPropertyBlock(mpb);

                yield return null;
            }

            animRoutine = null;
        }

        void OnClickAction()
        {
            onClick?.Invoke();
            PlayFresnelAnimation();
        }
    }
}