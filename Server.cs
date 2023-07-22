//using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace  SwarmAdvertiser
{
    class Server
    {
        IPEndPoint ip;
        DockerClient dockerClient;
        String joinToken = "";
        public Server() {
            dockerClient = new DockerClientConfiguration().CreateClient();

            ip = new IPEndPoint(IPAddress.Any, Config.PORT_NUMBER);

            GetDockerSwarmInfo();
        }

        //Thread? t = null;
        public void Start()
        {
            /** if (t != null)
            {
                throw new Exception("Already started, stop first");
            }**/
            Console.WriteLine("Started listening");
            StartListening();
        }
        public void Stop()
        {
            try
            {
                udp.Close();
                Console.WriteLine("Stopped listening");
            }
            catch { /* don't care */ }
        }

        private readonly UdpClient udp = new UdpClient(Config.PORT_NUMBER);

        private void StartListening()
        {
            while (true) {
                Receive();
                //ar_ =  udp.BeginReceive(Receive, new object());
            }
        }
        private void Receive()
        {
            byte[] bytes = udp.Receive(ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("From {0} received: {1} ", ip.Address.ToString(), message);
            
            // Send response
            //UdpClient client = new UdpClient(Config.PORT_NUMBER);
            byte[] response = Encoding.UTF8.GetBytes(joinToken);
            udp.Send(response, response.Length, ip);
            Console.WriteLine("Sent response to client.");            
        }

        public void GetDockerSwarmInfo() {
           // SwarmInitParameters x = new SwarmInitParameters();
            var swarmParameters = new SwarmInitParameters
            {
                //AdvertiseAddr = "192.168.1.100:2377", // Replace with your node's IP address and a port
                ListenAddr = "0.0.0.0:5000", // The listen address (interface and port) for the swarm managers
                ForceNewCluster = false // true to force creating a new swarm, even if one already exists
            };

            try {
                Task<String> t = dockerClient.Swarm.InitSwarmAsync(swarmParameters);

                Task<String> task = Task<String>.Run(async () => { return await t; });
                task.Wait();

                Console.WriteLine("Task result:"+task.Result);
            } catch (Docker.DotNet.DockerApiException) {
                Console.WriteLine("Probably swarm already exists1");
            } catch (System.AggregateException) {
                Console.WriteLine("Probably swarm already exists2");
            }
            Task<SwarmInspectResponse> t2 = dockerClient.Swarm.InspectSwarmAsync();
            Task<SwarmInspectResponse> task2 = Task<SwarmInspectResponse>.Run(async () => { return await t2; });
            task2.Wait();
            joinToken = task2.Result.JoinTokens.Worker;
            Console.WriteLine("Join token:"+joinToken);

        }

   }

    
    
}