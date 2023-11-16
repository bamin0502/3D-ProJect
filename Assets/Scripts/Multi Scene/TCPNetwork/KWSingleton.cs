using System;
using UnityEngine;
using System.Collections;

namespace MNF
{
    public abstract class KWSingleton<T> : MonoBehaviour where T : KWSingleton<T>
    {	
        private static T ms_Instance = null;

        public void OnDestroy()
        {
            ms_Instance = null;
        }

        public static T instance
        {
            get
            {
                if( null == ms_Instance )
                {
                    ms_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
				
                    if( null == ms_Instance )
                    {
                        Debug.Log($"No instance of  {typeof(T).ToString()} a temporary on is created.");
                        ms_Instance = new GameObject( typeof(T).ToString(), typeof(T) ).GetComponent<T> ();
					
                        if( null == ms_Instance )
                        {
                            Debug.LogError($"Failed to create a  {typeof(T).ToString()}");					
                        }				
                    }
                }
			
                return ms_Instance;
            }
        }
	
        virtual public void Awake()
        {
            if( null == ms_Instance )
            {
                ms_Instance = this as T;
            }
		
            DontDestroyOnLoad(this);
        }
	
        private void OnApplicationQuit()
        {
            ms_Instance = null;	
        }
    }
}