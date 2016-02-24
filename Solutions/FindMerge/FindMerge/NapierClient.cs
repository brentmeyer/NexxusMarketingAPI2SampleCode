//===================================================================
//=																	=
//= Copyright (c) 2016 IMS Health Incorporated. All rights reserved.=
//=																	=
//===================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace FindMerge
{
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
