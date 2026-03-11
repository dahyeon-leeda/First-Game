using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMove4 : MonoBehaviour
{
// 피격
public float knockbackForce = 4f;   // 가로 넉백
public float knockbackUpForce = 2f; // 위로 튀는 힘
public float hitStunTime = 0.2f;

bool isHit = false;                 // 피격 상태

    
    // 플레이어 관련
    

    public Transform player;      // 플레이어 위치 (Inspector에서 Player 드래그)
    public float chaseSpeed = 2f; // 플레이어 추적 속도


    public float speed = 1f;      // 기본 이동 속도 (패트롤 속도)

    int movementFlag = 0;         // 이동 방향
                                  // 1 = 왼쪽
                                  // 2 = 오른쪽

    float dir;                   // 실제 이동 방향 (-1 또는 1)


    
    // Aggro 상태
    

    bool isAggro = false;        // 맞기 전까지 false
                                 // 맞으면 true → 플레이어 추적

    bool isDead = false; //죽기전까지 false



    // 컴포넌트


    Rigidbody2D rigid;           // 물리 이동용
    Animator animator;

    // 초기화
    void Start()
    {
    movementFlag = 2;
        // Rigidbody 가져오기
        rigid = GetComponent<Rigidbody2D>();

        // 처음 이동 방향 설정
        movementFlag = 2; // 오른쪽 이동 시작
        animator = GetComponentInChildren<Animator>();
    }

    // ===============================
    // 물리 업데이트 (FixedUpdate)
    // ===============================

    void FixedUpdate()
    {        
        //죽으면 안움직임
     if (isDead) return;
   // 피격 상태면 움직이지 않음
    if (isHit)
       return;

    // 1 몬스터가 맞은 상태면 플레이어 추적        
    if (isAggro)
       {
        Chase();   // 플레이어 추적
            return;    // 패트롤 코드 실행 안함
       }
    
        // 2 맞지 않았으면 패트롤

        Patrol();
    }


    // 패트롤 이동 (방목 이동)

    void Patrol()
    {
        // 이동 방향 설정
        if (movementFlag == 1)
            dir = -1; // 왼쪽
        else
            dir = 1;  // 오른쪽


        // --------------------------------
        // 낭떠러지 체크
        // --------------------------------

        // 몬스터 앞쪽 위치 계산
        Vector2 frontVec =
            new Vector2(rigid.position.x + dir * 0.3f, rigid.position.y);

        // Scene 창에서 Ray 확인용
        Debug.DrawRay(frontVec, Vector3.down, Color.green);

        // 바닥 체크
        RaycastHit2D rayHit =
            Physics2D.Raycast(frontVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));

        // 바닥이 없으면 방향 변경
        if (rayHit.collider == null)
        {
            movementFlag = (movementFlag == 1) ? 2 : 1;
        }


        // --------------------------------
        // 실제 이동
        // --------------------------------

        rigid.linearVelocity =
            new Vector2(dir * speed, rigid.linearVelocity.y);


        // 캐릭터 방향 뒤집기
        if (dir == -1)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }


    // ===============================
    // 플레이어 추적
    // ===============================

    void Chase()
    {
        // 플레이어 위치 기준 방향 계산
        float dir =
            (player.position.x > transform.position.x) ? 1 : -1;

        // 플레이어 방향으로 이동
        rigid.linearVelocity =
            new Vector2(dir * chaseSpeed, rigid.linearVelocity.y);

        // 캐릭터 방향 뒤집기
        transform.localScale =
            new Vector3(dir, 1, 1);
    }


    // ===============================
    // 몬스터가 공격 맞았을 때 호출
    // ===============================

    void OnHit()
    {
        // 맞으면 Aggro 상태
        isAggro = true;

        StartCoroutine(HitReaction());
    }
//죽을때 애니메이션
void OnDeath()
{
    //죽으면 멈춤
     if (isDead) return;
    isDead = true;

    animator.SetTrigger("Death");
    rigid.linearVelocity = Vector2.zero;
}

IEnumerator HitReaction()
{
    isHit = true;

    float hitDir = transform.position.x - player.position.x;
    hitDir = Mathf.Sign(hitDir);

    rigid.linearVelocity = new Vector2(hitDir * knockbackForce, knockbackUpForce);

    yield return new WaitForSeconds(0.2f);

    isHit = false;
}
}