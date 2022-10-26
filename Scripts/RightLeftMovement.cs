using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightLeftMovement : MonoBehaviour
{
    public float speed = 2.0f;
    private float xCenter;

    // Start is called before the first frame update
    void Start()
    {   
        xCenter = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // use the same Mathf.PingPong() function to the other axes if you need to move in those axes as well
        transform.position =
            new Vector3(xCenter - Mathf.PingPong(Time.time * speed, 4),
                transform.position.y,
                transform.position.z);
    }
}
