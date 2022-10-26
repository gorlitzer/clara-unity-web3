using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightMovement : MonoBehaviour
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
        // move the cube from (0, 0, 0) to (5, 0, 0) and back to (0, 0, 0) and so on.
        // use the same Mathf.PingPong() function to the other axes if you need to move in those axes as well
        transform.position =
            new Vector3(1 - Mathf.PingPong(Time.time * speed, 3),
                transform.position.y,
                transform.position.z);
    }
}
