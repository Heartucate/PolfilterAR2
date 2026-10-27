using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Heartucate.Core.Presentation
{
    public class PopupTest : MonoBehaviour
    {
        void Start()
        {
            // Create popup with two buttons:
            //  • “Ok” exits immediately
            //  • “Exit w/ Delay” waits 2 seconds before completing
            var popupData = 

            // fire-and-forget; ShowPopup handles its own lifetime
            _ = MenuPopupManager.instance.ShowPopup(GetTestPopupData());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                Start();
        }

        PopupData GetTestPopupData() =>
            new PopupData(
                "Das ist ein Test",
                new List<ButtonData>
                {
                    // Immediate-exit button
                    new ButtonData(
                        label: "Exit",
                        ButtonData.ButtonStyle.Action,
                        action: async () =>
                        {
                            Debug.Log("Ok pressed — immediate exit");
                        },
                        isExit: true
                    ),

                    // Delayed-exit button for testing loading indicator
                    new ButtonData(
                        label: "Exit with delay",
                        ButtonData.ButtonStyle.Action,
                        action: async () =>
                        {
                            Debug.Log("Waiting before exit…");
                            await Task.Delay(2000);
                            Debug.Log("Done waiting — now exit");
                        },
                        isExit: true
                    ),

                    new ButtonData(
                        label: "Exit this and open another popup",
                        ButtonData.ButtonStyle.Warning,
                        action: async () =>
                        {
                            _ = MenuPopupManager.instance.ShowPopup(GetTestPopupData());
                        },
                        isExit: true
                    ),

                    new ButtonData(
                        label: "Open another popup",
                        ButtonData.ButtonStyle.Discreet,
                        action: async () =>
                        {
                            await MenuPopupManager.instance.ShowPopup(GetTestPopupData());
                        },
                        isExit: false
                    ),
                },
                iconId: "Info"
            );
    }
}
