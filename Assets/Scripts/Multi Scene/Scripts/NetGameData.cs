using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using mino;

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
    public bool ANI_TYPE = false;
}


[Serializable]
public class PLAYER_MOVE
{
    public string USER = "";
    public int DATA = 0;
    public string POSITION = "";
    public int TARGET = -99;
    public bool ISBOSS = false;
}

[Serializable]
public class PLAYER_WEAPON
{
    public string USER = "";
    public int DATA = 0;
    public int WEAPON_INDEX = 0;
}

[Serializable]
public class PLAYER_ITEM
{
    public string USER = "";
    public int DATA = 0;
    public int ITEM_INDEX = -99;
    public int ITEM_COUNT = -99;
}


[Serializable]
public class PLAYER_STATUS
{
    public string USER = "";
    public int DATA = 0;
    public int Health = -99;
    public int PlayerHealth = -99;
    public int HEALTH { get; set; }
}
[Serializable]
public class WEAPON_DAMAGE
{ 
    WeaponType weaponType;
    public string USER = "";
    public int DATA = 0;
    public int WEAPON_INDEX = 0; 
    public int DAMAGE { get; set; }
}
// box=0,green=1,red=2
[Serializable]
public class MONSTER_SPAWN
{
    public int DATA = 0;
    public int MONSTER_CODE = 0;
    public string POSITION = "";
}
[Serializable]
public class ENEMY_ANIMATION
{
    public string USER = "";
    public int DATA = 0;
    public int MONSTER_INDEX = 0;
    public int ANI_NUM = 0;
    public bool ANI_TYPE = false;
}
[Serializable]
public class TARGET_SET //copy
{
    public String USER = " ";
    public int DATA = 0;
    public string TARGET_POSITION = "";

}

[Serializable]
public class BOSS_ANIMATION //copy
{
    public string USER = " ";
    public int DATA = 0;
    public int ANI_NUM = 0;
    public bool ANI_TYPE = false;
}
[Serializable]
public class ENEMY_ITEM
{
    public string USER = "";
    public int DATA = 0;
    public int ITEM_INDEX = 0;
    public string POSITION = "";
}

[Serializable]
public class PLAYER_SKILL
{
    public string USER = "";
    public int DATA = 0;
}

[Serializable]
public class THROW_ATTACK //활 공격, 수류탄 공격에 사용
{
    public string USER = "";
    public int DATA = 0;
    public int SKILL_TYPE = 0;
    public string PLAYER_POSITION = ""; 
    public string MOUSE_POSITION = "";
}

[Serializable]
public class PLAYER_ITEMUSE
{
    public string USER = "";
    public int DATA = 0;
    public int ITEM_TYPE = 0;
}
[Serializable]
public class ENEMY_ATTACK //몬스터 공격에 사용
{
    public string USER = "";
    public int DATA = 0;
    public int DAMAGE = 0;
}

[Serializable]
public class SECOND_CUTSCENE
{
    public string USER = "";
    public int DATA = 0;
    public int CUTSCENE_NUM = 0;
    public bool CUTSCENE_TYPE = false;
}
[Serializable]
public class LAST_CUTSCENE
{
    public string USER = "";
    public int DATA = 0;
    public int CUTSCENE_NUM = 0;
    public bool CUTSCENE_TYPE = false;
}

[Serializable]
public class PLAYER_TAKE_DAMAGE
{
    public string USER = "";
    public int DATA = 0;
    public int DAMAGE = 0;
    public string TARGET = "";
}

[Serializable]
public class ENEMY_CHASE
{
    public string USER = "";
    public int DATA = 0;
    public int ENEMY_INDEX = 0;
    public string TARGET = "";
}

[Serializable]
public class PLAYER_DEAD
{
    public string USER = "";
    public int DATA = 0;
    public string TARGET = "";
}

[Serializable]
public class ENEMY_SKILL
{
    public string USER = "";
    public int DATA = 0;
    public int SKILL_NUM = 0;
}


[Serializable]
public class ENEMY_TAKE_DAMAGE
{
    public string USER = "";
    public int DATA = 0;
    public int INDEX = 0;
    public int DAMAGE = 0;
    public bool ISBOSS = false;
}