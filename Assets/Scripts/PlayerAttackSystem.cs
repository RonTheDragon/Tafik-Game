using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackSystem : AttackSystem
{
    CharacterController CC;
    public LayerMask OnlyFloor;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        CC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (CanAttack())
        {
            RaycastHit hit;
            Ray r = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(r, out hit,OnlyFloor))
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
