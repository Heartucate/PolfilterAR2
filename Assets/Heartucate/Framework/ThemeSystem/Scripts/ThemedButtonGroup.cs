using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ThemedButtonGroup : MonoBehaviour
{
    public delegate void OnUpdateSelectedButtonDelegate(ThemedButton button, int index);
    public OnUpdateSelectedButtonDelegate onUpdateSelectedButton;

    [SerializeField][SelectableSettings] protected string _selectedColorsGuid;
    [SerializeField][SelectableSettings] protected string _unselectedColorsGuid;

    private List<ThemedButton> _childButtons = new();

    private readonly Dictionary<ThemedButton, UnityAction> _buttonActions
    = new Dictionary<ThemedButton, UnityAction>();

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ThemedButton themedButton = transform.GetChild(i).GetComponent<ThemedButton>();

            if (themedButton == null)
                continue;

            UnityAction action = () => OnButtonClicked(themedButton);
            _buttonActions[themedButton] = action;
            themedButton.Button.onClick.AddListener(action);
            _childButtons.Add(themedButton);
        }
    }

    private void OnDestroy()
    {
        foreach (var kv in _buttonActions)
            kv.Key.Button.onClick.RemoveListener(kv.Value);

        _buttonActions.Clear();
        _childButtons.Clear();
    }

    public void TrySelectButtonIndex(int index, bool notify = true)
    {
        if (_childButtons.Count - 1 < index)
        {
            Debug.LogError($"Could not select button with index {index}. Index is of range (length: {_childButtons.Count}).");
            return;
        }

        OnButtonClicked(_childButtons[index], notify);
    }

    public void TrySelectButtonId(string buttonId, bool notify = true)
    {
        ThemedButton button = null;
        foreach (var item in _childButtons)
        {
            if(item.id == buttonId)
                button = item;
        }

        if(button == null)
        {
            Debug.LogError($"Could not select button with id {buttonId}. Is was not found in the group.");
            return;
        }    

        OnButtonClicked(button, notify);
    }

    private void OnButtonClicked(ThemedButton button, bool notify = true)
    {
        _childButtons.ForEach(x =>
        {
            x.SetColors(button == x ? _selectedColorsGuid : _unselectedColorsGuid);
        });

        if (notify)
        {
            onUpdateSelectedButton?.Invoke(button, _childButtons.IndexOf(button));
        }

        Debug.Log($"Button {button.name} selected.");
    }
}
