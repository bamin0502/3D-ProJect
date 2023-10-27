using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

using UnityEngine;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

public class StreamBin
{
	public static string ExtendedTrim(string source)
	{
		string dest = source;
		int index = dest.IndexOf('\0');
		if( index > -1 )
		{
			dest = source.Substring(0,index+1);
		}
		return dest.TrimEnd('\0').Trim();
	}

	public static void ReadString_uni( BinaryReader br, ref string str )
	{
		UInt16	nSize = 0;
		nSize = br.ReadUInt16();
		str = nSize > 0 ? ExtendedTrim( Encoding.Unicode.GetString(br.ReadBytes(nSize*2)) ) : "";
	}
	
	public static string  ReadString_uni( BinaryReader br )
	{
		string str="";
		ReadString_uni(br ,ref str);
		return str;
	}

	public static void WriteString_uni( BinaryWriter bw, string str )
	{
		UInt16	nSize = (UInt16)str.Length;
		bw.Write( (short)nSize );
		byte[] btData = new byte[ nSize * 2 ];
		Encoding.Unicode.GetBytes( str , 0 , str.Length , btData , 0);
		if( nSize > 0 )bw.Write( btData );
	}

}


namespace MNF.Message
{
	[System.Serializable]
	public class BinData
	{
        public byte[] writeBuffer = new byte[5192];
        public MemoryStream ms;
        public BinaryWriter bw;
        public NetHead head = new NetHead();
        public int GetSize_Data() { return (int)ms.Position; }
		public byte[] GetBuff() { return writeBuffer; }
    }
    [System.Serializable]
	public	class	StreamBinData
	{
		static public List<BinData> listBinData = new List<BinData> { };

		static public BinData GetBinData()
		{
            return listBinData[0];
		}

        static public void WriteEnd()
        {
			listBinData.RemoveAt(0);
        }


        static public BinaryWriter WriteStart(byte byClass, byte byEvent)
		{
            BinData data = new BinData();
			data.head.MakeHead(byClass, byEvent);

            listBinData.Add(data);
			if(data.ms == null )
			{
                data.ms = new MemoryStream(data.writeBuffer, true);
                data.bw = new BinaryWriter(data.ms);
			}
            data.ms.Seek(0,SeekOrigin.Begin);

            return data.bw;
		}
	}


	public class StreamBin_RecvBuff
	{
		public byte[] recvBuffer = new byte[65536];

		public MemoryStream ms;
		public BinaryReader br;

		public void Init()
		{
			ms = new MemoryStream(recvBuffer,false); 
			br = new BinaryReader(ms);
		}

		public BinaryReader Read_Start()
		{
			ms.Seek(0,SeekOrigin.Begin);
			return br;
		}

		public void Copy( byte[] src , int startidx , int size )
		{
			Buffer.BlockCopy( src , 0 , recvBuffer , startidx , size );
		}
	}

