using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCamera : AbstractEnemy
{
    private float x;
    public float cameraBound;
    public float cameraMoveSpeed;

    private float originalRotationAngle;

    // Start is called before the first frame update
    private void Awake()
    {
        originalRotationAngle = transform.localEulerAngles.z;
    }
    void Start()
    {
        x = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime * cameraMoveSpeed;
        transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Mathf.Sin(x) * cameraBound + originalRotationAngle);
      

    }
}
