using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace SwarmAdvertiser {
    class Client
    {
        UdpClient client;
        IPEndPoint ip;

        public Client() {
            ip = new IPEndPoint(IPAddress.Any, Config.PORT_NUMBER);

            client = new UdpClient();

        }
        public void Request()
        {
            Send("Doit");
            Receive();
            client.Close();
        }

        private void Receive()
        {
            byte[] bytes = client.Receive(ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("From {0} received: {1} ", ip.Address.ToString(), message);
        }

        private void Send(string message)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("255.255.255.255"), Config.PORT_NUMBER);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            client.Send(bytes, bytes.Length, ip);
            //client.Close();
            Console.WriteLine("Sent: {0} ", message);
        }
    }
}
