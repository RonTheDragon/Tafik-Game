using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BAplayerController : MonoBehaviour
{
    public float MovementSpeed;
    public float RotationSpeed;

    CharacterController CC;
    PlayerInputActions playerInputActions;

    void Awake()
    {
        CC = transform.Find("ThePlayer").GetComponent<CharacterController>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 v2 = playerInputActions.Player.Move.ReadValue<Vector2>();
        Vector3 V3 = new Vector3(v2.x, 0, v2.y);
        CC.Move(V3.normalized* MovementSpeed * Time.deltaTime);

        if (V3 != Vector3.zero)
        {
            Vector3 _direction = (CC.transform.position + V3 - CC.transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            CC.transform.Find("PlayerModel").rotation = Quaternion.Slerp(CC.transform.Find("PlayerModel").rotation, _lookRotation, Time.deltaTime * RotationSpeed);
        }

    }
}
    


