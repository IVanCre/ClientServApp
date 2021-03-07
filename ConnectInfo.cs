using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Generic;


namespace PeerWebApp2._0
{
    class ConnectInfo
    {
        internal string myAdress, FriendAdress;//класс позволяет узнавать и передавать IPадрес текущей машины

        public string GetMyAdress()
        {
            var iplist = new List<string>();
            int num = 0;
            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ips = iface.GetIPProperties().UnicastAddresses;//собирает все назначенные адреса
                foreach (var ip in ips)
                {
                    num += 1;
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork && ip.Address.ToString() != "127.0.0.1")
                        iplist.Add(ip.Address.ToString());
                    if (num == 2)//2 элемент - текущий ipv4 адрес
                    {
                        myAdress = ip.Address.ToString();//получаем наш адрес
                    }
                }
            }

            
            return myAdress;
        }


    }
}
