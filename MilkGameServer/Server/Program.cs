﻿using GameServer.SocketFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {        
            Server server = new Server("127.0.0.1", 6688);
            server.Start();
            Console.ReadKey();
        }
    }
}
