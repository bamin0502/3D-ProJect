using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace MNF
{
    [System.Serializable]
    public class NetDateTime
    {
        public ushort m_YY = 0;
        public byte m_MM = 0;
        public byte m_DD = 0;
        public byte m_HH = 0;
        public byte m_MI = 0;
        public byte m_SS = 0;

        public NetDateTime()
        {
            m_YY = 2000;
            m_MM = 1;
            m_DD = 1;
            m_HH = 0;
            m_MI = 0;
            m_SS = 0;
        }

        public NetDateTime(DateTime clsTime)
        {
            m_YY = (ushort)clsTime.Year;
            m_MM = (byte)clsTime.Month;
            m_DD = (byte)clsTime.Day;
            m_HH = (byte)clsTime.Hour;
            m_MI = (byte)clsTime.Minute;
            m_SS = (byte)clsTime.Second;
        }

        public NetDateTime(ushort year, byte month, byte day, byte hour, byte minute, byte second)
        {
            m_YY = year;
            m_MM = month;
            m_DD = day;
            m_HH = hour;
            m_MI = minute;
            m_SS = second;
        }

        public void ReadBin(BinaryReader br)
        {
            m_YY = br.ReadUInt16();
            m_MM = br.ReadByte();
            m_DD = br.ReadByte();
            m_HH = br.ReadByte();
            m_MI = br.ReadByte();
            m_SS = br.ReadByte();
        }

        public void WriteBin(BinaryWriter bw)
        {
            bw.Write((UInt16)m_YY);
            bw.Write((byte)m_MM);
            bw.Write((byte)m_DD);
            bw.Write((byte)m_HH);
            bw.Write((byte)m_MI);
            bw.Write((byte)m_SS);
        }

        public void SetDateTime(DateTime date)
        {
            m_YY = (ushort)date.Year;
            m_MM = (byte)date.Month;
            m_DD = (byte)date.Day;
            m_HH = (byte)date.Hour;
            m_MI = (byte)date.Minute;
            m_SS = (byte)date.Second;
        }

        public DateTime GetDateTime()
        {
            if ((this.m_YY == 0) || (this.m_MM == 0) || (this.m_DD == 0))
            {
                DateTime date = new DateTime(2000, 1, 1);
                return date;
            }
            else
            {
                DateTime date = new DateTime(this.m_YY, this.m_MM, this.m_DD, this.m_HH, this.m_MI, this.m_SS);
                return date;
            }
        }

        public NetDateTime Copy()
        {
            NetDateTime copy = new NetDateTime();
            copy.m_YY = this.m_YY;
            copy.m_MM = this.m_MM;
            copy.m_DD = this.m_DD;
            copy.m_HH = this.m_HH;
            copy.m_MI = this.m_MI;
            copy.m_SS = this.m_SS;
            return copy;
        }
    }

    public class NetVector3
    {
        public Int64 m_X = 0;
        public Int64 m_Y = 0;
        public Int64 m_Z = 0;

        public NetVector3()
        {
            m_X = 0;
            m_Y = 0;
            m_Z = 0;
        }
        public NetVector3(float x, float y, float z)
        {
            m_X = (Int64)(x * 1000);
            m_Y = (Int64)(y * 1000);
            m_Z = (Int64)(z * 1000);
        }
        public NetVector3(Vector3 vec)
        {
            m_X = (Int64)(vec.x * 1000);
            m_Y = (Int64)(vec.y * 1000);
            m_Z = (Int64)(vec.z * 1000);
        }

        public void ReadBin(BinaryReader br)
        {
            m_X = br.ReadInt64();
            m_Y = br.ReadInt64();
            m_Z = br.ReadInt64();
        }

        public void WriteBin(BinaryWriter bw)
        {
            bw.Write((Int64)m_X);
            bw.Write((Int64)m_Y);
            bw.Write((Int64)m_Z); ;
        }
        public Vector3 GetVector3()
        {
            Vector3 vec = new Vector3();
            vec.x = m_X / 1000;
            vec.y = m_Y / 1000;
            vec.z = m_Z / 1000;
            return vec;
        }
    }
}
