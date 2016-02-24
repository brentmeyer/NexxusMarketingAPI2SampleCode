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
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Configuration;

namespace FindMerge
{
    /// <summary>
    /// Class to manage MergeExisting operation.
    /// </summary>
    class Merge
    {
        NapierClient api;
        string userName;
        string password;
        string bulkURL;

        public Merge() {
            api = new NapierClient();
            userName = ConfigurationManager.AppSettings["UserName"];
            password = ConfigurationManager.AppSettings["Password"];
            bulkURL = ConfigurationManager.AppSettings["BulkURL"];
        }

     }
}
