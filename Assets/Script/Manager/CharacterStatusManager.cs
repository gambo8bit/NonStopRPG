using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class CharacterStatusManager : MonoSingleton<CharacterStatusManager>
{

    Dictionary<string, CharacterTemplateData> DicTemplate = new Dictionary<string, CharacterTemplateData>();

    private void Awake()
    {
        TextAsset characterText = Resources.Load(ConstValue.CharacterTemplatePath) as TextAsset;    //캐릭터스테이터스 제이슨파일 텍스트로 가져옴

        if(characterText != null)   //제이슨파일 로드 제대로 했다면
        {
            JSONObject rootNodeText = JSON.Parse(characterText.text) as JSONObject; //파싱 <조사해볼것>


            if(rootNodeText != null)    //파싱 제대로 됐다면
            {
                JSONObject 
            }


        }
    }
}
