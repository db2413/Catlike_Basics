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
    [SerializeField] FunctionLib.FunctionName function;

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
        time += Time.deltaTime;
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
            position = f(u,v,time);

            point.localPosition = position;
        }
    }
}
