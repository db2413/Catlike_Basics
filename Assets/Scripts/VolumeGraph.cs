using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeGraph : MonoBehaviour
{
    public bool surfaceOrVolume;
    [SerializeField] Transform pointPrefab;
    Transform[] points;
    [SerializeField, Range(3,200)] int resolution = 3;
    float time;
    float tick;
    [SerializeField] FunctionLib.FunctionName function;
    public float functionDuration = 1;
    public float transitionDuration = 1;
    bool transitioning;
    FunctionLib.FunctionName transitionFunction;

    // Start by making a 2d grid of range -1 to 1 with a resolution
    void Awake()
    {
        points = new Transform[resolution * resolution];

        var scale = Vector3.one / resolution * 2;

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        tick += Time.deltaTime;
        if(tick > functionDuration) { 
            tick = 0;

            if (transitioning)
            {
                function = transitionFunction;
                transitioning = false;
            }
            else
            {
                transitionFunction = FunctionLib.GetNextFunctionName(function);
                transitioning = true;
            }
        }
        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }

    void UpdateFunction ()
    {
        float step = 2f / (resolution - 1);
        for (int i = 0; i < points.Length; i++)
        {
            int x = i % resolution;
            int z = i / resolution;
            FunctionLib.Function f = FunctionLib.GetFunction(function);
            float u = x * step - 1; // As x and z are no longer constant, we can't rely on their initial values
            float v = z * step - 1;
            Transform point = points[i];
            Vector3 position = point.localPosition;
            position = f(u, v, time);

            point.localPosition = position;
        }
    }

    void UpdateFunctionTransition()
    {
        float step = 2f / (resolution - 1);
        for (int i = 0; i < points.Length; i++)
        {
            int x = i % resolution;
            int z = i / resolution;
            FunctionLib.Function from = FunctionLib.GetFunction(function);
            FunctionLib.Function to = FunctionLib.GetFunction(transitionFunction);
            float u = x * step - 1; // As x and z are no longer constant, we can't rely on their initial values
            float v = z * step - 1;
            Transform point = points[i];
            Vector3 position = point.localPosition;
            position = FunctionLib.Morph(u, v, time, from, to, tick/transitionDuration);

            point.localPosition = position;
        }
    }
}
