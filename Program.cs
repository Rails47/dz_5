using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ConsoleApp14
{
    class Server
    {
        private TcpListener listener;
        private List<string> messages = new List<string>();

        public Server(string ipAddress, int port)
        {
            IPAddress address = IPAddress.Parse(ipAddress);
            listener = new TcpListener(address, port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected...");

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string received = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + received);

                if (received.Trim().ToUpper() == "EXIT")
                {
                    Console.WriteLine("Client requested to close the connection.");
                    break;
                }

                messages.Add(received);
            }

            stream.Close();
            client.Close();
        }
    }

    class Client
    {
        public void Start(string ipAddress, int port)
        {
            TcpClient client = new TcpClient(ipAddress, port);
            NetworkStream stream = client.GetStream();

            Console.WriteLine("Connected to server...");

            while (true)
            {
                Console.Write("Enter message (type 'exit' to quit): ");
                string message = Console.ReadLine();

                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                if (message.ToUpper() == "EXIT")
                    break;
            }

            stream.Close();
            client.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("192.168.100.3", 8888);
            server.Start();

            Client client = new Client();
            client.Start("192.168.100.3", 8888);
        }
    }
}
