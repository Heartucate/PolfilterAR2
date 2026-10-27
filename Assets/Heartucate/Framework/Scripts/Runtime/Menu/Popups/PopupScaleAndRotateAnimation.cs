using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Heartucate.Core.Presentation
{
    [RequireComponent(typeof(RectTransform))]
    public class PopupScaleAndRotateAnimation : MonoBehaviour, IPopupAnimation
    {
        [Header("Animation Settings")]
        [Tooltip("Total duration of the animation in seconds.")]
        public float duration = 1f;

        [Tooltip("Animate uniform scaling?")]
        public bool animateScale = true;
        [Tooltip("Curve controlling scale over normalized time.")]
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Tooltip("Animate Z-rotation?")]
        public bool animateRotation = true;
        [Tooltip("Curve controlling Z-rotation (degrees) over normalized time.")]
        public AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 360);

        private RectTransform _rt;
        private Coroutine _anim;

        void Awake()
        {
            _rt = GetComponent<RectTransform>();
            // start hidden until PlayIntroAsync is called
            if (animateScale)
                _rt.localScale = Vector3.zero;
            else
                _rt.localScale = Vector3.one;
            // reset rotation
            _rt.localRotation = Quaternion.identity;
        }

        public Task PlayIntroAsync() => Play(forward: true);
        public Task PlayOutroAsync() => Play(forward: false);

        private Task Play(bool forward)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (_anim != null) StopCoroutine(_anim);
            _anim = StartCoroutine(DoAnimation(forward, () => tcs.SetResult(true)));
            return tcs.Task;
        }

        private IEnumerator DoAnimation(bool forward, Action onComplete)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float sampleT = forward ? t : 1f - t;

                if (animateScale)
                {
                    float s = scaleCurve.Evaluate(sampleT);
                    _rt.localScale = Vector3.one * s;
                }

                if (animateRotation)
                {
                    float ang = rotationCurve.Evaluate(sampleT);
                    _rt.localRotation = Quaternion.Euler(0f, 0f, ang);
                }

                yield return null;
            }

            // final snap
            float endT = forward ? 1f : 0f;
            if (animateScale)
                _rt.localScale = Vector3.one * scaleCurve.Evaluate(endT);
            if (animateRotation)
                _rt.localRotation = Quaternion.Euler(0f, 0f, rotationCurve.Evaluate(endT));

            onComplete?.Invoke();
        }
    }
}
