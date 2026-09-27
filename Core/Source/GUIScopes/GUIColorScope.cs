using System;
using UnityEngine;

namespace Stats.GUIScopes;

internal readonly record struct GUIColorScope : IDisposable
{
    private readonly Color _color;

    public GUIColorScope(Color color)
    {
        _color = GUI.color;
        GUI.color = color;
    }

    public void Dispose() => GUI.color = _color;
}
