using System;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Filters;

public sealed class NumberFilter : FilterWithInputField<decimal, decimal>
{
    private decimal _value = 0m;
    private RelOperator<decimal, decimal> _operator = Operators.Default;
    private readonly Func<int, decimal> _getValue;
    private bool _inputIsValid = true;
    private string _textFieldText = "";
    private static readonly Color _errorColor = Color.red.ToTransparent(0.5f);

    public NumberFilter(Func<int, decimal> getValue, string? placeholder = null) : base(Operators.List, placeholder)
    {
        _getValue = getValue;
    }

    public override bool IsActive => _textFieldText.Length > 0 && _inputIsValid;

    private decimal Value
    {
        get => _value;
        set
        {
            // We don't check if value has changed here because:
            // - It will cause it to not update when it should.
            //   For example, when you input 0 into an empty
            //   input field.
            // - It is already checked in TextFieldText.

            _value = value;
            OnChange?.Invoke();
        }
    }

    protected override RelOperator<decimal, decimal> Operator
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

    public override event Action? OnChange;

    private string TextFieldText
    {
        set
        {
            if (_textFieldText == value)
            {
                return;
            }

            _textFieldText = value.Trim();

            if (_textFieldText.Length == 0)
            {
                _inputIsValid = true;
                Value = 0m;
            }
            else
            {
                _inputIsValid = decimal.TryParse(_textFieldText, out var num);

                if (_inputIsValid)
                {
                    Value = num;
                }
                else
                {
                    OnChange?.Invoke();
                }
            }

            Resize();
        }
    }

    protected override string InputFieldText => _textFieldText;

    protected override void DrawInputField(Rect rect)
    {
        if (_inputIsValid == false)
        {
            Verse.Widgets.DrawBoxSolid(rect, _errorColor);
        }

        TextFieldText = GUI.TextField(rect, _textFieldText);
    }

    public override bool Eval(int i)
    {
        return Operator.Eval(_getValue(i), Value);
    }

    protected override void ClearInputField()
    {
        _textFieldText = "";
        _inputIsValid = true;
        _value = 0m;
        Resize();
        OnChange?.Invoke();
    }

    private static class Operators
    {
        public static RelOperator<decimal, decimal> Default => IsGreaterThan.Instance;

        public static RelOperator<decimal, decimal>[] List { get; } = [
            IsEqualTo.Instance,
            IsNotEqualTo.Instance,
            IsGreaterThan.Instance,
            IsLesserThan.Instance,
            IsGreaterThanOrEqualTo.Instance,
            IsLesserThanOrEqualTo.Instance,
        ];

        public sealed class IsEqualTo : RelOperator<decimal, decimal>
        {
            private IsEqualTo() : base("==") { }

            public override bool Eval(decimal lhs, decimal rhs) => lhs == rhs;

            public static IsEqualTo Instance { get; } = new();
        }

        public sealed class IsNotEqualTo : RelOperator<decimal, decimal>
        {
            private IsNotEqualTo() : base("!=") { }

            public override bool Eval(decimal lhs, decimal rhs) => lhs != rhs;

            public static IsNotEqualTo Instance { get; } = new();
        }

        public sealed class IsGreaterThan : RelOperator<decimal, decimal>
        {
            private IsGreaterThan() : base(">") { }

            public override bool Eval(decimal lhs, decimal rhs) => lhs > rhs;

            public static IsGreaterThan Instance { get; } = new();
        }

        public sealed class IsLesserThan : RelOperator<decimal, decimal>
        {
            private IsLesserThan() : base("<") { }

            public override bool Eval(decimal lhs, decimal rhs) => lhs < rhs;

            public static IsLesserThan Instance { get; } = new();
        }

        public sealed class IsGreaterThanOrEqualTo : RelOperator<decimal, decimal>
        {
            private IsGreaterThanOrEqualTo() : base(">=") { }

            public override bool Eval(decimal lhs, decimal rhs) => lhs >= rhs;

            public static IsGreaterThanOrEqualTo Instance { get; } = new();
        }

        public sealed class IsLesserThanOrEqualTo : RelOperator<decimal, decimal>
        {
            private IsLesserThanOrEqualTo() : base("<=") { }

            public override bool Eval(decimal lhs, decimal rhs) => lhs <= rhs;

            public static IsLesserThanOrEqualTo Instance { get; } = new();
        }
    }
}
