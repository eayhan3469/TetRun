using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothFactor;

    private Vector3 offset;

    private void Awake()
    {
        offset = target.position - transform.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Slerp(transform.position, target.position - offset, smoothFactor * Time.fixedDeltaTime);
    }
}
