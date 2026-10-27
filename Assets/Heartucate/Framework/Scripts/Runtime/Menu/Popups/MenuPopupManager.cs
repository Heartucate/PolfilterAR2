using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Heartucate.Core.Presentation
{
    public class MenuPopupManager : MonoBehaviour
    {
        public static MenuPopupManager instance;
        public GameObject popupPrefab;
        public Transform canvasTransform;

        [Serializable] public class PopupIcon { public string id; public Sprite sprite; }
        public List<PopupIcon> popupIcons;

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        void Update()
        {
            //Debug.Log(EventSystem.current.currentSelectedGameObject);
        }

        public async Task ShowPopup(PopupData popupData)
        {
            Debug.Log($"Showing popup: {popupData.Message}");

            // 1) Instantiate & configure the popup
            var popup = Instantiate(popupPrefab, canvasTransform)
                            .GetComponent<MenuPopup>();
            if (popup == null)
            {
                Debug.LogError("Failed to instantiate popup.");
                return;
            }
            popup.SetMessageAndIcon(popupData.Message, popupData.IconSprite);

            //if (popupData.ShowLicenseInputField)
            //    popup.ShowLicenseInputField(async () =>
            //    {
            //        Destroy(popup.gameObject);

            //        if (popupData.LicenseSuccessAction != null)
            //            await popupData.LicenseSuccessAction();

            //        _completionSource.TrySetResult(true);
            //    });

            // 2) Instantiate all buttons up front
            var buttonPairs = new List<(ThemedButton Btn, ButtonData Data)>();
            foreach (var bd in popupData.ButtonData)
            {
                var btn = popup.InstantiateButton(bd);
                buttonPairs.Add((btn, bd));
            }

            // 3) Gather every IPopupAnimation now that buttons exist
            var allAnims = popup
                .GetComponentsInChildren<IPopupAnimation>(includeInactive: false)
                .ToArray();

            // 4) INITIAL INTRO: play every animation except those under LoadingIndicatorObject
            var initialAnims = allAnims
                .Where(a => !((Component)a).transform.IsChildOf(popup.LoadingIndicatorObject.transform))
                .ToArray();
            await Task.WhenAll(initialAnims.Select(a => a.PlayIntroAsync()));

            // 5) Wire up the “exit” button logic
            var tcs = new TaskCompletionSource<bool>();
            foreach (var (btn, bd) in buttonPairs)
            {
                btn.Button.onClick.AddListener(async () =>
                {
                    // 1) Start the user’s action immediately (for both exit and non-exit buttons)
                    var actionTask = bd.Action != null
                        ? bd.Action()
                        : Task.CompletedTask;

                    // 2) If this isn’t an exit button, just wait for the action and return
                    if (!bd.IsExit)
                    {
                        await actionTask;
                        return;
                    }

                    // 3) FIRST OUTRO: everything except background & loadingIndicator,
                    //    but include background here only if the action already completed
                    bool quick = actionTask.IsCompleted;
                    var firstOutro = allAnims
                        .Where(a =>
                        {
                            var t = ((Component)a).transform;
                            bool isBg = t.IsChildOf(popup.BackgroundObject.transform);
                            bool isLoader = t.IsChildOf(popup.LoadingIndicatorObject.transform);
                            return !isLoader && (quick || !isBg);
                        })
                        .ToArray();
                    await Task.WhenAll(firstOutro.Select(a => a.PlayOutroAsync()));

                    // 4) Give Unity one frame—if the action finished now, we'll skip showing the loader
                    await Task.Yield();

                    bool didShowLoader = false;
                    Task loaderIntro = null;
                    if (!actionTask.IsCompleted)
                    {
                        // action still running ⇒ show loader
                        didShowLoader = true;
                        loaderIntro = popup.IndicateLoadingAsync();
                    }

                    // 5) Race the action vs. the loader intro:
                    if (didShowLoader)
                    {
                        var winner = await Task.WhenAny(actionTask, loaderIntro);
                        if (winner == actionTask)
                        {
                            // action won ⇒ reverse any loader that began
                            var loadAnims = popup.LoadingIndicatorObject
                                .GetComponentsInChildren<IPopupAnimation>(false);
                            await Task.WhenAll(loadAnims.Select(a => a.PlayOutroAsync()));
                        }
                        else
                        {
                            // loader fully shown ⇒ wait for the action to finish
                            await actionTask;
                        }
                    }
                    else
                    {
                        // never showed loader ⇒ just wait for the action if it isn’t done
                        await actionTask;
                    }

                    // 6) FINAL OUTRO: background + loader (loader only if it actually showed)
                    var finalOutro = allAnims
                        .Where(a =>
                        {
                            var t = ((Component)a).transform;
                            bool isBg = t.IsChildOf(popup.BackgroundObject.transform);
                            bool isLoader = t.IsChildOf(popup.LoadingIndicatorObject.transform);
                            return isBg || (isLoader && didShowLoader);
                        })
                        .ToArray();
                    await Task.WhenAll(finalOutro.Select(a => a.PlayOutroAsync()));

                    // 7) Tear down
                    popup.gameObject.SetActive(false);
                    Destroy(popup.gameObject, 0.1f);
                    tcs.TrySetResult(true);
                });

            }

            // 6) Wait for the exit sequence to complete
            await tcs.Task;
        }
    }
}
