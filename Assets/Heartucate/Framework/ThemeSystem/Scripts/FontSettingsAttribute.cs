using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class FontSettingsAttribute : PropertyAttribute
{
    public string Heading { get; private set; }

    public FontSettingsAttribute(string heading = "")
    {
        Heading = heading;
    }
}
