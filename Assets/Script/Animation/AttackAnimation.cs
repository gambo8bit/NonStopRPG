using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimation : StateMachineBehaviour
{
    Actor TargetActor;
    
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TargetActor = animator.GetComponentInParent<Actor>(); //애니메이터를 전부 캐릭터 하위오브젝트에 뒀으므로 getcomponentinParent로 animator가 구동중인 캐릭터의 Actor 가져옴
        
        if(TargetActor != null && TargetActor.AI.CurrentAIState == eAIStateType.AI_STATE_ATTACK) //TargetActor가 존재하고 해당 Actor의 AI상태가 공격상태일때
        {
            TargetActor.AI.IsAttack = true;

        }
    }



    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        //normalizedTime : Animation 플레이타임을 0~1로 만들기  so 1보다 크다라고 조건을 건것은 애니메이션 끝났으면 이란뜻
        if (stateInfo.normalizedTime >= 1.0f && TargetActor.AI.IsAttack )
        {
            //animator.SetInteger("State", 1);

            if (TargetActor.AI.CurrentAIState == eAIStateType.AI_STATE_ATTACK)
            TargetActor.AI.IsAttack = false;
           
        }
    }

}
