# Socket-Programing

How the program works: in BookServer
Firstly, as private constants the port(used for establishing the connection) and 2 usernames(used by the ADD function to obtain access to append titles) are defined. 

        private const int port = 8272;
        private const string username1 = "eli";
        private const string username2 = "Vladimir Georgiev";
        
I chose to store the book titles read from the txt file as a list of strings because in order to satisfy the ADD function requirement I needed a data structure that can grow(not an array since I cannot predict how many titles the user will add).

      List<string> read = new List<string>();
  
 Using a while loop I read the Books.txt and add every line to the list using the build-in Add() function provided by the List class. 
 
        StreamReader reader = new StreamReader(@"C:\Users\user\source\repos\BookServer\BookServer\Books.txt");

            string line;
            while((line = reader.ReadLine()) != null){
                read.Add(line);
            }
            reader.Close();
            
The user can get a random title sending “GET” to the server, to which the server responds with OK and sends a random title. The random title is generated using an object from the Random class to access the Next() build-in function in the range from 0 to the number of strings in the list with book titles. 

        Random random = new Random();
        reply = "OK\n" + read[random.Next(0, read.Count)];
        
To add a title the user should send to the server “ADD“ + space + username (I have included 2 predefined usernames), the username of the client is checked by an if statement and if the client is authorized the client can add as many titles as the client want separated with new line. Inside the if statement there is another while loop that ensures that the client can enter as many titles as the client want, in order to get out of this loop the client should enter a new line that breaks the loop.

    if ((message == "ADD " + username1) || (message == "ADD " + username2))
       {
           while (true)
           {
              if(message.Length==0)
              {
                  reply = "OK";
                  replyByteArr = Encoding.ASCII.GetBytes(reply);
                  listener.Send(replyByteArr, replyByteArr.Length, remoteEP);
                                break;
              }…}…}

To append titles to the existing file: 

    if (message.Length>0)
       {
    StreamWriter write = new StreamWriter(@"C:\Users\user\source\repos\BookServer\BookServer\Books.txt", append: true);
           write.WriteLine(message);

           write.Close();
       }
       
If the titles was not saves to the file an error message is displayed. Otherwise, the server sends OK.
After that the client is still able to get titles and add titles. If the client is not authorized to add titles (the client provided username does not match the 2 predefined usernames) the client is forced to exit – the program terminates.

    if (!(message == "GET" || message == "ADD" + " " + username1 || message == "ADD" + " " + username2))
                    {
                        //return NOTOK
                        reply = "NOTOK";
                        replyByteArr = Encoding.ASCII.GetBytes(reply);
                        listener.Send(replyByteArr, replyByteArr.Length, remoteEP);

                        //client should quit
                        break;
                    }
                    
In the BookClient:
Loops send/receive info until the server is disconnected. When the client wishes to exit and send the appropriate command, BookServer terminates, and the client gets disconnected.
The logic of the program is that for every message send by the client to the server, the server has to send back a response and for every response send back from the server the server expects a new message.
