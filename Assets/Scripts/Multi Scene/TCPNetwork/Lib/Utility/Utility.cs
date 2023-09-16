using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

#if !NETFX_CORE
using System.Diagnostics;
#else
using System.Threading.Tasks;
using Windows.System.Diagnostics;
#endif

namespace MNF
{
    public static class JsonSupport
    {
        public static string Serialize<T>(T managedData)
        {
#if USE_JSON_FX
            return JsonFx.Json.JsonWriter.Serialize(managedData);
#else
			return UnityEngine.JsonUtility.ToJson(managedData);
#endif
        }

        public static object DeSerialize(string managedData, Type type)
        {
#if USE_JSON_FX
            return JsonFx.Json.JsonReader.Deserialize(managedData, type);
#else
			return UnityEngine.JsonUtility.FromJson(managedData, type);
#endif
        }
    }

    public static class Utility
    {
        public struct stLocalAddressInfo
        {
            public IPAddress localAddress;
            public IPAddress subnetAddress;

            public stLocalAddressInfo(IPAddress localAddress, IPAddress subnetAddress)
            {
                this.localAddress = localAddress;
                this.subnetAddress = subnetAddress;
            }
        }

        public static int Sizeof<T>(ref T structure)
        {
            return Marshal.SizeOf(structure);
        }

        public static int Sizeof<T>()
        {
#if !NETFX_CORE
            return Marshal.SizeOf(typeof(T));
#else
            return Marshal.SizeOf<T>();
#endif
        }

        public static Assembly GetAssembly(Type type)
        {
#if !NETFX_CORE
            return type.Assembly;
#else
            return type.GetTypeInfo().Assembly;
#endif
        }

        public static int GetCurrentThreadID()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        public static int GetProcessID()
        {
#if !NETFX_CORE
			return Process.GetCurrentProcess().Id;
#else
            return (int)ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId;
#endif
		}

        // for network
        public static IPEndPoint GetIPEndPoint(string ipString, string portString)
        {
            try
            {
                int portNum = Convert.ToInt32(portString);
                if (ipString.Length == 0)
                    return new IPEndPoint(IPAddress.Any, portNum);
                else
                    return new IPEndPoint(IPAddress.Parse(ipString), portNum);
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e, "IP({0}) Port({1}) Error", ipString, portString);
                return new IPEndPoint(IPAddress.Any, 0);
            }
        }

        // for thread
        public static void Sleep(int waitTick)
        {
#if !NETFX_CORE
            System.Threading.Thread.Sleep(waitTick);
#else
            Task.Delay(TimeSpan.FromMilliseconds(waitTick));
#endif
        }

        // for instance
        public static T GetInstance<T>(string type)
        {
            return (T)Activator.CreateInstance(Type.GetType(type));
        }

        public static object GetInstance(string type)
        {
            return Activator.CreateInstance(Type.GetType(type));
        }

        public static T GetInstance<T>(Type type)
        {
            return (T)Activator.CreateInstance(type);
        }

        public static object GetInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        // for enum
        public static Dictionary<int, string> EnumDictionary<T>()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            foreach (var foo in Enum.GetValues(typeof(T)))
            {
                dict.Add((int)foo, foo.ToString());
            }
            return dict;
        }

        // for delegate
        public static Delegate LoadDelegate(object targetObject, string methodName, Type delegateType)
        {
            MethodInfo methodInfo = targetObject.GetType().GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.FlattenHierarchy);
#if !NETFX_CORE
            Delegate loadedDelegate = Delegate.CreateDelegate(delegateType, targetObject, methodInfo);
#else
            Delegate loadedDelegate = methodInfo.CreateDelegate(delegateType, targetObject);
#endif
            return loadedDelegate;
        }

        // for datetime
        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }

        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return sTime.AddSeconds(unixtime);
        }

#if !NETFX_CORE
        public static bool ExportServerInfo(string[] args, out string serverIP, out string serverPort)
        {
            serverIP = "";
            serverPort = "";

            // get server ip, port
            string hostNameORaddress = "";
            foreach (string arg in args)
            {
                char[] delimiterChars = { '=' };
                string[] words = arg.Split(delimiterChars);
                if (words.Length != 2)
                    continue;

                switch (words[0])
                {
                    case "dns":
                        hostNameORaddress = words[1];
                        break;
                    case "port":
                        serverPort = words[1];
                        break;
                }
            }

            IPHostEntry host = Dns.GetHostEntry(hostNameORaddress);
            if (host.AddressList.Length == 0)
                return false;

            // find vaild ip
            if (hostNameORaddress.Length == 0)
            {
                // get local ip
                foreach (IPAddress ipAddress in host.AddressList)
                {
                    if (ipAddress.ToString().StartsWith("192.168.") == true)
                    {
                        serverIP = ipAddress.ToString();
                        break;
                    }
                }
                if (serverIP.Length == 0)
                {
                    serverIP = host.AddressList[0].ToString();
                }
            }
            else
            {
                // get remote ip
                serverIP = host.AddressList[0].ToString();
            }

            return true;
        }
