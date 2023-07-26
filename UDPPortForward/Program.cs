namespace UDPPortForwarder
{
    class Program
    {
        static void Main(string[] args)
        {
            string ListenAddr = args[0];
            string SendAddr = args[1];
            List<int> ports = new List<int>();
            for (int port_index = 2; port_index < args.Length; port_index++) {
                int port = int.Parse(args[port_index]);
                ports.Add(port);
            }
            UDPPortForwarder f = new UDPPortForwarder(ListenAddr, SendAddr, ports);
            f.Start();
        }
    }
}