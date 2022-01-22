using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    float MovementSpeed = 10;
    public float WalkSpeed = 10;
    public float SprintSpeed = 20;

    float charge;
    public float MaxCharge;
    public float strongArm;
    public float strongLeg;

    public float LookSpeed = 10;

    public float gravityMultiplier = 1f;
    public float JumpTime = 1f;
    public float jumpHeight = 2.4f;
    float jumpTimeLeft;
    float fallingTime;

    GameObject HeldObject;
    public LayerMask InteractableOnly;

    Transform PlayerBody;
    CharacterController CC;
    Camera cam;
    float pitch;
    float yaw;

    float GoingUp;

    // Start is called before the first frame update
    void Start()
    {
        PlayerBody = transform.GetChild(0);
        CC = PlayerBody.transform.GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Look();
        Jump();
        Holding();
        Kick();
    }

    void Walk()
    {
        MovementSpeed = WalkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MovementSpeed = SprintSpeed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            CC.Move(PlayerBody.forward * MovementSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            CC.Move(-PlayerBody.forward * MovementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            CC.Move(PlayerBody.right * MovementSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            CC.Move(-PlayerBody.right * MovementSpeed * Time.deltaTime);
        }
    }
    void Look()
    {
        pitch -= Input.GetAxisRaw("Mouse Y") * LookSpeed * 0.7f;
        pitch = Mathf.Clamp(pitch, -90, 90);
        yaw += Input.GetAxisRaw("Mouse X") * LookSpeed;
        PlayerBody.localRotation = Quaternion.Euler(0, yaw, 0);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

    }
    void Jump()
    {
        float Direction = 0; 

        if (jumpTimeLeft > 0)
        {
            jumpTimeLeft -= Time.deltaTime;
            Direction = jumpHeight* jumpTimeLeft;
        }
        else
        {
            fallingTime += Time.deltaTime;
            Direction = -gravityMultiplier*fallingTime;
            
        }

        if (CC.isGrounded && Input.GetKey(KeyCode.Space))
        {
            jumpTimeLeft = JumpTime;
            fallingTime = 0;
        }
   
        CC.Move(PlayerBody.up * Direction * Time.deltaTime);

        
    }
    void Holding()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5, InteractableOnly))
            {
                HeldObject = hit.transform.gameObject;

            }
        }
        if (Input.GetKey(KeyCode.E) && HeldObject != null)
        {
            Rigidbody HRB = HeldObject.GetComponent<Rigidbody>();
            float dist = Vector3.Distance(HeldObject.transform.position, cam.transform.position + cam.transform.forward * 2);
            if (dist > 1) { dist *= 3; }
            HRB.MovePosition(Vector3.MoveTowards(HeldObject.transform.position, cam.transform.position + cam.transform.forward * 2, dist * 20 * Time.deltaTime));
            HRB.velocity = new Vector3(0, 0, 0);
            HeldObject.transform.rotation = Quaternion.RotateTowards(HeldObject.transform.rotation, cam.transform.rotation, 1000 * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            HeldObject = null;
        }

        if (Input.GetMouseButton(0) && charge < MaxCharge)
        {
            charge += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0) && HeldObject != null)
        {
            Rigidbody HRB = HeldObject.GetComponent<Rigidbody>();
            HRB.velocity = cam.transform.forward * charge * strongArm;
            HeldObject = null;
            charge = 0;
        }
    }
    void Kick()
    {
        if (Input.GetKey(KeyCode.F) && charge < MaxCharge)
        {
            charge += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5, InteractableOnly))
            {
                Rigidbody HRB = hit.transform.GetComponent<Rigidbody>();
                if (charge < 0.5f) { charge = 0.5f; }
                HRB.velocity = cam.transform.forward * charge * strongLeg;
                charge = 0;
            }
        }
    }
}
