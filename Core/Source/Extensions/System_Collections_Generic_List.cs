using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stats.Extensions;

public static class System_Collections_Generic_List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Replace<T>(this List<T> list, int i1, int i2)
    {
        list[i1] = list[i2];
        list.RemoveAt(i2);
    }

    internal static void CopyTo<T>(this List<T> list, Span<T> span, int start)
    {
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = list[start + i];
        }
    }

    internal static void CopyTo<T>(this List<T> list, Span<T> span)
    {
        list.CopyTo(span, 0);
    }

    internal static void MoveBeforeElem<T>(this List<T> list, T target, T elem)
    {
        list.MoveBeforeElemAt(list.IndexOf(target), list.IndexOf(elem));
    }

    internal static void MoveBeforeElemAt<T>(this List<T> list, int targetIndex, int elemIndex)
    {
        T item = list[targetIndex];

        list.RemoveAt(targetIndex);

        if (targetIndex < elemIndex)
        {
            elemIndex--;
        }

        list.Insert(elemIndex, item);
    }

    internal static void MoveAfterElem<T>(this List<T> list, T target, T elem)
    {
        list.MoveAfterElemAt(list.IndexOf(target), list.IndexOf(elem));
    }

    internal static void MoveAfterElemAt<T>(this List<T> list, int targetIndex, int elemIndex)
    {
        list.MoveBeforeElemAt(targetIndex, elemIndex + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(this List<T> list, int i1, int i2)
    {
        (list[i2], list[i1]) = (list[i1], list[i2]);
    }
}
