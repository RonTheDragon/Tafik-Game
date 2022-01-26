using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent NMA;
    GameObject Player;
    float dist;
    public float DetectionRange;
    float alert;
    bool chasingPlayer;
    public float RoamCooldown = 10;
    float roamCooldown;
    public float RoamRadius = 10;
    public GameObject Weapon;

    public float AttackCooldown;
    float Acooldown;

    // Start is called before the first frame update
    void Awake()
    {
        NMA = GetComponent<NavMeshAgent>();     
    }

    void Start()
    {
        Player = GameManager.Player.transform.Find("ThePlayer").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyAI();
    }

    protected void EnemyAI()
    {
        
        if (canSeePlayer())
        {        
                alert += Time.deltaTime;       
        }
        else if(alert>0)
        {
            alert -= Time.deltaTime;
            if (alert > 1)chasingPlayer = true;
            else chasingPlayer = false;
        }

        if (chasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            Wonder();
        }

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

    void ChasePlayer()
    {
        NMA.SetDestination(Player.transform.position);
        if (Acooldown > 0) { Acooldown -= Time.deltaTime; }
        if (dist < NMA.stoppingDistance)
        {
            RotateTowards(Player.transform);
            if (Acooldown <= 0)
            {
                Acooldown = AttackCooldown;
                StartCoroutine(BasicAttackAnimation());
            }
        }
    }

    void Wonder()
    {
        if (roamCooldown > 0) { roamCooldown -= Time.deltaTime; }
        else
        {
            roamCooldown = Random.Range(0, RoamCooldown);
            float x = Random.Range(-RoamRadius, RoamRadius);
            float z = Random.Range(-RoamRadius + 0.1f, RoamRadius);
            Vector3 MoveTo = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
            NMA.SetDestination(MoveTo);
        }
    }

    bool eyesightcheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (Player.transform.position - transform.position).normalized, out hit, DetectionRange))
        {
            if (hit.transform.gameObject == Player)
            {
                return true;
            }
        }
        return false;
    }

    bool canSeePlayer()
    {
        dist = Vector3.Distance(transform.position, Player.transform.position);
        if (dist <= DetectionRange && alert < 3)
        {
            if (eyesightcheck())
                return true;
            else return false;
        }
        else return false;
    }

    public void GotHit()
    {
        alert += 2;
    }

    private void RotateTowards(Transform target)
    {
        Vector3 targetlocation = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Vector3 direction = (targetlocation - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * NMA.angularSpeed);
    }
}
