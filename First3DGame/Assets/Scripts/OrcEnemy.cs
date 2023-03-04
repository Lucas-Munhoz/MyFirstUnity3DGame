using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrcEnemy : MonoBehaviour
{
    [Header("Atributtes")]
    public float totalHealth = 150f;
    public float attackDamage;
    public float movementSpeed;
    public float rotationSpeed;
    public float lookRadius;
    
    

    [Header("Components")]
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;

    [Header("Others")]
    private Transform player;

    private float colliderRadius = 1.3f;
    private bool walking;
    private bool attacking;
    private bool hiting;

    private bool waitFor;

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
        if(totalHealth > 0){
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
                    StartCoroutine("Attack");
                    LookTarget();
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
    }

    IEnumerator Attack(){
        if(waitFor == false && hiting == false){
            waitFor = true;
            attacking = true;
            walking = false;
            anim.SetBool("Walk Forward", false);
            anim.SetTrigger("Melee Attack");
            yield return new WaitForSeconds(1f);
            GetPlayerToAttack();
            yield return new WaitForSeconds(1.5f);
            waitFor = false;
        }
        
    }

    void GetPlayerToAttack(){
        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius)){
            if(c.gameObject.CompareTag("Player")){
                Debug.Log("Attack");
            }
        }
    }

    public void GetHit(float damage){
        totalHealth -= damage;
        if(totalHealth > 0){
            StopCoroutine("Attack");
            anim.SetTrigger("Take Damage");
            hiting = true;
            StartCoroutine("RecoveryFromHit");
        }
        else{
            anim.SetTrigger("Die");
        }
    }

    IEnumerator RecoveryFromHit(){
        yield return new WaitForSeconds(1f);
        anim.SetBool("Walk Forward", false);
        hiting = false;
        waitFor = false;
    }

    void LookTarget(){
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere((transform.position + (transform.forward*1.5f) * colliderRadius), colliderRadius);
    }
}
