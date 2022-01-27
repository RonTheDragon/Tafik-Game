using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

    }
    public void Attack()
    {
        if (CanAttack())
        {
            Acooldown = AttackCooldown;
            StartCoroutine(BasicAttackAnimation());
        }
    }
}
