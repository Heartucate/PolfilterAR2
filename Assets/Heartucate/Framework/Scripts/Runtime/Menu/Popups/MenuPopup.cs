using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using Heartucate.Licensing.Presentation;

namespace Heartucate.Core.Presentation
{
    public class MenuPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _modalText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Transform _buttonsRoot;
        [SerializeField] private GameObject _actionButton;
        [SerializeField] private GameObject _warningButton;
        [SerializeField] private GameObject _discreetButton;
        [SerializeField] private ThemedText _licenseInputInfoText;
        //[SerializeField] private LicenseKeyInputField _licenseInputField;
        //[SerializeField] private ThemedText _licenseInputInfoText;

        // NEW: the two special roots
        [Header("Special Roots")]
        [Tooltip("Animations on this GameObject will animate in at start and out last.")]
        [SerializeField] private GameObject _backgroundObject;
        [Tooltip("Animations on this GameObject will animate in on loading and out last.")]
        [SerializeField] private GameObject _loadingIndicatorObject;

        public GameObject BackgroundObject => _backgroundObject;
        public GameObject LoadingIndicatorObject => _loadingIndicatorObject;

        public void SetMessageAndIcon(string message, Sprite iconSprite)
        {
            _modalText.text = message;
            _iconImage.sprite = iconSprite;
            _iconImage.transform.parent.gameObject.SetActive(iconSprite != null);
        }

        public ThemedButton InstantiateButton(ButtonData buttonData)
        {
            var prefab = buttonData.Style switch
            {
                ButtonData.ButtonStyle.Action => _actionButton,
                ButtonData.ButtonStyle.Warning => _warningButton,
                ButtonData.ButtonStyle.Discreet => _discreetButton,
                _ => _actionButton
            };
            var btn = Instantiate(prefab, _buttonsRoot).GetComponent<ThemedButton>();
            btn.TrySetText(buttonData.Label);
            return btn;
        }

        /// <summary>
        /// Hide the buttons, then play all IPopupAnimation.Intro on the loading?indicator root.
        /// </summary>
        public async Task IndicateLoadingAsync()
        {
            // hide the buttons
            _buttonsRoot.gameObject.SetActive(false);

            // play only the loading?indicator’s intro animations
            var loadAnims = _loadingIndicatorObject
                .GetComponentsInChildren<IPopupAnimation>(false);

            await Task.WhenAll(loadAnims.Select(a => a.PlayIntroAsync()));
        }
    }
}
