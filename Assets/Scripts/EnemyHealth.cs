using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    Enemy enemy;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        TakeKnockback();
    }
    public override void TakeDamage(float Damage,float Knock,Vector3 ImpactLocation)
    {
        base.TakeDamage(Damage,Knock,ImpactLocation);
        enemy.GotHit();
    }

    void TakeKnockback()
    {
        if (TheKnockback > 0)
        {
            TheKnockback -= TheKnockback * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, TheImpactLocation, -TheKnockback * Time.deltaTime);
        }
    }
}
