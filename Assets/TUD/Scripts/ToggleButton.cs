// ToggleButton.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System.Collections;
using UnityEngine;

namespace Heartucate.TUD
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _offIMG;
        [SerializeField] private UnityEngine.UI.Image _onIMG;

        [SerializeField] private VisibilityButton _visBustton;
        [SerializeField] private GameObject _toggler;

        [Header("Toggle Positionen (lokale Y-Achse)")]
        [SerializeField] private float togglePos1 = 0.0f;
        [SerializeField] private float togglePos2 = 60.0f;

        [Header("Text Transparenz")]
        [Range(0f, 1f)][SerializeField] private float disabledAlpha = 0.3f;
        [Range(0f, 1f)][SerializeField] private float enabledAlpha = 1f;

        [Header("Animation")]
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool isOn = false;
        private Coroutine toggleRoutine;

        private void Start()
        {
            SetInstantOn();
        }

        public void SetInstantOn()
        {
            isOn = true;

            if (toggleRoutine != null)
            {
                StopCoroutine(toggleRoutine);
                toggleRoutine = null;
            }

            // Toggler sofort positionieren
            Vector3 pos = _toggler.transform.localPosition;
            pos.y = togglePos2;
            _toggler.transform.localPosition = pos;

            // Alpha-Werte sofort setzen
            SetAlpha(_offIMG, disabledAlpha);
            SetAlpha(_onIMG, enabledAlpha);
            _visBustton.SetVisibleInstant(isOn);
        }

        public void Toggle()
        {
            isOn = !isOn;
            _visBustton.SetVisibleInstant(isOn);
            if (toggleRoutine != null)
                StopCoroutine(toggleRoutine);

            toggleRoutine = StartCoroutine(AnimateToggle(isOn));
        }

        private IEnumerator AnimateToggle(bool state)
        {
            float startY = _toggler.transform.localPosition.y;
            float targetY = state ? togglePos2 : togglePos1;

            float time = 0f;
            while (time < moveDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / moveDuration);
                float curveValue = moveCurve.Evaluate(t);

                // Toggler-Position animieren
                Vector3 pos = _toggler.transform.localPosition;
                pos.y = Mathf.Lerp(startY, targetY, curveValue);
                _toggler.transform.localPosition = pos;

                // Texte Alpha interpolieren
                float alpha1 = Mathf.Lerp(state ? disabledAlpha : enabledAlpha, state ? enabledAlpha : disabledAlpha, curveValue);
                float alpha2 = Mathf.Lerp(state ? enabledAlpha : disabledAlpha, state ? disabledAlpha : enabledAlpha, curveValue);

                SetAlpha(_onIMG, alpha1);
                SetAlpha(_offIMG, alpha2);

                yield return null;
            }

            // Endwerte setzen
            Vector3 finalPos = _toggler.transform.localPosition;
            finalPos.y = targetY;
            _toggler.transform.localPosition = finalPos;

            SetAlpha(_onIMG, state ? enabledAlpha : disabledAlpha);
            SetAlpha(_offIMG, state ? disabledAlpha : enabledAlpha);

            toggleRoutine = null;
        }

        private void SetAlpha(UnityEngine.UI.Image img, float alpha)
        {
            Color c = img.color;
            c.a = Mathf.Clamp01(alpha);
            img.color = c;
        }
    }
}