// LightUI.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System.Collections;
using TMPro;
using UnityEngine;

namespace Heartucate.TUD
{
    public class LightUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lightDisplay;
        [SerializeField] private AnimationCurve _shakeCurve;
        [SerializeField] private GameObject _light;
        [SerializeField] private GameObject _lightColor;
        [SerializeField] private float _shakeDuration = 0.3f; // Dauer der Wackel-Animation
        [SerializeField] private float _shakeMagnitude = 5f;  // Stärke der Wackel-Bewegung
        [SerializeField] private float _scaleMagnitude = 5f;

        private Material _laserMat;
        private Color emission;
        private float _actLight = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private Vector3 _originalPosition;
        private Vector3 _originalScale;

        private void Start()
        {
            _laserMat = _lightColor.GetComponent<Renderer>().material;

            emission = _laserMat.GetColor("_EmissionColor");
            _originalPosition = _lightDisplay.transform.localPosition;
            _originalScale = _lightDisplay.transform.localScale;
            _lightDisplay.text = _actLight + "°";
            SetLight(1.0f);

        }

        public void SetLight(float newLight)
        {
            _actLight = newLight;
            _light.transform.localScale = new Vector3(newLight + 0.2f, newLight + 0.2f, newLight + 0.2f);
            _lightDisplay.text = "l = " + (_actLight * 100).ToString("F2") + "%";
            StartCoroutine(ShakeText());
            _laserMat.SetColor("_EmissionColor", emission * newLight);
            //Debug.Log("           ---" + emission * newLight);
        }

        private IEnumerator ShakeText()
        {
            float elapsed = 0f;
            while (elapsed < _shakeDuration)
            {
                float t = elapsed / _shakeDuration;
                float curveValue = _shakeCurve.Evaluate(t);

                // Position wackeln
                float offsetX = Random.Range(-1f, 1f) * _shakeMagnitude * curveValue;
                float offsetY = Random.Range(-1f, 1f) * _shakeMagnitude * curveValue;
                _lightDisplay.transform.localPosition = _originalPosition + new Vector3(offsetX, offsetY, 0f);

                // Skalierung animieren
                float scaleFactor = 1f + _scaleMagnitude * curveValue;
                _lightDisplay.transform.localScale = _originalScale * scaleFactor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Zurücksetzen
            _lightDisplay.transform.localPosition = _originalPosition;
            _lightDisplay.transform.localScale = _originalScale;
        }
    }
}