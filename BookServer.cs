using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace BookServer
{
    class Program
    {
        private const int port = 8272;
        private const string username1 = "eli";
        private const string username2 = "Vladimir Georgiev";
        static void Main(string[] args)
        {
            
            List<string> read = new List<string>();
            StreamReader reader = new StreamReader(@"C:\Users\user\source\repos\BookServer\BookServer\Books.txt");

            string line;

            //store the book titles in list of strings one by one
            while((line = reader.ReadLine()) != null){
                read.Add(line);
            }
            reader.Close();
           
            //establish connection
            UdpClient listener = new UdpClient(port);
            Console.WriteLine("Started listening...");

            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
                string message = "";
                
                //start looping client message <=> server respond
                while (true)
                {
                    string reply = "";
                    byte[] replyByteArr;

                    //get client message
                    byte[] recievedByteArr = listener.Receive(ref remoteEP);
                    message = Encoding.ASCII.GetString(recievedByteArr);
                    Console.WriteLine("Recieved: {0} from {1}", message, remoteEP.ToString());

                    //if client is not authorized to add titles or send anything diffarent from ADD + username or GET
                    if (!(message == "GET" || message == "ADD" + " " + username1 || message == "ADD" + " " + username2))
                    {
                        //return NOTOK
                        reply = "NOTOK";
                        replyByteArr = Encoding.ASCII.GetBytes(reply);
                        listener.Send(replyByteArr, replyByteArr.Length, remoteEP);

                        //client should quit
                        break;
                    }

                    //GET command
                    if (message == "GET")
                    {
                        Random random = new Random();

                        //return OK + random title
                        reply = "OK\n" + read[random.Next(0, read.Count)]; //generating random number and using that number to get a titles from the list at the random number's position and send it as a reply
                        replyByteArr = Encoding.ASCII.GetBytes(reply);
                        listener.Send(replyByteArr, replyByteArr.Length, remoteEP);
                        
                    }
                    //ADD command
                    if ((message == "ADD " + username1) || (message == "ADD " + username2))
                    {
                        while (true)
                        {
                            //if client send a new line it will stop adding titles
                            if(message.Length==0)
                            {
                                reply = "OK";
                                replyByteArr = Encoding.ASCII.GetBytes(reply);
                                listener.Send(replyByteArr, replyByteArr.Length, remoteEP);
                                break;
                            }
                            //return OK - after every title added
                            reply = "OK";
                            replyByteArr = Encoding.ASCII.GetBytes(reply);
                            listener.Send(replyByteArr, replyByteArr.Length, remoteEP);


                            //get client message - recieve title
                            recievedByteArr = listener.Receive(ref remoteEP);
                            message = Encoding.ASCII.GetString(recievedByteArr);
                            Console.WriteLine("Recieved: {0} from {1}", message, remoteEP.ToString());

                            try
                            {
                                //append the title to the Books.txt
                                if (message.Length>0)
                                {
                                    StreamWriter write = new StreamWriter(@"C:\Users\user\source\repos\BookServer\BookServer\Books.txt", append: true);
                                    write.WriteLine(message);

                                    write.Close();
                                }
                                
                            }catch(Exception e)
                            {
                                //return ERR
                                reply = "ERR\n" + e.ToString();
                                replyByteArr = Encoding.ASCII.GetBytes(reply);
                                listener.Send(replyByteArr, replyByteArr.Length, remoteEP);
                            }
                        }
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            listener.Close();

        }
    }
}
