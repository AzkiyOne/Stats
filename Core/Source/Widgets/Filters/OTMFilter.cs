using System;
using System.Collections.Generic;

namespace Stats.Widgets.Filters;

public sealed class OTMFilter<TOption> : NTMFilter<TOption, TOption>
{
    public OTMFilter(Func<int, TOption> getValue, IEnumerable<NTMFilterOption<TOption>> options, string? label = null)
        : base(getValue, options, Operators.List, Operators.IsIn.Instance, label)
    {
    }

    private static class Operators
    {
        public static RelOperator<TOption, HashSet<TOption>>[] List { get; } = [
            IsIn.Instance,
            IsNotIn.Instance,
        ];

        public sealed class IsIn : RelOperator<TOption, HashSet<TOption>>
        {
            private IsIn() : base("∈", "Is one of") { }

            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs);

            public static IsIn Instance { get; } = new();
        }

        // ∉
        public sealed class IsNotIn : RelOperator<TOption, HashSet<TOption>>
        {
            private IsNotIn() : base("!∈", "Is not one of") { }

            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs) == false;

            public static IsNotIn Instance { get; } = new();
        }
    }
}
