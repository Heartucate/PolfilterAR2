using System;
using UnityEngine;

public abstract class ThemeGroup
{
    [HideInNormalInspector] [SerializeField] private string _name;
    [HideInNormalInspector] [SerializeField] private string _guid;

    public string name { get => _name; }
    public string guid
    {
        get
        {
            if (string.IsNullOrEmpty(_guid))
                _guid = Guid.NewGuid().ToString();

            return _guid;
        }
    }
}