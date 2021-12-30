using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    /* Done All */
    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string Date = DateTime.Now.ToString();
            string header;
            if (redirectoinPath != null)
            {

                header = "ContentType:" + contentType + "\r\n" + "ContentLength" + content.Length + "\r\n" + "Date:" + Date + "\r\n" + "Location:" + redirectoinPath + "\r\n";

            }
            else
            {
                header = "ContentType:" + contentType + "\r\n" + "ContentLength" + content.Length + "\r\n" + "Date:" + Date + "\r\n";
            }


            // TODO: Create the request string
            switch (code)
            {
                case StatusCode.OK:
                    responseString = GetStatusLine(code) + " OK" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.InternalServerError:
                    responseString = GetStatusLine(code) + " Internal Server Error" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.NotFound:
                    responseString = GetStatusLine(code) + " Not Found" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.BadRequest:
                    responseString = GetStatusLine(code) + " Bad Request" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.Redirect:
                    responseString = GetStatusLine(code) + " Redirectied" + "\r\n" + header + "\r\n" + content;
                    break;
                default:
                    responseString = "Error in Response Class";
                    break;
            }
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string httpVersion = Configuration.ServerHTTPVersion;
            string statusLine = httpVersion + code.ToString();
            return statusLine;
        }
    }
}
