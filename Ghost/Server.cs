using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

// code mostly stolen from:
// http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server

namespace Ghost {
    class Server {
        private TcpListener tcpListener;
        private Thread listenThread;

        public Server() {
            Console.WriteLine("making server");
            this.tcpListener = new TcpListener(IPAddress.Loopback , 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients() {
            Console.WriteLine("in listen for clients");
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
            Console.WriteLine("in handle client comm");
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true) {
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
                ASCIIEncoding encoder = new ASCIIEncoding();
                Console.WriteLine(encoder.GetString(message, 0, bytesRead));

                // send a message
                Console.WriteLine("replying...\n");
                ASCIIEncoding reply = new ASCIIEncoding();
                byte[] buffer = reply.GetBytes("\nHello Client!\n\n");

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }

            tcpClient.Close();
        }
    }
}
