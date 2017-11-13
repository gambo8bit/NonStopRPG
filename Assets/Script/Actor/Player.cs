using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Actor
{

    JoyStick Stick;

    private void Start()
    {
        ISPLAYER = true;    //플레이어냐?(Actor 변수)
        Stick = JoyStick.Instance;  //조이스틱
        NAV_MESH_AGENT = this.GetComponent<NavMeshAgent>();  //네브메쉬
        ANI = GetComponentInChildren<Animator>();  //애니메이터
    }

    protected override void Update()
    {
        if (Stick.IsPressed)    //조이스틱 작동중이면
        {

        


            Vector3 movePosition = transform.position;
            movePosition +=
                new Vector3(Stick.Axis.x, 0, Stick.Axis.y);

            AI.InputMove(movePosition);
           
        }
        else
        {
            if (Stick.IsAnyKeyDown == false)
            base.Update();  //인풋을 받아 행동하는게 아닌 경우 Actor의 Update를 돌린다
            //Anim.SetInteger("State", (int)eAIStateType.AI_STATE_IDLE);
        }

        if (Stick.IsPressAttackKey)
        {

            AI.InputAttack();
            
                
                return;
        }

    }





}
