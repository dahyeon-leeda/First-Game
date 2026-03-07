using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class EnemyMove2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 1; //속도변수
    
    Rigidbody2D rigid;
    public int nextMove;
    Animator animator;
    Vector3 movement;
    float dir;
    int movementFlag = 0;
    
    // override Function
    
    // Initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator> ();
        
        movementFlag = 2; //오른쪽부터 움직이기

        //layer이름 확인(안떨어지기 1)
        int platformLayer = LayerMask.NameToLayer("Platform");
        Debug.Log("Platform layer number: " + platformLayer);
    }

    // Coroutine
    

    // Update is called once per frame
    void FixedUpdate()
    {
        //platform check (안떨어지기 2)
        if(movementFlag == 1) dir =-1;
        else if(movementFlag== 2) dir =1;
        else return;
        
        Vector2 frontVec = new Vector2(rigid.position.x + dir*0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
       if (rayHit.collider == null)
        {
          movementFlag =(movementFlag == 1)? 2 : 1;
        }

        //move
         Move ();
        
}
    void Move()
    {
        Vector3 moveVelocity= Vector3.zero;

        if(movementFlag == 1){
            moveVelocity = Vector3.left;
            transform.localScale = new Vector3 (-1,1,1);   
        }
        else if(movementFlag == 2){
            moveVelocity = Vector3.right;
            transform.localScale = new Vector3 (1,1,1);
        }
        rigid.linearVelocity = new Vector2(dir * speed, rigid.linearVelocity.y);
    }
}

