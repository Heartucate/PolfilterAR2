using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeApplier : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    //public static void ApplyImageColor(Image image, ColorSettings colorSettings)
    //{
    //    if (ThemeManager.Instance != null && ThemeManager.Instance.currentTheme != null)
    //    {
    //        ColorTheme theme = ThemeManager.Instance.currentTheme;

    //        ColorTheme.ColorStyle colorStyle = colorSettings.colorStyle == ColorTheme.ColorStyle.Default ? theme.defaultColorStyle : colorSettings.colorStyle;
    //        UIGradient customGradient = image.GetComponent<UIGradient>();

    //        switch (colorStyle)
    //        {
    //            case ColorTheme.ColorStyle.Color:
    //                image.color = theme.GetGroupByGuid<ColorGroup>(image, colorSettings.colorGuid).color;

    //                if (customGradient != null)
    //                    customGradient.enabled = false;

    //                break;

    //            case ColorTheme.ColorStyle.Gradient:
    //                image.color = Color.white;

    //                if (customGradient == null)
    //                    customGradient = image.gameObject.AddComponent<UIGradient>();

    //                customGradient.enabled = false;
    //                GradientGroup group = theme.GetGroupByGuid<GradientGroup>(image, colorSettings.gradientGuid);
    //                customGradient.m_color1 = group.color1;
    //                customGradient.m_color2 = group.color2;
    //                customGradient.m_angle = theme.defaultGradientAngle;
    //                customGradient.enabled = true;
    //                break;
    //        }
    //    }
    //}
    public static void ApplyColorSettings(Component component, ColorSettings colorSettings)
    {
        if (ThemeManager.Instance == null || ThemeManager.Instance.currentTheme == null)
            return;

        if (colorSettings == null)
            return;

        ColorTheme theme = ThemeManager.Instance.currentTheme;
        ColorTheme.ColorStyle colorStyle = colorSettings.colorStyle == ColorTheme.ColorStyle.Default ? theme.defaultColorStyle : colorSettings.colorStyle;

        if (component is Graphic graphic)
        {
            ApplyColor(graphic, colorSettings, theme, colorStyle);
        }
        else
        {
            Debug.LogError($"Unsupported component type.");
        }
    }

    private static void ApplyColor(Graphic graphic, ColorSettings colorSettings, ColorTheme theme, ColorTheme.ColorStyle colorStyle)
    {
        if (colorSettings == null)
            return;

        switch (colorStyle)
        {
            case ColorTheme.ColorStyle.Color:
                ApplySolidColor(graphic, theme, colorSettings.colorGuid, colorSettings.alpha);
                DisableGradients(graphic);
                break;

            case ColorTheme.ColorStyle.Gradient:
                ApplyGradientColor(graphic, theme, colorSettings.gradientGuid, colorSettings.alpha);
                break;
        }
    }

    private static void ApplySolidColor(Graphic graphic, ColorTheme theme, string colorGuid, float alpha)
    {
        Color color = theme.GetGroupByGuid<ColorGroup>(graphic, colorGuid).color;
        color.a *= alpha;
        graphic.color = color;
    }

    private static void ApplyGradientColor(Graphic graphic, ColorTheme theme, string gradientGuid, float alpha)
    {
        graphic.color = Color.white;
        GradientGroup group = theme.GetGroupByGuid<GradientGroup>(graphic, gradientGuid);

        if (group == null)
            return;

        if (graphic is Image image)
        {
            UIGradient uiGradient = image.GetComponent<UIGradient>() ?? image.gameObject.AddComponent<UIGradient>();
            ConfigureUIGradient(uiGradient, group, theme.defaultGradientAngle, alpha);
        }
        else if (graphic is TextMeshProUGUI tmpText)
        {
            TMPTextGradient tmpGradient = tmpText.GetComponent<TMPTextGradient>() ?? tmpText.gameObject.AddComponent<TMPTextGradient>();
            ConfigureTMPTextGradient(tmpGradient, group, theme.defaultGradientAngle, alpha);
        }
    }

    private static void DisableGradients(Graphic graphic)
    {
        UIGradient uiGradient = graphic.GetComponent<UIGradient>();
        TMPTextGradient tmpGradient = graphic.GetComponent<TMPTextGradient>();

        if (uiGradient != null) uiGradient.enabled = false;
        if (tmpGradient != null) tmpGradient.enabled = false;
    }

    private static void ConfigureUIGradient(UIGradient uiGradient, GradientGroup group, float angle, float alpha)
    {
        uiGradient.enabled = false;
        uiGradient.m_color1 = group.color1;
        uiGradient.m_color1.a *= alpha;
        uiGradient.m_color2 = group.color2;
        uiGradient.m_color2.a *= alpha;
        uiGradient.m_angle = angle;
        uiGradient.enabled = true;
    }

    private static void ConfigureTMPTextGradient(TMPTextGradient tmpGradient, GradientGroup group, float angle, float alpha)
    {
        tmpGradient.m_color1 = group.color1;
        tmpGradient.m_color1.a *= alpha;
        tmpGradient.m_color2 = group.color2;
        tmpGradient.m_color2.a *= alpha;
        tmpGradient.m_angle = angle;
        tmpGradient.enabled = true;
    }


    public static void ApplyImageBorder(Image image, ColorTheme.BorderStyle style, float overriddenMultiplier, bool isInnerBorder)
    {
        if (ThemeManager.Instance == null || ThemeManager.Instance.currentTheme == null)
            return;

        ColorTheme theme = ThemeManager.Instance.currentTheme;
        ColorTheme.BorderStyle borderStyle = style == ColorTheme.BorderStyle.Default ? theme.defaultBorderStyle : style;

        switch (borderStyle)
        {
            case ColorTheme.BorderStyle.Square:
                if (image.sprite != theme.defaultBorderSprite)
                    return;

                image.sprite = null;
                break;

            case ColorTheme.BorderStyle.Round:
                if (image.sprite != theme.defaultBorderSprite && image.sprite != null)
                    return;

                image.sprite = theme.defaultBorderSprite;
                image.type = Image.Type.Sliced;
                image.pixelsPerUnitMultiplier = (overriddenMultiplier <= 0 ? theme.defaultBorderMultiplier : overriddenMultiplier);

                if(isInnerBorder)
                    image.pixelsPerUnitMultiplier += image.pixelsPerUnitMultiplier * theme.defaultInnerBorderMultiplier;

                break;
        }
    }

    public static void ApplyTextSettings(TextMeshProUGUI text, string textGuid)
    {
        if (ThemeManager.Instance == null || ThemeManager.Instance.currentTheme == null)
            return;

        ColorTheme theme = ThemeManager.Instance.currentTheme;

        TextGroup textGroup = theme.GetGroupByGuid<TextGroup>(text, textGuid);

        text.fontSize = textGroup.fontSize;
        text.font = textGroup.fontAsset;
    }
}

[System.Serializable]
public class ColorSettings
{
    public ColorTheme.ColorStyle colorStyle;
    [ThemeColor] public string colorGuid;
    [ThemeGradient] public string gradientGuid;
    public float alpha = 1;
}