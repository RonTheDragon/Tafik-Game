using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5;
    public float NormalSpeed = 5;
    public float SprintSpeed = 10;
    public float LookSpeed = 10;
    public float JumpHigh = 10;
    public float strongArm = 200;
    Rigidbody rb;
    GameObject PlayerBody;
    Transform PB;
    GameObject CameraHolder;
    float pitch;
    float yaw;
    public LayerMask LM;
    public LayerMask CLM;
    public LayerMask MLM;

    public float CameraDist;

    public float gravity;

    float charge;
    public float MaxCharge;

    GameObject HeldObject;

    // Start is called before the first frame update
    void Start()
    {
        PlayerBody = transform.GetChild(0).gameObject;
        PB = PlayerBody.transform;
        CameraHolder=Camera.main.transform.parent.parent.gameObject;
        rb = PlayerBody.GetComponent<Rigidbody>();
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
        MoveSpeed = NormalSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = SprintSpeed;
        }


        Vector3 MoveTo = rb.transform.position;
        if (Input.GetKey(KeyCode.W) && !Physics.Raycast(PB.position, PB.forward, 1, MLM))
        {
            MoveTo += PB.forward;
        }
        else if (Input.GetKey(KeyCode.S) && !Physics.Raycast(PB.position, -PB.forward, 1, MLM))
        {
            MoveTo -= PB.forward;
        }
        if (Input.GetKey(KeyCode.D) && !Physics.Raycast(PB.position, PB.right, 1, MLM))
        {
            MoveTo += PB.right;
        }
        else if (Input.GetKey(KeyCode.A) && !Physics.Raycast(PB.position, -PB.right, 1, MLM))
        {
            MoveTo -= PB.right;
        }
        if (Input.GetKey(KeyCode.Space) && !Physics.Raycast(PB.position, Vector3.up, 1, MLM))
        {
            MoveTo += Vector3.up;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && !Physics.Raycast(PB.position, -Vector3.up, 1, MLM))
        {
            MoveTo -= Vector3.up;
        }

        rb.MovePosition(Vector3.MoveTowards(rb.transform.position, MoveTo, MoveSpeed * Time.deltaTime));




        /*
        if (Input.GetKey(KeyCode.W) && !Physics.Raycast(PB.position, PB.forward, 1))
        {
            PB.position += PB.forward * MoveSpeed * Time.deltaTime;
           
        }
        else if (Input.GetKey(KeyCode.S) && !Physics.Raycast(PB.position, -PB.forward, 1))
        {
            PB.position -= PB.forward * MoveSpeed * 0.6f * Time.deltaTime;
           
        }
        if (Input.GetKey(KeyCode.D) && !Physics.Raycast(PB.position, PB.right, 1))
        {
            PB.position += PB.right * MoveSpeed * 0.8f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A) && !Physics.Raycast(PB.position, -PB.right, 1))
        {
            PB.position -= PB.right * MoveSpeed * 0.8f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) && !Physics.Raycast(PB.position, Vector3.up, 1))
        {
            PB.position += Vector3.up * MoveSpeed * 0.8f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && !Physics.Raycast(PB.position, -Vector3.up, 1))
        {
            PB.position -= Vector3.up * MoveSpeed * 0.8f * Time.deltaTime;
        }

        
       */

        
        if (rb.velocity.x > 0)
        {
            rb.velocity -= new Vector3(Time.deltaTime * 10, 0, 0);
        }
       if (rb.velocity.x < 0)
        {
            rb.velocity += new Vector3(Time.deltaTime * 10, 0, 0);
        }
        if (rb.velocity.z > 0)
        {
            rb.velocity -= new Vector3(0, 0, Time.deltaTime*10);
        }
        if (rb.velocity.z < 0)
        {
            rb.velocity += new Vector3(0, 0, Time.deltaTime * 10);
        }
        if (rb.velocity.y > 0)
        {
            rb.velocity -= new Vector3(0, Time.deltaTime * 10, 0);
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += new Vector3(0,Time.deltaTime * 10,0);
        }

    

    }

 

    void Look()
    {
        pitch -= Input.GetAxisRaw("Mouse Y") * LookSpeed*0.7f;
        pitch = Mathf.Clamp(pitch, -90, 90);
        yaw += Input.GetAxisRaw("Mouse X") * LookSpeed;
        PB.localRotation = Quaternion.Euler(pitch, yaw, 0);


        RaycastHit hit;
        if (Physics.Raycast(CameraHolder.transform.position, -CameraHolder.transform.forward, out hit, CameraDist, CLM))
        {
            CameraHolder.transform.GetChild(0).transform.position = hit.point;
        }
        else
        {
            CameraHolder.transform.GetChild(0).transform.position = CameraHolder.transform.position - CameraHolder.transform.forward * CameraDist;
        }

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
            Rigidbody HRB = HeldObject.GetComponent<Rigidbody>();
            float dist = Vector3.Distance(HeldObject.transform.position, CameraHolder.transform.position + CameraHolder.transform.forward * 4);
            if (dist > 1) { dist *= 3; }
            HRB.MovePosition(Vector3.MoveTowards(HeldObject.transform.position, CameraHolder.transform.position + CameraHolder.transform.forward * 4,dist* 10 * Time.deltaTime));
            HRB.velocity = new Vector3(0, 0 , 0);
            HeldObject.transform.rotation = Quaternion.RotateTowards(HeldObject.transform.rotation, CameraHolder.transform.rotation, 1000*Time.deltaTime);
           // HeldObject.transform.rotation = CameraHolder.transform.rotation;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            HeldObject = null;
        }

        if (Input.GetMouseButton(0) && charge < MaxCharge)
        {
            charge += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0)&& HeldObject != null)
        {
            Rigidbody HRB = HeldObject.GetComponent<Rigidbody>();
            HRB.velocity = PB.forward * charge * strongArm;
            HeldObject = null;
            charge = 0;
        }
    }
}
