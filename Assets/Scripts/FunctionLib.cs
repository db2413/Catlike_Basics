using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLib
{
    // Take a grid and time as input and output y
    public delegate float Function(float x, float z, float t);
    static Function[] functions = { XWave, Wave, DoubleWave, TripleWave, Ripple };
    public enum FunctionName { XWave, Wave, DoubleWave, TripleWave, Ripple };

    public static Function GetFunction (FunctionName name)
    {
        return functions[(int) name];
    }
    public static float XWave(float x, float z, float t)
    {
        return Sin(PI * (x + t));
    }
    public static float Wave (float x, float z, float t) {
        return Sin(PI * ((x + z)/2 + t));
    }

    public static float DoubleWave (float x, float z, float t)
    {
        float y = Wave(x,z,t);
        y += Sin(2*PI * (z + t*0.7f))/2;
        return y/1.5f;
    }

    public static float TripleWave(float x, float z, float t)
    {
        float y = Wave(x, z, t);
        y += 0.5f * Sin(2 * PI * (z + t * 0.7f));
        y += Sin(PI * (x + z + 0.25f * t));
        return y / 2.5f;
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
    public static float Ripple (float x, float z, float t)
    {
        float d = Sqrt(x*x+z*z);
        return Sin(4 * PI * (d - t*0.2f)) / (1f + 10f * d);
    }
}
