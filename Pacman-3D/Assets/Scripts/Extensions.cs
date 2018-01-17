using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    public static int Next<T>(this List<T> list, int index)
    {
        return (index + 1) % list.Count;
    }

    public static int Previous<T>(this List<T> list, int index)
    {
        return (list.Count + index - 1) % list.Count;
    }
}
