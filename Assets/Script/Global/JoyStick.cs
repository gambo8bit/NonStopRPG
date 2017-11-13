using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : BaseObject
{

    
    public bool NormalizedPower = false;    // 방향만 나타내게 값으로 바꿀것인가
    public bool IsAnyKeyDown = false;   //키보드나 마우스 어떤값이라도 들어왔냐
    public bool IsMoveKeyInput = false;    //키보드 입력을 하였나
    public bool IsPressed = false;  // 조이스틱을 터치하였나
    public bool IsPressAttackKey = false;

    //조이스틱 출력을 위한 UI_Camera
    public Camera UI_Camera;

    static JoyStick _instance = null;
    public static JoyStick Instance
    {
        get
        {
            return _instance;   
        }
    }

    // 실제 조이스틱 , 캐릭터 원하는 방향으로 움직이게하는 파워 역할
    private Vector2 _Axis;
    public Vector2 Axis
    {
        get
        {
            if (IsPressed)  // 조이스틱이 눌러졌다고 판단될때만 값을 제대로 반환
                return _Axis;
            else
                return Vector2.zero;
        }
    }

    //스틱 바디와 헤드에 대한 데이터
        Vector3 StickCenterPosition = Vector3.zero;    //조이스틱 중심 위치
        Vector3 HeadPosition = Vector3.zero;    //조이스틱 헤드 위치
        Transform BodyTrans = null;
        Transform HeadTrans = null;
        UIWidget BodyWidget = null;
        UIWidget HeadWidget = null;
        float BodyRadius = 0;
        float HeadRadius = 0;






    private void Awake()
    {
        _instance = this;       //실제 게임에서 동작할 조이스틱의 인스턴스 저장(이후 연결의 편리함을 위해 지칭할수있는 데이터 만드는거)
    }



    private void OnEnable() //this의 게임 오브젝트가 활성화되면 호출하는 메서드
    {
        //UI 카메라 받아옴
        UI_Camera = UICamera.mainCamera;
        if(UI_Camera == null)
        {
            Debug.LogError("UICamera를 찾지 못했습니다.");
            return;
        }

        // UI의 World 좌표를 스크린상의 좌표로 변환하여 계산한다
        StickCenterPosition = UI_Camera.WorldToScreenPoint(this.transform.position);

        //UI 위젯 컴포넌트 받아와 조이스틱 몸통과 머리의 반지름 구하기
        BodyTrans = this.GetChild("Body");
        BodyWidget = BodyTrans.GetComponent<UIWidget>();
        BodyRadius = BodyWidget.width * 0.5f; //위젯의 가로크기의 반이 조이스틱 바디의 반지름

        // 위와 동일하게 조이스틱 헤드도 구함
        HeadTrans = this.GetChild("Head");
        HeadWidget = HeadTrans.GetComponent<UIWidget>();
        HeadRadius = HeadWidget.width * 0.5f;


    }


    //조이스틱 눌렀을때(터치했을때)
    void OnPress(bool Pressed)  
    {
        if (IsMoveKeyInput)    //키보드 입력받을시 작동 X
            return;

        if(Pressed)
        {
            IsPressed = true; //입력을 받았냐 안받았냐
            HeadPosition = UICamera.currentTouch.pos; //Head의 위치값을 UI에서 현재 터치받은 좌표로 바꿈
        }
        else
        {
            IsPressed = false;
            HeadPosition = StickCenterPosition;
        }

        Movement();


    }

    
    
    //조이스틱 드래그 했을때
    void OnDrag()
    {
        if(IsPressed)
        {
            HeadPosition = UICamera.currentTouch.pos; //현재 터치된 위치를 헤드 위치로
            Movement();
        }
    }



    void Movement()
    {
        Vector2 MovePosition = HeadPosition - StickCenterPosition;

        if (MovePosition.magnitude < BodyRadius * 0.2f)
        {
            MovePosition = Vector2.zero;
        }
        else if (MovePosition.magnitude >= (BodyRadius - HeadRadius))
        {
            MovePosition = MovePosition.normalized * (BodyRadius - HeadRadius);
        }

        HeadTrans.localPosition = MovePosition;

        if (NormalizedPower)
            MovePosition = MovePosition.normalized * BodyRadius;

        _Axis.x = MovePosition.x / BodyRadius;
        _Axis.y = MovePosition.y / BodyRadius;
    }


    private void Update()
    {

        if (Input.anyKey)
        {
            IsAnyKeyDown = true;
        }
        else
            IsAnyKeyDown = false;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            IsPressAttackKey = true;
            
            return;
        }
        else
        {
            IsPressAttackKey = false;
        }

        //키보드 인풋으로 움직이게하기
        Vector3 movePosition =
            new Vector3(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical"));

        if (movePosition != Vector3.zero)
        {
            IsPressAttackKey = false;
            // 키보드 입력
            IsMoveKeyInput = true;
            IsPressed = true;
            HeadPosition = StickCenterPosition+ movePosition * BodyRadius;
            Movement();
        }
        else
        {
            if (IsMoveKeyInput == true)
            {
                IsPressed = false;
                IsMoveKeyInput = false;
            }
        }

    }












    




}
