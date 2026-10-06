using System;

namespace Stats.Widgets.Filters;

public abstract class Filter : Widgets_Legacy.Widget
{
    public abstract bool IsActive { get; }

    public abstract event Action? OnChange;

    public abstract bool Eval(int i);

    protected abstract class RelOperator<TLhs, TRhs>
    {
        protected RelOperator(string symbol = "", string description = "")
        {
            Symbol = symbol;
            Description = description;
        }

        public string Symbol { get; }

        public string Description { get; }

        public abstract bool Eval(TLhs lhs, TRhs rhs);
    }
}
