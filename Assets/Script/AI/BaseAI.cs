using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NextAI //다음 AI행동을 명령하기위한 필요한 기본데이터단위
{
    public eAIStateType StateType;
    public BaseObject TargetObject;
    public Vector3 Position;
}





public class BaseAI : BaseObject
{
    protected List<NextAI> ListNextAI = new List<NextAI>(); // 다음 AI 행동을 결정하는 NextAI들을 담아둘 리스트
    protected eAIStateType _CurrentAIState = eAIStateType.AI_STATE_IDLE; //현재 AI상태를 나타내는 ENUM형 변수
    public eAIStateType CurrentAIState
    {
        get { return _CurrentAIState; }
    }

    bool bUpdateAI = false; //AI업데이트를 한 상태인가 아닌 상태인가 (false이면 AIUpdate돌리고 true면 이미 했다는 의미이니 업데이트이후 false로 바꿔줌)
    bool bAttack = false; //공격중인가  아닌가
    public bool IsAttack    //bAttack 전역에서 접근위해 겟터 셋터
    {
        get { return bAttack; }
        set { bAttack = value; }
    }

     bool bEnd = false; //뒤졌나 살았냐?
    public bool END
    {
        get { return bEnd; }
        set { bEnd = value;}
    }


    public bool NextAttackReady = true;    //공격처리 코루틴 끝나면 true로 바꿔서 다음공격 준비되었다고 알려줌

    Animator Anim = null; //AI 돌고있을때 사용할 Animator
    NavMeshAgent Agent = null;  //AI 돌때 사용할 NavMeshAgent

    private void Start()
    {
        Anim = SelfTransform.GetComponent<Actor>().ANI;
        Agent = SelfTransform.GetComponent<Actor>().NAV_MESH_AGENT;
    }





    //Animation을 _CurrentAIState상태에 따라 바꿔주는 메서드
    void ChangeAniamation() 
    {
        if(Anim == null)
        {
            Debug.LogError(SelfObject.name + "에 Animator 가 없습니다.");
            return;
        }
        Anim.SetInteger("State", (int)_CurrentAIState); //현재 AI상태에 따른 애니메이션 파라미터값 변환

    }


    
    
    
    
    //nextAI 새로 생성 & 리스트에 추가
    protected void AddNextAI(eAIStateType nextAIStateType,BaseObject targetObject = null, Vector3 position = new Vector3())
    {
        //넘겨받은 데이터로 세팅한 NextAI 생성
        NextAI nextAI = new NextAI  
        {
            StateType = nextAIStateType,
            TargetObject = targetObject,
            Position = position
          
        };
        //리스트에 추가
        ListNextAI.Add(nextAI);

    }










    //실행할 nextAI 안의 데이터의 값에 맞춰 세팅(ThrowEvent,애니메이션&CurrentState 세팅)
    void SetNextAI(NextAI nextAI)
    {
        if (nextAI.TargetObject != null) //NEXTAI에 타겟오브젝트값이 입력됬으면
        {
            //HITTARGET 설정
            TargetComponent.ThrowEvent(ConstValue.ActorData_SetTarget, nextAI.TargetObject);
        }

        switch (nextAI.StateType)
        {   //_CurrentAIState 값을 상태에 맞게 바꿔주고 애니메이션 처리
            case eAIStateType.AI_STATE_IDLE:
                ProcessIdle();
                break;

            case eAIStateType.AI_STATE_RUN:
                ProcessMove();
                break;

            case eAIStateType.AI_STATE_ATTACK:
                ProcessAttack();
                break;
            case eAIStateType.AI_STATE_DIE:
                ProcessDie();
                break;
        }
    }



    //============================CurrentState 값 최신화 & CurrentState값에 맞는 애니메이션 처리====================================
    protected virtual void ProcessIdle()    //하위 AI에 다르게 처리를 할수있게 가상으로 선언
    {
        //ChangeAnimation은 _CurrentAIState의 값에따라 애니메이션이 정해지므로 맞는 값으로 바꿔줌
        _CurrentAIState = eAIStateType.AI_STATE_IDLE; //현재 상태를 행동하려는것에 맞춰 바꿔줌
        ChangeAniamation();

    }

    protected virtual void ProcessMove()
    {
        _CurrentAIState = eAIStateType.AI_STATE_RUN;
        ChangeAniamation();
    }

    protected virtual void ProcessAttack()
    {
        
        _CurrentAIState = eAIStateType.AI_STATE_ATTACK;
        ChangeAniamation();
    }


    


