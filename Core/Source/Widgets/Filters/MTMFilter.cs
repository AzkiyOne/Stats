using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.Widgets.Filters;

public sealed class MTMFilter<TOption> : NTMFilter<IEnumerable<TOption>, TOption>
{
    public MTMFilter(Func<int, IEnumerable<TOption>> getValue, IEnumerable<NTMFilterOption<TOption>> options, string? label = null)
        : base(getValue, options, Operators.List, Operators.IntersectsWith.Instance, label)
    {
    }

    public override bool IsActive =>
        base.IsActive
        || Operator == Operators.IsEqualTo.Instance
        || Operator == Operators.IsNotEqualTo.Instance;

    // Reasons for using "! + operator" instead of proper negated set operator symbols:
    // - It looks like there is no negated form for "∩".
    // - Because of font the game uses, "⊄"/"⊅" are unrecognizable in the game.
    private static class Operators
    {
        public static RelOperator<IEnumerable<TOption>, HashSet<TOption>>[] List { get; } = [
            IntersectsWith.Instance,
            NotIntersectsWith.Instance,
            IsSubsetOf.Instance,
            IsSupersetOf.Instance,
            IsEqualTo.Instance,
            IsNotEqualTo.Instance,
        ];

        public sealed class IsEqualTo : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsEqualTo() : base("==", "Is equal to") { }

            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.SetEquals(lhs);

            public static IsEqualTo Instance { get; } = new();
        }

        public sealed class IsNotEqualTo : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsNotEqualTo() : base("!=", "Is not equal to") { }

            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.SetEquals(lhs) == false;

            public static IsNotEqualTo Instance { get; } = new();
        }

        public sealed class IntersectsWith : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IntersectsWith() : base("∩", "Contains at least one of") { }

            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains);

            public static IntersectsWith Instance { get; } = new();
        }

        public sealed class NotIntersectsWith : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private NotIntersectsWith() : base("!∩", "Does not contain any of") { }

            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains) == false;

            public static NotIntersectsWith Instance { get; } = new();
        }

        public sealed class IsSubsetOf : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsSubsetOf() : base("⊆", "Is subset of") { }

            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.IsSupersetOf(lhs);

            public static IsSubsetOf Instance { get; } = new();
        }

        public sealed class IsSupersetOf : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsSupersetOf() : base("⊇", "Is superset of") { }

            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.IsSubsetOf(lhs);

            public static IsSupersetOf Instance { get; } = new();
        }
    }
}
