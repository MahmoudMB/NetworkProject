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

          
            

            // Parse Request line

            if (!ParseRequestLine())
                return false;


            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;

            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;




            return true;

        }

        private bool ParseRequestLine()
        {

            string[] tokens2 = requestLines[0].Split(' ');
            
            switch (tokens2[2])
            {
                case "HTTP/1.0":
                    this.httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    this.httpVersion = HTTPVersion.HTTP11;
                    break;
            }
           // method = (RequestMethod)Enum.Parse(typeof(RequestMethod), Line1[0]);
            //relativeURI = tokens2[1].Substring(1);
            //da bdal da bardo


            if (tokens2.Length >=2)
            {
                tokens2[0] = tokens2[0].ToUpper();
                if (tokens2[0].Equals(RequestMethod.GET))
                {
                    method = RequestMethod.GET;
                }

                else if (tokens2[0].Equals(RequestMethod.POST))
                {
                    method = RequestMethod.POST;
                }

                else if (tokens2[0].Equals(RequestMethod.HEAD))
                {
                    method = RequestMethod.HEAD;
                }



                if (ValidateIsURI(tokens2[1]))
                {
                    string[] tmp = tokens2[1].Split('/');
                    relativeURI = tmp[1];
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }








            // throw new NotImplementedException();
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            // throw new NotImplementedException();

            /// 
            headerLines = new Dictionary<string, string>();
        
            string[] Delimeter = new string[] { ": " };
            for (int index = 1; index < this.requestLines.Length - 2; index++)
            {
                string[] HeaderLine = requestLines[index].Split(Delimeter, StringSplitOptions.None);
                if (HeaderLine.Length < 2) return false;
                this.HeaderLines.Add(HeaderLine[0], HeaderLine[1]);
            }
            return true;






        }

        private bool ValidateBlankLine()
        {



            if (this.requestLines[requestLines.Length - 2] == "")
                return true;
            return false;

            
        

          
            



        }

    }
}
