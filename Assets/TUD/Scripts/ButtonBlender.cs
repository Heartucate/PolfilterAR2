// ButtonBlender.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Heartucate.TUD
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ButtonBlender : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.35f;
        [SerializeField]
        private AnimationCurve animationCurve =
            AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Visual Animation")]
        [SerializeField] private Vector3 hiddenScale = new Vector3(0.8f, 0.8f, 1f);
        [SerializeField] private Vector3 shownScale = Vector3.one;
        [SerializeField] private Vector3 hiddenLocalOffset = new Vector3(0, -30f, 0);

        private CanvasGroup canvasGroup;
        private RectTransform rect;
        private Vector3 shownLocalPos;
        private Coroutine animationRoutine;
        private bool isVisible = true;

        private Button button;

        private void Awake()
        {
            Init();
            shownLocalPos = rect.localPosition;
            SetHiddenInstant();
        }

        private void Init()
        {
            if (rect == null)
                rect = GetComponent<RectTransform>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (button == null)
                button = GetComponent<Button>();
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.B))
            {
                if (isVisible) Hide();
                else Show();
            }
        }

        public void Show() => StartAnimation(true);
        public void Hide() => StartAnimation(false);

        private void StartAnimation(bool show)
        {
            isVisible = show;
            if (animationRoutine != null)
                StopCoroutine(animationRoutine);

            animationRoutine = StartCoroutine(Animate(show));

        }

        private IEnumerator Animate(bool show)
        {
            float time = 0f;

            Vector3 startScale = show ? hiddenScale : shownScale;
            Vector3 targetScale = show ? shownScale : hiddenScale;

            Vector3 startPos = show ? shownLocalPos + hiddenLocalOffset : shownLocalPos;
            Vector3 targetPos = show ? shownLocalPos : shownLocalPos + hiddenLocalOffset;

            float startAlpha = show ? 0f : 1f;
            float targetAlpha = show ? 1f : 0f;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            while (time < animationDuration)
            {
                time += Time.unscaledDeltaTime; // 🔑 WICHTIG
                float t = animationCurve.Evaluate(time / animationDuration);

                rect.localScale = Vector3.Lerp(startScale, targetScale, t);
                rect.localPosition = Vector3.Lerp(startPos, targetPos, t);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                yield return null;
            }

            rect.localScale = targetScale;
            rect.localPosition = targetPos;
            canvasGroup.alpha = targetAlpha;

            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
        }

        public void SetHiddenInstant()
        {
            Init();
            rect.localScale = hiddenScale;
            rect.localPosition = shownLocalPos + hiddenLocalOffset;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isVisible = false;
        }

        public void SetInteractable(bool value)
        {
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;

            if (button != null)
                button.interactable = value;
        }
    }
}