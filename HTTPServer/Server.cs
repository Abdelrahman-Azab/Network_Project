using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    /* Done All */
    class Server
    {

        Socket serverSocket;
        string contentType = "text/html";
        

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
            LoadRedirectionRules(redirectionMatrixPath);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(ip, portNumber);
            serverSocket.Bind(hostEndPoint);
        }

        public void StartServer() 
        {
            // TODO: Listen to connections, with large backlog.
            Console.WriteLine("\t \t \t \t \t** Welcome to My Server **\t \t \t \t \t");
            Console.WriteLine("Listening.........");
            serverSocket.Listen(800);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New Client accepted:{0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj) 
        {
            // TODO: Create client socket 
            Socket recievedClient = (Socket)obj;
            byte[] data;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            recievedClient.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            int recievedLength;
            while (true)
            {
                try
                {
                    data = new byte[1024];
                    // TODO: Receive request
                    recievedLength = recievedClient.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if(recievedLength==0)
                    {
                        Console.WriteLine("Client with IP:{0} ended the connection with the server", recievedClient.RemoteEndPoint);
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data,0,recievedLength));

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);

                    // TODO: Send Response back to client
                    data = Encoding.ASCII.GetBytes(response.ResponseString);
                    recievedClient.Send(data);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket

            recievedClient.Close();
        }

        Response HandleRequest(Request request) 
        {
            string content;
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest() == false)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    Console.WriteLine("Response Code: 400");
                    return new Response(StatusCode.BadRequest, contentType, content, null);
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                //TODO: check for redirect
                if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {
                    string redirectedPage = Configuration.RedirectionRules[request.relativeURI];
                    content = LoadDefaultPage(GetRedirectionPagePathIFExist(redirectedPage));
                    Console.WriteLine("Response Code: 301");
                    return new Response(StatusCode.Redirect, contentType, content, redirectedPage);
                }
                if (request.relativeURI == "")
                {
                    if (File.Exists(Path.Combine(Configuration.RootPath, Configuration.PageName)))
                    {
                        content = LoadDefaultPage(Configuration.PageName);
                        Console.WriteLine("Response Code: 200");
                        return new Response(StatusCode.OK, contentType, content, null);
                    }
                }



                //TODO: check file exists

                //TODO: read the physical file

                // Create OK response
                content = LoadDefaultPage(request.relativeURI);
                if (content != "")
                {
                    Console.WriteLine("Response Code: 200");
                    return new Response(StatusCode.OK, contentType, content, null);
                }
                //404 Not Found
                content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                Console.WriteLine("Response Code: 404");
                return new Response(StatusCode.NotFound, contentType, content, null);

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                Console.WriteLine($"Response Status Code: {500}");
                return new Response(StatusCode.InternalServerError, contentType, content, null);
                
            }

        }

        private string GetRedirectionPagePathIFExist(string relativePath)//
        {
         /*   // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;
         */

            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (File.Exists(Configuration.RootPath + "\\" + relativePath))
            {
                return relativePath;
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)//
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception("Default Page:" + defaultPageName + " doesn't exist"));
                return string.Empty;
            }
            // else read file and return its content
            return File.ReadAllText(filePath);



        }

        private void LoadRedirectionRules(string filePath)//
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                int cnt = 0;
               
                string[] slashrslashn = new string[] { "\r\n" };
                string[] redirectionalRules = File.ReadAllText(filePath).Split(slashrslashn, StringSplitOptions.RemoveEmptyEntries);
                
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while (cnt < redirectionalRules.Length)
                {
                    string[] Page = redirectionalRules[cnt].Split(',');
                    Configuration.RedirectionRules.Add(Page[0], Page[1]);
                    cnt++;
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
