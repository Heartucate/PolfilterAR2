// PauseMenuManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using Heartucate.Core.Presentation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartucate.TUD
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("Background")]
        [SerializeField] private CanvasGroup backgroundCanvas;
        [SerializeField] private CanvasGroup levelChangerCanvas;
        [SerializeField] private float backgroundFadeDuration = 0.25f;

        [Header("Buttons")]
        [SerializeField] private ButtonBlender[] buttons;
        [SerializeField] private float buttonStaggerDelay = 0.08f;

        [Header("LevelPreview")]
        [SerializeField] private LevelChanger _levelChanger;

        [Header("Input")]
        [SerializeField] private KeyCode toggleKey = KeyCode.Escape;


        private bool isPaused;
        private Coroutine menuRoutine;

        private void Awake()
        {
            _levelChanger.enabled = true;

        }

        private void Start()
        {
            HideInstant();
            _levelChanger.enabled = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                TogglePauseMenu();
        }

        public void TogglePauseMenu()
        {
            ShowPauseMenu(!isPaused);
        }

        private void ShowPauseMenu(bool show)
        {
            if (menuRoutine != null)
                StopCoroutine(menuRoutine);

            menuRoutine = StartCoroutine(AnimatePauseMenu(show));
            _levelChanger.ToggleLevelVisibility(show);
            isPaused = show;

            //Time.timeScale = show ? 0f : 1f;
        }

        private IEnumerator AnimatePauseMenu(bool show)
        {
            if (show)
            {
                yield return StartCoroutine(FadeCanvas(backgroundCanvas, true));

                foreach (var btn in buttons)
                {
                    btn.Show();
                    yield return new WaitForSecondsRealtime(buttonStaggerDelay);
                }
            }
            else
            {
                for (int i = buttons.Length - 1; i >= 0; i--)
                {
                    buttons[i].Hide();
                    yield return new WaitForSecondsRealtime(buttonStaggerDelay);
                }

                yield return StartCoroutine(FadeCanvas(backgroundCanvas, false));
            }
        }

        private IEnumerator FadeCanvas(CanvasGroup canvas, bool show)
        {
            float time = 0f;
            float start = show ? 0f : 1f;
            float target = show ? 1f : 0f;

            canvas.blocksRaycasts = show;
            canvas.interactable = show;

            while (time < backgroundFadeDuration)
            {
                time += Time.unscaledDeltaTime;
                canvas.alpha = Mathf.Lerp(start, target, time / backgroundFadeDuration);
                yield return null;
            }

            canvas.alpha = target;
        }

        private void HideInstant()
        {
            // Background sofort aus
            if (backgroundCanvas != null)
            {
                backgroundCanvas.alpha = 0f;
                backgroundCanvas.interactable = false;
                backgroundCanvas.blocksRaycasts = false;
            }
            if (backgroundCanvas != null)
            {
                backgroundCanvas.alpha = 0f;
                backgroundCanvas.interactable = false;
                backgroundCanvas.blocksRaycasts = false;
            }
            // Buttons sofort aus
            foreach (var btn in buttons)
            {
                btn.SetHiddenInstant();
            }

            isPaused = false;
            //Time.timeScale = 1f;
        }

        public void ResetTracking()
        {
            Heartucate.Tracking.CalibrationManager.instance.ResetSession();
            TogglePauseMenu();
        }

        public void MainMenu()
        {
            _ = MenuPopupManager.instance.ShowPopup(MainMenuHintPopupData());
        }

        private PopupData MainMenuHintPopupData() =>
             new PopupData("Möchtest du das Experiment wirklich beenden und zum Hauptmenü zurückkehren?", new List<ButtonData>
             {
            new ButtonData("Ja", ButtonData.ButtonStyle.Action, async() =>
            {
                GameState.CurrentMode = GameState.GameMode.NONE;
                ResetTracking();
                SceneManager.LoadScene(0);
            }, true),
            new ButtonData("Abbruch", ButtonData.ButtonStyle.Action, null, true)
             }, iconId: "Check");

        public void SendMicrocontrollerResetCommand()
        {
            MicrocontrollerManager.SendResetCommand();
            TogglePauseMenu();
        }
    }
}