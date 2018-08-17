using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace WOL.Utility
{
    public class NetworkManager
    {
        public static void GetNetworkIpAndMask(out IPAddress ip, out IPAddress subnetMask)
        {
            var address = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet || n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                .SelectMany(n => n.GetIPProperties().UnicastAddresses);

            ip = null;
            subnetMask = null;
            foreach (var item in address)
            {
                var temp = item.Address.MapToIPv4();
                if (Ping(temp.ToString(), 15))
                {
                    ip = temp;
                    subnetMask = item.IPv4Mask.MapToIPv4();
                }
            }
        }

        static public IPAddress CalSubnetMask(IPAddress ip)
        {
            uint firstOctet = ip.GetAddressBytes()[0];

            string res = "";
            if (firstOctet >= 0 && firstOctet <= 127)
                res = "255.0.0.0";
            else if (firstOctet >= 128 && firstOctet <= 191)
                res = "255.255.0.0";
            else if (firstOctet >= 192 && firstOctet <= 223)
                res = "255.255.255.0";
            else
                res = "0.0.0.0";

            return IPAddress.Parse(res);
        }

        public static IPAddress CalBroadcast(IPAddress ip, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = ip.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress CalNetworkAddress(IPAddress ip, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = ip.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetDefaultGateway()
        {
            var gateway = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties().GatewayAddresses);

            foreach (var item in gateway)
            {
                if (Ping(item.Address.MapToIPv4().ToString(), 15))
                {
                    return item.Address;
                }
            }

            return null;
        }

        public static bool Ping(string ip, int timeout)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(ip, timeout);

            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<string[]> GetClientMac()
        {
            List<string[]> arps = new List<string[]>();

            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = "cat",
                        CreateNoWindow = false,
                        RedirectStandardInput = false,
                        RedirectStandardOutput = true,
                        Arguments = "/proc/net/arp",
                        UseShellExecute = false
                    };

                    Process process = Process.Start(psi);

                    while (!process.StandardOutput.EndOfStream)
                    {
                        string arp = process.StandardOutput.ReadLine();

                        if (arp.Contains("0x2"))
                        {
                            arp = Regex.Replace(arp, @"\s+", " ");
                            string[] raw = arp.Split(' ');
                            string[] info = new string[] { raw[0], raw[3].Replace(':', '-').ToUpper() };
                            arps.Add(info);
                        }
                    }

                    process.WaitForExit();
                }
                else if (Device.RuntimePlatform == Device.UWP)
                {

                }
            }
            catch (Exception)
            {
                
            }

            return arps;
        }
    }
}
