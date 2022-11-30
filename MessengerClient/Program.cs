﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClient
{
    class Program
    {
        static IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 15031);
        static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static byte[] buffer = new byte[1024];

        static void Main(string[] args)
        {
            Console.WriteLine("Client starting.");

            // Trying to connect to server
            while (!client.Connected)
            {
                try
                {
                    Console.WriteLine("Trying to connect...");
                    client.Connect(serverEndPoint);
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection attempt failed.");
                }
            }
            Console.WriteLine("Connection established.");

            Task receiveHandler = Task.Run(() => HandleReceiveMessages());
            Task sendHandler = Task.Run(() => HandleSendMessages());
            Console.WriteLine("Now you can send and received messages.");

            while (true) ;

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void HandleReceiveMessages()
        {
            while (true)
            {
                int receivedBytesCount = client.Receive(buffer);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytesCount);

                Console.WriteLine($"<Server> {receivedMessage}");
            }
        }

        private static void HandleSendMessages()
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                client.Send(messageBytes);
            }
        }
    }
}
