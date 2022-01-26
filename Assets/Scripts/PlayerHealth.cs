using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    CharacterController CC;

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
        TakeKnockback();
    }
    void TakeKnockback()
    {
        if (TheKnockback > 0)
        {
            TheKnockback -= TheKnockback * Time.deltaTime * 2;
            CC.Move((transform.position - TheImpactLocation).normalized * TheKnockback * Time.deltaTime);
        }
    }
}
