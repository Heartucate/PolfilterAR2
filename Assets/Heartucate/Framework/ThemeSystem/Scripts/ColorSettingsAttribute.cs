using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ColorSettingsAttribute : PropertyAttribute
{
    public string Heading { get; private set; }

    public ColorSettingsAttribute(string heading = "")
    {
        Heading = heading;
    }
}
