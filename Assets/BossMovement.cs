using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class BossMovement : MonoBehaviour
{
    private Rigidbody2D boss_rb;//보스의 리지드 바디
    [SerializeField] float health, maxHealth = 3f;
    [SerializeField] FloatingHealthBar healthBar;
    [SerializeField]private float boss_walkSpeed =5;// 보스 이동속도
    private Vector2 targetPosition;
    public Animator boss_Animator;
    public Transform player_pos;
    public bool isFlipped = false;
    public float attackRange =5f;
    public bool bossMoveOn = false;
    public static BossMovement instance;
    bool bossMoveInitial = false;
    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        boss_rb = GetComponent<Rigidbody2D>();
        boss_Animator = GetComponent<Animator>();
        healthBar.UpdateHeathBar(health, maxHealth);

    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
        BossMove();
       
        if (Math.Abs(Vector2.Distance(player_pos.position, boss_rb.position)) <= attackRange)
        {

            boss_Animator.SetBool("Running", false);
            boss_Animator.SetBool("Attacking", true);
        }
       
        else
        {
            boss_Animator.SetBool("Attacking", false);
        }
        

    }

     async UniTask BossMove() 
    {
       
        await UniTask.Delay(2000);
        boss_Animator.SetBool("Running", true);
        if(!bossMoveInitial)
        {
            bossMoveOn = true;
            bossMoveInitial = true;
        }
        Vector2 target = new Vector2(player_pos.position.x, boss_rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(boss_rb.position, target, boss_walkSpeed*Time.fixedDeltaTime);
        boss_rb.MovePosition(newPos);
     
    }
    public void TakeDamage(float damageAmount)
    {
        health -=damageAmount;
        healthBar.UpdateHeathBar(health, maxHealth);
    }
   
    public async void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player_pos.position.x && isFlipped)
        {
            transform.localScale = flipped;
           
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player_pos.position.x && !isFlipped)
        {
            transform.localScale = flipped;   
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    async UniTask Attackflag ()
    {


        await UniTask.Delay(200);
       
        //await UniTask.Delay(200);
       // boss_Animator.SetBool("Attacking", true);




    }

    async UniTask DamageTask()
    {


       
        boss_Animator.SetBool("hurt", true);
        TakeDamage(0.5f);
        
        await UniTask.Delay(200);
        boss_Animator.SetBool("hurt", false);
        if (health <= 0)
        {
            bossMoveOn = false;
            boss_Animator.SetBool("IsDead", true);
            await UniTask.Delay(2000);
            boss_Animator.SetBool("IsDead", false);

        }
        //await UniTask.Delay(200);
        // boss_Animator.SetBool("Attacking", true);


    }
    private void OnCollisionEnter2D(Collision2D collision) // 데미지 애니메이션 구현 
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name.Contains("Player"))
        {
            if(PlayerScript.instance.gethitBoxStatus())
            {
                Debug.Log("hit!!");
                DamageTask();
            }
            

        } 
    }

}
