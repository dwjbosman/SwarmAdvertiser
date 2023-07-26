namespace UDPPortForwarder { 

using System.Net.Sockets;
using System.Net;
using System.Text;

public struct UdpState
{
    public UdpClient server;
    public UdpClient client;
    //public IPEndPoint e;
    public int index;
    public IPEndPoint SendIP;
    public IPEndPoint ReceiveIP;
}

public class UDPPortForwarder {

    List<UdpState> states;

    public UDPPortForwarder(string ListenAddress, string SendAddress, List<int> ports) {
        states = new List<UdpState>();
        for (int i = 0; i < ports.Count(); i++) {
            IPEndPoint ListenIp = new IPEndPoint(IPAddress.Parse(ListenAddress),ports[i]);
            
            
            UdpState s = new UdpState();
            s.index = i;
            s.ReceiveIP = ListenIp;
            s.server = new UdpClient(s.ReceiveIP);
            s.SendIP = new IPEndPoint(IPAddress.Parse(SendAddress),ports[i]);
            s.client = new UdpClient();
            states.Add(s);

            Console.WriteLine($"Forward messages from {s.ReceiveIP.ToString()} to {s.SendIP.ToString()}");
        }
    }

    public void Start() {
        foreach (UdpState s in states) {
            s.server.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }
        while (true) {
            Thread.Sleep(100);
        }
    }
 


    public static void ReceiveCallback(IAsyncResult ar)
    {
        if (ar.AsyncState==null) {
            return;
        }
        UdpState s = ((UdpState)(ar.AsyncState));
        UdpClient server = s.server;
        IPEndPoint SendIP = s.SendIP;
        IPEndPoint ReceiveIP = s.ReceiveIP;
        
        IPEndPoint? ReceivedFromIP = null;
        byte[] receiveBytes = server.EndReceive(ar, ref ReceivedFromIP);

        if (ReceivedFromIP!=null) {
            Console.WriteLine($"Received message on {ReceiveIP.ToString()}, {receiveBytes.Length} from: {ReceivedFromIP.ToString()}, forward to {SendIP.ToString()}");
        } else {
            Console.WriteLine($"Received message on {ReceiveIP.ToString()}, forward to {SendIP.ToString()}");
        }
	s.client.SendAsync(receiveBytes,receiveBytes.Length,SendIP);
	//int sent = server.Send(receiveBytes,receiveBytes.Length,SendIP);
	//int sent = sender.Send(receiveBytes,receiveBytes.Length,SendIP);
        //Console.WriteLine($"Bytes sent {sent}");
        
	server.BeginReceive(new AsyncCallback(ReceiveCallback), s);

    }   
}


}
