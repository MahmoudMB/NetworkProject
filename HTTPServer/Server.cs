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
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket

            IPEndPoint iep = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(iep);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientSocket = this.serverSocket.Accept();

                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));

                newthread.Start(clientSocket);
                //TODO: accept connections and start thread for each accepted connection.

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSock = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSock.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request

                    byte[] requestRecived = new byte[1024 * 1024];
                    int receivedLen = clientSock.Receive(requestRecived);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                        break;
                    // TODO: Call HandleRequest Method that returns the response
                    string requeststring = Encoding.ASCII.GetString(requestRecived, 0, receivedLen);
                    Request request = new Request(requeststring);

                    Response response = HandleRequest(request);

                    // TODO: Send Response back to client
                    byte[] responsebytes = Encoding.ASCII.GetBytes(response.ResponseString);
                    clientSock.Send(responsebytes);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            //  clientSock.Close();
        }

        Response HandleRequest(Request request)
        {
            //  throw new NotImplementedException();
            string content;
            string contentType = "text/html";
            StatusCode statuecode;
            try
            {

                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    statuecode = HTTPServer.StatusCode.BadRequest;
                    return new Response(statuecode, contentType, content, GetRedirectionPagePathIFExist(request.relativeURI));


                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string redirectedPath = GetRedirectionPagePathIFExist(request.relativeURI);


                if (redirectedPath != "")
                {
                    physicalPath = redirectedPath;
                    statuecode = HTTPServer.StatusCode.Redirect;
                    //TODO: read the physical file

                    //StreamReader reader = new StreamReader(physicalPath);
                    //  content = reader.ReadToEnd();
                    //  reader.Close();

                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);



                    // return new Response(statuecode, contentType, content, redirectedPath);
                    return new Response(statuecode, contentType, content, physicalPath);
                }
                //TODO: check file exists
                if (File.Exists(physicalPath))
                {                //TODO: read the physical file

                    StreamReader reader = new StreamReader(physicalPath);
                    content = reader.ReadToEnd();
                    reader.Close();

                    // Create OK response
                    statuecode = HTTPServer.StatusCode.OK;
                    return new Response(statuecode, contentType, content, redirectedPath);

                }
                else
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    statuecode = HTTPServer.StatusCode.NotFound;

                    return new Response(statuecode, contentType, content, redirectedPath);
                }



            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                statuecode = HTTPServer.StatusCode.InternalServerError;
                return new Response(statuecode, contentType,
                    content, GetRedirectionPagePathIFExist(request.relativeURI));


            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            try
            {
                string redirectedPage = Configuration.RedirectionRules[relativePath];
                string filePath = Path.Combine(Configuration.RootPath, redirectedPage);
                if (File.Exists(filePath))
                {
                    return redirectedPage;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;

            }

            return string.Empty;
        }


        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception("Default Page " + defaultPageName + " not exist"));

            }
            else
            {
                StreamReader reader = new StreamReader(filePath);
                string file = reader.ReadToEnd();
                reader.Close();
                return file;
            }

            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                Configuration.RedirectionRules = new Dictionary<string, string>();
                // TODO: using the filepath paramter read the redirection rules from file 

                StreamReader reader = new StreamReader(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                // then fill Configuration.RedirectionRules dictionary 

                while (!reader.EndOfStream)
                {
                    string temp = reader.ReadLine();
                    string[] result = temp.Split(',');
                    Configuration.RedirectionRules.Add(result[0], result[1]);
                }

                reader.Close();





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