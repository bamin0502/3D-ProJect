using System;
using System.Runtime.InteropServices;

namespace MNF
{
    /*
     * https://msdn.microsoft.com/ko-kr/library/2x07fbw8(v=vs.110).aspx
    VT_EMPTY
    System.DBNull
    VT_NULL
    System.Runtime.InteropServices.ErrorWrapper
    VT_ERROR
    System.Reflection.Missing
    E_PARAMNOTFOUND가 includes VT_ERROR
    System.Runtime.InteropServices.DispatchWrapper
    VT_DISPATCH
    System.Runtime.InteropServices.UnknownWrapper
    VT_UNKNOWN
    System.Runtime.InteropServices.CurrencyWrapper
    VT_CY
    System.Boolean
    VT_BOOL
    System.SByte
    VT_I1
    System.Byte
    VT_UI1
    System.Int16
    VT_I2
    System.UInt16
    VT_UI2
    System.Int32
    VT_I4
    System.UInt32
    VT_UI4
    System.Int64
    VT_I8
    System.UInt64
    VT_UI8
    System.Single
    VT_R4
    System.Double
    VT_R8
    System.Decimal
    VT_DECIMAL
    System.DateTime
    VT_DATE
    System.String
    VT_BSTR
    System.IntPtr
    VT_INT
    System.UIntPtr
    VT_UINT
    System.Array
    VT_ARRAY    
    */
    public static class MarshalHelper
    {
        public static IntPtr AllocGlobalHeap(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        public static void DeAllocGlobalHeap(IntPtr buffer)
        {
            Marshal.FreeHGlobal(buffer);
        }

        public static byte[] RawSerialize<T>(T managedData)
        {
            byte[] RawData = null;
            try
            {
                int objectSize = GetManagedDataSize(managedData);
                RawData = new byte[objectSize];
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(RawData, 0);
                Marshal.StructureToPtr(managedData, buffer, false);
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "Object({0}) Marshal serialize failed", managedData.GetType().Name);
            }
            return RawData;
        }

        public static void RawSerialize<T>(T managedData, byte[] rawData, ref int marshaledSize)
        {
            try
            {
                marshaledSize = GetManagedDataSize(managedData);
                if (rawData.Length < marshaledSize)
                    rawData = new byte[marshaledSize];

                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(rawData, 0);
                Marshal.StructureToPtr(managedData, buffer, false);
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "Object({0}) Marshal serialize failed", managedData.GetType().Name);
            }
        }

        public static void RawSerialize<T>(T managedData, byte[] rawData, int beginIndex, ref int marshaledSize)
        {
            try
            {
                marshaledSize = GetManagedDataSize(managedData);
                if ((rawData.Length - beginIndex) < marshaledSize)
                    rawData = new byte[marshaledSize + beginIndex];

                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(rawData, beginIndex);
                Marshal.StructureToPtr(managedData, buffer, false);
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "Object({0}) Marshal serialize failed", managedData.GetType().Name);
            }
        }

        public static object RawDeSerialize(byte[] RawData, Type managedDataType)
        {
            int RawSize = GetManagedDataSize(managedDataType);

            //Size check
            if (RawSize > RawData.Length)
                return null;

            IntPtr buffer = Marshal.AllocHGlobal(RawSize);
            Marshal.Copy(RawData, 0, buffer, RawSize);
            object retobj = Marshal.PtrToStructure(buffer, managedDataType);
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        public static object RawDeSerialize(byte[] RawData, Type managedData, int beginIndex, ref IntPtr allocatedBuffer, ref int allocatedBufferSize)
        {
            int RawSize = GetManagedDataSize(managedData);

            //Size check
            if (RawSize > RawData.Length)
                return null;

            if (allocatedBufferSize < RawSize)
            {
                if (allocatedBufferSize > 0)
                    Marshal.FreeHGlobal(allocatedBuffer);

                allocatedBuffer = Marshal.AllocHGlobal(RawSize);
                allocatedBufferSize = RawSize;
            }

            Marshal.Copy(RawData, beginIndex, allocatedBuffer, RawSize);
            object retobj = Marshal.PtrToStructure(allocatedBuffer, managedData);
            return retobj;
        }

        public static int GetManagedDataSize(object managedData)
        {
            return Marshal.SizeOf(managedData);
        }

        public static int GetManagedDataSize(Type managedData)
        {
            return Marshal.SizeOf(managedData);
        }
    }
}
