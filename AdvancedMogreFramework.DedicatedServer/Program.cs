using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMogreFramework.DedicatedServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer server = new GameServer(6900);
            server.Run();
        }
    }
}
