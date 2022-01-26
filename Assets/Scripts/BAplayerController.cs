using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BAplayerController : MonoBehaviour
{
    public float MovementSpeed;
    public float RotationSpeed;
    public float Gravity = 10;

    public float AttackCooldown;
    float Acooldown;

    public GameObject Weapon;

    CharacterController CC;
    PlayerInputActions playerInputActions;

    void Awake()
    {
        GameManager.Player = gameObject;
        CC = transform.Find("ThePlayer").GetComponent<CharacterController>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInputActions.Player.Fire.performed += Attack;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Combat();
    }

    void Move()
    {
        Vector2 v2 = playerInputActions.Player.Move.ReadValue<Vector2>();
        Vector3 V3 = new Vector3(v2.x, 0, v2.y);
        CC.Move(V3.normalized* MovementSpeed * Time.deltaTime);
        CC.Move(Vector3.down * Gravity * Time.deltaTime);

        if (V3 != Vector3.zero && Acooldown<=0)
        {
            Vector3 _direction = (CC.transform.position + V3 - CC.transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            CC.transform.Find("PlayerModel").rotation = Quaternion.Slerp(CC.transform.Find("PlayerModel").rotation, _lookRotation, Time.deltaTime * RotationSpeed);
        }
    }

    void Combat()
    {
        if (Acooldown > 0) { Acooldown -=Time.deltaTime; }
    }

    IEnumerator BasicAttackAnimation()
    {
        Collider c = Weapon.GetComponent<Collider>();
        MeshRenderer m = Weapon.GetComponent<MeshRenderer>();
        m.enabled = true;
        c.enabled = true;
        yield return new WaitForSeconds(0.3f);
        m.enabled = false;
        c.enabled = false;
    }


    void Attack(InputAction.CallbackContext context)
    {
        if (Acooldown <= 0)
        {
            RaycastHit hit;
            Ray r = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(r, out hit))
            {
                Vector3 v3 = hit.point;
                v3.y = CC.transform.position.y;
                CC.transform.Find("PlayerModel").LookAt(v3);
            }        
            Acooldown = AttackCooldown;
            StartCoroutine(BasicAttackAnimation());
        }
    }

}
public static class GameManager
{
    public static GameObject Player;
}
    


