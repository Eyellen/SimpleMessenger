using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    class Program
    {
        const string eom = @"\eom";

        static byte[] buffer = new byte[64];

        static IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 15031);
        static Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket connection;

        static void Main(string[] args)
        {
            Console.WriteLine("Server starting.");

            listener.Bind(serverEndPoint);
            listener.Listen(0);

            Console.WriteLine("Listening for connection requests.");
            connection = listener.Accept();
            Console.WriteLine("Connection established.");

            Task receiveHandler = Task.Run(() => HandleReceiveMessages());
            Task sendHandler = Task.Run(() => HandleSendMessages());
            Console.WriteLine("Now you can send and receive messages.");

            while (true) ;

            connection.Shutdown(SocketShutdown.Both);

            connection.Close();
            listener.Close();
        }

        private static void HandleReceiveMessages()
        {
            while (true)
            {
                int receivedBytesCount;
                string receivedChunk;
                string message = string.Empty;

                do
                {
                    receivedBytesCount = connection.Receive(buffer);
                    receivedChunk = Encoding.UTF8.GetString(buffer, 0, receivedBytesCount);

                    message += receivedChunk;
                } while (!message.Contains(eom));

                Console.WriteLine($"<Client> {message.Replace(eom, string.Empty)}");
            }
        }

        private static void HandleSendMessages()
        {
            while (true)
            {
                string message = Console.ReadLine() + eom;
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                connection.Send(messageBytes);
            }
        }
    }
}
