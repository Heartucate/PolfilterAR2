using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class SelectableSettingsAttribute : PropertyAttribute
{
    public string Heading { get; private set; }

    public SelectableSettingsAttribute(string heading = "")
    {
        Heading = heading;
    }
}
