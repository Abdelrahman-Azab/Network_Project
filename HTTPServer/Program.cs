using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{ /*     Done All    */

    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
           
            //Start server
            // 1) Make server object on port 1000
            Server s = new Server(1000, "redirectionRules.txt");
            // 2) Start Server
            s.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            FileStream fs = new FileStream("redirectionRules.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("aboustus.html,aboutus2.html");
            sw.Close();
        }
         
    }
}
