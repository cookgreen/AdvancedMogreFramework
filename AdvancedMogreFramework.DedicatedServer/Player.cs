using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AdvancedMogreFramework.DedicatedServer
{
    class Player
    {
        private int id;
        private Socket clientScoket;
        private Thread thread;
        private string name;

        public int Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public event Action<int> PlayerExit;
        public Player(int PId, Socket socket)
        {
            id = PId;
            clientScoket = socket;
            name = "UNRECONIZED TOKEN";
        }

        public void StartThread()
        {
            thread = new Thread(PlayerThread);
            thread.Start();
        }

        public void PlayerThread()
        {
            while(true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int length = clientScoket.Receive(buffer);
                    if (length > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer);
                        string[] msgs = data.Split(';');
                        foreach (string msg in msgs)
                        {
                            string[] cmdTokens = msg.Split(' ');
                            switch (cmdTokens[0])
                            {
                                case "USRNAME":
                                    name = cmdTokens[1];
                                    Console.WriteLine(string.Format("[LOG]: {0} join the server.", name));
                                    break;
                                case "MSG":
                                    Console.WriteLine(string.Format("[LOG]: {0}: " + cmdTokens[1], name));
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    PlayerExit?.Invoke(id);
                }
            }
        }

        public void ReceiveMsg()
        {

        }

        public void SendMsg()
        {

        }

        public void ReceiveMsg(string message)
        {
            clientScoket.Send(Encoding.UTF8.GetBytes(message));
        }

        public string GetIP()
        {
            return clientScoket.RemoteEndPoint.ToString();
        }
    }
}
