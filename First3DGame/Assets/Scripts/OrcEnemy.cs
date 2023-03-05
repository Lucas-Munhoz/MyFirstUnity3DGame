using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrcEnemy : MonoBehaviour
{
    [Header("Atributtes")]
    public float totalHealth = 150f;
    public float attackDamage = 65;
    public float movementSpeed;
    public float rotationSpeed;
    public float lookRadius;
    
    

    [Header("Components")]
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;

    [Header("Others")]
    private Transform player;

    [Header("WayPoints")]
    public List<Transform> wayPoints = new List<Transform>();
    public int currentPathIndex;
    public float pathDistance;

    private float colliderRadius = 1.3f;
    private bool walking;
    private bool attacking;
    private bool isHitting;
    private bool playerisDead;

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
                anim.SetBool("Walk Forward", false);
                walking = false;
                attacking = false;
                MoveToWayPoint();
            }
        }
    }

    void MoveToWayPoint(){
        if(wayPoints.Count > 0){
            float distance = Vector3.Distance(wayPoints[currentPathIndex].position, transform.position);
            agent.destination = wayPoints[currentPathIndex].position;

            if(distance <= pathDistance){
                currentPathIndex = Random.Range(0, wayPoints.Count);
            }

            anim.SetBool("Walk Forward", true);
            walking = true;
        }
    }

    IEnumerator Attack(){
        if(waitFor == false && isHitting == false && playerisDead == false){
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

        if(playerisDead){
            anim.SetBool("Walk Forward", false);
            walking = false;
            attacking = false;
            agent.isStopped = true;
        }
        
    }

    void GetPlayerToAttack(){
        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius)){
            if(c.gameObject.CompareTag("Player")){
                c.gameObject.GetComponent<Player>().GetHit(attackDamage);
                playerisDead = c.gameObject.GetComponent<Player>().isDead;
            }
        }
    }

    public void GetHit(float damage){
        totalHealth -= damage;
        if(totalHealth > 0){
            StopCoroutine("Attack");
            anim.SetTrigger("Take Damage");
            isHitting = true;
            StartCoroutine("RecoveryFromHit");
        }
        else{
            anim.SetTrigger("Die");
        }
    }

    IEnumerator RecoveryFromHit(){
        yield return new WaitForSeconds(1f);
        anim.SetBool("Walk Forward", false);
        isHitting = false;
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
