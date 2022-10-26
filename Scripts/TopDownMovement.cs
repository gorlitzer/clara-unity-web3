using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public float speed = 2.0f;
    private float yCenter;

    // Start is called before the first frame update
    void Start()
    {
        yCenter = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // move the cube from (0, 0, 0) to (0, 3, 0) and back to (0, 0, 0) and so on.
        // use the same Mathf.PingPong() function to the other axes if you need to move in those axes as well
        transform.position =
            new Vector3(transform.position.x,
                yCenter - Mathf.PingPong(Time.time * speed, 2),
                transform.position.z);
    }
}
