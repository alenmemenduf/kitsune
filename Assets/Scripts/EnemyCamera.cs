using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(FieldOfView))]
public class EnemyCamera : AbstractEnemy
{
    private float x;
    public float cameraBound;
    public float cameraMoveSpeed;

    private FieldOfView fow;
    // Start is called before the first frame update
    void Start()
    {
        x = transform.position.z;
        
    }

    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime * cameraMoveSpeed;
        transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Mathf.Sin(-x) * cameraBound + 90 * Math.Sign(transform.rotation.z));

    }
}
