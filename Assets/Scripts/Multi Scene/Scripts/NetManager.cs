using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using LitJson;
using MNF.Message;

namespace MNF
{
	public class NetManager : KWSingleton<NetManager>
	{
		private float m_fSendCheck		= 0.0f;
		private DateTime m_dtConnectionTest	= new DateTime();

		override public void Awake( )
		{
			base.Awake();

            MKWMessageDispatcher.ActionReceivePacket += OnNetReceivePacket;

            MKWSession.ActionOnConnectSuccess += OnNetConnectSuccess;
            MKWSession.ActionOnConnectFail += OnNetConnectFail;
            MKWSession.ActionOnDisconnect += OnNetConnectDisconnect;
            
            
		}

		public void Init(string ip, int port, bool isIntranet = false)
		{
            ConnectServer(ip, port, isIntranet);
        }
        

        public IPAddress GetIP(string serverIP)
        {
            IPAddress thisIp = null;
            IPHostEntry iphostentry = Dns.GetHostEntry(serverIP);// Find host name
            foreach (var ipAddress in iphostentry.AddressList)// Grab the first IP addresses
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    thisIp = ipAddress;
            }

            return thisIp;
        }

		private void Update ( )
		{
			m_fSendCheck += Time.deltaTime;	
		}

		private bool IsSendEnable()
		{
			if ( m_fSendCheck < 0.5f)
				return false;

			m_fSendCheck = 0.0f;

			return true;
		}

		public void InitConnectTest()
		{
			m_dtConnectionTest = DateTime.Now;
		}

		public bool IsConnected()
		{
			bool bConnect = MKWNetwork.instance.IsConnected();
			if (bConnect == false)
				return false;

			DateTime dtNow = DateTime.Now;
			TimeSpan ts = dtNow - m_dtConnectionTest;
			if (ts.Minutes > 2)
				return false;

			return true;
		}

		public void OnNetReceivePacket( BinaryReader br)
		{
            NetHead head = new NetHead();
            head.ReadBin(br);

            int size = br.ReadInt32();

            if (head.m_Class == 255 && head.m_Event == 254)
            {
                BinaryWriter bw = StreamBinData.WriteStart((byte)255, (byte)253);
                MKWNetwork.instance.SendData((byte)255, (byte)253);

                m_dtConnectionTest = DateTime.Now;
                return;
            }

            if (MKWNetwork.instance.m_dicRecvCallBack.TryGetValue(head, out var netRecvObj))
            {
                if (netRecvObj.m_ObjFunction != null)
                {
                    netRecvObj.m_ObjFunction.SendMessage(netRecvObj.m_Func, br, SendMessageOptions.DontRequireReceiver);
                    return;
                }
            }

            OnPrcNetRecvPacket(head, br);
        }

		public void ConnectServer( string szIP, int nPort, bool isIntranet = false )
		{
			if (isIntranet)
			{
                MKWNetwork.instance.ConnectServer(szIP, nPort.ToString());
			}
			else
			{
				IPHostEntry host = Dns.GetHostEntry(szIP);
				foreach (IPAddress ip in host.AddressList)
				{
                    MKWNetwork.instance.ConnectServer(ip.ToString(), nPort.ToString());
					break;
				}
			}
        }

		public void OnNetConnectSuccess(int nRet)
		{
			Debug.Log("OnNetConnectSuccess : " + nRet.ToString());
            MKWSession session = MKWNetwork.instance.m_nowSession;
        }

		public void OnNetConnectFail(int nRet)
		{
			Debug.Log("OnNetConnectFail : " + nRet.ToString());
		}

		public void OnNetConnectDisconnect(int nRet)
		{
			Debug.LogError("OnNetConnectDisconnect : " + nRet.ToString());
            LobbyScene.Instance.ReconnectImage.transform.gameObject.SetActive(true);
            TcpHelper.Instance.IsRunning = true;
        }
        
