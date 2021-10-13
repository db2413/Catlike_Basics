using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform follower;
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follower.position - follower.forward * distance + 3*Vector3.up;
        transform.forward = follower.position-transform.position;
    }
}
