using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkAdapterRouteControl.WinApi
{
    internal class IpHelper
    {
        public static List<AdapterInfo> GetAdaptersInfo()
        {
            var ptr = IntPtr.Zero;
            var size = 10 * Marshal.SizeOf(typeof(NativeMethods.IP_ADAPTER_INFO));
            var adapterInfoTable = new List<AdapterInfo>();
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                var result = NativeMethods.GetAdaptersInfo(ptr, ref size);
                if (result == ApiError.ERROR_BUFFER_OVERFLOW)
                {
                    ptr = Marshal.ReAllocHGlobal(ptr, new IntPtr(size));
                    result = NativeMethods.GetAdaptersInfo(ptr, ref size);
                }
                else if (result != ApiError.ERROR_SUCCESS)
                {
                    throw new Win32Exception(result);
                }

                var nodePtr = ptr;
                while (nodePtr != IntPtr.Zero)
                {
                    var info = (NativeMethods.IP_ADAPTER_INFO) Marshal.PtrToStructure(nodePtr, typeof(NativeMethods.IP_ADAPTER_INFO));
                    IPAddress dhcpServer = null;
                    IPAddress primaryGateway = null;
                    if (info.GatewayList.Context > 0) IPAddress.TryParse(info.GatewayList.IpAddress.Address, out primaryGateway);
                    if (info.DhcpEnabled > 0) IPAddress.TryParse(info.DhcpServer.IpAddress.Address, out dhcpServer);
                    var ip4adapterInfo = new AdapterInfo()
                    {
                        Index = Convert.ToInt32(info.Index),
                        Description = info.AdapterDescription,
                        PrimaryGateway = primaryGateway,
                        DhcpServer = dhcpServer
                    };
                    adapterInfoTable.Add(ip4adapterInfo);
                    nodePtr = info.Next;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return adapterInfoTable;
        }

        public static List<RouteEntry> GetRouteTable(bool order = true)
        {
            var tablePtr = IntPtr.Zero;
            List<RouteEntry> routeTable;
            try
            {
                var size = 4096;
                tablePtr = Marshal.AllocHGlobal(size);
                var result = NativeMethods.GetIpForwardTable(tablePtr, ref size, order);

                if (result == ApiError.ERROR_INSUFFICIENT_BUFFER)
                {
                    tablePtr = Marshal.ReAllocHGlobal(tablePtr, new IntPtr(size));
                    result = NativeMethods.GetIpForwardTable(tablePtr, ref size, order);
                }
                else if (result != ApiError.ERROR_SUCCESS)
                {
                    throw new Win32Exception(result);
                }

                var table = (NativeMethods.MIB_IPFORWARDTABLE) Marshal.PtrToStructure(tablePtr,
                    typeof(NativeMethods.MIB_IPFORWARDTABLE));
                routeTable = new List<RouteEntry>(Convert.ToInt32(table.Size));

                var rowPtr = new IntPtr(tablePtr.ToInt64() + Marshal.SizeOf(table.Size));
                for (var i = 0; i < table.Size; i++)
                {
                    var row = (NativeMethods.MIB_IPFORWARDROW) Marshal.PtrToStructure(rowPtr,
                        typeof(NativeMethods.MIB_IPFORWARDROW));
                    var entry = new RouteEntry
                    {
                        DestinationIP = new IPAddress((long) row.dwForwardDest),
                        SubnetMask = new IPAddress((long) row.dwForwardMask),
                        GatewayIP = new IPAddress((long) row.dwForwardNextHop),
                        InterfaceIndex = Convert.ToInt32(row.dwForwardIfIndex),
                        ForwardType = Convert.ToInt32(row.dwForwardType),
                        ForwardProtocol = Convert.ToInt32(row.dwForwardProto),
                        ForwardAge = Convert.ToInt32(row.dwForwardAge),
                        Metric = Convert.ToInt32(row.dwForwardMetric1)
                    };
                    routeTable.Add(entry);
                    rowPtr = new IntPtr(rowPtr.ToInt64() + Marshal.SizeOf(typeof(NativeMethods.MIB_IPFORWARDROW)));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tablePtr);
            }

            return routeTable;
        }

        public static void CreateRoute(RouteEntry routeEntry)
        {
            var route = new NativeMethods.MIB_IPFORWARDROW
            {
                dwForwardDest =
                    BitConverter.ToUInt32(IPAddress.Parse(routeEntry.DestinationIP.ToString()).GetAddressBytes(), 0),
                dwForwardMask =
                    BitConverter.ToUInt32(IPAddress.Parse(routeEntry.SubnetMask.ToString()).GetAddressBytes(), 0),
                dwForwardNextHop =
                    BitConverter.ToUInt32(IPAddress.Parse(routeEntry.GatewayIP.ToString()).GetAddressBytes(), 0),
                dwForwardMetric1 = Convert.ToUInt32(routeEntry.Metric),
                dwForwardType = Convert.ToUInt32(routeEntry.ForwardType),
                dwForwardProto = Convert.ToUInt32(NativeMethods.MIB_IPFORWARD_PROTO.MIB_IPPROTO_NETMGMT),
                dwForwardAge = 0,
                dwForwardIfIndex = Convert.ToUInt32(routeEntry.InterfaceIndex)
            };

            var rowPtr = IntPtr.Zero;
            try
            {
                rowPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.MIB_IPFORWARDROW)));
                Marshal.StructureToPtr(route, rowPtr, false);
                var result = NativeMethods.CreateIpForwardEntry(rowPtr);
                if (result == ApiError.ERROR_OBJECT_ALREADY_EXISTS)
                {
                    SetRoute(routeEntry);
                }
                else if (result != ApiError.ERROR_SUCCESS)
                {
                    throw new Win32Exception(result);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(rowPtr);
            }
        }

        public static void SetRoute(RouteEntry routeEntry)
        {
            var route = new NativeMethods.MIB_IPFORWARDROW
            {
                dwForwardDest = BitConverter.ToUInt32(IPAddress.Parse(routeEntry.DestinationIP.ToString()).GetAddressBytes(), 0),
                dwForwardMask = BitConverter.ToUInt32(IPAddress.Parse(routeEntry.SubnetMask.ToString()).GetAddressBytes(), 0),
                dwForwardNextHop = BitConverter.ToUInt32(IPAddress.Parse(routeEntry.GatewayIP.ToString()).GetAddressBytes(), 0),
                dwForwardMetric1 = Convert.ToUInt32(routeEntry.Metric),
                dwForwardType = Convert.ToUInt32(routeEntry.ForwardType),
                dwForwardProto = Convert.ToUInt32(NativeMethods.MIB_IPFORWARD_PROTO.MIB_IPPROTO_NETMGMT),
                dwForwardAge = 0,
                dwForwardIfIndex = Convert.ToUInt32(routeEntry.InterfaceIndex)
            };

            var rowPtr = IntPtr.Zero;
            try
            {
                rowPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.MIB_IPFORWARDROW)));
                Marshal.StructureToPtr(route, rowPtr, false);
                var result = NativeMethods.SetIpForwardEntry(rowPtr);
                if (result == ApiError.ERROR_NOT_FOUND)
                {
                    CreateRoute(routeEntry);
                }
                else if (result != ApiError.ERROR_SUCCESS && result != ApiError.ERROR_FILE_NOT_FOUND)
                {
                    throw new Win32Exception(result);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(rowPtr);
            }
        }

        public static void DeleteRoute(RouteEntry routeEntry)
        {
            var route = new NativeMethods.MIB_IPFORWARDROW
            {
                dwForwardDest =
                    BitConverter.ToUInt32(IPAddress.Parse(routeEntry.DestinationIP.ToString()).GetAddressBytes(), 0),
                dwForwardMask =
                    BitConverter.ToUInt32(IPAddress.Parse(routeEntry.SubnetMask.ToString()).GetAddressBytes(), 0),
                dwForwardNextHop =
                    BitConverter.ToUInt32(IPAddress.Parse(routeEntry.GatewayIP.ToString()).GetAddressBytes(), 0),
                dwForwardMetric1 = Convert.ToUInt32(routeEntry.Metric),
                dwForwardType = Convert.ToUInt32(routeEntry.ForwardType),
                dwForwardProto = Convert.ToUInt32(NativeMethods.MIB_IPFORWARD_PROTO.MIB_IPPROTO_NETMGMT),
                dwForwardAge = 0,
                dwForwardIfIndex = Convert.ToUInt32(routeEntry.InterfaceIndex)
            };

            var rowPtr = IntPtr.Zero;
            try
            {
                rowPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.MIB_IPFORWARDROW)));
                Marshal.StructureToPtr(route, rowPtr, false);
                var result = NativeMethods.DeleteIpForwardEntry(rowPtr);
                if (result != ApiError.ERROR_SUCCESS &&
                    result != ApiError.ERROR_NOT_FOUND)
                {
                    throw new Win32Exception(result);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(rowPtr);
            }
        }

        public static void SetMetric(int interfaceUIndex, int metric)
        {
            var rowPtr = IntPtr.Zero;
            try
            {
                var row = new NativeMethods.MIB_IPINTERFACE_ROW
                {
                    Family = 2,
                    InterfaceIndex = Convert.ToUInt32(interfaceUIndex)
                };
                rowPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.MIB_IPINTERFACE_ROW)));
                Marshal.StructureToPtr(row, rowPtr, false);

                var result = NativeMethods.GetIpInterfaceEntry(rowPtr);
                if (result != ApiError.ERROR_SUCCESS) throw new Win32Exception(result);
                row = (NativeMethods.MIB_IPINTERFACE_ROW)Marshal.PtrToStructure(rowPtr, typeof(NativeMethods.MIB_IPINTERFACE_ROW));

                if (row.Metric != (uint)metric)
                {
                    row.Metric = Convert.ToUInt32(metric);
                    row.SitePrefixLength = 0;
                    Marshal.StructureToPtr(row, rowPtr, false);
                    result = NativeMethods.SetIpInterfaceEntry(rowPtr);
                    if (result != ApiError.ERROR_SUCCESS) throw new Win32Exception(result);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(rowPtr);
            }
        }
    }

    public class AdapterInfo
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public IPAddress PrimaryGateway { get; set; }
        public IPAddress DhcpServer { get; set; }
        public override string ToString()
        {
            return Description.ToString();
        }
    }

    public class RouteEntry
    {
        public IPAddress DestinationIP { get; set; }
        public IPAddress SubnetMask { get; set; }
        public IPAddress GatewayIP { get; set; }
        public int InterfaceIndex { get; set; }
        public int ForwardType { get; set; }
        public int ForwardProtocol { get; set; }
        public int ForwardAge { get; set; }
        public int Metric { get; set; }

        public override string ToString()
        {
            return $"{DestinationIP.ToString()} ({InterfaceIndex})";
        }
    }

    internal class NativeMethods
    {
        private const int MAX_ADAPTER_DESCRIPTION_LENGTH = 128;
        private const int MAX_ADAPTER_NAME_LENGTH = 256;
        private const int MAX_ADAPTER_ADDRESS_LENGTH = 8;

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/iphlp/managing-routing
        /// https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-rrasm/5dca234b-bea4-4e67-958e-5459a32a7b71
        /// </summary>

        [StructLayout(LayoutKind.Sequential)]
        public struct IP_ADDRESS_STRING
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string Address;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IP_ADDR_STRING
        {
            public IntPtr Next;
            public IP_ADDRESS_STRING IpAddress;
            public IP_ADDRESS_STRING IpMask;
            public int Context;
        }

        [ComVisible(false)]
        [StructLayout(LayoutKind.Sequential)]
        public struct IP_ADAPTER_INFO
        {
            public IntPtr Next;
            public uint ComboIndex;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_NAME_LENGTH + 4)]
            public string AdapterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_DESCRIPTION_LENGTH + 4)]
            public string AdapterDescription;
            public uint AddressLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ADAPTER_ADDRESS_LENGTH)]
            public byte[] Address;
            public uint Index;
            public uint Type;
            public uint DhcpEnabled;
            public IntPtr CurrentIpAddress;
            public IP_ADDR_STRING IpAddressList;
            public IP_ADDR_STRING GatewayList;
            public IP_ADDR_STRING DhcpServer;
            public byte HaveWins;
            public IP_ADDR_STRING PrimaryWinsServer;
            public IP_ADDR_STRING SecondaryWinsServer;
            public int LeaseObtained;
            public int LeaseExpires;
        };

        [ComVisible(false)]
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IPFORWARDTABLE
        {
            public uint Size;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public MIB_IPFORWARDROW[] Table;
        };

        [ComVisible(false)]
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IPFORWARDROW
        {
            internal uint dwForwardDest;
            internal uint dwForwardMask;
            internal uint dwForwardPolicy;
            internal uint dwForwardNextHop;
            internal uint dwForwardIfIndex;
            internal uint dwForwardType;
            internal uint dwForwardProto;
            internal uint dwForwardAge;
            internal uint dwForwardNextHopAS;
            internal uint dwForwardMetric1;
            internal uint dwForwardMetric2;
            internal uint dwForwardMetric3;
            internal uint dwForwardMetric4;
            internal uint dwForwardMetric5;
        };

        public enum MIB_IPFORWARD_TYPE : uint
        {
            MIB_IPROUTE_TYPE_OTHER = 1,
            MIB_IPROUTE_TYPE_INVALID = 2,
            MIB_IPROUTE_TYPE_DIRECT = 3,
            MIB_IPROUTE_TYPE_INDIRECT = 4
        }


        public enum MIB_IPFORWARD_PROTO : uint
        {
            MIB_IPPROTO_OTHER = 1,
            MIB_IPPROTO_LOCAL = 2,
            MIB_IPPROTO_NETMGMT = 3,
            MIB_IPPROTO_ICMP = 4,
            MIB_IPPROTO_EGP = 5,
            MIB_IPPROTO_GGP = 6,
            MIB_IPPROTO_HELLO = 7,
            MIB_IPPROTO_RIP = 8,
            MIB_IPPROTO_IS_IS = 9,
            MIB_IPPROTO_ES_IS = 10,
            MIB_IPPROTO_CISCO = 11,
            MIB_IPPROTO_BBN = 12,
            MIB_IPPROTO_OSPF = 13,
            MIB_IPPROTO_BGP = 14,
            MIB_IPPROTO_NT_AUTOSTATIC = 10002,
            MIB_IPPROTO_NT_STATIC = 10006,
            MIB_IPPROTO_NT_STATIC_NON_DOD = 10007
        }


        [ComVisible(false)]
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IPINTERFACE_ROW
        {
            public uint Family;
            public ulong InterfaceLuid;
            public uint InterfaceIndex;
            public uint MaxReassemblySize;
            public ulong InterfaceIdentifier;
            public uint MinRouterAdvertisementInterval;
            public uint MaxRouterAdvertisementInterval;
            public byte AdvertisingEnabled;
            public byte ForwardingEnabled;
            public byte WeakHostSend;
            public byte WeakHostReceive;
            public byte UseAutomaticMetric;
            public byte UseNeighborUnreachabilityDetection;
            public byte ManagedAddressConfigurationSupported;
            public byte OtherStatefulConfigurationSupported;
            public byte AdvertiseDefaultRoute;
            public uint RouterDiscoveryBehavior;
            public uint DadTransmits;
            public uint BaseReachableTime;
            public uint RetransmitTime;
            public uint PathMtuDiscoveryTimeout;
            public uint LinkLocalAddressBehavior;
            public uint LinkLocalAddressTimeout;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] ZoneIndices;
            public uint SitePrefixLength;
            public uint Metric;
            public uint NlMtu;
            public byte Connected;
            public byte SupportsWakeUpPatterns;
            public byte SupportsNeighborDiscovery;
            public byte SupportsRouterDiscovery;
            public uint ReachableTime;
            public byte TransmitOffload;
            public byte ReceiveOffload;
            public byte DisableDefaultRoutes;
        };


        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public static extern int GetAdaptersInfo(IntPtr AdapterInfo, ref int /* PULONG */ SizePointer);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public static extern int GetIpForwardTable(IntPtr /* PMIB_IPFORWARDTABLE */ pIpForwardTable,
            ref int /* PULONG */ pdwSize, bool /* BOOL */ bOrder);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public static extern int CreateIpForwardEntry(IntPtr /* PMIB_IPFORWARDROW */ pRoute);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public static extern int DeleteIpForwardEntry(IntPtr /* PMIB_IPFORWARDROW */ pRoute);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public static extern int SetIpForwardEntry(IntPtr /* PMIB_IPFORWARDROW */ pRoute);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public static extern int SetIpInterfaceEntry(IntPtr /* MIB_IPINTERFACE_ROW */ Row);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public static extern int GetIpInterfaceEntry(IntPtr /* MIB_IPINTERFACE_ROW */ Row);
    }
}