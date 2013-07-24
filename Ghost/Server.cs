using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using QBXMLRP2Lib;

// code mostly stolen from:
// http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server

namespace Ghost {
    class Server {
        private TcpListener tcpListener;
        private Thread listenThread;

        public Server() {
            this.tcpListener = new TcpListener(IPAddress.Loopback , 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients() {
            this.tcpListener.Start();

            while (true) {
                // blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();

                // create a thread to handle communication
                // with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client) {
            Console.WriteLine("Client Attached.");
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true) {
                Console.WriteLine("Waiting");
                
                bytesRead = 0;
                try {
                    // blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                } catch {
                    // a socket error has occured
                    break;
                }
                if (bytesRead == 0) {
                    // the client has disconnected from the server
                    break;
                }

                // message has successfully been received
                UTF8Encoding encoder = new UTF8Encoding();
                string msg = encoder.GetString(message);
                Console.WriteLine("Received: " + bytesRead + " bytes, with MD5: " + MD5Hash(msg));
                string reply = QBConnect(msg);

                // send a message
                Console.Write("Replying...");
                byte[] buffer = encoder.GetBytes(reply);
                clientStream.Write(buffer, 0, buffer.Length);
                Console.Write(" Done!\n  Sent " + buffer.Length + " bytes.\n\n");
                
                // cleaning up...
                clientStream.Flush();
                Array.Clear(message, 0, message.Length);
            }

            Console.WriteLine("Closing...");
            tcpClient.Close();
        }

        static string QBConnect(string data) {
            RequestProcessor2 rp = null;
            string ticket = null;
            string response = null;

            try {
                rp = new RequestProcessor2();
                rp.OpenConnection("", "Ghost QuickBooks XML Bridge");
                ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare);
                response = rp.ProcessRequest(ticket, data);
            } catch (System.Runtime.InteropServices.COMException ex) {
                return "COM Error Description = " + ex.Message + "\n";
            } finally {
                if (ticket != null) {
                    rp.EndSession(ticket);
                }
                if (rp != null) {
                    rp.CloseConnection();
                }
            };
            return response;
        }

        // this unction dosn't exsist in the std lib?
        // not even in a crypto lib?
        // ffs dudes!!!  come on!!!!!
        static string MD5Hash(string input) {
            // step 1, calculate MD5 hash from input
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
