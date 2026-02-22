using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class EnemyMove1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed; //속도변수
    public Rigidbody2D target; //물리적인 목표변수

    bool islive; //생존여부
    
    Rigidbody2D rigid;
    public int nextMove;
    SpriteRenderer spriter;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Think();
        Invoke("Think", 3);
        spriter = GetComponent<SpriteRenderer>();
        //layer이름 확인
        int platformLayer = LayerMask.NameToLayer("Platform");
        Debug.Log("Platform layer number: " + platformLayer);
    }
        
    // Update is called once per frame
    void FixedUpdate()
    {
        //move
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
        //platform check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
       if (rayHit.collider == null)
        {
          nextMove *= -1;
          CancelInvoke();
          Invoke("Think", 3);
        }
        
}
    void Think()
    {
        nextMove = Random.Range(-1, 2);

         Invoke("Think", 3);
    }
}

