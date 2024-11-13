using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private float walkspeed = 20;
    [SerializeField] float health, maxHealth = 3f;
    private float xAxis;
    [SerializeField] private float jumpforce = 28;
    [SerializeField] private GameObject playerHitmap;
    private Animator anim;
    private bool playerIsDead = false;
    [SerializeField] private PlayerHealthBar playerHealthBar;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckPointY = 0.2f;
    [SerializeField] private float groundCheckPointX = 0.5f;
    [SerializeField] private LayerMask WhatisGround;

    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] AudioClip playerAttackSound;
    [SerializeField] AudioClip playerHurtSound;
    [SerializeField] AudioClip playerDeathSound;
    [SerializeField] SpriteRenderer playerSpriteRenderer;
    bool playerHitBoxOn = false;
    private bool movementLocked = false;
    public bool playerMovementEnd = false;
    public bool GethitBoxStatus()
    {
        return playerHitBoxOn;
    }

    void Start()
    {

        playerHealthBar = GetComponentInChildren<PlayerHealthBar>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealthBar.SetMaxHealth(health);
        playerHealthBar.UpdateHeathBar(health);
    }

    public void LockMovement()
    {
        movementLocked = true;
    }
    public void UnlockMovement()
    {
        movementLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        if (movementLocked || playerMovementEnd)
        {
            return;
        }

        Move();
        Flip();
        Attack();
        jump();
    }

    void GetInputs()
    {

        xAxis = Input.GetAxis("Horizontal");

    }
    public bool Grounded()
    {

        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckPointY, WhatisGround)
           || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckPointX, 0, 0), Vector2.down, groundCheckPointY, WhatisGround)
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
        if (Input.GetKeyDown(KeyCode.V))
        {
            anim.SetBool("Attacking", true);
            playerAudioSource.PlayOneShot(playerAttackSound);
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
        var scale = transform.localScale;
        if (xAxis < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && Grounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpforce);

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
        await UniTask.Delay(100);
        playerHitmap.SetActive(false);

    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        playerHealthBar.UpdateHeathBar(health);
    }
    async UniTask PlayerDamageTask()
    {


        if (playerIsDead) return;
        anim.SetBool("Damaged", true);
        playerAudioSource.PlayOneShot(playerHurtSound);
        movementLocked = true;
        TakeDamage(0.3f);
        await UniTask.Delay(1000);
        anim.SetBool("Damaged", false);
        if (health <= 0)
        {

            playerMovementEnd = true;

            await playerSpriteFadeOut(playerSpriteRenderer);
            await bgmEndTask();

        }
            movementLocked = false;


    }


    private async void OnCollisionEnter2D(Collision2D collision) // 데미지 애니메이션 구현
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name.Contains("BossHitmap")|| collision.gameObject.name.Contains("Boss"))
        {
          await  PlayerDamageTask();

        }
    }
    async UniTask playerSpriteFadeOut(SpriteRenderer _sprite)
    {
        playerHealthBar.gameObject.SetActive(false);

       playerAudioSource.PlayOneShot(playerDeathSound);
       playerIsDead = true;

        float fadeoutTime = 0.15f;
        Color tmpColor= _sprite.color;

        while(tmpColor.a>0)
        {
            tmpColor.a -= Time.deltaTime / fadeoutTime;
            _sprite.color = tmpColor;
            if (tmpColor.a < 0)
                tmpColor.a = 0;
            await UniTask.Delay(100);

        }
        _sprite.color = tmpColor;
    }
    async UniTask bgmEndTask()
    {

        var floor = FindAnyObjectByType<FloorControl>();
        var boss = FindAnyObjectByType<BossMovement>();
        float fadeoutTime = 0.2f;
        float tmp_volme = floor.floorAudioSource.volume;

        while (tmp_volme > 0)
        {
            tmp_volme -= Time.deltaTime / fadeoutTime;
            floor.floorAudioSource.volume = tmp_volme;
            if (tmp_volme < 0)
                tmp_volme = 0;
            await UniTask.Delay(100);

        }
        floor.floorAudioSource.volume = tmp_volme;
        boss.bossMoveEnd = true;
        await UniTask.Delay(3000);

        SceneManager.LoadScene(2);
    }


}
