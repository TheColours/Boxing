using System;
using System.Collections;
using UnityEngine;

public static class Common 
{
    public static Vector3 With(this Vector3 v, float ?x = null, float ?y = null, float ?z = null)
    {
        v.x = x != null ? (float)x : v.x;
        v.y = y != null ? (float)y : v.y;
        v.z = z != null ? (float)z : v.z;
        return v;
    }
    public static Vector2 With(this Vector2 v, float? x = null, float? y = null)
    {
        v.x = x != null ? (float)x : v.x;
        v.y = y != null ? (float)y : v.y;
        return v;
    }
    public static Color With(this Color color, float ?a)
    {
        color.a = a != null ? (float)a : color.a;
        return color;
    }
    public static IEnumerator DelayEndOfFrame(Action callback)
    {
        yield return new WaitForEndOfFrame();
        callback?.Invoke();
    }
    public static IEnumerator Delay(float inteval, Action callback)
    {
        yield return new WaitForSeconds(inteval);
        callback?.Invoke();
    }
}