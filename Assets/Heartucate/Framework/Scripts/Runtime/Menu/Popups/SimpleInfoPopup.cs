using System.Collections.Generic;
using UnityEngine;

namespace Heartucate.Core.Presentation
{
    public class SimpleInfoPopup : MonoBehaviour
    {
        [SerializeField] private ThemedButton _button;
        [TextArea(3,10)]
        [SerializeField] private string _prompt;
        [SerializeField] private string _iconId;

        void Start()
        {
            _button.Button.onClick.AddListener(async () =>
            {
                if(MenuPopupManager.instance == null)
                {
                    Debug.LogError("PopupManager Instance not found");
                    return;
                }
                await MenuPopupManager.instance.ShowPopup(CreateSimplePopupData());
            });
        }

        public PopupData CreateSimplePopupData() =>
            new(_prompt, new List<ButtonData>
            {
                    new ButtonData("Schlieﬂen", ButtonData.ButtonStyle.Action, null)
            },
            iconId: _iconId);
    }
}