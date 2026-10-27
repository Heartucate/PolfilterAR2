// LevelChanger.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using Heartucate.Core.Presentation;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Heartucate.TUD.GameState;

namespace Heartucate.TUD
{
    public class LevelChanger : MonoBehaviour
    {
        [SerializeField] private ButtonBlender _prevButton;
        [SerializeField] private ButtonBlender _nextButton;

        [SerializeField] private TextMeshProUGUI _levelDisplay;
        [SerializeField] private Image _LevelBG;
        [Header("DegreeReset")]
        [SerializeField] private DegreeAdjuster _degAdj1;
        [SerializeField] private DegreeAdjuster _degAdj2;
        [SerializeField] private DegreeAdjuster _degAdj3;
        [SerializeField] private GameObject _polUI1;
        [SerializeField] private GameObject _polUI2;

        [Header("Animation")]
        [SerializeField] private AnimationCurve _fadeCurve;
        [SerializeField] private AnimationCurve _scaleCurve;
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private float _scaleFactor = 0.85f;

        private Vector3 _originalScale;
        private Coroutine _animRoutine;

        [SerializeField] private string[] _experimentNames;
        [SerializeField] private string _exp1Text;
        [TextArea]
        [SerializeField] private string _exp2TextHybrid;
        [TextArea]
        [SerializeField] private string _exp2TextAR;
        [TextArea]
        [SerializeField] private string _exp2TextSimul;

        [Header("Level Change Animation")]
        [SerializeField] private AnimationCurve _levelFadeOutCurve;
        [SerializeField] private AnimationCurve _levelFadeInCurve;
        [SerializeField] private AnimationCurve _levelScaleCurve;
        [SerializeField] private float _levelChangeDuration = 0.25f;
        [SerializeField] private float _levelChangeScaleFactor = 0.9f;
        [SerializeField] private PopUpManager _popUpManager;


        private Coroutine _levelChangeRoutine;

        private void Awake()
        {
            _originalScale = _levelDisplay.transform.localScale;
            SetAlpha(0f, true);
            SetInitialLevel1();
        }

        public void ToggleLevelVisibility(bool show)
        {
            if (_animRoutine != null)
                StopCoroutine(_animRoutine);

            _animRoutine = StartCoroutine(Animate(show));

            if (show)
            {
                _prevButton.Show();
                _nextButton.Show();
            }
            else
            {
                _prevButton.Hide();
                _nextButton.Hide();
            }
        }

        private IEnumerator Animate(bool show)
        {
            float time = 0f;

            float startAlpha = show ? 0f : 1f;
            float targetAlpha = show ? 1f : 0f;

            Vector3 startScale = show
                ? _originalScale * _scaleFactor
                : _originalScale;

            Vector3 targetScale = show
                ? _originalScale
                : _originalScale * _scaleFactor;

            SetAlpha(startAlpha, true);
            _levelDisplay.transform.localScale = startScale;

            while (time < _animationDuration)
            {
                time += Time.unscaledDeltaTime;

                float normalizedTime = time / _animationDuration;

                float fadeT = _fadeCurve.Evaluate(normalizedTime);

                //  No Clamping
                float scaleMultiplier = _scaleCurve.Evaluate(normalizedTime);

                Vector3 scaledStart = startScale;
                Vector3 scaledTarget = targetScale;

                _levelDisplay.transform.localScale =
                    Vector3.LerpUnclamped(scaledStart, scaledTarget, scaleMultiplier);

                SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, fadeT), true);

                yield return null;
            }

