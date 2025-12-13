using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 Nextposition;
    void Start()
    {
        Nextposition = pointA.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Nextposition, Time.deltaTime);
        if (transform.position == Nextposition)
        {
            Nextposition = (Nextposition == pointA.position ? pointB.position : pointA.position);
        }
    }
}
