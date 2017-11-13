using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour     //편의기능제공(컴포넌트 딕셔너리에 자동저장, 타겟을 설정하여 타겟의 데이터를 가져오는일 등)
{
    Dictionary<string, UnityEngine.Component> DicComponent = new Dictionary<string, Component>();   //SelfComponent로 만들어진 컴포넌트 저장


    //타겟오브젝트
    BaseObject TargetObject = null;

    public BaseObject TargetComponent
    {
        get { return TargetObject; }
        set { TargetObject = value; }
    }


    // 타겟의 베이스오브젝트의 상태를 리턴받아야할때 사용
    eBaseObjectState _ObjectState = eBaseObjectState.STATE_NORMAL;  //해당 BaseObject의 상태를 나타내는 enum값 기본값은 NORMAL로 지정
    public eBaseObjectState OBJECTSTATE
    {
        get
        {
            if (TargetComponent == null)
                return _ObjectState;    //타겟오브젝트 지정되어있지않으면 나의 상태값 반환
            else
                return TargetComponent._ObjectState;
        }

        set
        {
            if (TargetComponent == null)
                _ObjectState = value;
            else
                TargetComponent._ObjectState = value;
        }
    }








    //셀프 오브젝트(자신 or 타겟오브젝트의 게임오브젝트를 가져올때 사용할꺼)
    public GameObject SelfObject
    {
        get
        {
            if (TargetComponent == null)
                return this.gameObject; //타겟오브젝트가 설정되있지않으면 나의 게임오브젝트 반환
            else
                return TargetComponent.gameObject;  //타겟오브젝트 설정되있으면 타겟오브젝트의 게임오브젝트 반환
        }
    }


    //셀프 트랜스폼(자신 or 타겟오브젝트의 Transform 가져올때 사용할꺼)
    public Transform SelfTransform
    {
        get
        {
            if (TargetComponent == null)
                return this.transform;
            else
                return TargetComponent.transform;
        }
    }


    //(타겟오브젝트 or 자신) 의 자식오브젝트의 트랜스폼을 가져올때 사용 [실제 로직은 _GetChild에서 돔]
    public Transform GetChild(string strName)
    {


        return _GetChild(strName, SelfTransform);
    }

    //실제 자식을 가지고오는 메서드(Transform.GetChild() <- 유니티제공함수을 재귀함수로 돌려서 자식을 이름 문자열로 쉽게 얻어오기위해 사용
    private Transform _GetChild(string strName, Transform trans)
    {
        if (trans.name == strName)
            return trans;          //찾고자입력한 자식의 게임오브젝트이름과 현재 탐색중인 자식들의 이름이 같으면 현재 탐색중이던 Transform반환

        for(int i = 0; i < trans.childCount; i++)
        {
            Transform returnTrans = _GetChild(strName, trans.GetChild(i));
            if (returnTrans != null) //_GetChild 메소드를 돌려 반환된값을 담은 returnTrans가 null 이 아니면 찾았다는 의미이므로 그값을 반환
                return returnTrans;
        }

        return null;
    }



    // 타겟 or 자신의 T 컴포넌트 딕셔너리 저장 or 이미 딕셔너리에 저장되어있으면 꺼내서 반환 
    public T SelfComponent<T>() where T : Component
    {
        string objectName = "";
        string typeName = typeof(T).ToString();

        T tempComponent = default(T);

        if(TargetComponent == null) //타겟오브젝트 지정되어있지않으면
        {
            objectName = SelfObject.name;

            //typeName 키를 포함하고 있는지.
            if(DicComponent.ContainsKey(typeName))
            {
                tempComponent = DicComponent[typeName] as T;
            }
            else //딕셔너리에 반환받으려는 컴포넌트가 저장되어있지않으면
            {
                tempComponent = this.GetComponent<T>();
                if (tempComponent != null)
                    DicComponent.Add(typeName, tempComponent);  //딕셔너리에 저장
            }
        }
        else  //타겟오브젝트 지정되어있으면
        {
            tempComponent = TargetComponent.SelfComponent<T>();
        }

        if(tempComponent == null) //위의 과정을 거쳤음에도 tempComponent에 아무것도 값이 안들어왔다면
        {
            Debug.LogError("GameObject Name : " + objectName +'\n'+ "null Component" + typeName);
        }

        return tempComponent;


    }


    virtual public void ThrowEvent(string keyData,params object[] datas)    //자식부에서 구현
    {

    }


    virtual public object GetData(string keyData,params object[] datas) //원하는 데이터 반환 받아야할때 사용
    {
        return null;
    }





}



    
	
