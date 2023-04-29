//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GameSceneManager : MonoBehaviour
//{
//    public Action<int> OnSpawnEvent;

//    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
//    {
//        GameObject go = Managers.Resource.Instantiate(path, parent);

//        switch (type)
//        {
//            case Define.WorldObject.Monster:
//                _monsters.Add(go);
//                if (OnSpawnEvent != null)
//                    OnSpawnEvent.Invoke(1);
//                break;
//            case Define.WorldObject.Player:
//                _player = go;
//                break;
//        }

//        return go;
//    }

//    public void Despawn(GameObject go)
//    {
//        Define.WorldObject type = GetWorldObjectType(go);

//        switch (type)
//        {
//            case Define.WorldObject.Monster:
//                {
//                    if (_monsters.Contains(go))
//                    {
//                        _monsters.Remove(go);
//                        if (OnSpawnEvent != null)
//                            OnSpawnEvent.Invoke(-1);
//                    }
//                }
//                break;
//            case Define.WorldObject.Player:
//                {
//                    if (_player == go)
//                        _player = null;
//                }
//                break;
//        }

//        Managers.Resource.Destroy(go);
//    }
//}
