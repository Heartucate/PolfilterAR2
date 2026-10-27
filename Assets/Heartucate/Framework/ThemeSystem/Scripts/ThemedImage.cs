using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ThemedImage : ThemedComponent
{
    private Image _image;
    private RectTransform _rectTransform;

    [FormerlySerializedAs("colorSettings")][SerializeField] [ColorSettings] private ColorSettings _colorSettings;
    [SerializeField] private ColorTheme.BorderStyle _borderStyle;
    [SerializeField] private bool _overrideBorderMultiplier;
    [SerializeField] [Range(0,100)] private float _overriddenBorderMultiplier;
    [SerializeField] private bool _isInnerElement;
    //[SerializeField] [Range(0, 100)] private float _innerElementMultiplier = 1;

    private new void OnEnable()
    {
        if(_image == null)
            _image = GetComponent<Image>();

        _image.enabled = true;

        base.OnEnable();
    }

    private new void OnDisable()
    {
        if (_image == null)
            _image = GetComponent<Image>();

        _image.enabled = false;

        base.OnDisable();
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    protected override void ApplyTheme()
    {
        if(_image == null)
            _image = GetComponent<Image>();

        if (_isInnerElement && _rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        ThemeApplier.ApplyColorSettings(_image, _colorSettings);

        //float innerMultiplier = _isInnerElement ? _rectTransform.offsetMin.x * _innerElementMultiplier : 0;

        //if (_isInnerElement)
        //    Debug.Log(innerMultiplier);

        ThemeApplier.ApplyImageBorder(_image, _borderStyle, _overrideBorderMultiplier ? _overriddenBorderMultiplier : -1, _isInnerElement);
    }

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetColorSettings(ColorSettings settings)
    {
        _colorSettings = settings;
        ApplyTheme();
    }

    public void SetBorderStyle(ColorTheme.BorderStyle style)
    {
        _borderStyle = style;
        ApplyTheme();
    }
}
