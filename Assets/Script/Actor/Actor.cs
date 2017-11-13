using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Actor : BaseObject     //플레이어나 에너미같은 움직이고 행동하는 게임오브젝트들의 공통된 부분을 관장
{
    bool _IsPlayer = false; //해당 Actor가 플레이어인지 판단(플레이어 단독적인 처리를 위해 구분값을 만든것)
    public bool ISPLAYER
    {
        get { return _IsPlayer; }
        set { _IsPlayer = value; }
    }

    Animator Ani;
    public Animator ANI
    {
        get { return Ani; }
        set { Ani = value; }
    }
    NavMeshAgent Nav;
    public NavMeshAgent NAV_MESH_AGENT
    {
        get { return Nav; }
        set { Nav = value; }
    }
    // AIType
    [SerializeField]
    eAIType AIType = eAIType.NONE_AI; //해당 Actor의 AIType
    public eAIType AITYPE
    {
        get { return AIType; }
    }

    // TeamType
    [SerializeField]
    eTeamType _TeamType;
    public eTeamType TeamType
    {
        get { return _TeamType; }
    }


    //Actor의 AI
    BaseAI _AI = null;
    public BaseAI AI
    {
        get { return _AI; }
        set { _AI = value; }
    }

    
        
    BaseObject HitTarget;   //공격대상


    // AI 생성
    private void Awake()
    {
        //본인 AI에 맞는 게임오브젝트 생성
        switch (AIType)
        {

            case eAIType.BasicAI:
                {
                    GameObject go = new GameObject(AITYPE.ToString(), typeof(BasicAI)); //Actor의 AIType에 맞는 AI컴포넌트 게임오브젝트 생성
                    go.transform.SetParent(SelfTransform); // AI 오브젝트를 해당 Actor의 하위에 둠
                    _AI = go.GetComponent<BasicAI>();
                }
                break;


            case eAIType.NONE_AI:
                break;
        }


        AI.TargetComponent = this; //AI의 타겟오브젝트를 해당 Actor의 베이스오브젝트로 설정  



        ActorManager.Instance.AddActor(this);       //모든 액터들은 생성되면 바로 Awake()에서 ActorManager의 DicActor라는 Actor들을 보관하는 딕셔너리에 저장 해놓는다
    }



    //AI 업데이트
    protected virtual void Update()
    {
        AI.UpdateAI();  //캐릭터가 본인의 개별적인 업데이트를 구동하고있지않다면 캐릭터가 가지고 있는 AI의 업데이트를 돌린다(EX.Player 인풋받고있지않을땐 AI 실행)
        
        if(AI.END)  // AI가 해당 캐릭터가 죽었는지 살았는지 알려주게 만들꺼
        {
            Destroy(SelfObject);    //해당 캐릭터의 게임오브젝트 제거~
        }
    }



    public override void ThrowEvent(string keyData, params object[] datas)
    {
        switch(keyData)
        {
            case ConstValue.ActorData_SetTarget:
                {
                    HitTarget = datas[0] as BaseObject; //공격대상 설정
                }
                break;

        }
    }


    public override object GetData(string keyData, params object[] datas)
    {
        switch(keyData)
        {
            case ConstValue.ActorData_Team:     //엑터매니저 GetSearchEnemy()사용할때 thisActor의 팀타입 받아올때 사용
                return TeamType;
                
        }
        return null;
    }



    public virtual void OnDestroy()
    {
        if(ActorManager.Instance != null)
        ActorManager.Instance.RemoveActor(this); //Actor들 삭제는 ActorManager에서 일괄 처리


    }




}
