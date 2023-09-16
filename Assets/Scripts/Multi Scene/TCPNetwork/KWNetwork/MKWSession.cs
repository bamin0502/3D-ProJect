using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MNF;

public class MKWSession : StreamBinSession
{
	public static event System.Action<int> ActionOnConnectSuccess	= delegate {};
	public static event System.Action<int> ActionOnConnectFail		= delegate {};
	public static event System.Action<int> ActionOnDisconnect		= delegate {};

	public override int OnConnectSuccess()
	{
		LogManager.Instance.Write("onConnectSuccess : {0}:{1}", ToString(), GetType());

		// Add connected client.
		int nHashCode = GetHashCode();
		TcpHelper.Instance.AddClientSession(nHashCode, this);

		ActionOnConnectSuccess(nHashCode);

		return 0;
	}
	/**
     * @brief Called when Basic_1_ClientSession fails to connect to the server.
     * @return Returns 0 if it is normal, otherwise it returns an arbitrary value.
     */
	public override int OnConnectFail()
	{
		LogManager.Instance.Write("onConnectFail : {0}:{1}", ToString(), GetType());

		ActionOnConnectFail(0);
		return 0;
	}

	/**
     * @brief Called when Basic_1_ClientSession is disconnected from the server.
     * @return Returns 0 if it is normal, otherwise it returns an arbitrary value.
     */
	public override int OnDisconnect()
	{
		LogManager.Instance.Write("onDisconnect : {0}:{1}", ToString(), GetType());

		// Remove disconnected client.
		TcpHelper.Instance.RemoveClientSession(GetHashCode());

		ActionOnDisconnect(0);

		return 0;
	}
}
