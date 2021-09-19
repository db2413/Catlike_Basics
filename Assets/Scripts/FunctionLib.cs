using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLib
{
    // Take a grid and time as input and output y
    public delegate Vector3 Function(float u, float v, float t);
    static Function[] functions = { Wave, DoubleWave, TripleWave, Ripple, Sphere, Cylinder, Torus, Test };
    public enum FunctionName {  Wave, DoubleWave, TripleWave, Ripple, Sphere, Cylinder, Torus, Test };

    public static Function GetFunction (FunctionName name)
    {
        return functions[(int) name];
    }
    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + t));
        p.z = v;

        return p;
    }

    public static Vector3 DoubleWave(float u, float v, float t)
    {
        Vector3 p = Wave(u,v,t);
        p.y += Sin(2*PI * (v + t*0.7f))/2;
        p.y /= 1.5f;
        return p;
    }

    public static Vector3 TripleWave(float u, float v, float t)
    {
        Vector3 p = Wave(u, v, t);
        p.y += 0.5f * Sin(2 * PI * (v + t * 0.7f));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y /= 2.5f;
        return p;
    }

    /// <summary>
    /// Sine wave move away from the origin, 
    /// instead of always traveling in the same direction. 
    /// We can do this by basing it on the distance from the center,
    /// which is the absolute of X.
    /// </summary>
    /// <param name="u"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 Ripple (float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        float d = Sqrt(u * u + v * v);
        p.y = Sin(4 * PI * (d - t * 0.2f)) / (1f + 10f * d);
        p.z = v;
        return p;
    }

    public static Vector3 Sphere (float u, float v, float t)
    {
        //float r = 0.5f + 0.5f * Sin(PI * t); // radius scaled by time 0 to 1
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t)); //twisty thing
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u) ;
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u) ;

        return p;
    }

    public static Vector3 Cylinder(float u, float v, float t)
    {
        Vector3 p;
        p.x = Sin(PI * u);
        p.y = v;
        p.z = Cos(PI * u);

        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.25f;
        float r2 = 0.75f;

        // Effects
        Vector3 p = Ripple(u,v,t);
        //r2 = 0.7f + 0.051f * Sin(PI * (16f * u + 0.15f * t));
        //r1 = 0.15f + 0.05f * Sin(PI * (v + u + t));

        float s = r2 + r1 * Cos(PI * v);
        p.x = s * Sin(PI * u);
        p.y = r1 * Sin(PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }

    public static Vector3 Test(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = 0.5f + 0.5f * Sin(PI * t);
        p.z = v;

        return p;
    }
}
