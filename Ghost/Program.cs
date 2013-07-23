using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QBXMLRP2Lib;

namespace Ghost {
    class Program {
        static void Main(string[] args) {
            // start up the server!
            Console.WriteLine("Starting up the server,\n  please allow this to run in QuickBooks...");
            Server srv = new Server();
        }
    }
}
