using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
    public float MaxStamina = 100;
    [HideInInspector]
    public float Stamina;
    public float StaminaRegan = 30;
    public float StaminaCost = 30;
    public float Tired = 30;

    public float AttackCooldown;
    [HideInInspector]
    public float Acooldown;
    public GameObject Weapon;

    // Start is called before the first frame update
    protected void Start()
    {
        Stamina = MaxStamina;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (Acooldown > 0) { Acooldown -= Time.deltaTime; }

        if (Stamina<0) { Stamina = 0; }
        else if (Stamina < MaxStamina)
        {
            if (StaminaRegan > 0)
                Stamina += Time.deltaTime * StaminaRegan;
        }
        else
        {
            Stamina = MaxStamina;
        }
    }

    protected IEnumerator BasicAttackAnimation()
    {
        Collider c = Weapon.GetComponent<Collider>();
        MeshRenderer m = Weapon.GetComponent<MeshRenderer>();
        m.enabled = true;
        c.enabled = true;
        yield return new WaitForSeconds(0.3f);
        m.enabled = false;
        c.enabled = false;
    }

    protected bool CanAttack()
    {
        if (Acooldown <= 0 && Stamina >= StaminaCost)
        {
            Stamina -= StaminaCost;
            return true;
        }
        return false;
    }

}
