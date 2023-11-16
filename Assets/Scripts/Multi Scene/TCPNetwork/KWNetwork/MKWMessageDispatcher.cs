using UnityEngine;
using MNF;
using MNF.Message;
using System.IO;

public class MKWMessageDispatcher : DefaultDispatchHelper<MKWSession, MKWMessageDefine, MKWMessageDefine.SC>
{
	public static event System.Action <BinaryReader> ActionReceivePacket	= delegate {};

	public int onRecv(MKWSession session, object message)
	{
		StreamBin_RecvBuff  buff_obj =	message as  StreamBin_RecvBuff;
		var br =	buff_obj!.Read_Start();

		ActionReceivePacket(br);

		StreamBinRecver.ReturnRecvBuffObj( buff_obj );

		return 0;
	}
}
