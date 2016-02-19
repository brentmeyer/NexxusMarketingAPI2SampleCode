//===================================================================
//=																	=
//= Copyright (c) 2015 IMS Health Incorporated. All rights reserved.=
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

namespace NexxusMarketingAPISamples
{
    class BulkSamples
    {
        public List<Resource> FindResources(string url, string userName, string password) {
            List<Resource> resources = new List<Resource>();
            WebClient wc = new WebClient();

            //Set up basic authorization.
            var credential = userName + ":" + password;
            wc.Headers["Authorization"] =
                "Basic " +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));

            try {
                //Make the query and download to file.
                string result = wc.DownloadString(url);
                resources = Hydrate.HydrateBulkResources(result);

                if (resources.Count == 0) {
                    Console.WriteLine("No resource(s) in the query were found  - " + url);
                }
            }
            catch (WebException e) {
                if (e.Status == WebExceptionStatus.ProtocolError) {
                    string nexxusErrorString = e.Response.Headers["NexusErrorString"];

                    if (nexxusErrorString != null) {
                        Console.WriteLine("Nexxus Error Message : " + nexxusErrorString);
                    }
                    else {
                    }
                    Console.WriteLine("Operation Failed : " + e.Message);
                }
                else {
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return resources;
        }

        public void WebClientFindToFile(string url, string userName, string password)
        {
            WebClient wc = new WebClient();
            WebHeaderCollection headers = null;
            //Set up basic authorization.
            var credential = userName + ":" + password;
            wc.Headers["Authorization"] =
                "Basic " +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));

            try{
                //Make the query and download to file.
                wc.DownloadFile(url, "QueryResults.PSV");
                headers = wc.ResponseHeaders;
                Console.WriteLine("File QueryResults.PSV created.");
            }
            catch (WebException e){
                if (e.Status == WebExceptionStatus.ProtocolError){
                    string nexxusErrorString = e.Response.Headers["NexusErrorString"];

                    if (nexxusErrorString != null){
                        Console.WriteLine("Nexxus Error Message : " + nexxusErrorString);
                    }
                    Console.WriteLine("Operation Failed : " + e.Message);
                }
                else{
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }



        public void WebClientBulkCreateUsingFile(string url, string fileName, string userName, string password)
        {
            WebClient wc = new WebClient();
            WebHeaderCollection headers = null;
            var credential = userName + ":" + password;
            wc.Headers["Authorization"] =
                "Basic " +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));
            try
            {
                byte[] resp = wc.UploadFile(url, fileName);
                headers = wc.ResponseHeaders;
                Console.WriteLine("Completed - Job Identifier is " + headers["NexusApiJobId"]);

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    string nexxusErrorString = e.Response.Headers["NexusErrorString"];

                    if (nexxusErrorString != null)
                    {
                        Console.WriteLine("Nexxus Error Message : " + nexxusErrorString);
                    }
                    Console.WriteLine("Operation Failed : " + e.Message);
                }
                else
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
        }

        public void WebClientBulkCreateUsingBody(string url, string userName, string password)
        {
            WebClient wc = new WebClient();
            WebHeaderCollection headers = null;
            var credential = userName + ":" + password;
            wc.Headers["Authorization"] =
                "Basic " +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));

            //Build a contact record.
            StringBuilder data = new StringBuilder();
            data.Append("FirstName|LastName|Email\n");
            data.Append("Fake|Person|fperson@fake.com\n");
            byte[] bytedata = UTF8Encoding.UTF8.GetBytes(data.ToString());

            //gzip the record.
            
            wc.Headers.Add("Content-Encoding: gzip");
            byte[] compressedBytes;
            using (var outStream = new MemoryStream())
            {
                using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                using (var mStream = new MemoryStream(bytedata))
                    mStream.CopyTo(tinyStream);
                compressedBytes = outStream.ToArray();
            }
            //Call the operation.
            try{     //change compressedBytes to bytedata if you are not using gzip.              
                wc.UploadData(url, compressedBytes);
                headers = wc.ResponseHeaders;
                Console.WriteLine("Completed - Nexxus Job Identifier is " + headers["NexusApiJobId"]);
            }
            catch (WebException e){
                if (e.Status == WebExceptionStatus.ProtocolError){
                    string nexxusErrorString = e.Response.Headers["NexusErrorString"];

                    if (nexxusErrorString != null){
                        Console.WriteLine("Nexxus Error Message : " + nexxusErrorString);
                    }
                    Console.WriteLine("Operation Failed : " + e.Message);
                }
                else{
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e){
                Console.WriteLine(e.Message);

            }

        }


    }
}
