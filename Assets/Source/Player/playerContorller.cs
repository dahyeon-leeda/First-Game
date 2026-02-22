using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerContorller : MonoBehaviour
{
    [Header("Player Movement Settings")]
    public float moveSpeed;
    public float jumpForce;
    public float dashForce;

    [Header("Player Jump Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private float moveInput;
    private bool isFacingRight = true;
    private bool isGrounded;
    private int jumpCount = 0;

    private Rigidbody2D rd;
    private Animator anim;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        // 1. MoveCharacter() 함수에서 Rigidbody2D의 velocity를 사용하여 플레이어의 이동을 구현합니다.
        rd.linearVelocity = new Vector2(moveInput * moveSpeed, rd.linearVelocity.y);
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
}
