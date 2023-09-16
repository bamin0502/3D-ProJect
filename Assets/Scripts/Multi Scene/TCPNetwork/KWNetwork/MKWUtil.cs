#pragma warning disable 219

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace MNF
{
	public class NetString
	{
		public static void ReadString(BinaryReader br, ref string str)
		{
			str = "";

			UInt16 nSize = (UInt16)br.ReadUInt16();
			if (nSize > 0)
				str = ExtendedTrim(Encoding.Unicode.GetString(br.ReadBytes(nSize * 2)));
		}

		public static void WriteString(BinaryWriter bw, string str)
		{
			UInt16 nSize = (UInt16)str.Length;
			bw.Write((UInt16)nSize);

			byte[] btData = new byte[nSize * 2];
			Encoding.Unicode.GetBytes(str, 0, str.Length, btData, 0);
			if (nSize > 0) bw.Write(btData);
		}

		public static string ExtendedTrim(string source)
		{
			string dest = source;

			int index = dest.IndexOf('\0');
			if (index > -1)
			{
				dest = source.Substring(0, index + 1);
			}
			return dest.TrimEnd('\0').Trim();
		}
	}

	public class KWAllocObj
	{
		public int m_ID;
		public bool m_isUsing;
		public GameObject m_go;

		virtual public KWAllocObj NewObj()
		{
			return new KWAllocObj();
		}

		virtual public void OnAlloc()
		{
		}
	}
}
