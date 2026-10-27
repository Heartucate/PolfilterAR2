using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ThemedButton : ThemedSelectable
{
    private Button _button;
    private TextMeshProUGUI _textField;
    public string id;

    public Button Button
    {
        get
        {
            if (_button == null)
                _button = GetComponent<Button>();

            return _button;
        }
        private set { }
    }

    public void TrySetText(string text)
    {
        if (_textField == null)
            _textField = GetComponentInChildren<TextMeshProUGUI>();

        if (_textField == null)
        {
            Debug.LogWarning("Tried to set text on button with no assigned text field.");
            return;
        }

        _textField.text = text;
    }

    //public override bool IsInteractable()
    //{
    //    if (_button == null)
    //        _button = GetComponent<Button>();

    //    return _button.interactable;
    //}
    //public override void SetInteractable(bool value)
    //{
    //    if (_button == null)
    //        _button = GetComponent<Button>();

    //    _button.interactable = value;
    //}

    //public Button GetButton()
    //{
    //    if (_button == null)
    //        _button = GetComponent<Button>();

    //    return _button;
    //} 

    //public bool interactable = true;
    //protected bool _interactable;

    //[SerializeField] protected Image _image;
    //[SerializeField] protected Image _contentImage;
    //[SerializeField] protected Image _underlineImage;
    //[SerializeField] protected TextMeshProUGUI _contentText;
    //[HideInNormalInspector] [SerializeField] protected Button _button;

    //[Header("Default")]
    //[SerializeField] [ColorSettings("Default Image Color")] protected ColorSettings _defaultColorSettings;
    //[SerializeField] [ColorSettings("Default Content Color")] protected ColorSettings _defaultContentColorSettings;
    //[Header("Pressed")]
    //[SerializeField] [ColorSettings("Pressed Image Color")] protected ColorSettings _pressColorSettings;
    //[SerializeField] [ColorSettings("Pressed Content Color")] protected ColorSettings _pressContentColorSettings;
    //[Header("Selected")]
    //[SerializeField] [ColorSettings("Selected Image Color")] protected ColorSettings _selectedColorSettings;
    //[SerializeField] [ColorSettings("Selected Content Color")] protected ColorSettings _selectedContentColorSettings;
    //[Header("Disabled")]
    //[SerializeField] [ColorSettings("Disabled Image Color")] protected ColorSettings _disabledColorSettings;
    //[SerializeField] [ColorSettings("Disabled Content Color")] protected ColorSettings _disabledContentColorSettings;
    //[SerializeField] protected ColorTheme.BorderStyle _borderStyle;
    //[SerializeField] [FontSettings] protected string _textGuid;

    //protected Button.ButtonClickedEvent _onClick = new Button.ButtonClickedEvent();

    //protected bool _isPointerDown;
    //protected bool _isSelected;

    //public Button.ButtonClickedEvent onClick;

    //private void Reset()
    //{
    //    _button = GetComponent<Button>();
    //    _button.transition = Selectable.Transition.None;
    //}

    //protected override void ApplyTheme()
    //{
    //    if (_isSelected)
    //    {
    //        ThemeApplier.ApplyColorSettings(_image, _selectedColorSettings);
    //        if (_contentText != null && _contentText.gameObject.activeSelf)
    //        {
    //            ThemeApplier.ApplyColorSettings(_contentText, _selectedContentColorSettings);
    //            ThemeApplier.ApplyTextSettings(_contentText, _textGuid);
    //        }
    //        if (_contentImage != null && _contentImage.gameObject.activeSelf)
    //        {
    //            ThemeApplier.ApplyColorSettings(_contentImage, _selectedContentColorSettings);
    //        }
    //        if (_underlineImage != null && _underlineImage.gameObject.activeSelf)
    //        {
    //            ThemeApplier.ApplyColorSettings(_underlineImage, _selectedContentColorSettings);
    //        }
    //    }
    //    else
    //    {
    //        ThemeApplier.ApplyColorSettings(_image, interactable ? (_isPointerDown ? _pressColorSettings : _defaultColorSettings) : _disabledColorSettings);
    //        ThemeApplier.ApplyImageBorder(_image, _borderStyle, -1, false);

    //        if (_contentText != null && _contentText.gameObject.activeSelf)
    //        {
    //            ThemeApplier.ApplyColorSettings(_contentText, interactable ? (_isPointerDown ? _pressContentColorSettings : _defaultContentColorSettings) : _disabledContentColorSettings);
    //            ThemeApplier.ApplyTextSettings(_contentText, _textGuid);
    //        }
    //        if (_contentImage != null && _contentImage.gameObject.activeSelf)
    //        {
    //            ThemeApplier.ApplyColorSettings(_contentImage, interactable ? (_isPointerDown ? _pressContentColorSettings : _defaultContentColorSettings) : _disabledContentColorSettings);
    //        }
    //        if (_underlineImage != null && _underlineImage.gameObject.activeSelf)
    //        {
    //            ThemeApplier.ApplyColorSettings(_underlineImage, interactable ? (_isPointerDown ? _pressContentColorSettings : _defaultContentColorSettings) : _disabledContentColorSettings);
    //        }
    //    }
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    _isPointerDown = true;
    //    ApplyTheme();
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    _isPointerDown = false;
    //    ApplyTheme();
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (!interactable)
    //        return;

    //    onClick?.Invoke();
    //    Debug.Log("Themed button was pressed.");
    //}

    //public void OnSelect(BaseEventData eventData)
    //{
    //    Debug.Log("Themed button was selected.");

    //    _isSelected = true;
    //    ApplyTheme();
    //}

    //public void OnDeselect(BaseEventData eventData)
    //{
    //    Debug.Log("Themed button was deselected.");

    //    _isSelected = false;
    //    ApplyTheme();
    //}

    //public void SetInteractable(bool value)
    //{
    //    interactable = value;
    //    ApplyTheme();
    //}

    //void Update()
    //{
    //    if (_interactable != interactable)
    //    {
    //        ApplyTheme();
    //        _interactable = interactable;
    //    }
    //}

    //public void TrySetText(string text)
    //{
    //    if (_contentText == null)
    //    {
    //        Debug.LogWarning("Tried to set text on button with no assigned text field.");
    //        return;
    //    }

    //    _contentText.text = text;
    //}

    //public void SetDefaultColorSettings(ColorSettings settings)
    //{
    //    _defaultColorSettings = settings;
    //    ApplyTheme();
    //}

    //public void SetPressColorSettings(ColorSettings settings)
    //{
    //    _pressColorSettings = settings;
    //    ApplyTheme();
    //}

    //public void SetDisabledColorSettings(ColorSettings settings)
    //{
    //    _disabledColorSettings = settings;
    //    ApplyTheme();
    //}

    //public void SetSelectedColorSettings(ColorSettings settings)
    //{
    //    _selectedColorSettings = settings;
    //    ApplyTheme();
    //}
}
