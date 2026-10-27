using System.Collections.Generic;
using UnityEngine;

namespace Heartucate.Core.Presentation
{
    public class PopUpManager : MonoBehaviour
    {
        [SerializeField] private string _exp1Text;
       
        private void Start()
        {
            OpenPopUp(_exp1Text);
        }

        public void OpenPopUp(string t)
        {
            Debug.Log("PopUp:   "+ t);
            var popupData =

            // fire-and-forget; ShowPopup handles its own lifetime
            _ = MenuPopupManager.instance.ShowPopup(GetTestPopupData(t));
        }

        PopupData GetTestPopupData(string text) =>
            new PopupData(
                text,
                new List<ButtonData>
                {
                    // Immediate-exit button
                    new ButtonData(
                        label: "Weiter",
                        ButtonData.ButtonStyle.Action,
                        action: async () =>
                        {
                            Debug.Log("Ok pressed — immediate exit");
                        },
                        isExit: true
                    )
                },
                iconId: "Info"
            );
    }
}
