//===================================================================
//=																	=
//= Copyright (c) 2016 IMS Health Incorporated. All rights reserved.=
//=																	=
//===================================================================

// Completed code snippets for topics in the Nexxus Marketing Resources section of the Nexxus Marketing API documentation.
// Remember to update the user name and password in AppConfigFile.config before running.
// Contact: crees@us.imshealth.com 


using System;
using System.Configuration;
using System.Net;
using System.Collections.Generic;


namespace NexxusMarketingAPISamples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Insert calling code here. Either call an updated ExerciseSOAPOperations or one of the Bulk functions below. 

        }


    }

    public class NapierClient : Napier
    {

        string userName;
        string password;


        public NapierClient()
        {
            userName = ConfigurationManager.AppSettings["UserName"];
            password = ConfigurationManager.AppSettings["Password"];

        }

        protected override WebRequest GetWebRequest(Uri uri)
        {

            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(new Uri(this.Url));
            string credential = this.userName + ":" + this.password;
            request.Headers["Authorization"] =
              "Basic " +
              Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credential));
            return request;
        }
    }
}
