using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Heartucate.Core.Presentation
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PopupFadeAnimation : MonoBehaviour, IPopupAnimation
    {
        [Header("Fade Settings")]
        [Tooltip("Total duration of the fade in seconds.")]
        public float duration = 0.5f;

        [Tooltip("Curve controlling alpha from 0→1 over normalized time.")]
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private CanvasGroup _cg;
        private Coroutine _fade;

        void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
            // start hidden until PlayIntroAsync is called
            _cg.alpha = 0f;
        }

        public Task PlayIntroAsync()
            => Fade(from: _cg.alpha, to: 1f);

        public Task PlayOutroAsync()
            => Fade(from: _cg.alpha, to: 0f);

        private Task Fade(float from, float to)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (_fade != null) StopCoroutine(_fade);
            _fade = StartCoroutine(DoFade(from, to, () => tcs.SetResult(true)));
            return tcs.Task;
        }

        private IEnumerator DoFade(float from, float to, Action onComplete)
        {
            float elapsed = 0f;
            _cg.alpha = from;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float curveT = fadeCurve.Evaluate(t);
                _cg.alpha = Mathf.Lerp(from, to, curveT);
                yield return null;
            }
            _cg.alpha = to;
            onComplete?.Invoke();
        }
    }
}
