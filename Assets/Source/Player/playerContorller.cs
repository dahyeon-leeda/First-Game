using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;

public class playerContorller : MonoBehaviour
{
    [Header("Player Movement Settings")]
    public float jumpForce;
    public float dashForce;

    [Header("Player Jump Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Player Attack Settings")]
    public LayerMask enemyLayer;
    public float attackStopDuration = 0.3f;
    private bool isAttacking = false;

    [Header("Player Basic Attack Settings")]
    public GameObject attackPrefab;
    public float attackDamage = 10.0f;
    public float attackCooldown = 0.5f;
    public float attackManaCost = 10.0f;
    public Vector2 spawnOffset = new Vector2(2.0f, 3.0f);
    public Vector2 fallVelocity = new Vector2(0f, -10.0f);
    public float effectLifeTime = 2.0f;
    private float lastAttackTime;

    private float moveInput;
    private bool isFacingRight = true;
    private bool isGrounded;
    private int jumpCount = 0;

    private Rigidbody2D rd;
    private Animator anim;
    private HealthComponent health;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<HealthComponent>();
    }

    void Update()
    {
        // 1. Update() 함수에서 Physics2D.OverlapCircle() 함수를 사용하여 플레이어가 땅에 닿아 있는지 확인하고, isGrounded 변수를 업데이트합니다.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. Update() 함수에서 플레이어가 땅에 닿아 있는지 확인하여 isGrounded 변수를 업데이트하고, 점프 카운트를 초기화합니다.
        if (isGrounded && rd.linearVelocity.y <= 0)
        {
            jumpCount = 0;
            anim.SetTrigger("isGrounded");
        }

        if(Mathf.Abs(rd.linearVelocity.x) > 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (!isGrounded && rd.linearVelocity.y < 0)
        {
            anim.SetTrigger("isFalling");
        }
        else if (!isGrounded && rd.linearVelocity.y > 0)
        {
            anim.SetTrigger("isJumping");
        }

        Flip();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void OnMove(InputValue value)
    {
        // 1. Input System을 사용하여 플레이어의 이동 입력을 받아 moveInput 변수에 저장합니다.
        Vector2 inputVector = value.Get<Vector2>();
        moveInput = inputVector.x;
    }

    private void Flip()
    {
        // 1. 플레이어가 이동 방향을 바꿀 때마다 Flip() 함수를 호출하여 캐릭터의 방향을 전환합니다.
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void MoveCharacter()
    {
        if (isAttacking)
        {
            rd.linearVelocity = new Vector2(0, rd.linearVelocity.y);
            return;
        }
        // 1. MoveCharacter() 함수에서 Rigidbody2D의 velocity를 사용하여 플레이어의 이동을 구현합니다.
        rd.linearVelocity = new Vector2(moveInput * health.moveSpeed, rd.linearVelocity.y);
    }

    void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        
        if (isGrounded)
        {
            // 1. OnJump() 함수에서 플레이어가 점프할 때마다 jumpCount 변수를 증가시키고, 최대 2회까지 점프할 수 있도록 구현합니다.
            rd.linearVelocity = new Vector2(rd.linearVelocity.x, jumpForce);
            jumpCount = 1;
        }
        else if (jumpCount == 1)
        {
            // 2. OnJump() 함수에서 플레이어가 공중에서 점프할 때, 대시 방향과 힘을 적용하여 공중 대시를 구현합니다.
            rd.linearVelocity = new Vector2(rd.linearVelocity.x, jumpForce);

            jumpCount = 2;
        }
    }

    async void OnAttack(InputValue value)
    {
        Debug.Log("Attack!");

        try
        {
            // 1. OnAttack() 함수에서 공격 입력이 들어오면, 공격 쿨다운이 끝났는지 확인하고, 공격에 필요한 마나가 충분한지 확인합니다. 조건이 충족되면 PerformBasicAttack() 함수를 호출하여 공격을 수행합니다.
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (health != null && health.UseMana(attackManaCost))
                {
                    lastAttackTime = Time.time;
                    await PerformBasicAttack(this.GetCancellationTokenOnDestroy());
                }
            }
        }
        catch (System.OperationCanceledException)
        {

        }
    }

    private async UniTask PerformBasicAttack(CancellationToken token)
    {
        // 1. PerformBasicAttack() 함수에서 공격이 시작되면 isAttacking 변수를 true로 설정하여 플레이어의 이동을 제한합니다.
        isAttacking = true;

        // 2. 플레이어 방향 확인
        float direction = isFacingRight ? 1f : -1f;

        // 3. PerformBasicAttack() 함수에서 spawnOffset를 사용하여 공격 이펙트가 생성될 위치를 계산합니다.
        Vector3 targetSpawnPos = transform.position + new Vector3(spawnOffset.x * direction, spawnOffset.y, 0);

        // 4. PerformBasicAttack() 함수에서 Physics2D.Raycast() 함수를 사용하여 공격 이펙트가 생성될 위치에 장애물이 있는지 확인하고, 장애물이 있다면 공격 이펙트의 위치를 조정합니다.
        Vector2 rayDir = (targetSpawnPos - transform.position).normalized;
        float rayDistance = Vector2.Distance(transform.position, targetSpawnPos);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, rayDistance, groundLayer);
        Vector3 finalSpawnPos = targetSpawnPos;

        if (hit.collider != null)
        {
            finalSpawnPos = (Vector3)hit.point - (Vector3)(rayDir * 0.1f);
        }

        GameObject effect = Instantiate(attackPrefab, finalSpawnPos, Quaternion.identity);

        try
        {
            // 5. PerformBasicAttack() 함수에서 공격 이펙트가 생성된 후, BasicAttack 스크립트의 Setup() 함수를 호출하여 공격 데미지와 레이어 정보를 전달합니다.
            if (effect.TryGetComponent<BasicAttack>(out var effectScript))
            {
                float totalDamage = attackDamage + health.attack;
                effectScript.Setup(totalDamage, enemyLayer, groundLayer, fallVelocity);
            }

            try
            {
                // 6. PerformBasicAttack() 함수에서 공격이 시작된 후, attackStopDuration 동안 플레이어의 이동을 제한하고, 이후에 isAttacking 변수를 false로 설정하여 이동을 다시 허용합니다.
                await UniTask.Delay(System.TimeSpan.FromSeconds(attackStopDuration), cancellationToken: token);
            }
            finally
            {
                isAttacking = false;
            }

            // 7. PerformBasicAttack() 함수에서 공격 이펙트가 생성된 후, effectLifeTime 동안 공격 이펙트가 유지되도록 구현합니다. 이때, 공격이 시작된 후 attackStopDuration 동안은 플레이어의 이동이 제한되므로, 공격 이펙트의 생존 시간을 attackStopDuration 이후로 계산하여 적용합니다.
            float remainingTime = Mathf.Max(0, effectLifeTime - attackStopDuration);
            await UniTask.Delay(System.TimeSpan.FromSeconds(remainingTime), cancellationToken: token);
        }
        finally
        {
            // 8. PerformBasicAttack() 함수에서 공격이 끝난 후, Instantiate() 함수를 사용하여 생성된 공격 이펙트를 일정 시간 후에 Destroy() 함수를 사용하여 제거합니다.
            if (effect != null) Destroy(effect);
        }
    }
}
