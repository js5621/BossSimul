using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class BossMovement : MonoBehaviour
{
    private Rigidbody2D boss_rb;//보스의 리지드 바디
    [SerializeField] float health, maxHealth = 3f;
    [SerializeField] private FloatingHealthBar healthBar;
    [SerializeField] private float boss_walkSpeed = 20;// 보스 이동속도
    [SerializeField] private GameObject bossHitmap;
    [SerializeField] AudioSource bossAudioSource;
    [SerializeField] AudioClip bossAttackSound;
    [SerializeField] AudioClip bossHurtSound;
    [SerializeField] AudioClip appearEffect;
    [SerializeField] AudioClip disappearEffect;

    private Vector2 targetPosition;
    public Animator boss_Animator;
    public Transform player_pos;
    public bool isFlipped = false;
    public float attackRange = 5f;

    private bool moveTowardPlayer = false;
    public bool bossMoveEnd = false;
    private bool lookAtPlayer = true;
    private bool onAttack=false;
    private bool bossIsDead = false;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        boss_rb = GetComponent<Rigidbody2D>();
        boss_Animator = GetComponent<Animator>();
        healthBar.SetMaxHealth(health);
        healthBar.UpdateHeathBar(health);

    }

    // Update is called once per frame
    async void Update()
    {
        if (bossMoveEnd)
            return;
       await LookAtPlayer();

       await tryAttack();




    }

    public async UniTask PlayEncounterEffectAsync()
    {
        await UniTask.Delay(500);
        bossAudioSource.PlayOneShot(appearEffect);
        await UniTask.Delay(2000);
        // TODO: 적당한 등장 연출 애니메이션 처리

        boss_Animator.SetBool("Running", true);
        moveTowardPlayer = true;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        healthBar.UpdateHeathBar(health);
    }

    async UniTask LookAtPlayer()
    {
        if(!lookAtPlayer)
        {
            return;
        }
        var flipped = transform.localScale;
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

    void MoveTowardPlayer()
    {
        if (!moveTowardPlayer||bossMoveEnd)
        {
            return;
        }

        var target = new Vector2(player_pos.position.x, boss_rb.position.y);
        var newPos = Vector2.MoveTowards(boss_rb.position, target, boss_walkSpeed * Time.deltaTime);
        boss_rb.MovePosition(newPos);
    }

    async UniTask Attackflag()
    {


        await UniTask.Delay(200);

        //await UniTask.Delay(200);
        // boss_Animator.SetBool("Attacking", true);




    }

    async UniTask DamageTask()
    {


        if(bossIsDead) return;
        boss_Animator.SetBool("hurt", true);
        bossAudioSource.PlayOneShot(bossHurtSound);
        await UniTask.Delay(200);
        TakeDamage(0.5f);

        await UniTask.Delay(50);
        boss_Animator.SetBool("hurt", false);
        if (health <= 0)
        {
           await PlayDeadAnimation();

            // await boss_Animator.Play("Dead");
            this.gameObject.SetActive(false);

            await bgmEndTask();

        }
        //await UniTask.Delay(200);
        // boss_Animator.SetBool("Attacking", true);


    }

    async UniTask tryAttack()
    {
        if (onAttack)
        // Early exit 패턴입니다. 들여쓰기 단계를 줄여줘서 좋습니다.
        {
            return;
        }
        try
        {
            onAttack = true;
            MoveTowardPlayer();
            if (Math.Abs(Vector2.Distance(player_pos.position, boss_rb.position)) <= attackRange&&!bossMoveEnd)
            {
                moveTowardPlayer = false;
                lookAtPlayer = false;
                boss_Animator.SetBool("Running", false);
                bossAudioSource.PlayOneShot(bossAttackSound);  //공격음 재생
                await UniTask.Delay(50);
                boss_Animator.SetBool("Attacking", true);

                await hitmapSpawn();
                boss_Animator.SetBool("Attacking", false);
                await UniTask.Delay(50);
                moveTowardPlayer = true;
                lookAtPlayer = true;
            }
        }
        finally
        {
            // 위에 try 구문 내에서 오류가 발생해서 코드가 중단된다 하더래도
            // finally 구문은 무조건 불리기 때문에 onAttack 상태가 꺼지게 만들면
            // 보스가 버그때문에 영원히 공격 못하는 상황은 발생하지 않습니다.
            onAttack = false;
        }


    }
    async UniTask PlayDeadAnimation()
    {

        bossMoveEnd = true;
        await UniTask.Delay(200);
        healthBar.gameObject.SetActive(false);
        bossAudioSource.PlayOneShot(disappearEffect);
        boss_Animator.SetBool("IsDead", true);


        //await UniTask.WaitUntil(() => boss_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        await UniTask.Delay(2000);
        boss_Animator.SetBool("IsDead", false);
        await UniTask.Delay(500);
    }


    private async void OnCollisionEnter2D(Collision2D collision) // 데미지 애니메이션 구현
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "PlayerHitmap")
        {
           await DamageTask();
        }
    }
    async UniTask hitmapSpawn()
    {

        await UniTask.Delay(1000);
        bossHitmap.SetActive(true);

        await UniTask.Delay(200);
        bossHitmap.SetActive(false);
    }

    async UniTask bgmEndTask()
    {

        var floor = FindAnyObjectByType<FloorControl>();
        var player = FindAnyObjectByType<PlayerScript>();
        float fadeoutTime = 0.2f;
        float tmp_volme = floor.floorAudioSource.volume;
        bossIsDead = true;
        while (tmp_volme > 0)
        {
            tmp_volme -= Time.deltaTime / fadeoutTime;
            floor.floorAudioSource.volume = tmp_volme;
            if (tmp_volme < 0)
                tmp_volme = 0;
            await UniTask.Delay(100);

        }
        floor.floorAudioSource.volume = tmp_volme;
        player.playerMovementEnd = true;
        await UniTask.Delay(4000);

        SceneManager.LoadScene(3);
    }


}
