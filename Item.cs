using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;


    [UnityEngine.Scripting.Preserve]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true,null,"Assembly-CSharp")]
    [System.Serializable]
public class Item
    {
        public string itemName;
        public string itemType;
        public int health;
        public int damage;
        public int dot;
        public int sight;
        public int rapidTime;
        public GameObject prefab;

        public Item(string itemName, string itemType, int health, int damage, int dot, int sight, int rapidTime, GameObject prefab)
        {
            this.itemName = itemName;
            this.itemType = itemType;
            this.health = health;
            this.damage = damage;
            this.dot = dot;
            this.sight = sight;
            this.rapidTime = rapidTime;
            this.prefab = prefab;
        }
    }





