// DegreeAdjuster.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Heartucate.TUD
{
    public class DegreeAdjuster : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject _adjusterUI;
        [SerializeField] private GameObject _degreeUI;
        [SerializeField] private List<GameObject> _manualControlArrows;
        [SerializeField] private TextMeshProUGUI _degreeDisplay;

        [Header("Degree")]
        [SerializeField] private int _startDegree = 0;
        [SerializeField] private bool _use360deg;
        [SerializeField] private int[] _polDegrees = { 0, 1 };

        [Header("Shake")]
        [SerializeField] private AnimationCurve _shakeCurve;
        [SerializeField] private float _shakeDuration = 0.3f;
        [SerializeField] private float _shakeMagnitude = 5f;
        [SerializeField] private float _scaleMagnitude = 5f;

        [Header("Rotation")]
        [SerializeField] private Transform _uiPos;
        [SerializeField] private GameObject[] _possibleFilter;
        [SerializeField] private GameObject _arrowTransform;
        [SerializeField] private float rotationDuration = 0.5f;

        [Header("Data")]
        [SerializeField] private PolarisationData _polarisationData;
        [SerializeField] private int _id = 0;

        [Header("Visuals")]
        [ColorUsage(true, true)]
        [SerializeField] private Color _shaderColor;
        [SerializeField] private ObjectButtonTouch _button1;
        [SerializeField] private ObjectButtonTouch _button2;
        [SerializeField] private ShaderColorChanger _arrow;

        private int _actDegree;
        private int _currentIndex;

        private Vector3 _originalPosition;
        private Vector3 _originalScale;

        private Coroutine _shakeCoroutine;
        private Coroutine _rotateCoroutine;

        // -------------------- LIFECYCLE --------------------

        private void Awake()
        {
            if (_use360deg)
            {
                _polDegrees = new int[360];
                for (int i = 0; i < 360; i++)
                    _polDegrees[i] = i;
            }

            _button1.SetShaderColor(_shaderColor);
            _button2.SetShaderColor(_shaderColor);
            _arrow.SetShaderColor(_shaderColor);
        }

        private void OnEnable()
        {
            // UI kann sich beim Reaktivieren geändert haben  neu speichern
            _originalPosition = _degreeDisplay.transform.localPosition;
            _originalScale = _degreeDisplay.transform.localScale;

            // Safety Reset
            _degreeDisplay.transform.localScale = _originalScale;
            _degreeDisplay.transform.localPosition = _originalPosition;
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            if (_degreeDisplay != null)
            {
                _degreeDisplay.transform.localScale = _originalScale;
                _degreeDisplay.transform.localPosition = _originalPosition;
            }
        }

        private void Start()
        {
            _currentIndex = Mathf.Clamp(_startDegree, 0, _polDegrees.Length - 1);
            ApplyDegree();

            if (GameState.CurrentMode == GameState.GameMode.HYBRID)
            {
                SetUIVisibility(true);
                SetButtonVisibility(false);
            }
            else
            {
                SetUIVisibility(true);
                SetButtonVisibility(true);
                SetDegree(0, false);
            }
        }

        // -------------------- DEGREE --------------------

        public void IncreaseDegree()
        {
            _currentIndex = (_currentIndex + 1) % _polDegrees.Length;
            ApplyDegree();
        }

        public void DecreaseDegree()
        {
            _currentIndex = (_currentIndex - 1 + _polDegrees.Length) % _polDegrees.Length;
            ApplyDegree();
        }

        public void ApplyDegree()
        {
            // If currentIndex is valid, use the mapped degree from _polDegrees.
            // Otherwise use _actDegree which may have been set directly (e.g. via SetDegree from incoming text).
            if (_currentIndex >= 0 && _currentIndex < _polDegrees.Length)
                _actDegree = _polDegrees[_currentIndex];

            _degreeDisplay.text = _actDegree + "°";

            StartShake();
            StartRotation(_actDegree + 24f, 0);

            _polarisationData.SetPolDegree(_id, _actDegree);
        }

        public void SetDegree(float newDeg, bool changedByMicrocontroller)
        {
            // Degree was changed using microcontroller - deactivating arrows
            if (changedByMicrocontroller)
                _manualControlArrows.ForEach(x => x.SetActive(false));

            // Set degree coming from external source (e.g. RealtimeDataProcessor).
            _actDegree = Mathf.RoundToInt(newDeg);
            if (_actDegree > 359)
                _actDegree = 0;

            int idx = System.Array.IndexOf(_polDegrees, _actDegree);
            _currentIndex = idx >= 0 ? idx : -1;

            // Apply visuals and data update consistently via ApplyDegree
            ApplyDegree();
        }

        // -------------------- SHAKE --------------------

        private void StartShake()
        {
            if (_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);

            if (gameObject.activeInHierarchy)
                _shakeCoroutine = StartCoroutine(ShakeText());
        }

        private IEnumerator ShakeText()
        {
            float elapsed = 0f;

            while (elapsed < _shakeDuration)
            {
                float t = elapsed / _shakeDuration;
                float curveValue = _shakeCurve.Evaluate(t);

                float offsetX = Random.Range(-1f, 1f) * _shakeMagnitude * curveValue;
                float offsetY = Random.Range(-1f, 1f) * _shakeMagnitude * curveValue;

                _degreeDisplay.transform.localPosition =
                    _originalPosition + new Vector3(offsetX, offsetY, 0f);

                float scaleFactor = 1f + _scaleMagnitude * curveValue;
                scaleFactor = Mathf.Max(0.1f, scaleFactor);

                _degreeDisplay.transform.localScale = _originalScale * scaleFactor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            _degreeDisplay.transform.localPosition = _originalPosition;
            _degreeDisplay.transform.localScale = _originalScale;
        }

        // -------------------- ROTATION --------------------

        private void StartRotation(float degree, int axis)
        {
            if (_rotateCoroutine != null)
                StopCoroutine(_rotateCoroutine);

            if (gameObject.activeInHierarchy)
            {
                _rotateCoroutine = StartCoroutine(
                    RotateCoroutine(GetClosestObject(_uiPos.position), degree, axis)
                );
            }
        }

        private IEnumerator RotateCoroutine(GameObject target, float targetDegree, int axis)
        {
            if (target == null) yield break;

            Quaternion startRotation = target.transform.localRotation;
            Quaternion arrowStartRotation = _arrowTransform.transform.localRotation;
            Quaternion endRotation = axis switch
            {
                0 => Quaternion.Euler(targetDegree, 0f, 0f),
                1 => Quaternion.Euler(0f, targetDegree, 0f),
                2 => Quaternion.Euler(0f, 0f, targetDegree),
                _ => startRotation
            };

            Quaternion arrowEndRotation = axis switch
            {
                0 => Quaternion.Euler(-targetDegree + 48, 0f, 0f),
                1 => Quaternion.Euler(0f, -targetDegree + 48, 0f),
                2 => Quaternion.Euler(0f, 0f, -targetDegree + 48),
                _ => arrowStartRotation
            };

            float elapsed = 0f;

            while (elapsed < rotationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / rotationDuration;

                Quaternion rot = Quaternion.Lerp(startRotation, endRotation, t);
                Quaternion arrowRot = Quaternion.Lerp(arrowStartRotation, arrowEndRotation, t);
                target.transform.localRotation = rot;
                _arrowTransform.transform.localRotation = arrowRot;

                yield return null;
            }

            target.transform.localRotation = endRotation;
            _arrowTransform.transform.localRotation = arrowEndRotation;
        }

        // -------------------- HELPERS --------------------

        public GameObject GetClosestObject(Vector3 position)
        {
            if (_possibleFilter == null || _possibleFilter.Length == 0)
                return null;

            GameObject closest = null;
            float minDistanceSqr = Mathf.Infinity;

            foreach (GameObject go in _possibleFilter)
            {
                if (go == null) continue;

                float distanceSqr = (go.transform.position - position).sqrMagnitude;
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    closest = go;
                }
            }

            return closest;
        }

        public void SetComponentRotation(float actRot, int axis)
        {
            StartRotation(actRot, axis);
        }

        public void SetUIVisibility(bool visible)
        {
            _degreeUI.SetActive(visible);
        }

        public void SetButtonVisibility(bool visible)
        {
            _adjusterUI.SetActive(visible);
        }
    }
}