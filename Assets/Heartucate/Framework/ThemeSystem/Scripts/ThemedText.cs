using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemedText : ThemedComponent
{
    private TextMeshProUGUI _textField;
    [FontSettings] public string textGuid;
    [SerializeField] [ColorSettings] private ColorSettings _colorSettings;


    private void Awake()
    {
        _textField = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        if (_textField == null)
            Awake();

        _textField.text = text;
    }

    public void SetColorSettings(ColorSettings settings)
    {
        _colorSettings = settings;
        ApplyTheme();
    }

    protected override void ApplyTheme()
    {
        if(_textField == null)
            _textField = GetComponent<TextMeshProUGUI>();

        ThemeApplier.ApplyColorSettings(_textField, _colorSettings);
        ThemeApplier.ApplyTextSettings(_textField, textGuid);
    }
}
