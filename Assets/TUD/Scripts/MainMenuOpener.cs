// MainMenuOpener.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System.Collections;
using UnityEngine;

namespace Heartucate.TUD
{
    public class MainMenuOpener : MonoBehaviour
    {
        [Header("Objekte")]
        public GameObject[] objects;

        [Header("Animation")]
        public float fadeDuration = 1f;
        public float delayBetweenObjects = 0.2f;

        [Tooltip("0 = unsichtbar, 0.5 = 50% der Endgröße")]
        [Range(0f, 1f)]
        public float startScaleMultiplier = 0f;

        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3[] endScales;

        private void Awake()
        {
            endScales = new Vector3[objects.Length];

            // Initialzustand aller Objekte
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject obj = objects[i];
                if (obj == null) continue;

                obj.SetActive(true);

                endScales[i] = obj.transform.localScale;
                obj.transform.localScale = endScales[i] * startScaleMultiplier;

                CanvasGroup cg = obj.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 0f;
                }
            }
        }

        private void Start()
        {
            StartCoroutine(StartFadeInSequence());
        }

        private IEnumerator StartFadeInSequence()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    StartCoroutine(FadeInObject(objects[i], endScales[i]));
                }

                // ❗ Nur Startabstand – kein Warten auf Abschluss
                yield return new WaitForSeconds(delayBetweenObjects);
            }
        }

        private IEnumerator FadeInObject(GameObject obj, Vector3 endScale)
        {
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
            Vector3 startScale = endScale * startScaleMultiplier;

            float time = 0f;

            while (time < fadeDuration)
            {
                float t = time / fadeDuration;
                float curveValue = animationCurve.Evaluate(t);

                obj.transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = curveValue;
                }

                time += Time.deltaTime;
                yield return null;
            }

            // Endwerte absichern
            obj.transform.localScale = endScale;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}