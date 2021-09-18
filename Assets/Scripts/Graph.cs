using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    Transform[] points;
    [SerializeField, Range(10,200)] int resolution = 10;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        points = new Transform[resolution];

        var position = Vector3.zero;
        var scale = Vector3.one / resolution;
        float step = 2f / resolution; // -1 to 1 x range

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
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
        time += Time.deltaTime;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            position.y = 0.5f*Mathf.Sin(Mathf.PI*(position.x+time));

            point.localPosition = position;
        }
    }
}
