using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLib
{
    public delegate float Function(float x, float t);
    static Function[] functions = { Wave, MultiWave, Ripple };
    public enum FunctionName { Wave, MultiWave, Ripple };

    public static Function GetFunction (FunctionName name)
    {
        return functions[(int) name];
    }
    public static float Wave (float x, float t) {
        return Sin(PI * (x + t));
    }

    public static float MultiWave (float x, float t)
    {
        float y = Wave(x,t);
        y += Sin(2*PI * (x + t*0.7f))/2;
        return y/1.5f;
    }

    /// <summary>
    /// Sine wave move away from the origin, 
    /// instead of always traveling in the same direction. 
    /// We can do this by basing it on the distance from the center,
    /// which is the absolute of X.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float Ripple (float x, float t)
    {
        float d = Abs(x);
        return Sin(4 * PI * (d - t*0.2f)) / (1f + 10f * d);
    }
}
