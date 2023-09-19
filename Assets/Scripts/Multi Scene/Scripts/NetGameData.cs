using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GAME_START
{
    //게임 시작
    public string   USER = "";
    public int      DATA = 0;
}

[Serializable]
public class LOBBY_STATE
{
    public string   USER = "";
    public int      DATA = 0;
    public bool ISADMIN = false;
    public bool ISREADY = false;
}


[Serializable]
public class GAME_CHAT
{
    public string USER = "";
    public int DATA = 0;
    public string CHAT = "";
}

[Serializable]
public class PLAYER_ANIMATION
{
    public string USER = "";
    public int DATA = 0;
    public int ANI_NUM = 0;
}