            _levelDisplay.transform.localScale = targetScale;
            SetAlpha(targetAlpha, true);
        }

        private void SetAlpha(float alpha, bool backg)
        {
            Color c = _levelDisplay.color;
            c.a = alpha;
            _levelDisplay.color = c;

            if (backg)
            {
                Color bg = _LevelBG.color;
                bg.a = alpha;
                _LevelBG.color = bg;
            }

        }

        public void IncreaseLevel()
        {
            int currentIndex = (int)GameState.CurrentLevel;
            int maxIndex = System.Enum.GetValues(typeof(Level)).Length - 1;

            if (currentIndex < maxIndex)
            {
                GameState.CurrentLevel = (Level)(currentIndex + 1);
                VisibilityChanger.ChangeVisibility();
                if (currentIndex == 0)
                {
                    if (GameState.CurrentMode == GameState.GameMode.HYBRID) _popUpManager.OpenPopUp(_exp2TextHybrid);
                    else if (GameState.CurrentMode == GameState.GameMode.SIMULATION)
                    {
                        _degAdj3.ApplyDegree();
                        _polUI2.transform.localRotation = Quaternion.Euler(114f, 0f, 0f);
                        _polUI1.transform.localRotation = Quaternion.Euler(24f, 0f, 0f);

                        _popUpManager.OpenPopUp(_exp2TextSimul);
                    }
                    else
                    {
                        _degAdj3.ApplyDegree();
                        _polUI2.transform.localRotation = Quaternion.Euler(114f, 0f, 0f);
                        _polUI1.transform.localRotation = Quaternion.Euler(24f, 0f, 0f);

                        _popUpManager.OpenPopUp(_exp2TextAR);
                    }
                }
                if (currentIndex == 1)
                {
                    _degAdj1.ApplyDegree();
                    _degAdj2.ApplyDegree();

                    _popUpManager.OpenPopUp(_exp1Text);
                }
                UpdateLevelDisplay();
                UpdateButtons();

            }
        }

        public void DecreaseLevel()
        {
            int currentIndex = (int)GameState.CurrentLevel;

            if (currentIndex > 0)
            {
                GameState.CurrentLevel = (Level)(currentIndex - 1);
                VisibilityChanger.ChangeVisibility();
                if (currentIndex == 0)
                {
                    if (GameState.CurrentMode == GameState.GameMode.HYBRID) _popUpManager.OpenPopUp(_exp2TextHybrid);
                    else if (GameState.CurrentMode == GameState.GameMode.SIMULATION)
                    {
                        _degAdj3.ApplyDegree();
                        _polUI2.transform.localRotation = Quaternion.Euler(114f, 0f, 0f);
                        _polUI1.transform.localRotation = Quaternion.Euler(24f, 0f, 0f);

                        _popUpManager.OpenPopUp(_exp2TextSimul);
                    }
                    else
                    {
                        _degAdj3.ApplyDegree();
                        _polUI2.transform.localRotation = Quaternion.Euler(114f, 0f, 0f);
                        _polUI1.transform.localRotation = Quaternion.Euler(24f, 0f, 0f);

                        _popUpManager.OpenPopUp(_exp2TextAR);
                    }
                }
                if (currentIndex == 1)
                {
                    _degAdj1.ApplyDegree();
                    _degAdj2.ApplyDegree();

                    _popUpManager.OpenPopUp(_exp1Text);
                }
                UpdateLevelDisplay();
                UpdateButtons();

            }
        }

        public IEnumerator SetInitialLevel1()
        {
            GameState.CurrentLevel = Level.LEVEL1;

            yield return new WaitForEndOfFrame();
            _levelDisplay.text = _experimentNames[(int)Level.LEVEL1];
            _levelDisplay.transform.localScale = _originalScale;

            UpdateButtons();
            VisibilityChanger.ChangeVisibility();
        }


        private void UpdateLevelDisplay()
        {
            int index = (int)GameState.CurrentLevel;

            if (index < 0 || index >= _experimentNames.Length)
                return;

            if (_levelChangeRoutine != null)
                StopCoroutine(_levelChangeRoutine);

            _levelChangeRoutine =
                StartCoroutine(AnimateLevelChange(_experimentNames[index]));
        }


        private void UpdateButtons()
        {
            int index = (int)GameState.CurrentLevel;
            int max = System.Enum.GetValues(typeof(Level)).Length - 1;

            _prevButton.SetInteractable(index > 0);
            _nextButton.SetInteractable(index < max);
        }

        private IEnumerator AnimateLevelChange(string newText)
        {
            float time = 0f;

            Vector3 startScale = _originalScale;
            Vector3 targetScale = _originalScale * _levelChangeScaleFactor;


            while (time < _levelChangeDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / _levelChangeDuration;

                float fadeT = _levelFadeOutCurve.Evaluate(t);
                float scaleT = _levelScaleCurve.Evaluate(t);

                SetAlpha(Mathf.Lerp(1f, 0f, fadeT), false);
                _levelDisplay.transform.localScale =
                    Vector3.LerpUnclamped(startScale, targetScale, scaleT);

                yield return null;
            }


            _levelDisplay.text = newText;

            time = 0f;


            while (time < _levelChangeDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / _levelChangeDuration;

                float fadeT = _levelFadeInCurve.Evaluate(t);
                float scaleT = _levelScaleCurve.Evaluate(t);

                SetAlpha(Mathf.Lerp(0f, 1f, fadeT), false);
                _levelDisplay.transform.localScale =
                    Vector3.LerpUnclamped(targetScale, startScale, scaleT);

                yield return null;
            }


            SetAlpha(1f, false);
            _levelDisplay.transform.localScale = _originalScale;
        }
    }
}