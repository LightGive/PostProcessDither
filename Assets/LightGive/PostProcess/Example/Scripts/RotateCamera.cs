using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float rotSpd = 1.0f;
    [SerializeField]
    private float dist = 2.5f;
    [SerializeField]
    private float height = 0.8f;
    
    private float angle = 0.0f;

    void Start()
    {
        
    }

    void Update()
    {
        angle +=rotSpd * Time.deltaTime;
        var vec = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)).normalized * dist;
        transform.position = target.position + vec + new Vector3(0.0f, height, 0.0f);
        transform.LookAt(target);
    }
}