    public static class StreamBinMessageBuffer
    {
        public static int MaxMessageSize()
        {
            return 1024 * 64;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class StreamBinMessageHeader
    {
		[MarshalAs(UnmanagedType.U2)]
		public	ushort 	m_Head1;

		[MarshalAs(UnmanagedType.U2)]
		public	ushort 	m_Head2;

		[MarshalAs(UnmanagedType.U2)]
		public	ushort 	m_Head3;

		[MarshalAs(UnmanagedType.U2)]
		public	ushort 	m_Head4;

		[MarshalAs(UnmanagedType.U2)]
		public	ushort 	m_Check;

		[MarshalAs(UnmanagedType.U1)]
		public	byte 	byClass;

		[MarshalAs(UnmanagedType.U1)]
		public	byte 	byEvent; 

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 messageSize;
    }

    public class StreamBinMessageSerializer : Serializer<StreamBinMessageHeader>
    {
        public StreamBinMessageSerializer() : base(StreamBinMessageBuffer.MaxMessageSize())
        {
        }

		protected override void _Serialize<T>(int messageID, T managedData)
        {
			MemoryStream	ms = new MemoryStream(GetSerializedBuffer(),true); 
			BinaryWriter	bw = new BinaryWriter(ms);

			BinData binData = StreamBinData.GetBinData();

            int total_size = binData.GetSize_Data() + SerializedHeaderSize;
            binData.head.WriteBin(bw);
			bw.Write( total_size );
			bw.Write(binData.GetBuff() , 0 , total_size - SerializedHeaderSize );

//			string szLog = string.Format("SERIALIZE {0} {1} {2} {3}", total_size, SerializedHeaderSize, binData.head.m_Class, binData.head.m_Event);
//			Debug.Log(szLog);

            bw.Close();
			ms.Close();

			SerializedLength = total_size;

			StreamBinData.WriteEnd();
        }
    }

    public class StreamBinMessageDeserializer : Deserializer<StreamBinMessageHeader>
    {
        readonly IntPtr marshalAllocatedBuffer;
        readonly int marshalAllocatedBufferSize;

        public StreamBinMessageDeserializer() : base(StreamBinMessageBuffer.MaxMessageSize())
        {
            marshalAllocatedBufferSize = BinaryMessageBuffer.MaxMessageSize();
            marshalAllocatedBuffer = MarshalHelper.AllocGlobalHeap(marshalAllocatedBufferSize);
        }

        ~StreamBinMessageDeserializer()
        {
            MarshalHelper.DeAllocGlobalHeap(marshalAllocatedBuffer);
        }

        protected override void _Deserialize(SessionBase session, ref ParsingResult parsingResult)
        {
            var tcpSession = session as TCPSession;

            if (tcpSession!.RecvCircularBuffer.ReadableSize < SerializedHeaderSize)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_INCOMPLETE;
                return;
            }

            // read header

			byte[] bytes_header = new byte[ SerializedHeaderSize ];

            if (tcpSession.RecvCircularBuffer.read(bytes_header, SerializedHeaderSize) == false)
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_ERROR;
                return;
            }

			MemoryStream	ms = new MemoryStream(bytes_header,false); 
			BinaryReader	br = new BinaryReader(ms);

			NetHead netHead = new NetHead();
			netHead.ReadBin(br);

			int total_size = br.ReadInt32();

			br.Close();
			ms.Close();

		// 전체 사이즈 체크 추가..
           if (tcpSession.RecvCircularBuffer.ReadableSize < total_size )
            {
                parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_INCOMPLETE;
                return;
            }


			tcpSession.RecvCircularBuffer.read(SerializedBuffer, SerializedHeaderSize , total_size - SerializedHeaderSize );

			ms = new MemoryStream(SerializedBuffer,false); 
			br = new BinaryReader(ms);


			StreamBin_RecvBuff buff_obj =	StreamBinRecver.GetRecvBuffObj();

			buff_obj.Copy( bytes_header ,0 , SerializedHeaderSize );
			buff_obj.Copy( SerializedBuffer , SerializedHeaderSize , total_size - SerializedHeaderSize );

			var dispatchInfo = tcpSession.DispatchHelper.TryGetMessageDispatch(0);

            parsingResult.parsingResultEnum = ParsingResult.ParsingResultEnum.PARSING_COMPLETE;
            parsingResult.dispatcher = dispatchInfo.dispatcher;
            parsingResult.message = buff_obj;
            parsingResult.messageSize = total_size;

			tcpSession.RecvCircularBuffer.pop(total_size);

			br.Close();
			ms.Close();
        }
    }

	public	class StreamBinRecver
	{
		static public StreamBinRecver g;

		public delegate void OnRecv_KW( NetHead head , BinaryReader br );
		static public OnRecv_KW	onrecv_kw = null;

		public readonly int max_buffObj = 200;
		public readonly Queue<StreamBin_RecvBuff>qRecvBuff = new Queue<StreamBin_RecvBuff>();

		static public void Alloc( )
		{
			g = new StreamBinRecver();
			g.init();
		}

		public void init()
		{
			for( int i = 0 ; i < max_buffObj ; i++ )
			{
				StreamBin_RecvBuff obj =	new StreamBin_RecvBuff();
				obj.Init();
				qRecvBuff.Enqueue( obj );
			}
		}


		static public StreamBin_RecvBuff GetRecvBuffObj()
		{
			if( g.qRecvBuff.Count < 1 )
			{
				Debug.LogError( "!!! GetRecvBuffObj : g.qRecvBuff.Count < 1 " );
				return null;
			}
			return g.qRecvBuff.Dequeue();
		}

		static public void ReturnRecvBuffObj( StreamBin_RecvBuff buff_obj )
		{
			g.qRecvBuff.Enqueue( buff_obj );
		}
	}

}