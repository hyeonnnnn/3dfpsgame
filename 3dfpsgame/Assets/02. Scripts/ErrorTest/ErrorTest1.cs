using UnityEngine;

public class ErrorTest1 : MonoBehaviour
{

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Debug.Log(rb.linearVelocity);
    }
}
