using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5;
    public float LookSpeed = 10;
    public float JumpHigh = 10;
    public float strongArm = 200;
    Rigidbody rb;
    GameObject CameraHolder;
    float pitch;
    float yaw;
    public LayerMask LM;

    public float gravity;

    float charge;
    public float MaxCharge;

    GameObject HeldObject;

    // Start is called before the first frame update
    void Start()
    {
        CameraHolder = transform.GetChild(0).GetChild(0).gameObject;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Look();
        Holding();
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * MoveSpeed * 0.6f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * MoveSpeed * 0.8f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * MoveSpeed * 0.8f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) && Physics.Raycast(rb.transform.position, Vector3.down, 1 + 0.001f))
            rb.velocity = new Vector3(rb.velocity.x, JumpHigh, rb.velocity.z);

        rb.velocity -= new Vector3(0, gravity * Time.deltaTime, 0);

       if (rb.velocity.x > 0)
        {
            rb.velocity -= new Vector3(Time.deltaTime * 2, 0, 0);
        }
       if (rb.velocity.x < 0)
        {
            rb.velocity += new Vector3(Time.deltaTime * 2, 0, 0);
        }
        if (rb.velocity.z > 0)
        {
            rb.velocity -= new Vector3(0, 0, Time.deltaTime*2);
        }
        if (rb.velocity.z < 0)
        {
            rb.velocity += new Vector3(0, 0, Time.deltaTime * 2);
        }


    }
    void Look()
    {
        pitch -= Input.GetAxisRaw("Mouse Y") * LookSpeed*0.7f;
        pitch = Mathf.Clamp(pitch, -90, 90);
        yaw += Input.GetAxisRaw("Mouse X") * LookSpeed;
        CameraHolder.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        transform.localRotation = Quaternion.Euler(0, yaw, 0);
    }
    void Holding()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(CameraHolder.transform.position, CameraHolder.transform.forward,out hit,5, LM)){
                HeldObject = hit.transform.gameObject;
                
            }
        }
        if (Input.GetKey(KeyCode.E)&&HeldObject!=null)
        {
            //HeldObject.transform.position = CameraHolder.transform.position + CameraHolder.transform.forward * 2;
            Rigidbody HRB = HeldObject.GetComponent<Rigidbody>();
            HeldObject.transform.position = Vector3.MoveTowards(HeldObject.transform.position, CameraHolder.transform.position + CameraHolder.transform.forward * 2, 10 * Time.deltaTime);
            HRB.velocity = new Vector3(0, 0 , 0);
            HeldObject.transform.rotation = CameraHolder.transform.rotation;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            HeldObject = null;
        }

        if (Input.GetMouseButton(0) && charge < MaxCharge)
        {
            charge += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Rigidbody HRB = HeldObject.GetComponent<Rigidbody>();
            HRB.velocity = transform.forward * charge * strongArm;
            HeldObject = null;
            charge = 0;
        }
    }
}
