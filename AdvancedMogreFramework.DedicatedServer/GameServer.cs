using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Management;

namespace AdvancedMogreFramework.DedicatedServer
{
    public class GameServer
    {
        private bool running;
        private Socket srvSocket;
        private List<Player> players;

        public GameServer(int port, int maxPlayer = 20)
        {
            init(port, maxPlayer);
            players = new List<Player>();
            running = true;
        }

        public void Run()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("--Advanced Mogre Framework Game Server--");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Version: 1.2");
            Console.WriteLine("Server Status: Started");
            while(running)
            {
                Socket clinetSock = srvSocket.Accept();
                Player p = new Player(players.Count, clinetSock);
                p.StartThread();
                p.PlayerExit += PlayerExit;
                players.Add(p);
            }
            Console.WriteLine("Server Stopped!");
        }

        private void PlayerExit(int pId)
        {
            Player player = players.Find(o => o.Id == pId);
            if (player != null)
            {
                Console.WriteLine(string.Format("[LOG]: {0} exited from the server.", player.Name));
                players.Remove(player);
            }
        }

        private void init(int port, int maxPlayer)
        {
            srvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine(getIpAddress());
            srvSocket.Bind(new IPEndPoint(new IPAddress(new byte[] {127,0,0,1 }), port));
            srvSocket.Listen(maxPlayer);
        }

        private string getIpAddress()
        {
            string stringMAC = "";
            string stringIP = "";
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                if ((bool)managementObject["IPEnabled"] == true)
                {
                    stringMAC += managementObject["MACAddress"].ToString();
                    string[] IPAddresses = (string[])managementObject["IPAddress"];
                    if (IPAddresses.Length > 0)
                    {
                        stringIP = IPAddresses[0];
                    }
                }
            }
            return stringIP.ToString();
        }

        public void SendMsgToPlayer(string message, int playerId)
        {
            Player player = players.Find(o => o.Id == playerId);
            if (player != null)
            {
                player.ReceiveMsg(message);
            }
        }

        public void SendMsgToAllPlayers(string message)
        {
            foreach(Player player in players)
            {
                player.ReceiveMsg(message);
            }
        }
    }
}
