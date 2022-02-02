using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackSystem : AttackSystem
{
    BAplayerController BAP;
    CharacterController CC;
    public LayerMask OnlyFloor;
    public LayerMask Attackable;
    Transform PlayerModel;

    ParticleSystem Particle;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        PlayerModel = transform.Find("PlayerModel");
        CC = GetComponent<CharacterController>();
        BAP = transform.parent.GetComponent<BAplayerController>();
        Particle = PlayerModel.GetComponent<ParticleSystem>();
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
            if (Physics.Raycast(r, out hit, OnlyFloor))
            {
                Vector3 v3 = hit.point;
                v3.y = CC.transform.position.y;
                BAP.SetAimPoint(v3);
            }
            Acooldown = AttackCooldown;
            if (BAP.Aim) Shoot();
            else StartCoroutine(BasicAttackAnimation());
        }
    }
    void Shoot()
    {
        Particle.Emit(1);
        RaycastHit hit;
        if (Physics.Raycast(PlayerModel.position, PlayerModel.forward,out hit,100f))
        {
            if(Attackable == (Attackable | (1 << hit.transform.gameObject.layer)))
            {
                EnemyHealth HP = hit.transform.GetComponent<EnemyHealth>();
                if (HP != null)
                {
                    HP.TakeDamage(30, 10, transform.position);
                    
                }
            }
        }
    }
}
