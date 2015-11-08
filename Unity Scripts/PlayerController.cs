using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    AndroidJavaObject jo;
    int command;
    private Rigidbody rb;
    private float moveSpeed = 450.0f; // placeholder value
    private float magnitude = 15.0f; // placeholder value
    void Start ()
    {
        jo = new AndroidJavaObject("com.example.alan.bluetoothreceive");
        jo.Call("findBT");
        jo.Call("openBT");
        rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(moveSpeed, 0.0f, moveSpeed);
        rb.AddForce(movement);

    }

    void Update ()
    {
        command = jo.Call<int>("readBT");
        if (command == 1)
        {
            Vector3 jump = new Vector3 (0.0f, 150.0f, 0.0f);
            rb.AddForce(jump);
        }
        else if (command == 2)
        {
            rb.velocity = Quaternion.AngleAxis(90.0f, Vector3.up) * rb.velocity;
            rb.velocity = new Vector3(((rb.velocity.x)/(rb.velocity.magnitude)) * magnitude, ((rb.velocity.y) / (rb.velocity.magnitude)) * magnitude, ((rb.velocity.z) / (rb.velocity.magnitude)) * magnitude);
        }
        else if (command == 3)
        {
            rb.velocity = Quaternion.AngleAxis(-90.0f, Vector3.up) * rb.velocity;
            rb.velocity = new Vector3(((rb.velocity.x)/(rb.velocity.magnitude)) * magnitude, ((rb.velocity.y) / (rb.velocity.magnitude)) * magnitude, ((rb.velocity.z) / (rb.velocity.magnitude)) * magnitude);
        }
    }

    void onApplicationQuit ()
    {
        jo.Call("closeBT");
    }
}