using System;
using UnityEngine;

namespace Stats.Widgets.Filters;

public sealed class StringFilter : FilterWithInputField<string, string>
{
    private string _value = "";
    private RelOperator<string, string> _operator = Operators.Default;
    private readonly Func<int, string> _getValue;

    public StringFilter(Func<int, string> getValue, string? placeholder = null) : base(Operators.List, placeholder)
    {
        _getValue = getValue;
    }

    public override bool IsActive => Value.Length > 0;

    private string Value
    {
        get => _value;
        set
        {
            if (_value == value)
            {
                return;
            }

            _value = value;
            Resize();
            OnChange?.Invoke();
        }
    }

    protected override RelOperator<string, string> Operator
    {
        get => _operator;
        set
        {
            if (_operator == value)
            {
                return;
            }

            _operator = value;
            Resize();
            OnChange?.Invoke();
        }
    }

    protected override string InputFieldText => Value;

    public override event Action? OnChange;

    protected override void DrawInputField(Rect rect)
    {
        Value = GUI.TextField(rect, Value);
    }

    public override bool Eval(int i)
    {
        return Operator.Eval(_getValue(i), Value);
    }

    public override void Reset()
    {
        _operator = Operators.Default;
        ClearInputField();
    }

    protected override void ClearInputField()
    {
        _value = "";
        Resize();
        OnChange?.Invoke();
    }

    public override void NotifyChanged()
    {
        OnChange?.Invoke();
    }

    private static class Operators
    {
        public static RelOperator<string, string> Default => Contains.Instance;

        public static RelOperator<string, string>[] List { get; } = [
            Contains.Instance,
            NotContains.Instance,
        ];

        public sealed class Contains : RelOperator<string, string>
        {
            public Contains() : base("~=", "Contains") { }

            public override bool Eval(string lhs, string rhs) => lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase);

            public static Contains Instance { get; } = new();
        }

        public sealed class NotContains : RelOperator<string, string>
        {
            public NotContains() : base("!~=", "Does not contains") { }

            public override bool Eval(string lhs, string rhs) => lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase) == false;

            public static NotContains Instance { get; } = new();
        }
    }
}
