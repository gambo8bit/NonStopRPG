using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : Actor
{
    Rigidbody rigid;
    CapsuleCollider collider;
    //NavMeshAgent Agent;
    //Transform Target = null;  //추적할 대상
    
    private void Start()
    {
        NAV_MESH_AGENT = this.GetComponent<NavMeshAgent>();  //네브메쉬
        ANI = GetComponentInChildren<Animator>();  //애니메이터
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }
    


    private void OnTriggerEnter(Collider other) //Collision 안에 플레이어 진입할시 플레이어 추격하는 코루틴 돔
    {
        if (other.gameObject.name.Contains("Sword") && other.gameObject.transform.GetComponentInParent<Actor>().AI.CurrentAIState == eAIStateType.AI_STATE_ATTACK)
        {
            rigid.isKinematic = false;
            rigid.AddForce(-transform.forward * 10, ForceMode.Force);
            collider.enabled = false;
        }



    }


        //private void OnTriggerExit(Collider other)
        //{
        //    if(other.gameObject.name.Contains("Sword") && rigid.isKinematic == false)
        //    {
        //        rigid.isKinematic = true;
        //        collider.enabled = true;
        //    }
        //}





        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.gameObject.name.Contains("Player"))
        //    {
        //        Target = null;  //쫓아갈 타겟 null로 초기화
        //        Agent.isStopped = true; //NavMeshAgent isStopped 멈춤 설정
        //        StopCoroutine("ChaseTarget");
        //    }
        //}





        //IEnumerator ChaseTarget()
        //{
        //    //if(Target == null)
        //    //{
        //    //    Debug.LogError("Target 이 NULL입니다");
        //    //}
        //    while (Target != null)
        //    {
        //        Agent.isStopped = false;
        //        Agent.SetDestination(Target.position);
        //        yield return new WaitForSeconds(1f);
        //    }
        //    yield return null; //0.5초마다 코루틴 동작
        //}

    }
