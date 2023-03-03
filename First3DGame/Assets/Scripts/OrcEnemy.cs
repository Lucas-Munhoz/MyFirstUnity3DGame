using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrcEnemy : MonoBehaviour
{
    [Header("Atributtes")]
    public float totalHealth;
    public float attackDamage;
    public float movementSpeed;
    public float lookRadius;

    [Header("Components")]
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;

    [Header("Others")]
    private Transform player;


    private bool walking;
    private bool attacking;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if(distance <= lookRadius){
            agent.isStopped = false;
            if(!attacking){
                agent.SetDestination(player.position);
                anim.SetBool("Walk Forward", true);
                walking = true;
            }
            
            //Attack=========
            if(distance <= agent.stoppingDistance){
                agent.isStopped = true;
                anim.SetBool("Walk Forward", false);
                //Attack method

            }
            else{
                attacking = false;
            }
            //===============
        }
        else{
            agent.isStopped = true;
            anim.SetBool("Walk Forward", false);
            walking = false;
            attacking = false;
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
