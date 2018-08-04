using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace WOL.Utility
{
    public class WolManager
    {
        public static async void Wake(string broadcast, int port, byte[] mac)
        {
            using (UdpClient udp = new UdpClient())
            {
                udp.EnableBroadcast = true;

                byte[] packet = new byte[6 + 16 * 6];

                for (int i = 0; i < 6; i++)
                {
                    packet[i] = 0xFF;
                }

                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        packet[6 + i * 6 + j] = mac[j];
                    }
                }

                await udp.SendAsync(packet, packet.Length, broadcast, port);
            }
        }
    }
}
