using System;
using System.Collections.Generic;

namespace Stats.Extensions;

public static class System_Function
{
    // Memoize every function call.
    public static Func<TArg, TRes> Memoized<TArg, TRes>(this Func<TArg, TRes> function)
    {
        Dictionary<TArg, TRes> cache = [];

        return (arg) =>
        {
            bool resultIsCached = cache.TryGetValue(arg, out TRes? result);

            if (resultIsCached)
            {
                return result;
            }

            return cache[arg] = function(arg);
        };
    }

    public static Func<TArg, TRes> Safe<TArg, TRes>(this Func<TArg, TRes> function)
    {
        return (arg) =>
        {
            try
            {
                return function(arg);
            }
            catch
            {
                return default;
            }
        };
    }
}
