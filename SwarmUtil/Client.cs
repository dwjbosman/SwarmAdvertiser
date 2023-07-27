//using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Diagnostics;

namespace SwarmAdvertiser {
    class Client
    {
        UdpClient client;
        IPEndPoint ip;
        DockerClient dockerClient;
        
        String joinToken = "";

        public Client() {
            dockerClient = new DockerClientConfiguration().CreateClient();

            ip = new IPEndPoint(IPAddress.Any, Config.PORT_NUMBER);

            client = new UdpClient();

        }
        public void Request(string ip)
        {
            if (ip!="") {
                Send("Doit",ip);
            } else {
                Send("Doit");
            }
            Receive();
            client.Close();
            JoinDockerSwarm();
        }

        public void RequestToken(string ip) {
            if (ip!="") {
                Send("Doit",ip);
            } else {
                Send("Doit");
            }
            Receive();
            client.Close();
        }

        private void Receive()
        {
            byte[] bytes = client.Receive(ref ip);
            joinToken = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("From {0} received: {1} ", ip.Address.ToString(), joinToken);
    	}


        public void JoinDockerSwarm() {
           // SwarmInitParameters x = new SwarmInitParameters();

            string managerIp = ip.Address.ToString()+":5000";
            
	    var cliProcess = new Process() {
		    StartInfo = new ProcessStartInfo("./get_win_host_ip.sh", ip.Address.ToString()) {
			    UseShellExecute = false, RedirectStandardOutput = true
		    }
	    };
	    cliProcess.Start();
	    string localIp = cliProcess.StandardOutput.ReadToEnd();
	    localIp = localIp.Trim();
	    cliProcess.WaitForExit();
	    cliProcess.Close();

	    Console.WriteLine("Join {0} from client '{1}'", managerIp, localIp); 
            var swarmParameters = new SwarmJoinParameters
            {
                RemoteAddrs = new List<string> { managerIp }, // Replace with your manager's IP address and port
                ListenAddr = "0.0.0.0:5000", // The listen address (interface and port) for the node
                AdvertiseAddr = localIp, // Replace with your node's IP address and a port
                JoinToken = joinToken // true to force creating a new swarm, even if one already exists
            };

            try {
                Task t = dockerClient.Swarm.JoinSwarmAsync(swarmParameters);
                

                Task task = Task<String>.Run(async () => { await t; });
                task.Wait();

                Console.WriteLine("Joined");
            } catch (Docker.DotNet.DockerApiException) {
                Console.WriteLine("Did not join1");
            } catch (System.AggregateException e) {
                Console.WriteLine("Did not join2:"+e);
            }
            /**
            Task<SwarmInspectResponse> t2 = dockerClient.Swarm.InspectSwarmAsync();
            Task<SwarmInspectResponse> task2 = Task<SwarmInspectResponse>.Run(async () => { return await t2; });
            task2.Wait();
            joinToken = task2.Result.JoinTokens.Worker;
            Console.WriteLine("Join token:"+joinToken);
            **/

        }



        private void Send(string message, string destIp="255.255.255.255")
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(destIp), Config.PORT_NUMBER);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            client.Send(bytes, bytes.Length, ip);
            //client.Close();
            Console.WriteLine("Sent: {0}  to {1}", message, ip.Address.ToString());
        }
    }
}