#endif

#if !NETFX_CORE
        static public IPAddress getLocalAddress()
        {
            IPAddress mLocalAddress = null;

            try
            {
                foreach (NetworkInterface NetworkIntf in NetworkInterface.GetAllNetworkInterfaces())
                {
                    IPInterfaceProperties IPInterfaceProperties = NetworkIntf.GetIPProperties();
                    UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;

                    foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                    {
                        if (UnicastIPAddressInformation.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        if (UnicastIPAddressInformation.Address.ToString() == "127.0.0.1")
                        {
                            if (mLocalAddress == null)
                                mLocalAddress = UnicastIPAddressInformation.Address;
                            break;
                        }

                        mLocalAddress = UnicastIPAddressInformation.Address;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                mLocalAddress = IPAddress.Parse("127.0.0.1");
                LogManager.Instance.WriteException(e, "set default localAddress({0})", mLocalAddress);
            }

            return mLocalAddress;
        }
#endif

#if !NETFX_CORE
        public static IPAddress getSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }
#endif

#if !NETFX_CORE
        public static stLocalAddressInfo getLocalAddressInfo()
        {
            stLocalAddressInfo localAddressInfo = new stLocalAddressInfo(null, null);

            foreach (NetworkInterface NetworkIntf in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties IPInterfaceProperties = NetworkIntf.GetIPProperties();
                UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;

                foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                {
                    if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localAddressInfo.localAddress = UnicastIPAddressInformation.Address;
                        localAddressInfo.subnetAddress = UnicastIPAddressInformation.IPv4Mask;
                    }
                }
            }
            return localAddressInfo;
        }
#endif
    }

    //      // don't use this function
    //// because this function will be crash in unity3d
    //      public static string getLocalIPAddress()
    //      {
    //	var host = Dns.GetHostEntry(Dns.GetHostName());
    //          foreach (var ip in host.AddressList)
    //          {
    //              if (ip.AddressFamily == AddressFamily.InterNetwork)
    //              {
    //                  return ip.ToString();
    //              }
    //          }
    //	return "127.0.0.1";
    //      }

    //static T GetByName<T>(object target, string methodName)
    //{
    //    MethodInfo method = target.GetType().GetMethod(methodName,
    //                   BindingFlags.Public
    //                   | BindingFlags.Instance
    //                   | BindingFlags.FlattenHierarchy);

    //    // Insert appropriate check for method == null here

    //    return (T)Delegate.CreateDelegate(typeof(T), target, method);
    //}

    //static T GetByName(object target, string methodName)
    //{
    //    return (T)Delegate.CreateDelegate
    //        (typeof(T), target, methodName);
    //}
    //public class TestLoadEnumsInClass
    //{
    //    public class TestingEnums
    //    {
    //        public enum Color { Red, Blue, Yellow, Pink }

    //        public enum Styles { Plaid = 0, Striped = 23, Tartan = 65, Corduroy = 78 }

    //        public string TestingProperty { get; set; }

    //        public string TestingMethod()
    //        {
    //            return null;
    //        }
    //    }

    //    public void btnTest_Click(object sender, EventArgs e)
    //    {
    //        var t = typeof(TestingEnums);
    //        var nestedTypes = t.GetMembers().Where(item => item.MemberType == MemberTypes.NestedType);
    //        foreach (var item in nestedTypes)
    //        {
    //            var type = Type.GetType(item.ToString());
    //            if (type == null || type.IsEnum == false)
    //                continue;

    //            string items = " ";
    //            foreach (MemberInfo x in type.GetMembers())
    //            {
    //                if (x.MemberType != MemberTypes.Field)
    //                    continue;

    //                if (x.Name.Equals("value__") == true)
    //                    continue;

    //                items = items + (" " + Enum.Parse(type, x.Name));
    //                items = items + (" " + (int)Enum.Parse(type, x.Name));
    //            }
    //            Console.WriteLine(items);
    //        }
    //    }
    //}

}
