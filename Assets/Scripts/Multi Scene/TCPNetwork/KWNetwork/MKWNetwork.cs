using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MNF;
using MNF.Message;

using System.IO;

public	class _SendDummy
{
	public _SendDummy()
	{
	}
}

public class MKWNetwork : KWSingleton<MKWNetwork>
{
	public bool IsInit { get; private set; }

	private MKWSession m_nowSession = null;
	public	NetHead_Equal	m_eqSJNetHead	= new NetHead_Equal();
	public	Dictionary<NetHead,NetRecvCallBack> 	m_dicRecvCallBack;

    override public void Awake()
	{
		base.Awake();

		IsInit = false;

		MNF.Message.StreamBinRecver.Alloc();

		if (LogManager.Instance.Init() == false)
			Debug.Log("LogWriter init failed");

		m_dicRecvCallBack = new Dictionary<NetHead, NetRecvCallBack>(m_eqSJNetHead);
	}

	void Start()
    {

	}

	void OnApplicationQuit()
	{
        if (IsInit == true)
		{
			m_nowSession.Disconnect();
			m_nowSession.IsConnected = false;

			IsInit = false;
 		}

		TcpHelper.Instance.Stop();
		LogManager.Instance.Release();
	}

	/**
     * @brief A function that is called every frame in Unity.
     * @details Basic_1_ClientSession can handle messages received by calling TcpHelper.Instance.dipatchNetworkInterMessage ().
     */
	void Update()
	{
        if (IsInit == false)
            return;

        // The message received by Basic_1_ClientSession is managed as a queue inside the MNF,
        // and it is necessary to call the dipatchNetworkInterMessage () function to process the message loaded in the queue.
        TcpHelper.Instance.DipatchNetworkInterMessage();
	}


	public bool ConnectServer(string szIP, string szPort)
	{
        if (IsInit == true)
		{
			m_nowSession.Disconnect();
			m_nowSession.IsConnected = false;
			IsInit = false;
 		}

		if (TcpHelper.Instance.IsRunning == false)
		{
			if (TcpHelper.Instance.Start(isRunThread:false) == false)
			{
				LogManager.Instance.WriteError("TcpHelper.Instance.run() failed");
				return false;
			}
		}

		m_nowSession = TcpHelper.Instance.AsyncConnect<MKWSession, MKWMessageDispatcher>(szIP, szPort);
		
		if (m_nowSession == null)
		{
			LogManager.Instance.WriteError("TcpHelper.Instance.AsyncConnect() failed");
			return false;
		}

		IsInit = true;

		return true;
	}

	//csj 임시..
	public	void	Disconnect()
	{
		m_nowSession.Disconnect();
		m_nowSession.IsConnected = false;
		IsInit = false;
	}

    public bool IsConnected()
    {
        if (IsInit)
            return m_nowSession.IsConnected;
        return false;
    }

	public	void	SendData(byte byClass, byte byEvent, GameObject goFunction = null, string szRecvFunc = "")
	{
        NetHead head = new NetHead();
		head.MakeHead(byClass, byEvent);

		if( goFunction != null )
		{
			NetRecvCallBack netRecvObj;
			if (MKWNetwork.instance.m_dicRecvCallBack.TryGetValue(head, out netRecvObj) == false)
			{
				NetRecvCallBack 	netCallBack = new NetRecvCallBack();
				netCallBack.m_ObjFunction	= goFunction;
				netCallBack.m_Func = szRecvFunc;

				m_dicRecvCallBack[head] = netCallBack;
			}
			else
			{
				netRecvObj.m_ObjFunction = goFunction;
				netRecvObj.m_Func = szRecvFunc;
			}
		}

        m_nowSession.AsyncSend();
	}

	public void SetRecvCallBack(byte uClass, byte uEvent, GameObject goFunction, string szRecvFunc)
	{
		NetHead head = new NetHead();
		head.MakeHead(uClass, uEvent);

		if( goFunction != null )
		{
			NetRecvCallBack netRecvObj;
			if (MKWNetwork.instance.m_dicRecvCallBack.TryGetValue(head, out netRecvObj) == false)
			{
				NetRecvCallBack 	netCallBack = new NetRecvCallBack();
				netCallBack.m_ObjFunction	= goFunction;
				netCallBack.m_Func = szRecvFunc;

				m_dicRecvCallBack[head] = netCallBack;
			}
			else
			{
				netRecvObj.m_ObjFunction = goFunction;
				netRecvObj.m_Func = szRecvFunc;
			}
		}
	}
}
