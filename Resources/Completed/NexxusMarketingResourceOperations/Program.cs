//===================================================================
//=																	=
//= Copyright (c) 2015 IMS Health Incorporated. All rights reserved.=
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

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        //Sample code for calling SOAP operations - in this case Show.
        static void ExerciseSOAPOperations()
        {
            NapierSamples ns = new NapierSamples();
            ns.ShowSample();
        }

        /// <summary>
        /// Sample code for calling BulkCreate with the resources supplied in the REST body.
        /// </summary>
        static void BulkCallBulkCreateWithBody()
        {
            string userName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            string bulkURL = ConfigurationManager.AppSettings["BulkURL"];
            BulkSamples bulkSamples = new BulkSamples();
            string create = "/api/bulk/2.0/Create/Contact";
            bulkSamples.WebClientBulkCreateUsingBody(bulkURL + create, userName, password);

        }
        /// <summary>
        /// Sample code for creating Contact resources with the resource descriptions provided in a file.
        /// </summary>
        static void BulkCallBulkCreateWithInputFile()
        {
            string userName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            string bulkURL = ConfigurationManager.AppSettings["BulkURL"];
            BulkSamples bulkSamples = new BulkSamples();
            string create = "/api/bulk/2.0/Create/Contact";
            bulkSamples.WebClientBulkCreateUsingFile(bulkURL + create, "upload.psv", userName, password);

        }

        /// <summary>
        /// Sample code for finding a single resource - requires the query field to be a unique value. In this sample ExternalContactId.
        /// </summary>
        static void FindSingleResource()
        {
            string userName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            string bulkURL = ConfigurationManager.AppSettings["BulkURL"];
            string externalContactId = "IDX99";

            BulkSamples bulkSamples = new BulkSamples();
            string query = "/api/bulk/2.0/find/Contact?fields=Id, ExternalContactId,FirstName,LastName,Email,Phone&query=ExternalContactId='" + externalContactId + "'";

            List<Resource> resources = bulkSamples.FindResources(bulkURL + query, userName, password);
            if (resources.Count > 0) {
                Resource resource = resources[0];  // ExternalContactId is unique, there will be one or zero resources returned.
                foreach (var field in resource.Field) {
                    Console.WriteLine("Field: " + field.Id + " Value: " + field.Value);
                }
            }
            else {
                Console.WriteLine("Contact with ExternalContactId {0} could not be retrieved or does not exist.", externalContactId);
            }

            Console.WriteLine("Done");
            Console.ReadKey();

        }
        /// <summary>
        /// Sample code for querying multiple resources and storing the results in a file.
        /// </summary>
        static void BulkFindMultipleResources()
        {
            //  Find Single Resource
            string userName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            string bulkURL = ConfigurationManager.AppSettings["BulkURL"];
            BulkSamples bulkSamples = new BulkSamples();

            string query = "/api/bulk/2.0/find/Contact?fields=Id,FirstName,LastName,Email,Phone&query=LastName+IS+NOT+NULL&limit=10";
            bulkSamples.WebClientFindToFile(bulkURL+query, userName, password);
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