    protected virtual void ProcessDie()
    {
        _CurrentAIState = eAIStateType.AI_STATE_DIE;
        ChangeAniamation();
    }
    //======================================================================================================================
    
        ////안쓸꺼 내용만 이해
        //protected bool MoveCheck()  //AI가 현재 이동할수있는상태인지 TRUF false 반환 해서 알려주는 메서드
        //{
        //    if (Agent.pathStatus == NavMeshPathStatus.PathComplete) //현재 AI의 네브메쉬의 길찾기 상태가 목적지 도착상태이고
        //    {
        //        if (Agent.hasPath == false ||                       //네브메쉬가 현재 경로를 가지고있지않거나 계산중인경로ㄱ가 없으면
        //            Agent.pathPending == false)
        //        {
        //            return true;    //AI는 현재 목적지도없고 멈춰져있으니 움직일수있다고 true값 반환
        //        }
        //    }

        //    return false;   //현재 이동중이니 움직일수없음이라고 false 반환해서 알려줌
        //}



        //실제 캐릭터 이동(NavmeshAgent 새로운 경로 설정)
    protected void SetMove(Vector3 position)
    {
        Agent.isStopped = false;
        Agent.SetDestination(position);
    }


    //이동 멈추기(NavmeshAgent isStopped = true 처리해서 이동할수없게 함)
    protected void Stop()
    {
        Agent.isStopped = true;
    }


    public void InputMove(Vector3 position)
    {
        bUpdateAI = false;  //BaseAI Update부 돌릴지 말지 결정하는 부분
        ClearAllAI();   //인풋이동시는 AI 제거(혹시 처리안된 AI가 남아있을수 있으므로)
        SetMove(position);  //실제 위치 이동
        ProcessMove();  //for Animation
        
    }

    public void InputAttack()
    {
        if (NextAttackReady)
        {
            ProcessIdle();
            bUpdateAI = false;
            ClearAllAI();   //인풋받아 실행하기에 NextAI있는거 다 삭제
            Stop();         //

            ProcessAttack();
            StartCoroutine("Attack");
        }

    }



    public void UpdateAI()
    {
        if (END == true)
            return;         //캐릭터가 죽은 상태이면 Update돌리지않음

        if (bUpdateAI == true)  //업데이트를 한상태이면 돌리지않음
            return;

        if(ListNextAI.Count > 0)    //ListNextAI에 NextAI가 저장된 숫자가 1개이상있을경우
        {
            SetNextAI(ListNextAI[0]);   //이번에 실행할 NextAI에 설정한 AIStateType의 값을 CurrentAIState값으로 설정 & 그 값에 맞는 애니메이션 처리
            ListNextAI.RemoveAt(0);
        }

        //BaseObject 상태가 죽어있으면 NextAI리스트 다 비우고 죽음 처리
        if(OBJECTSTATE == eBaseObjectState.STATE_DIE)
        {
            ListNextAI.Clear();
            ProcessDie();
        }

        bUpdateAI = true; //Update 돌았다고 표시해 계속 업데이트가 도는것을 방지

        switch (_CurrentAIState)    //SetNextAI에서 설정해준 _CurrentAIState값에 맞는 코루틴 동작(디테일한 연산처리)
        {
            case eAIStateType.AI_STATE_IDLE:
                StartCoroutine("Idle");
                break;

            case eAIStateType.AI_STATE_RUN:
                StartCoroutine("Move");
                break;

            case eAIStateType.AI_STATE_ATTACK:
                StartCoroutine("Attack");
                break;

            case eAIStateType.AI_STATE_DIE:
                StartCoroutine("Die");
                break;
        }
    }



    protected virtual IEnumerator Idle()    //AI 종류마다 다르게 돌게하기위해 이 부분은 가상함수로 구현
    {
        bUpdateAI = false; // 여기서만 bUpdateAI값 false로 만들어주기때문에 여기를 거치기 전까지 업데이트는 한번이상 돌지않음(한가지 명령을 완료하기전에 다른명령을 받고 업데이트하는것을 제한)
        yield break; //코루틴 종료
    }

    protected virtual IEnumerator Move()
    {
        bUpdateAI = false;
        yield break;
    }

    protected virtual IEnumerator Attack()
    {
        bUpdateAI = false;
        NextAttackReady = true;
        
        yield break;
    }


    protected virtual IEnumerator Die()
    {
        bUpdateAI = false;
        yield break;
    }







    public void ClearAllAI()    //인풋받아 움직일때는 AI삭제하게 하기위해
    {
        ListNextAI.Clear();
    }

}