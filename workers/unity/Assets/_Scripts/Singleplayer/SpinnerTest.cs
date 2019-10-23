using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerTest : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, rb.transform.eulerAngles.y + (speed * Time.fixedDeltaTime), 0f));
    }
}
