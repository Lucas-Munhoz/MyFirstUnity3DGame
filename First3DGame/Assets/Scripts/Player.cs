using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    public float speed;
    public float gravity;

    private Transform cam;

    private Vector3 moveDirection;

    public float smoothRotTime;
    private float turnSmoothVelocity;

    public float colliderRadius;
    public List<Transform> enemyList = new List<Transform>();

    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GetMouseInput();
    }

    void Move(){
        //Detecta se está encostando no chão. Original da classe.
        if(controller.isGrounded){
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            if(direction.magnitude > 0){

                if(!anim.GetBool("attacking")){
                    float angle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, smoothRotTime);
                
                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                    moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;
                    anim.SetInteger("transition",1);

                    isRunning = true;
                }
                else{
                    anim.SetBool("running", false);
                    moveDirection = Vector3.zero;
                }
                
            }
            else if(isRunning == true){
                anim.SetBool("running", false);
                anim.SetInteger("transition",0);
                moveDirection = Vector3.zero;
                isRunning = false;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void GetMouseInput(){
        if(controller.isGrounded){
            if(Input.GetMouseButtonDown(0)){
                if(anim.GetBool("running")){
                    anim.SetBool("running",false);
                    anim.SetInteger("transition",0);
                }
                if(!anim.GetBool("running")){
                    StartCoroutine("Attack");
                }
                
            }
        }
    }

    IEnumerator Attack(){
        anim.SetBool("attacking", true);
        anim.SetInteger("transition",2);
        //Espera em segundos e depois executa o restante do codigo
        yield return new WaitForSeconds(0.4f);
        GetEnemiesList();
        foreach(Transform enemies in enemyList){
            Debug.Log(enemies.name);
        }
        yield return new WaitForSeconds(1f);
        anim.SetInteger("transition",0);
        anim.SetBool("attacking", false);
    }

    void GetEnemiesList(){
        enemyList.Clear();
        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius + transform.up), colliderRadius)){
            if(c.gameObject.CompareTag("Enemy")){
                enemyList.Add(c.transform);
            }
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward + transform.up, colliderRadius);
    }
}
