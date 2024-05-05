using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private float walkspeed = 20;
    private float xAxis;
    [SerializeField]private float jumpforce = 28;
    [SerializeField] GameObject playerHitmap;
    Animator anim;
    [SerializeField] private Transform groundCheckPoint;
    
    [SerializeField] private float groundCheckPointY = 0.2f;
    [SerializeField] private float groundCheckPointX = 0.5f;
    [SerializeField] private LayerMask WhatisGround;
    bool playerHitBoxOn =false;
    public static PlayerScript instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public bool gethitBoxStatus()
    { return playerHitBoxOn; }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        if (BossMovement.instance.bossMoveOn)
        {
            Move();
            //Flip();
            Attack();
            jump();
        }
    }

    void GetInputs()
    {

        xAxis = Input.GetAxis("Horizontal");

    }
    public bool Grounded()
    {

        if(Physics2D.Raycast(groundCheckPoint.position,Vector2.down,groundCheckPointY,WhatisGround)
           || Physics2D.Raycast(groundCheckPoint.position+new Vector3(groundCheckPointX,0,0), Vector2.down, groundCheckPointY, WhatisGround)
           || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckPointX, 0, 0), Vector2.down, groundCheckPointY, WhatisGround)
           )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Move()
    {

        rb.velocity = new Vector2(walkspeed * xAxis, rb.velocity.y);
        anim.SetBool("Running", rb.velocity.x != 0 && Grounded());
    }

    private void Attack()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            anim.SetBool("Attacking", true);
            hitmapSpawn();
        }
        else 
        {
            anim.SetBool("Attacking", false);
           // playerHitmap.SetActive(false);
        }
    }
    void Flip()//방향 바꾸기
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y,transform.localScale.z);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    void jump() 
    {
        if(Input.GetButtonDown("Jump")&& Grounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpforce) ;

        }
        /*
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

        }
        */
    }
    async UniTask hitmapSpawn()
    {
        playerHitmap.SetActive(true);
        playerHitBoxOn = true;
        await UniTask.Delay(200);
        playerHitmap.SetActive(false);
        playerHitBoxOn = false;
    }



}
