using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoSingleton<ActorManager>
{
    //Actor들을 TeamType을 기준으로 보관해놓을 딕셔너리
    Dictionary<eTeamType, List<Actor>> DicActor = new Dictionary<eTeamType, List<Actor>>();
    

	public Actor PlayerLoad()
    {
        //프리팹 로드
        GameObject playerPrefab = Resources.Load("Prefabs/Actor/" + "Player") as GameObject;

        // Scene에 생성
        GameObject go = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        return go.GetComponent<Actor>(); //플레이어 Actor 컴포넌트 반환

    }




   public void AddActor(Actor actor)
    {
        List<Actor> listActor = null;           //DicActor에 새로 넣거나 혹은 DicActor에 존재하는 List를 받아오기위한 변수
        eTeamType teamType = actor.TeamType;    //추가하려는 actor의 팀타입 받아놓음 

        if(DicActor.ContainsKey(teamType) == false)     //DicActor에 actor의 팀타입에맞는 리스트가 저장된있지않으면
        {
            listActor = new List<Actor>(); //DicActor에 저장할 새로운 리스트를 만듬
            DicActor.Add(teamType, listActor); //actor의 teamType을 키값으로하는 새로만든 listActor를 저장
        }
        else
        {
            DicActor.TryGetValue(teamType, out listActor); //else로 들어왔다는건 DicActor에 actor의 팀타입에 해당하는 데이터가 있다는것이므로 DicActor에서 해당 리스트를 꺼내 listActor로 연결
        }

        listActor.Add(actor); //새로만든 or DicActor에서 꺼낸 리스트에 매개변수로 준 actor를 저장함 (actor는 list에 저장 list는 dictionary에 저장되는식으로 최종적으로 dictionary안에 actor가 저장)

    }

    public void RemoveActor(Actor actor, bool bDelete = false) //bDelete = true로 넘겨줄경우 게임오브젝트또한 삭제
    {
        eTeamType teamType = actor.TeamType;

        if(DicActor.ContainsKey(teamType) == true)
        {
            List<Actor> removeListActor = null;
            DicActor.TryGetValue(teamType, out removeListActor);
            removeListActor.Remove(actor);
        }
        else
        {
            Debug.LogError("존재하지않는 엑터를 삭제하려고 합니다.");
        }

        if (bDelete)
            Destroy(actor.SelfObject);
    }



    // 근처에 있는 thisActor의 적을 탐색(Player의AI가 돌리면 Enemy를 탐색할것이고 Enemy의 AI가 돌리면 Player를 탐색해 반환해줄것이다[TeamType으로 구분])
    public BaseObject FindNearEnemy(BaseObject thisActor,out float dist, float radius = 100.0f )
    {
        //thisActor에서 바로 참조하지않고 GetData를 쓰는이유는 thisActor를 BaseObject형으로 받고있고 BaseObject는 Actor의 부모기 때문에 부모에서 자식의 멤버를 참조하는게 불가능하므로 BaseObject의
        // Virtual함수로 Actor의 메소드를 실행하여 Actor의 멤버변수인 TeamType을 받아오고있음
        eTeamType teamType = (eTeamType)thisActor.GetData(ConstValue.ActorData_Team);   //thisActor의 팀타입을 받아옴(비교해서 thisActor의 팀타입과 다른애들을 적으로간주할것이기때문에)
        
        
        
        
        Vector3 myPosition = thisActor.SelfTransform.position; //thisActor의 위치를 받아와야함(적들의 거리를 비교해서 가장가까운놈을 타겟으로 삼을꺼기때문에)

        float nearDistance = radius; //thisActor와 thisActor에게 가장가까이있는적 둘 사이의 거리(초기값은 우리가 지정해놓은 값으로(so radius보다 적들이 다 멀리있으면 가까운적 못찾음)

        Actor nearEnemy = null; //반환해줄 가장가까운적을 담을 변수
        

        foreach(KeyValuePair<eTeamType,List<Actor>> pair in DicActor) //DicActor안을 전체 탐색
        {
            if (pair.Key == teamType) //DicActor안에 thisActor의 팀타입과 같은 팀타입의Actor들은 적으로 간주안하고 스킵
                continue;

            //여기에 도달했다는것은 thisActor와 다른 팀타입에 관한 데이터 즉 적으로 간주해야할 애들
            List<Actor> EnemyList = pair.Value;

            for(int i = 0; i < EnemyList.Count; i++) //DicActor에서 꺼낸 Enemy들의 리스트를 또 전체 탐색
            {
                if (EnemyList[i].SelfObject.activeSelf == false) //꺼낸 Enemy들중 비활성화상태인 애들은 스킵
                    continue;

                if (EnemyList[i].OBJECTSTATE == eBaseObjectState.STATE_DIE) //꺼낸 Enemy들중 죽어있는 상태인애들또한 스킵
                    continue;

                float DistanceMeEnemy = Vector3.Distance(myPosition, EnemyList[i].SelfTransform.position); //thisActor와 지금 탐색중인 적과의 거리를 구함

                if(DistanceMeEnemy < nearDistance) //위에서 구한 거리가 nearDistance 즉 최소탐색거리보다 작으면 이후에는 제일 가까운거리로 사용해야하므로 nearDistance를 현재 거리로 최신화
                {
                    nearDistance = DistanceMeEnemy;
                    nearEnemy = EnemyList[i];  // 여기에 들어왔다는건 현재까지 가장 가까운 위치에 있는 적이기에 반환해줄 제일가까운적 데이터변수인 nearEnemy에 넣어줌
                }
            }
        }

        //위에서 처리한 과정 요약
        // DicActor에서 thisActor와 다른 팀타입을 탐색 -> 그 팀타입을 담고있는 List<Actor> 안을 전부 탐색해 제일 thisActor와 가까운적을 찾아 nearEnemy에 담고 thisActor와의 거리값도 담아놓음
        dist = nearDistance; //매개변수로 받은 dist에다 가장가까운적과의 거리를 넣어줌
        return nearEnemy; //가장가까운적을 반환



    }
}
