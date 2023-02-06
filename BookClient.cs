using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BookClient
{
    class Program
    {
        private const int serverPort = 8272;
        private const string serverAddress = "127.0.0.1";

        static void Main(string[] args)
        {
            //establish connection
            UdpClient client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            client.Connect(ep);

            while (true)
            {

                Console.WriteLine("Enter GET to optain a title\n Enter ADD + your username to add 1 or multiple titles\n Enter blank line to quit");

                //client input
                string text = Console.ReadLine();

                try
                {
                    //send client input to server
                    byte[] sendBuffer = Encoding.ASCII.GetBytes(text);
                    client.Send(sendBuffer, sendBuffer.Length);

                    //recieve message from server
                    byte[] receivedData = client.Receive(ref ep);
                    string reply = Encoding.ASCII.GetString(receivedData);
                    Console.WriteLine("Server reply: {0}", reply);

                    
                    
                }
                catch (SocketException s)
                {
                    Console.WriteLine("You are disconnected.");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
            }
        }
    }
}