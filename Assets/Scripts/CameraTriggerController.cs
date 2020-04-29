using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerController : MonoBehaviour
{
    public Vector3 cameraChange;
    public Vector3 playerChange;
    private Camera cam;

    private float previousX;
    private float previousY;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(collision.GetComponent<Player>().velocity.x > 1)
            {
                previousX = cam.transform.position.x;
                cam.transform.position = new Vector3(cameraChange.x,cameraChange.y,cam.transform.position.z);
                collision.transform.position += playerChange;   
            }
            else
            {
                cam.transform.position = new Vector3(previousX, cameraChange.y, cam.transform.position.z);
                collision.transform.position -= playerChange;
            }
        }
    }
}
