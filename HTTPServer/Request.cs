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
            if (tokens2.Length == 3)
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


            string[] temp = requestLines;

            for (int i = 0; i < temp.Length - 1; i++)
            {
                if (!temp[i + 1].Equals(""))
                {

                    temp[i] = temp[i + 1];
                }
                else
                {
                    temp[i] = "";
                    break;
                }
            }



            string[] headerLinesTokens;
            headerLines = new Dictionary<string, string>();
            //headerLinesTokens = requestLines[2].Split(new[] { "\r\n" }, StringSplitOptions.None);
            headerLinesTokens = temp;


            try
            {
                for (int i = 0; i < headerLinesTokens.Length; i++)
                {

                    int separator = headerLinesTokens[i].IndexOf(':');
                    if (separator == -1)
                    {
                        // throw new Exception("invalid http header line: " + headerLinesTokens[i]);
                        break;
                    }
                    String name = headerLinesTokens[i].Substring(0, separator);
                    int pos = separator + 1;
                    while ((pos < headerLinesTokens[i].Length) && (headerLinesTokens[i][pos] == ' '))
                    {
                        pos++; // strip any spaces
                    }

                    string value = headerLinesTokens[i].Substring(pos, headerLinesTokens[i].Length - pos);
                    //  Console.WriteLine("header: {0}:{1}", name, value);
                    headerLines.Add(name, value);
                }
            }

            catch (Exception ex)
            {
            }
            return true;




        }

        private bool ValidateBlankLine()
        {
            //   throw new NotImplementedException();

            if (!requestString.Contains("\r\n\r\n"))
                return false;

            return true;




        }

    }
}
