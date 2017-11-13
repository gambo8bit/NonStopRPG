using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : BaseAI
{
    //공격 범위로 사용할 거리값
    [SerializeField]
    float attackRange = 3f;

    protected override IEnumerator Idle()
    {
        // 적 탐지
        float distance = 0f; //제일 가까운적과의 거리를 저장할 변수
        //TargetComponent 즉 현재 돌고있는 AI의 Actor의 가장가까운 적 Actor를 반환받고 그 적과의 거리까지 반환받는 FindNearEnemy()메서드를 실행
        BaseObject nearEnemyObject = ActorManager.Instance.FindNearEnemy(TargetComponent, out distance);

        if (nearEnemyObject != null) //근처의 적을 반환 받았다면
        {
            

            if(distance < attackRange)
            {
                Stop(); //움직임을 멈추고 정지
                TargetComponent.transform.LookAt(nearEnemyObject.transform.position);
                AddNextAI(eAIStateType.AI_STATE_ATTACK, nearEnemyObject); //다음 실행할 AI를 공격상태 & 타겟은 가장 가까운 적으로 구한 nearEnemyObject로 설정
            }
            else //공격범위거리 안에 들어오지않았으면 이동명령
            {
                AddNextAI(eAIStateType.AI_STATE_RUN);
            }
        }

        yield return StartCoroutine(base.Idle()); //부모인 BaseAI의 Idle 코루틴 실행(bUpdate 초기화를 위해)
    }


    //위의 기본상태인 idle과 비슷 다만 가까운적을찾고 공격범위안에들어오면 공격명령하고 공격범위가 아니라면 그제서야 실제 이동처리를 한다
    protected override IEnumerator Move()
    {
        // 적 탐지
        float distance = 0f; //제일 가까운적과의 거리를 저장할 변수
        //TargetComponent 즉 현재 돌고있는 AI의 Actor의 가장가까운 적 Actor를 반환받고 그 적과의 거리까지 반환받는 FindNearEnemy()메서드를 실행
        BaseObject nearEnemyObject = ActorManager.Instance.FindNearEnemy(TargetComponent, out distance);

        if (nearEnemyObject != null) //근처의 적을 반환 받았다면
        {
           

            if (distance < attackRange)
            {
                Stop(); //움직임을 멈추고 정지
                TargetComponent.transform.LookAt(nearEnemyObject.transform.position);
                AddNextAI(eAIStateType.AI_STATE_ATTACK, nearEnemyObject); //다음 실행할 AI를 공격상태 & 타겟은 가장 가까운 적으로 구한 nearEnemyObject로 설정
            }
            else //공격범위거리 안에 들어오지않았으면 실제 이동처리
            {
                SetMove(nearEnemyObject.SelfTransform.position);
            }
        }

        
        yield return StartCoroutine(base.Move());
    }


    protected override IEnumerator Attack()
    {
        NextAttackReady = false;
        yield return new WaitForEndOfFrame();   //바로 공격처리를 하면 공격이 씹히거나 할수있으므로 한프레임 기다리고 처리

        //IsAttack의 값은 공격애니메이션 스크립트에서 애니메이션의 진행정도에 따라 바꿔줄것임
        while (IsAttack)    //IsAttack이 true일때 즉 현재 Actor가 공격하고있는상태고 현재 이 메서드를 돌고있는 상태일때 현재 이 메서드 처리중인 Actor가 죽은상태가 아닌지 체크해줌 
        {
            if (OBJECTSTATE == eBaseObjectState.STATE_DIE)  // 공격처리하고있는 AI의 Actor의 상태가 뒤진상태이면 공격 처리 중단 (공격중에 뒤지면 바로 공격처리를 멈춰야되기때메)
                break;

            yield return new WaitForEndOfFrame();   
        }

        AddNextAI(eAIStateType.AI_STATE_IDLE);  //공격애니메이션이 끝나면 다음 AI상태를 기본상태로 추가

        yield return StartCoroutine(base.Attack());
    }


    protected override IEnumerator Die()
    {
        END = true; //END를 true로 만들어주면 일단 AI의 Update가 돌지않음
        yield return StartCoroutine(base.Die());
    }
}
