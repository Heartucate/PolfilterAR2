using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ColorTheme", menuName = "Theme/ColorTheme")]
public class ColorTheme : ScriptableObject
{
    public enum ColorStyle
    {
        Default,
        Color,
        Gradient
    }

    public enum BorderStyle
    {
        Default,
        Square,
        Round
    }

    [Header("COLORS & GRADIENTS")]
    public List<ColorGroup> colorGroups = new List<ColorGroup>();
    public List<GradientGroup> gradientGroups = new List<GradientGroup>();
    public List<TextGroup> textGroups = new List<TextGroup>();
    public List<SelectableGroup> selectableGroups = new List<SelectableGroup>();
    public ColorStyle defaultColorStyle;
    [Range(-180,180)] public float defaultGradientAngle;
    public BorderStyle defaultBorderStyle;
    public Sprite defaultBorderSprite;
    [Range(0.01f, 100)] public float defaultBorderMultiplier;
    [Range(0, 1)] public float defaultInnerBorderMultiplier;

    public T GetGroupByGuid<T>(Component caller, string guid) where T : ThemeGroup
    {
        ThemeGroup group = null;

        if (typeof(T) == typeof(ColorGroup))
        {
            group = colorGroups.Find(g => g.guid == guid) as T;
        }
        else if (typeof(T) == typeof(GradientGroup))
        {
            group = gradientGroups.Find(g => g.guid == guid) as T;
        }
        else if (typeof(T) == typeof(TextGroup))
        {
            group = textGroups.Find(g => g.guid == guid) as T;
        }
        else if (typeof(T) == typeof(SelectableGroup))
        {
            group = selectableGroups.Find(g => g.guid == guid) as T;
        }

        if (group == null)
        {
            Debug.LogError($"Cannot find group of type {typeof(T).Name} for {caller.gameObject.name}. Guid {guid} does not exist in current Theme.");
        }

        return group as T;
    }

    private void OnValidate()
    {
        if (ThemeManager.Instance != null && ThemeManager.Instance.currentTheme == this)
            ThemeManager.Instance.UpdateTheme();
    }
}
