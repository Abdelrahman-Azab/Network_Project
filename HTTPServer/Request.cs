using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    /* Done All*/
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
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line

            // Validate blank line exists

            // Load header lines into HeaderLines dictionary
            if (ParseRequestLine() || LoadHeaderLines() || ValidateBlankLine())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            contentLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (contentLines.Length >= 4)
            {
                requestLines = contentLines[0].Split(' ');
                if (requestLines.Length != 3)
                {
                    return false;
                }
                if (requestLines[0] == "GET")
                {
                    method = RequestMethod.GET;
                }
                else if (requestLines[0] == "POST")
                {
                    method = RequestMethod.POST;
                }
                else
                {
                    method = RequestMethod.HEAD;
                }
                
                if (!ValidateIsURI(requestLines[1]))
                {
                    return false;
                }
                relativeURI = requestLines[1].Remove(0, 1);
                if (requestLines[2] == "HTTP/1.1")
                {
                    httpVersion = HTTPVersion.HTTP11;
                }
                else if (requestLines[2] == "HTTP/1.0")
                {
                    httpVersion = HTTPVersion.HTTP10;
                }
                else
                {
                    httpVersion = HTTPVersion.HTTP09;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
           
            string[] seprator = new string[] { ": " };
            headerLines = new Dictionary<string, string>();
            int cnt = 1;
            while (cnt < contentLines[cnt].Length)
            {
                string[] Headers = contentLines[cnt].Split(seprator, StringSplitOptions.RemoveEmptyEntries);
                if (Headers.Length == 0)
                {
                    break;
                }
                headerLines.Add(Headers[0], Headers[1]);
                cnt++;
            }
            return headerLines.Count > 1;
        }

        private bool ValidateBlankLine()
        {
            return requestString.EndsWith("\r\n\r\n");
        }

    }
}
