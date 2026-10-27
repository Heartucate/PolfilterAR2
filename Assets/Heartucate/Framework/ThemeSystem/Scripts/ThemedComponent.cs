using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public abstract class ThemedComponent : MonoBehaviour
{
    private bool _isSubscribed;

    protected virtual void Start()
    {
        if (!_isSubscribed)
            OnEnable();
    }

    protected virtual void OnEnable()
    {
        if (ThemeManager.Instance == null || ThemeManager.Instance.currentTheme == null)
            return;

        ThemeManager.Instance.Subscribe(ApplyTheme);
        ApplyTheme();
        _isSubscribed = true;
    }

    protected virtual void OnDisable()
    {
        if (ThemeManager.Instance == null || ThemeManager.Instance.currentTheme == null)
            return;

        ThemeManager.Instance.Unsubscribe(ApplyTheme);
        _isSubscribed = false;
    }

    protected virtual void OnDestroy()
    {
        if (_isSubscribed)
            OnDisable();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (ThemeManager.Instance == null || ThemeManager.Instance.currentTheme == null)
            return;

        if (gameObject.activeInHierarchy)
        {
            Undo.RecordObject(this, "Apply Theme Changes");
            ApplyTheme();
            EditorUtility.SetDirty(this);
        }
    }
#endif

    protected abstract void ApplyTheme();
}