		public void OnPrcNetRecvPacket(NetHead head, BinaryReader br)
		{
            // 전체유저에게 전송되는 패킷 처리
			if (head.m_Class == HeadClass.SOCK_MENU)
			{
			}
            else if (head.m_Class == HeadClass.SOCK_ROOM)
            {
				if (head.m_Event == HeadEvent.ROOM_BROADCAST)
					NetGameManager.instance.Recv_ROOM_BROADCAST(br);
				else if (head.m_Event == HeadEvent.ROOM_USER_DATA_UPDATE)
					NetGameManager.instance.Recv_ROOM_USER_DATA_UPDATE(br);
                else if (head.m_Event == HeadEvent.ROOM_USER_MOVE_DIRECT)
                    NetGameManager.instance.Recv_ROOM_USER_MOVE_DIRECT(br);
                else if (head.m_Event == HeadEvent.ROOM_USER_ITEM_UPDATE)
                    NetGameManager.instance.Recv_ROOM_USER_ITEM_UPDATE(br);
                else if (head.m_Event == HeadEvent.ROOM_ENTER)
					NetGameManager.instance.Recv_ROOM_ENTER(br);
				else if (head.m_Event == HeadEvent.ROOM_MAN_IN)
					NetGameManager.instance.Recv_ROOM_MAN_IN(br);
				else if (head.m_Event == HeadEvent.ROOM_MAN_OUT)
					NetGameManager.instance.Recv_ROOM_MAN_OUT(br);
				else if (head.m_Event == HeadEvent.ROOM_DATA_UPDATE)
					NetGameManager.instance.Recv_ROOM_DATA_UPDATE(br);
                else if (head.m_Event == HeadEvent.ROOM_UPDATE)
                    NetGameManager.instance.Recv_ROOM_UPDATE(br);
            }
		}
		public void Send_WAIT_LOGIN(string szUserID, byte uGroup, GameObject objCallback)
		{
			BinaryWriter bw = StreamBinData.WriteStart(HeadClass.SOCK_WAIT, HeadEvent.WAIT_LOGIN);
			bw.Write(NetSetting.NET_PROTOCOL_VER);
			NetString.WriteString(bw, szUserID);
			bw.Write((byte)uGroup);

            MKWNetwork.instance.SendData(HeadClass.SOCK_WAIT, HeadEvent.WAIT_LOGIN, objCallback, "OnRecvWaitLogin");
		}

		public void Send_ROOM_BROADCAST( string szData)
		{
			BinaryWriter bw = StreamBinData.WriteStart(HeadClass.SOCK_ROOM, HeadEvent.ROOM_BROADCAST);
			NetString.WriteString(bw, szData);
            MKWNetwork.instance.SendData(HeadClass.SOCK_ROOM, HeadEvent.ROOM_BROADCAST);
		}
		public void Send_ROOM_USER_DATA_UPDATE( UserSession user)
		{
			BinaryWriter bw = StreamBinData.WriteStart(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_DATA_UPDATE);
			user.WriteBin(bw);
            MKWNetwork.instance.SendData(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_DATA_UPDATE);
		}
        public void Send_ROOM_USER_MOVE_DIRECT(UserSession user)
        {
            BinaryWriter bw = StreamBinData.WriteStart(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_MOVE_DIRECT);
            user.WriteBin(bw);
            MKWNetwork.instance.SendData(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_MOVE_DIRECT);
        }
        public void Send_ROOM_USER_ITEM_UPDATE(UserSession user)
        {
            BinaryWriter bw = StreamBinData.WriteStart(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_ITEM_UPDATE);
            user.WriteBin(bw);
            MKWNetwork.instance.SendData(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_ITEM_UPDATE);
        }
        public void Send_ROOM_DATA_UPDATE(RoomSession room)
        {
            BinaryWriter bw = StreamBinData.WriteStart(HeadClass.SOCK_ROOM, HeadEvent.ROOM_DATA_UPDATE);
			for (int i = 0; i < NetConst.SIZE_ROOM_DATA; i++)
				bw.Write((int)room.m_nRoomData[i]);            
            MKWNetwork.instance.SendData(HeadClass.SOCK_ROOM, HeadEvent.ROOM_DATA_UPDATE);
        }
        public void Send_ROOM_USER_MOVE( UserSession user)
		{
			BinaryWriter bw = StreamBinData.WriteStart(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_MOVE);
			user.WriteBin(bw);
            MKWNetwork.instance.SendData(HeadClass.SOCK_ROOM, HeadEvent.ROOM_USER_MOVE);
		}
    }
}
