using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLib
{
    public static float Wave (float x, float t) {
        return Sin(PI * (x + t));
    }

    public static float MultiWave (float x, float t)
    {
        float y = Wave(x,t);
        y += Sin(2*PI * (x + t*0.7f))/2;
        return y/1.5f;
    }

}
