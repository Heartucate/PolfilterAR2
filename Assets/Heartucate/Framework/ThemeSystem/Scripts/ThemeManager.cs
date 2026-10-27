using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }
    public ColorTheme currentTheme;
    private event Action OnThemeChanged;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("ThemeManager initialized");
            //if (Application.isPlaying)
            //{
            //    DontDestroyOnLoad(gameObject);
            //}
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        UpdateTheme();
    }

    public void Subscribe(Action callback)
    {
        OnThemeChanged += callback;
    }

    public void Unsubscribe(Action callback)
    {
        OnThemeChanged -= callback;
    }

    public void UpdateTheme()
    {
        OnThemeChanged?.Invoke();
    }
}
