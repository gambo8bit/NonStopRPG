public enum eBaseObjectState    //BaseObject의 상태 살아있다 죽었다만 판단
{
    STATE_NORMAL,
    STATE_DIE
}

public enum eAIStateType
{
    AI_STATE_IDLE = 1,
    AI_STATE_RUN,
    AI_STATE_ATTACK,
    AI_STATE_DIE,
    

}

public enum eAIType
{
    NONE_AI,
    BasicAI,
}

public enum eTeamType
{
    A_Team,
    B_Team,
}
