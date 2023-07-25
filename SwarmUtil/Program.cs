namespace SwarmAdvertiser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "Server") {
                Console.WriteLine("Server");
                
                Server s = new Server();
                s.Start();
            }
            if (args[0] == "Client") {
                Console.WriteLine("Client");
                Client c = new Client();

                string ip = "";
                try {
                    ip = args[1];
                } catch (Exception) {

                }

                c.Request(ip);
            }
            if (args[0] == "RequestToken") {
                Console.WriteLine("Client");
                Client c = new Client();
                string ip = "";
                try {
                    ip = args[1];
                } catch (Exception) {

                }

                c.RequestToken(ip);
            }
        }
    }
}