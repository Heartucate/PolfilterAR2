using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Heartucate.Core.Presentation
{
    public class PopupData
    {
        public string Message { get; }
        public List<ButtonData> ButtonData { get; }
        public bool ShowLicenseInputField { get; private set; }
        public Func<Task> LicenseSuccessAction { get; }
        public Sprite IconSprite { get; }

        public PopupData(
            string message,
            List<ButtonData> buttonData,
            bool showLicenseInputField = false,
            Func<Task> licenseSuccessAction = null,
            string iconId = ""
        )
        {
            Message = message;
            ButtonData = buttonData;
            LicenseSuccessAction = licenseSuccessAction;

            if (!string.IsNullOrEmpty(iconId) && MenuPopupManager.instance != null)
            {
                var ico = MenuPopupManager.instance.popupIcons
                          .FirstOrDefault(x => x.id == iconId);
                if (ico != null) IconSprite = ico.sprite;
            }
        }
    }
}
