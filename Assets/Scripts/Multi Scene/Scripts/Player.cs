using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MNF;

public class Player : MonoBehaviour
{
    public TextMesh userID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name != NetGameManager.instance.m_userHandle.m_szUserID)
            return;

        if (Input.GetKey(KeyCode.LeftArrow))
	    {
            transform.Translate(Vector3.left * 10.0f * Time.deltaTime);
		}
        else if (Input.GetKey(KeyCode.RightArrow))
		{
            transform.Translate(Vector3.right * 10.0f * Time.deltaTime);
		}
        else if (Input.GetKey(KeyCode.UpArrow))
		{
            transform.Translate(Vector3.forward * 10.0f * Time.deltaTime);
		}
        else if (Input.GetKey(KeyCode.DownArrow))
		{
            transform.Translate(Vector3.back * 10.0f * Time.deltaTime);
		}

        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        userSession.m_userTransform[0] = new NetVector3(transform.position);
    }

	public void Init(UserSession user)
	{
        gameObject.name = user.m_szUserID;
        userID.text = user.m_szUserID;
	}
}
