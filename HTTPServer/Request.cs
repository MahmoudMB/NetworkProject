using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //  throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            requestLines = requestString.Split(new[] { "\r\n" }, StringSplitOptions.None);

            if (!requestLines[1].Contains("Host"))
            {
                Logger.LogException(new Exception("Host Header " + requestLines[0] + " is missing"));
                return false;
            }

            // Parse Request line
            if (!ParseRequestLine())
            {
                return false;
            }

            // Validate blank line exists
            if (!ValidateBlankLine())
            {
                return false;
            }
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
            {
                return false;
            }


            return true;

        }

        private bool ParseRequestLine()
        {

            string[] requestLine = requestLines[0].Split(' ');


            if (requestLine.Length >=2 )
            {
                requestLine[0] = requestLine[0].ToUpper();
                if (requestLine[0].Equals(RequestMethod.GET))
                {
                    method = RequestMethod.GET;
                }

                else if (requestLine[0].Equals(RequestMethod.POST))
                {
                    method = RequestMethod.POST;
                }

                else if (requestLine[0].Equals(RequestMethod.HEAD))
                {
                    method = RequestMethod.HEAD;
                }



                if (ValidateIsURI(requestLine[1]))
                {
                    string[] tmp = requestLine[1].Split('/');
                    relativeURI = tmp[1];
                   
                }
                else
                {

                    Logger.LogException(new Exception("Relative Uri " + requestLines[1] + " is Invalid"));
                    return false;
                }

                requestLine[2] = requestLine[2].ToUpper();

                if (requestLine[2].Equals(HTTPVersion.HTTP09))
                {
                    httpVersion = HTTPVersion.HTTP09;
                }

                else if (requestLine[2].Equals(HTTPVersion.HTTP10))
                {
                    httpVersion = HTTPVersion.HTTP10;
                }

                else if (requestLine[2].Equals(HTTPVersion.HTTP11))
                {
                    httpVersion = HTTPVersion.HTTP11;
                }


            }
            else
            {
                Logger.LogException(new Exception("RequstLine " + requestLines[0] + " is missing informations"));
                return false;
            }



            return true;

            // throw new NotImplementedException();
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            // throw new NotImplementedException();

           

            headerLines = new Dictionary<string, string>();
        
            for (int i = 1; i < requestLines.Length - 2; i++)
            {
                string[] headerLine = requestLines[i].Split(new string[] { ": " }, StringSplitOptions.None);
                if (headerLine.Length == 2)
                {
                    HeaderLines.Add(headerLine[0], headerLine[1]);
                }
                else
                {
                    Logger.LogException(new Exception("Header Line " + headerLine[i] + " is missing"));
                    return false;
                }

            }
            return true;

        }

        private bool ValidateBlankLine()
        {

            int blankLinendex = requestLines.Length - 2;


            if (requestLines[blankLinendex] != string.Empty)
            {
                Logger.LogException(new Exception("Blank Line " + requestLines[0] + " is missing"));
                return false;
            }

            else

                return true;






        }

    }
}
