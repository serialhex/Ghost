using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zombie;
using QBXMLRP2Lib;

namespace Ghost {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("in main");
            
            // use this later... 
            //Server srv = new Server();

            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"6.0\"?><QBXML><QBXMLMsgsRq onError=\"stopOnError\"><CustomerQueryRq requestID=\"5001\" iterator=\"Start\"><MaxReturned>10</MaxReturned><IncludeRetElement>ListID</IncludeRetElement></CustomerQueryRq></QBXMLMsgsRq></QBXML>";
            // make request
            RequestProcessor2 rp = null;
            string ticket = null;
            string response = null;
            try {
                rp = new RequestProcessor2();
                rp.OpenConnection("", "Test app");
                ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare);
                response = rp.ProcessRequest(ticket, xml);

            } catch (System.Runtime.InteropServices.COMException ex) {
                Console.WriteLine("COM Error Description = " + ex.Message, "COM error");
                Pause();
                return;
            } finally {
                if (ticket != null) {
                    rp.EndSession(ticket);
                }
                if (rp != null) {
                    rp.CloseConnection();
                }
            };
            Console.Write(response);
            Pause();
        }
        
        static void Pause() {
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
}
