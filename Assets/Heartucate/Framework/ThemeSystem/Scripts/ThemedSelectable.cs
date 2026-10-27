using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ThemedSelectable : ThemedComponent, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] [SelectableSettings] protected string _colorsGuid;

    //public SelectableColorGroup _colors
    //{
    //    get
    //    {
    //        _colors = ThemeApplier.ApplySelectableSettings(this, _colorsGuid);
    //    }
    //    private set
    //    { }
    //}

//public bool interactable = true;
protected bool _interactable;

    [SerializeField] protected Image _image;
    //[SerializeField] protected Image _contentImage;
    [SerializeField] protected List<Image> _contentImages;
    //[SerializeField] protected Image _underlineImage;
    //[SerializeField] protected TextMeshProUGUI _contentText;
    [SerializeField] protected List<TextMeshProUGUI> _contentTexts;
    [SerializeField] protected List<TextMeshProUGUI> _subTexts;
    [SerializeField] protected List<TextMeshProUGUI> _tertiaryColorTexts;
    [SerializeField] protected List<Image> _tertiaryColorImages;

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
    [SerializeField] protected ColorTheme.BorderStyle _borderStyle;
    [SerializeField] [FontSettings] protected string _textGuid;
    [SerializeField] [FontSettings] protected string _subTextGuid;
    [SerializeField] private bool _isInnerElement;


    protected Button.ButtonClickedEvent _onClick = new Button.ButtonClickedEvent();

    protected bool _isPointerDown;
    protected bool _isSelected;

    [HideInNormalInspector] [SerializeField] private Selectable _selectable;

    //public Button.ButtonClickedEvent onClick;

    //private void Reset()
    //{
    //    _button = GetComponent<Button>();
    //    _button.transition = Selectable.Transition.None;
    //}

    protected void Awake()
    {
        try
        {
            _selectable = GetComponent<Selectable>();
            _interactable = _selectable.interactable;
        }
        catch (System.Exception)
        {
            Debug.LogError($"Please add a selectable UI component to {name}");
        }
    }

    protected override void ApplyTheme()
    {
        if (ThemeManager.Instance == null || ThemeManager.Instance.currentTheme == null)
            return;

        ColorTheme theme = ThemeManager.Instance.currentTheme;

        SelectableGroup textGroup = theme.GetGroupByGuid<SelectableGroup>(this, _colorsGuid);

        SelectableColorGroup _colors = textGroup.colors;


        if (_image == null)
            _image = GetComponent<Image>();

        if (_isSelected)
        {
            ThemeApplier.ApplyColorSettings(_image, _isPointerDown ? _colors.pressColors.primaryColor : _colors.selectedColors.primaryColor);
            //if (_contentText != null && _contentText.gameObject.activeSelf)
            //{
            //    ThemeApplier.ApplyColorSettings(_contentText, _selectedContentColorSettings);
            //    ThemeApplier.ApplyTextSettings(_contentText, _textGuid);
            //}
            //if (_contentImage != null && _contentImage.gameObject.activeSelf)
            //{
            //    ThemeApplier.ApplyColorSettings(_contentImage, _selectedContentColorSettings);
            //}
            //if (_underlineImage != null && _underlineImage.gameObject.activeSelf)
            //{
            //    ThemeApplier.ApplyColorSettings(_underlineImage, _selectedContentColorSettings);
            //}

            _contentTexts.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, _isPointerDown ? _colors.pressColors.secondaryColor : _colors.selectedColors.secondaryColor);
                ThemeApplier.ApplyTextSettings(x, _textGuid);
            });

            _subTexts.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, _isPointerDown ? _colors.pressColors.secondaryColor : _colors.selectedColors.secondaryColor);
                ThemeApplier.ApplyTextSettings(x, _subTextGuid);
            });

            _contentImages.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, _isPointerDown ? _colors.pressColors.secondaryColor : _colors.selectedColors.secondaryColor);
            });

            _tertiaryColorTexts.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, _isPointerDown ? _colors.pressColors.tertiaryColor : _colors.selectedColors.tertiaryColor);
                ThemeApplier.ApplyTextSettings(x, _textGuid);
            });

            _tertiaryColorImages.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, _isPointerDown ? _colors.pressColors.tertiaryColor : _colors.selectedColors.tertiaryColor);
            });
        }
        else
        {
            ThemeApplier.ApplyColorSettings(_image, IsInteractable() ? (_isPointerDown ? _colors.pressColors.primaryColor : _colors.defaultColors.primaryColor) : _colors.disabledColors.primaryColor);
            ThemeApplier.ApplyImageBorder(_image, _borderStyle, -1, _isInnerElement);

            _contentTexts.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, IsInteractable() ? (_isPointerDown ? _colors.pressColors.secondaryColor : _colors.defaultColors.secondaryColor) : _colors.disabledColors.secondaryColor);
                ThemeApplier.ApplyTextSettings(x, _textGuid);
            });

            _subTexts.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, IsInteractable() ? (_isPointerDown ? _colors.pressColors.secondaryColor : _colors.defaultColors.secondaryColor) : _colors.disabledColors.secondaryColor);
                ThemeApplier.ApplyTextSettings(x, _subTextGuid);
            });

            _contentImages.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, IsInteractable() ? (_isPointerDown ? _colors.pressColors.secondaryColor : _colors.defaultColors.secondaryColor) : _colors.disabledColors.secondaryColor);
            });

            _tertiaryColorTexts.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, IsInteractable() ? (_isPointerDown ? _colors.pressColors.tertiaryColor : _colors.defaultColors.tertiaryColor) : _colors.disabledColors.tertiaryColor);
                ThemeApplier.ApplyTextSettings(x, _textGuid);
            });

            _tertiaryColorImages.ForEach(x =>
            {
                ThemeApplier.ApplyColorSettings(x, IsInteractable() ? (_isPointerDown ? _colors.pressColors.tertiaryColor : _colors.defaultColors.tertiaryColor) : _colors.disabledColors.tertiaryColor);
            });

            //if (_contentText != null && _contentText.gameObject.activeSelf)
            //{
            //    ThemeApplier.ApplyColorSettings(_contentText, IsInteractable() ? (_isPointerDown ? _pressContentColorSettings : _defaultContentColorSettings) : _disabledContentColorSettings);
            //    ThemeApplier.ApplyTextSettings(_contentText, _textGuid);
            //}

            //if (_contentImage != null && _contentImage.gameObject.activeSelf)
            //{
            //    ThemeApplier.ApplyColorSettings(_contentImage, IsInteractable() ? (_isPointerDown ? _pressContentColorSettings : _defaultContentColorSettings) : _disabledContentColorSettings);
            //}
            //if (_underlineImage != null && _underlineImage.gameObject.activeSelf)
            //{
            //    ThemeApplier.ApplyColorSettings(_underlineImage, IsInteractable() ? (_isPointerDown ? _pressContentColorSettings : _defaultContentColorSettings) : _disabledContentColorSettings);
            //}
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        ApplyTheme();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
        ApplyTheme();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable())
            return;

        //onClick?.Invoke();
        Debug.Log("Themed button was pressed.");
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Themed button was selected.");

        _isSelected = true;
        ApplyTheme();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("Themed button was deselected.");

        _isSelected = false;
        ApplyTheme();
    }

    public void SetInteractable(bool value)
    {
        _selectable.interactable = value;
    }

    protected void Update()
    {
        if (_interactable != IsInteractable())
        {
            ApplyTheme();
            _interactable = IsInteractable();
        }
    }

    public void SetColors(string colorsGuid)
    {
        _colorsGuid = colorsGuid;
        ApplyTheme();
    }

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

    public bool IsInteractable()
    {
        if (_selectable == null)
            _selectable = GetComponent<Selectable>();

        return _selectable.interactable;
    }
}


[System.Serializable]
public class SelectableColorGroup
{
    public MultiColorGroup defaultColors;
    public MultiColorGroup pressColors;
    public MultiColorGroup selectedColors;
    public MultiColorGroup disabledColors;
}

[System.Serializable]
public class MultiColorGroup
{
    [ColorSettings("Primary Color")] public ColorSettings primaryColor;
    [ColorSettings("Secondary Color")] public ColorSettings secondaryColor;
    [ColorSettings("Tertiary Color")] public ColorSettings tertiaryColor;
}