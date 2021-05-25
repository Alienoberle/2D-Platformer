using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    public Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D.velocity = velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbody2D.velocity = velocity * Time.fixedDeltaTime;
    }
}
