using System;
using System.Collections.Generic;
using System.IO;

namespace MNF
{
	[System.Serializable]
	public class UserItem
	{
		public int		m_nIndex	= 0;
		public ushort	m_nID		= 0;
        public ushort	m_nCount	= 0;
        public byte		m_nEnable	= 0;

        virtual public void ReadBin(BinaryReader br)
		{
            m_nIndex = br.ReadInt32();
            m_nID    = br.ReadUInt16();
            m_nCount = br.ReadUInt16();
			m_nEnable = br.ReadByte();
		}

		virtual public void WriteBin(BinaryWriter bw)
		{
			bw.Write((int)m_nIndex);
			bw.Write((ushort)m_nID);
            bw.Write((ushort)m_nCount);
            bw.Write((byte)m_nEnable);
		}
        public UserItem Copy()
        {
            UserItem copy = new UserItem();
            copy.m_nIndex = this.m_nIndex;
            copy.m_nID = this.m_nID;
            copy.m_nCount = this.m_nCount;
            copy.m_nEnable = this.m_nEnable;

            return copy;
        }
    }

    [System.Serializable]
    public class UserBase
    {
        public byte m_byMainNo = 0; // 메인서버 번호
        public Int64 m_nUserNo = 0;     // 유저번호
        public string m_szUserID = "";   // 유저아이디

        virtual public void ReadBin(BinaryReader br)
        {
            m_byMainNo = br.ReadByte();
            m_nUserNo = br.ReadInt64();
            NetString.ReadString(br, ref m_szUserID);
        }

        virtual public void WriteBin(BinaryWriter bw)
        {
            bw.Write((byte)m_byMainNo);
            bw.Write((Int64)m_nUserNo);
            NetString.WriteString(bw, m_szUserID);
        }
        public UserBase Copy()
        {
            UserBase copy = new UserBase();
            copy.m_byMainNo = this.m_byMainNo;
            copy.m_nUserNo = this.m_nUserNo;
            copy.m_szUserID = this.m_szUserID;
            return copy;
        }
    }

    public class UserSession : UserBase
	{
		public NetVector3[] m_userTransform = new NetVector3[NetConst.SIZE_USER_TRASNFORM];
		public int [] m_nUserData = new int[NetConst.SIZE_USER_DATA];
        public List<UserItem> m_itemList = new List<UserItem>();

        override public void ReadBin(BinaryReader br)
		{
			base.ReadBin(br);

			for (int i = 0; i < NetConst.SIZE_USER_TRASNFORM; i++)
			{
				m_userTransform[i] = new NetVector3();
				m_userTransform[i].ReadBin(br);
			}

            for (int i = 0; i < NetConst.SIZE_USER_DATA; i++)
                m_nUserData[i] = br.ReadInt32();

            m_itemList.Clear();
            int size = br.ReadInt32();
            for (int i = 0; i < size; i++)
            {
                UserItem data = new UserItem();
                data.ReadBin(br);
                m_itemList.Add(data);
            }
        }

		override public void WriteBin(BinaryWriter bw)
		{
			base.WriteBin(bw);

			for(int i = 0; i < NetConst.SIZE_USER_TRASNFORM; i++)
				m_userTransform[i].WriteBin(bw);

            for (int i = 0; i < NetConst.SIZE_USER_DATA; i++)
				bw.Write((int)m_nUserData[i]);

            int size = m_itemList.Count;
            bw.Write((int)size);
            for (int i = 0; i < size; i++)
                m_itemList[i].WriteBin(bw);
        }

        public void UserDataUpdate(UserSession user)
        {
            for (int i = 0; i < NetConst.SIZE_USER_DATA; i++)
                m_nUserData[i] = user.m_nUserData[i];
        }
        public void UserMoveDirect(UserSession user)
        {
            for (int i = 0; i < NetConst.SIZE_USER_TRASNFORM; i++)
                m_userTransform[i] = user.m_userTransform[i];
        }
        public void UserItemUpdate(UserSession user)
        {
            m_itemList.Clear();
            int size = user.m_itemList.Count;
            for (int i = 0; i < size; i++)
            {
                UserItem data = user.m_itemList[i].Copy();
                m_itemList.Add(data);
            }
        }
    }

	public class UserHandle : UserBase
	{
		override public void ReadBin(BinaryReader br)
		{
			base.ReadBin(br);
		}

		override public void WriteBin(BinaryWriter bw)
		{

		}
	}
}
