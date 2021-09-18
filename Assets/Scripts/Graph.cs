using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    [SerializeField, Range(10,200)] int resolution = 10;

    // Start is called before the first frame update
    void Start()
    {
        var position = Vector3.zero;
        var scale = Vector3.one / resolution;
        float step = 2f / resolution; // -1 to 1 x range

        for (int i = 0; i < resolution; i++)
        {
            Transform point = Instantiate(pointPrefab);
            position.x = (i+0.5f) * step - 1;
            position.y = position.x * position.x * position.x;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
