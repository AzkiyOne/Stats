using System;
using Verse;

namespace Stats.NumberFormats;

public abstract class AbstractTemperature : NumberFormat, IDynamicNumberFormat
{
    private readonly string[] _formatStrings;

    public AbstractTemperature(NumberFormatProps props) : this(props.pattern)
    {
    }

    public AbstractTemperature(string pattern) : base(pattern)
    {
        const int TempModesCount = 3;
        _formatStrings = new string[TempModesCount];
        _formatStrings[(byte)TemperatureDisplayMode.Celsius] = pattern + "\\C";
        _formatStrings[(byte)TemperatureDisplayMode.Fahrenheit] = pattern + "\\F";
        _formatStrings[(byte)TemperatureDisplayMode.Kelvin] = pattern + "\\K";
    }

    protected override string FormatString => _formatStrings[(byte)Prefs.TemperatureMode];

    public event Action? OnChange
    {
        add => Events.PrefsChanged += value;
        remove => Events.PrefsChanged -= value;
    }
}

public sealed class TemperatureOffset : AbstractTemperature
{
    private readonly float[] _valueOffsets;

    public TemperatureOffset(NumberFormatProps props) : this(props.pattern)
    {
    }

    public TemperatureOffset(string pattern) : base(pattern)
    {
        const int TempModesCount = 3;
        _valueOffsets = new float[TempModesCount];
        _valueOffsets[(byte)TemperatureDisplayMode.Celsius] = 1f;
        _valueOffsets[(byte)TemperatureDisplayMode.Fahrenheit] = 1.8f;
        _valueOffsets[(byte)TemperatureDisplayMode.Kelvin] = 1f;
    }

    public override decimal ToDecimal(float value)
    {
        value *= _valueOffsets[(byte)Prefs.TemperatureMode];

        return base.ToDecimal(value);
    }
}

public sealed class Temperature : AbstractTemperature
{
    public Temperature(NumberFormatProps props) : this(props.pattern)
    {
    }

    public Temperature(string pattern) : base(pattern)
    {
    }

    public override decimal ToDecimal(float value)
    {
        value = Prefs.TemperatureMode switch
        {
            TemperatureDisplayMode.Celsius => value,
            TemperatureDisplayMode.Fahrenheit => value * 1.8f + 32f,
            TemperatureDisplayMode.Kelvin => value + 273.15f,
            _ => throw new InvalidOperationException(),
        };

        return base.ToDecimal(value);
    }
}
