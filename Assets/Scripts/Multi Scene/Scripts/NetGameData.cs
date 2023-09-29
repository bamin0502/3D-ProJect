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
    public List<ItemProperty> ItemProperties;
}

[System.Serializable]
public class ItemProperty
{
    public Sprite ITEM_IMAGE;
    public GameObject ITEM_PREFAB;
    public Item.ItemType itemType;
    public string ITEM_NAME;
}
[Serializable]
public class PLAYER_STATUS
{
    public string USER = "";
    public int Health = 0;
    public int Damage = 0;

}
