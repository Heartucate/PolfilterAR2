// CanvasGroupFader.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using System.Collections;

namespace Heartucate.TUD
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;

        private CanvasGroup _canvasGroup;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeIn()
        {
            StartFade(1f, true);
        }

        public void FadeOut()
        {
            StartFade(0f, false);
        }

        private void StartFade(float targetAlpha, bool interactable)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, interactable));
        }

        private IEnumerator FadeRoutine(float targetAlpha, bool interactable)
        {
            float startAlpha = _canvasGroup.alpha;
            float time = 0f;

            // Interactable sofort beim Einblenden aktivieren
            if (interactable)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / _fadeDuration;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;

            // Interactable erst nach Ausblenden deaktivieren
            if (!interactable)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }
    }
}