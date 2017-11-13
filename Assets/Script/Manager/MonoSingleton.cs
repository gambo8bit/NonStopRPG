using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    static bool bShutdown = false;
    private static T _instance = null;
    public static T Instance    // T의 인스턴스 Getter
    {
        get
        {
            if(_instance == null)
            {
                if(bShutdown == false)
                {
                    // T의 인스턴스가 없으면 T 인스턴스 생성
                    T instance = GameObject.FindObjectOfType<T>() as T; //이미 T 컴포넌트가 연결된 게임오브젝트가 있는지 체크 있으면 기존에있는 게임오브젝트 반환
                    if(instance == null) //여기를 넘어간다는것은 T를 가지고있는 게임오브젝트가 현재 월드상에 없다는 뜻
                    {
                        instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();   // T를 가지고있는 게임오브젝트 생성 & instance에 T컴포넌트 저장
                    }

                    InstanceInit(instance); //생성한 게임오브젝트의 컴포넌트 T를 _instance 변수와 연결해주고 Init 작업 ㄱ

                    Debug.Assert(_instance != null, typeof(T).ToString() + "싱글턴 생성 실패");    //여전히 T 인스턴스가 널이면 경고 띄운다
                }
            }

            return _instance;
        }
    }


   static void InstanceInit(T INSTANCE)
    {
        _instance = INSTANCE as T;
        _instance.Init();
    }

    public virtual void Init()
    {
        DontDestroyOnLoad(_instance);
    }

    public virtual void OnDestroy() //T가 제거될려할때 -> T instance 연결된걸 끊어줌 
    {
        _instance = null;
    }

    private void OnApplicationQuit()    //프로그램 종료하면 _instance 초기화
    {
        _instance = null;
        bShutdown = true;
    }

}
